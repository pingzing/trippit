﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using DigiTransit10.ExtensionMethods;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Services.SettingsServices;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;
using DigiTransit10.GraphQL;
using DigiTransit10.Helpers;
using DigiTransit10.Models.Geocoding;
using static DigiTransit10.Models.ModelEnums;
using HttpResponseMessage = Windows.Web.Http.HttpResponseMessage;

namespace DigiTransit10.Services
{
    public interface INetworkService
    {
        string DefaultGqlRequestUrl { get; }
        string DefaultGeocodingRequestUrl { get; }

        Task<ApiResult<GeocodingResponse>> SearchAddress(string searchString, CancellationToken token = default(CancellationToken));

        Task<ApiResult<List<ApiStop>>> GetStops(string searchString, CancellationToken token = default(CancellationToken));
        Task<ApiResult<ApiPlan>> PlanTrip(BasicTripDetails details, CancellationToken token = default(CancellationToken));
    }

    public class NetworkService : INetworkService
    {
        private readonly Backend.INetworkClient _networkClient;
        private readonly SettingsService _settingsService;
        private HttpRequestHeaderCollection _defaultHeaders = null;

        public string DefaultGqlRequestUrl { get; } = "https://api.digitransit.fi/routing/v1/routers/hsl/index/graphql";
        public string DefaultGeocodingRequestUrl { get; } = "https://api.digitransit.fi/geocoding/v1/";

        public NetworkService(Backend.INetworkClient networkClient, SettingsService settingsService)
        {
            _networkClient = networkClient;
            _settingsService = settingsService;
            _defaultHeaders = _networkClient.DefaultHeaders;
        }

        //---GEOCODING REQUESTS---

        public async Task<ApiResult<GeocodingResponse>> SearchAddress(string searchString, CancellationToken token = default(CancellationToken))
        {
            searchString = WebUtility.UrlEncode(searchString);
            string urlString = $"{DefaultGeocodingRequestUrl}" +
                $"search?text={searchString}" +
                $"&boundary.rect.min_lat={GeocodingConstants.BoundaryRectMinLat}" +
                $"&boundary.rect.max_lat={GeocodingConstants.BoundaryRectMaxLat}" +
                $"&boundary.rect.min_lon={GeocodingConstants.BoundaryRectMinLon}" +
                $"&boundary.rect.max_lon={GeocodingConstants.BoundaryRectMaxLon}" +
                $"&focus.point.lat={GeocodingConstants.FocusPointLat}" +
                $"&focus.point.lon={GeocodingConstants.FocusPointLon}" +
                $"&lang={_settingsService.CurrentLanguage.Substring(0, 2)}";
            Uri uri = new Uri(urlString);

            var response = await _networkClient.GetAsync(uri, token);

            if (!response.IsSuccessStatusCode)
            {
                LogHttpFailure(response).DoNotAwait();
                return ApiResult<GeocodingResponse>.Fail;
            }

            GeocodingResponse geoResponse = (await response.Content.ReadAsInputStreamAsync())
                .AsStreamForRead()
                .DeseriaizeJsonFromStream<GeocodingResponse>();

            return new ApiResult<GeocodingResponse>(geoResponse);
        }

        //---GRAPHQL REQUESTS---

        public async Task<ApiResult<List<ApiStop>>> GetStops(string searchString, CancellationToken token = default(CancellationToken))
        {
            searchString = WebUtility.UrlEncode(searchString);
            Uri uri = new Uri(DefaultGqlRequestUrl);

            GqlQuery query = new GqlQuery(ApiGqlMembers.stops)
                .WithParameters(new GqlParameter(ApiGqlMembers.name, searchString))
                .WithReturnValues(
                    new GqlReturnValue(ApiGqlMembers.id),
                    new GqlReturnValue(ApiGqlMembers.gtfsId),
                    new GqlReturnValue(ApiGqlMembers.lat),
                    new GqlReturnValue(ApiGqlMembers.lon),
                    new GqlReturnValue(ApiGqlMembers.name),
                    new GqlReturnValue(ApiGqlMembers.code),
                    new GqlReturnValue(ApiGqlMembers.routes,
                        new GqlReturnValue(ApiGqlMembers.type)
                    )
                );

            string parsedQuery = query.ParseToJsonString();

            HttpStringContent stringContent = CreateJsonStringContent(parsedQuery);
            try
            {
                var response = await _networkClient.PostAsync(uri, stringContent, token);
                if (!response.IsSuccessStatusCode)
                {
                    LogHttpFailure(response).DoNotAwait();
                    return ApiResult<List<ApiStop>>.Fail;
                }

                var result = await UnwrapGqlResposne<List<ApiStop>>(response);

                if (result.Count == 0)
                {
                    LogLogicFailure(ApiFailureReason.NoResults);
                    return ApiResult<List<ApiStop>>.FailWithReason(ApiFailureReason.NoResults);
                }

                return new ApiResult<List<ApiStop>>(result);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is COMException)
            {
                LogException(ex);
                return ApiResult<List<ApiStop>>.FailWithReason(ApiFailureReason.NoConnection);
            }
        }        

