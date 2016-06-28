using System;
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
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

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
            string requestString =
                "{\"query\": \"{" +
                    $"plan(from: {{lat:{details.FromPlaceCoords.Lat.ToString(NumberFormatInfo.InvariantInfo)}, lon:{details.FromPlaceCoords.Lon.ToString(NumberFormatInfo.InvariantInfo)}}}, " +
                        $"to: {{lat:{details.ToPlaceCoordinates.Lat.ToString(NumberFormatInfo.InvariantInfo)}, lon:{details.ToPlaceCoordinates.Lon.ToString(NumberFormatInfo.InvariantInfo)}}}, " +
                        "numItineraries: 5, " +
                        $"time: \\\"{details.Time.Hours.ToString(NumberFormatInfo.InvariantInfo)}:{details.Time.Minutes.ToString(NumberFormatInfo.InvariantInfo)}:{details.Time.Seconds.ToString(NumberFormatInfo.InvariantInfo)}\\\", " +
                        $"date: \\\"{details.Date.Year.ToString(NumberFormatInfo.InvariantInfo)}-{details.Date.Month.ToString(NumberFormatInfo.InvariantInfo)}-{details.Date.Day.ToString(NumberFormatInfo.InvariantInfo)}\\\", " +
                        $"arriveBy: {details.IsTimeTypeArrival.ToString().ToLowerInvariant()}" +
                        $")" +
                            "{itineraries " +
                                "{legs " +
                                    "{startTime " +
                                    "endTime " +
                                    "mode " +
                                    "duration " +
                                    "realTime " +
                                    "distance " +
                                    "transitLeg" +
                                "}" +
                            "}" +
                        "}" +
                    "}" +
                "\"}\"";
            HttpStringContent stringContent = CreateJsonStringContent(requestString);
            var response = await _networkClient.PostAsync(uri, stringContent, token);
            if (!response.IsSuccessStatusCode)
            {
                LogFailure(response).DoNotAwait();
                return null;
            }
            return (await response.Content.ReadAsInputStreamAsync())
                .AsStreamForRead()
                .DeseriaizeJsonFromStream<ApiPlan>();
        }

        private HttpStringContent CreateJsonStringContent(string requestString)
        {
            return new HttpStringContent(requestString, UnicodeEncoding.Utf8, "application/json");
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
