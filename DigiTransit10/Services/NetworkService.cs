﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using DigiTransit10.ExtensionMethods;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;
using DigiTransit10.GraphQL;

namespace DigiTransit10.Services
{
    public interface INetworkService
    {
        string DefaultRequestUrl { get; }

        Task<string> GetStop(int stopId, CancellationToken token = default(CancellationToken));
        Task<ApiPlan> PlanTrip(BasicTripDetails details, CancellationToken token = default(CancellationToken));
    }

    public class NetworkService : INetworkService
    {
        private readonly Backend.INetworkClient _networkClient;
        private HttpRequestHeaderCollection _defaultHeaders = null;

        public string DefaultRequestUrl { get; } = "https://api.digitransit.fi/routing/v1/routers/hsl/index/graphql";

        public NetworkService(Backend.INetworkClient networkClient)
        {
            _networkClient = networkClient;
            _defaultHeaders = _networkClient.DefaultHeaders;
        }

        public async Task<string> GetStop(int stopId, CancellationToken token = default(CancellationToken))
        {
            Uri uri = new Uri(DefaultRequestUrl);                        
            HttpStringContent stringContent = new HttpStringContent(@"{""query"": ""{stop(id: \""HSL:1173210\"") {name lat lon wheelchairBoarding}}""}", 
                Windows.Storage.Streams.UnicodeEncoding.Utf8, 
                "application/json");
            var response = await _networkClient.PostAsync(uri, stringContent, token:token);
            if(!response.IsSuccessStatusCode)
            {
                string errorResponse = await response?.Content?.ReadAsStringAsync();
                return null;
            }
            string stringResponse = await response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(stringResponse);
            return stringResponse;
        }

        /// <summary>
        /// Returns a travel plan, or null on failure.
        /// </summary>
        /// <param name="details"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ApiPlan> PlanTrip(BasicTripDetails details, CancellationToken token = default(CancellationToken))
        {
            Uri uri = new Uri(DefaultRequestUrl);

            GqlQuery query = new GqlQuery("plan")
                .WithParameters(new GqlParameter("from", new { lat = details.FromPlaceCoords.Lat.ToString(NumberFormatInfo.InvariantInfo), lon = details.FromPlaceCoords.Lon.ToString(NumberFormatInfo.InvariantInfo) }),
                    new GqlParameter("to", new { lat = details.ToPlaceCoordinates.Lat.ToString(NumberFormatInfo.InvariantInfo), lon = details.ToPlaceCoordinates.Lon.ToString(NumberFormatInfo.InvariantInfo) }),
                    new GqlParameter("numItineraries", 5),
                    new GqlParameter("time", $"{details.Time.Hours.ToString(NumberFormatInfo.InvariantInfo)}:{details.Time.Minutes.ToString(NumberFormatInfo.InvariantInfo)}:{details.Time.Seconds.ToString(NumberFormatInfo.InvariantInfo)}"),
                    new GqlParameter("date", $"{details.Date.Year.ToString(NumberFormatInfo.InvariantInfo)}-{details.Date.Month.ToString(NumberFormatInfo.InvariantInfo)}-{details.Date.Day.ToString(NumberFormatInfo.InvariantInfo)}"),
                    new GqlParameter("arriveBy", details.IsTimeTypeArrival)
                )
                .WithReturnValues(
                    new GqlReturnValue("itineraries", 
                        new GqlReturnValue("legs",
                            new GqlReturnValue("startTime"), 
                            new GqlReturnValue("endTime"),
                            new GqlReturnValue("mode"),
                            new GqlReturnValue("duration"),
                            new GqlReturnValue("realTime"),
                            new GqlReturnValue("distance"),
                            new GqlReturnValue("transitLeg"),
                            new GqlReturnValue("realTime")
                        )
                    )
                );
            string parsedQuery = query.ParseToJsonString();
            string requestString =
                "{\"query\": \"{" +
                    $"plan(from: {{lat:{details.FromPlaceCoords.Lat.ToString(NumberFormatInfo.InvariantInfo)}, lon:{details.FromPlaceCoords.Lon.ToString(NumberFormatInfo.InvariantInfo)}}}, " +
                        $"to: {{lat:{details.ToPlaceCoordinates.Lat.ToString(NumberFormatInfo.InvariantInfo)}, lon:{details.ToPlaceCoordinates.Lon.ToString(NumberFormatInfo.InvariantInfo)}}}, " +
                        "numItineraries: 5, " +
                        $"time: \\\"{details.Time.Hours.ToString(NumberFormatInfo.InvariantInfo)}:{details.Time.Minutes.ToString(NumberFormatInfo.InvariantInfo)}:{details.Time.Seconds.ToString(NumberFormatInfo.InvariantInfo)}\\\", " +
                        $"date: \\\"{details.Date.Year.ToString(NumberFormatInfo.InvariantInfo)}-{details.Date.Month.ToString(NumberFormatInfo.InvariantInfo)}-{details.Date.Day.ToString(NumberFormatInfo.InvariantInfo)}\\\", " +
                        $"arriveBy: {details.IsTimeTypeArrival.ToString().ToLowerInvariant()}" +
                        ")" +
                            "{itineraries " +
                                "{legs " +
                                    "{startTime " +
                                    "endTime " +
                                    "mode " +
                                    "duration " +
                                    "realTime " +
                                    "distance " +
                                    "transitLeg " +
                                    "realTime " +
                                "}" +
                            "}" +
                        "}" +
                    "}" +
                "\"}\"";

            HttpStringContent stringContent = CreateJsonStringContent(requestString);
            //todo: this needs to be wrapped in a try/catch with a generic handler, and a way to override/bypass that handler
            var response = await _networkClient.PostAsync(uri, stringContent, token);
            if (!response.IsSuccessStatusCode)
            {
                LogFailure(response).DoNotAwait();
                return null;
            }

            return await UnwrapServerResponse<ApiPlan>(response);
        }

        private HttpStringContent CreateJsonStringContent(string requestString)
        {
            return new HttpStringContent(requestString, UnicodeEncoding.Utf8, "application/json");
        }

        private async Task<T> UnwrapServerResponse<T>(HttpResponseMessage response)
        {            
            // The response comes back from the server wrapped in a "data" JSON object,
            // which itself contains an object which ITSELF contains the actual object we want back.
            // Thus: Data.First.First gets us the JSON object we really care about.
            // todo: this is a bit of bottleneck, because we have to read the entire response into memory
            // and manipulate it before returning it. 
            return (await response.Content.ReadAsInputStreamAsync())
                .AsStreamForRead()
                .DeseriaizeJsonFromStream<ApiDataContainer>()
                .Data.First.First.ToObject<T>();
        }

        private async Task LogFailure(HttpResponseMessage response)
        {
            //todo: add real logging
            if (response.Content != null)
            {
                string errorResponse = await response?.Content?.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(
                    $"GetPlan response failed: Error code: {response.StatusCode}. Response message:\n{errorResponse}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"GetPlan failed: Error code: {response.StatusCode}. Did not receive a response message.");
            }
        }       
    }
}