        /// <summary>
        /// Returns a travel plan.
        /// </summary>
        /// <param name="details"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ApiResult<ApiPlan>> PlanTrip(BasicTripDetails details, CancellationToken token = default(CancellationToken))
        {
            Uri uri = new Uri(DefaultGqlRequestUrl);

            GqlQuery query = new GqlQuery(ApiGqlMembers.plan)
                .WithParameters(
                    new GqlParameter(ApiGqlMembers.from, new GqlTuple { { ApiGqlMembers.lat, details.FromPlaceCoords.Lat}, { ApiGqlMembers.lon, details.FromPlaceCoords.Lon } }),
                    new GqlParameter(ApiGqlMembers.to, new GqlTuple { { ApiGqlMembers.lat, details.ToPlaceCoordinates.Lat}, { ApiGqlMembers.lon, details.ToPlaceCoordinates.Lon} }),
                    new GqlParameter(ApiGqlMembers.numItineraries, 5),
                    new GqlParameter(ApiGqlMembers.time, details.Time),
                    new GqlParameter(ApiGqlMembers.date, details.Date),
                    new GqlParameter(ApiGqlMembers.arriveBy, details.IsTimeTypeArrival),
                    new GqlParameter(ApiGqlMembers.modes, details.TransitModes)
                )
                .WithReturnValues(
                    new GqlReturnValue(ApiGqlMembers.from,
                        new GqlReturnValue(ApiGqlMembers.name)
                    ),
                    new GqlReturnValue(ApiGqlMembers.to,
                        new GqlReturnValue(ApiGqlMembers.name)
                    ),
                    new GqlReturnValue(ApiGqlMembers.itineraries,                        
                        new GqlReturnValue(ApiGqlMembers.legs,
                            new GqlReturnValue(ApiGqlMembers.startTime), 
                            new GqlReturnValue(ApiGqlMembers.endTime),
                            new GqlReturnValue(ApiGqlMembers.mode),
                            new GqlReturnValue(ApiGqlMembers.duration),
                            new GqlReturnValue(ApiGqlMembers.realTime),
                            new GqlReturnValue(ApiGqlMembers.distance),
                            new GqlReturnValue(ApiGqlMembers.transitLeg),
                            new GqlReturnValue(ApiGqlMembers.intermediateStops,
                                new GqlReturnValue(ApiGqlMembers.name),
                                new GqlReturnValue(ApiGqlMembers.lat),
                                new GqlReturnValue(ApiGqlMembers.lon)                                
                            ),
                            new GqlReturnValue(ApiGqlMembers.from,
                                new GqlReturnValue(ApiGqlMembers.name)
                            ),
                            new GqlReturnValue(ApiGqlMembers.to,
                                new GqlReturnValue(ApiGqlMembers.name)
                            ),
                            new GqlReturnValue(ApiGqlMembers.route,
                                new GqlReturnValue(ApiGqlMembers.shortName)
                            )
                        ),                    
                    new GqlReturnValue(ApiGqlMembers.fares,
                            new GqlReturnValue(ApiGqlMembers.type),
                            new GqlReturnValue(ApiGqlMembers.currency),
                            new GqlReturnValue(ApiGqlMembers.cents)
                        )
                    )
                );
            string parsedQuery = query.ParseToJsonString();           

            HttpStringContent stringContent = CreateJsonStringContent(parsedQuery);

            try
            {
                var response = await _networkClient.PostAsync(uri, stringContent, token);
                if (!response.IsSuccessStatusCode)
                {
                    LogHttpFailure(response).DoNotAwait();
                    return ApiResult<ApiPlan>.Fail;
                }

                var result = await UnwrapGqlResposne<ApiPlan>(response);

                if (result.Itineraries.Count == 0)
                {
                    LogLogicFailure(ApiFailureReason.NoResults);
                    return ApiResult<ApiPlan>.FailWithReason(ApiFailureReason.NoResults);
                }

                return new ApiResult<ApiPlan>(result);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is COMException)
            {
                LogException(ex);                
                return ApiResult<ApiPlan>.FailWithReason(ApiFailureReason.NoConnection);
            }
        }

        private HttpStringContent CreateJsonStringContent(string requestString)
        {
            return new HttpStringContent(requestString, UnicodeEncoding.Utf8, "application/json");
        }

        private async Task<T> UnwrapGqlResposne<T>(HttpResponseMessage response)
        {                        
            return (await response.Content.ReadAsInputStreamAsync())
                .AsStreamForRead()
                .DeseriaizeJsonFromStream<ApiDataContainer>()
                .Data.First.First.ToObject<T>();
        }

        private async Task LogHttpFailure(HttpResponseMessage response, [CallerMemberName] string callerMethod = "Unknown Method()")
        {
            //todo: add real logging
            if (response.Content != null)
            {
                string errorResponse = await response?.Content?.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine(
                    $"{callerMethod} call failed. Response failed: Error code: {response.StatusCode}. Response message:\n{errorResponse}");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine($"{callerMethod} call failed: Error code: {response.StatusCode}. Did not receive a response message.");
            }
        }

        private void LogLogicFailure(ApiFailureReason reason, [CallerMemberName]string callerMethod = "Unknown method()")
        {
            //todo: add real logging
            System.Diagnostics.Debug.WriteLine($"{callerMethod} call failed. Reason: {reason}.");
        }

        private void LogException(Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
        }
    }
}
