using System;
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
using static DigiTransit10.Helpers.Enums;
using System.Linq;
using MetroLog;
using Windows.Devices.Geolocation;

namespace DigiTransit10.Services
{
    public interface INetworkService
    {
        string DefaultGqlRequestUrl { get; }
        string DefaultGeocodingRequestUrl { get; }

        Task<ApiResult<GeocodingResponse>> SearchAddressAsync(string searchString, CancellationToken token = default(CancellationToken));

        Task<ApiResult<List<ApiStop>>> GetStopsAsync(string searchString, CancellationToken token = default(CancellationToken));
        Task<ApiResult<ApiPlan>> PlanTripAsync(TripQueryDetails details, CancellationToken token = default(CancellationToken));
        Task<ApiResult<IEnumerable<ApiRoute>>> GetLinesAsync(string searchString, CancellationToken token = default(CancellationToken));
        Task<ApiResult<IEnumerable<ApiStop>>> GetStopsByBoundingBox(GeoboundingBox boundingBox, CancellationToken token = default(CancellationToken));
    }

    public class NetworkService : INetworkService
    {
        private readonly Backend.INetworkClient _networkClient;
        private readonly SettingsService _settingsService;
        private readonly ILogger _logger;

        private HttpRequestHeaderCollection _defaultHeaders = null;

        public string DefaultGqlRequestUrl { get; } = "https://api.digitransit.fi/routing/v1/routers/hsl/index/graphql";
        public string DefaultGeocodingRequestUrl { get; } = "https://api.digitransit.fi/geocoding/v1/";

        public NetworkService(Backend.INetworkClient networkClient, SettingsService settingsService, ILogger logger)
        {
            _networkClient = networkClient;
            _settingsService = settingsService;
            _defaultHeaders = _networkClient.DefaultHeaders;
            _logger = logger;
        }

        //---GEOCODING REQUESTS---

        public async Task<ApiResult<GeocodingResponse>> SearchAddressAsync(string searchString, CancellationToken token = default(CancellationToken))
        {
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

            try
            {
                var response = await _networkClient.GetAsync(uri, token);

                if (response == null || !response.IsSuccessStatusCode)
                {
                    LogHttpFailure(response).DoNotAwait();
                    return ApiResult<GeocodingResponse>.Fail;
                }

                GeocodingResponse geoResponse = (await response.Content.ReadAsInputStreamAsync())
                    .AsStreamForRead()
                    .DeseriaizeJsonFromStream<GeocodingResponse>();

                return new ApiResult<GeocodingResponse>(geoResponse);
            }
            catch(Exception ex) when (ex is COMException || ex is HttpRequestException || ex is OperationCanceledException)
            {
                if (ex is OperationCanceledException)
                {
                    return ApiResult<GeocodingResponse>.FailWithReason(FailureReason.Canceled);
                }
                else
                {
                    LogException(ex);
                    return ApiResult<GeocodingResponse>.FailWithReason(FailureReason.NoConnection);
                }
            }
        }

        //---GRAPHQL REQUESTS---

        public async Task<ApiResult<List<ApiStop>>> GetStopsAsync(string searchString, CancellationToken token = default(CancellationToken))
        {
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
                        new GqlReturnValue(ApiGqlMembers.mode)
                    )
                );

            string parsedQuery = query.ParseToJsonString();

            HttpStringContent stringContent = CreateJsonStringContent(parsedQuery);
            try
            {
                var response = await _networkClient.PostAsync(uri, stringContent, token);
                if (response == null || !response.IsSuccessStatusCode)
                {
                    LogHttpFailure(response).DoNotAwait();
                    return ApiResult<List<ApiStop>>.Fail;
                }

                var result = await UnwrapGqlResposne<List<ApiStop>>(response);

                if (result.Count == 0)
                {
                    LogLogicFailure(FailureReason.NoResults);
                    return ApiResult<List<ApiStop>>.FailWithReason(FailureReason.NoResults);
                }

                return new ApiResult<List<ApiStop>>(result);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is COMException || ex is OperationCanceledException)
            {
                if (ex is OperationCanceledException)
                {
                    return ApiResult<List<ApiStop>>.FailWithReason(FailureReason.Canceled);
                }
                else
                {
                    LogException(ex);
                    return ApiResult<List<ApiStop>>.FailWithReason(FailureReason.NoConnection);
                }
            }
        }

        /// <summary>
        /// Returns a travel plan.
        /// </summary>
        /// <param name="details"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ApiResult<ApiPlan>> PlanTripAsync(TripQueryDetails details, CancellationToken token = default(CancellationToken))
        {
            Uri uri = new Uri(DefaultGqlRequestUrl);

            GqlQuery query = new GqlQuery(ApiGqlMembers.plan)
                .WithParameters(
                    new GqlParameter(ApiGqlMembers.from, new GqlTuple { { ApiGqlMembers.lat, details.FromPlaceCoords.Lat}, { ApiGqlMembers.lon, details.FromPlaceCoords.Lon } }),
                    new GqlParameter(ApiGqlMembers.intermediatePlaces, new GqlParameterArray(details.IntermediateCoords.Select(x => new GqlTuple { { ApiGqlMembers.lat, x.Lat }, { ApiGqlMembers.lon, x.Lon } }))),
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
                            new GqlReturnValue(ApiGqlMembers.legGeometry,
                                new GqlReturnValue(ApiGqlMembers.length),
                                new GqlReturnValue(ApiGqlMembers.points)
                            ),
                            new GqlReturnValue(ApiGqlMembers.intermediateStops,
                                new GqlReturnValue(ApiGqlMembers.name),
                                new GqlReturnValue(ApiGqlMembers.lat),
                                new GqlReturnValue(ApiGqlMembers.lon)
                            ),
                            new GqlReturnValue(ApiGqlMembers.from,
                                new GqlReturnValue(ApiGqlMembers.name),
                                new GqlReturnValue(ApiGqlMembers.lat),
                                new GqlReturnValue(ApiGqlMembers.lon)
                            ),
                            new GqlReturnValue(ApiGqlMembers.to,
                                new GqlReturnValue(ApiGqlMembers.name),
                                new GqlReturnValue(ApiGqlMembers.lat),
                                new GqlReturnValue(ApiGqlMembers.lon)
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
                if (response == null || !response.IsSuccessStatusCode)
                {
                    LogHttpFailure(response).DoNotAwait();
                    return ApiResult<ApiPlan>.Fail;
                }

                var result = await UnwrapGqlResposne<ApiPlan>(response);

                if (result.Itineraries.Count == 0)
                {
                    LogLogicFailure(FailureReason.NoResults);
                    return ApiResult<ApiPlan>.FailWithReason(FailureReason.NoResults);
                }

                return new ApiResult<ApiPlan>(result);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is COMException || ex is OperationCanceledException)
            {
                if (ex is OperationCanceledException)
                {
                    return ApiResult<ApiPlan>.FailWithReason(FailureReason.Canceled);
                }
                else
                {
                    LogException(ex);
                    return ApiResult<ApiPlan>.FailWithReason(FailureReason.NoConnection);
                }
            }
        }

        public async Task<ApiResult<IEnumerable<ApiRoute>>> GetLinesAsync(string searchString, CancellationToken token = default(CancellationToken))
        {
            GqlQuery query = new GqlQuery(ApiGqlMembers.routes)
                .WithParameters(new GqlParameter(ApiGqlMembers.name, searchString))
                .WithReturnValues(
                    new GqlReturnValue(ApiGqlMembers.shortName),
                    new GqlReturnValue(ApiGqlMembers.longName),
                    new GqlReturnValue(ApiGqlMembers.mode),
                    new GqlReturnValue(ApiGqlMembers.patterns,
                        new GqlReturnValue(ApiGqlMembers.stops,
                            new GqlReturnValue(ApiGqlMembers.name),
                            new GqlReturnValue(ApiGqlMembers.lat),
                            new GqlReturnValue(ApiGqlMembers.lon)
                        ),
                        new GqlReturnValue(ApiGqlMembers.geometry,
                            new GqlReturnValue(ApiGqlMembers.lat),
                            new GqlReturnValue(ApiGqlMembers.lon)
                        )
                    )
                );

            string parsedQuery = query.ParseToJsonString();
            HttpStringContent stringContent = CreateJsonStringContent(parsedQuery);
            Uri uri = new Uri(DefaultGqlRequestUrl);

            try
            {
                var response = await _networkClient.PostAsync(uri, stringContent, token);
                if (response == null || !response.IsSuccessStatusCode)
                {
                    LogHttpFailure(response).DoNotAwait();
                    return ApiResult<IEnumerable<ApiRoute>>.Fail;
                }

                IEnumerable<ApiRoute> result = await UnwrapGqlResposne<IEnumerable<ApiRoute>>(response);

                if (!result.Any())
                {
                    LogLogicFailure(FailureReason.NoResults);
                    return ApiResult<IEnumerable<ApiRoute>>.FailWithReason(FailureReason.NoResults);
                }

                return new ApiResult<IEnumerable<ApiRoute>>(result);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is COMException || ex is OperationCanceledException)
            {
                if (ex is OperationCanceledException)
                {
                    return ApiResult<IEnumerable<ApiRoute>>.FailWithReason(FailureReason.Canceled);
                }
                else
                {
                    LogException(ex);
                    return ApiResult<IEnumerable<ApiRoute>>.FailWithReason(FailureReason.NoConnection);
                }
            }
        }

        public async Task<ApiResult<IEnumerable<ApiStop>>> GetStopsByBoundingBox(GeoboundingBox boundingBox,
            CancellationToken token = default(CancellationToken))
        {
            GqlQuery query = new GqlQuery(ApiGqlMembers.stopsByBbox)
                .WithParameters(
                    new GqlParameter(ApiGqlMembers.minLat, boundingBox.NorthwestCorner.Latitude),
                    new GqlParameter(ApiGqlMembers.minLon, boundingBox.NorthwestCorner.Longitude),
                    new GqlParameter(ApiGqlMembers.maxLat, boundingBox.SoutheastCorner.Latitude),
                    new GqlParameter(ApiGqlMembers.maxLon, boundingBox.SoutheastCorner.Longitude)
                )
                .WithReturnValues(
                    new GqlReturnValue(ApiGqlMembers.name),
                    new GqlReturnValue(ApiGqlMembers.code),
                    new GqlReturnValue(ApiGqlMembers.lat),
                    new GqlReturnValue(ApiGqlMembers.lon),
                    new GqlReturnValue(ApiGqlMembers.patterns,
                        new GqlReturnValue(ApiGqlMembers.name),
                        new GqlReturnValue(ApiGqlMembers.route,
                            new GqlReturnValue(ApiGqlMembers.shortName),
                            new GqlReturnValue(ApiGqlMembers.longName)
                        )
                    )
                );

            string parsedQuery = query.ParseToJsonString();
            HttpStringContent stringContent = CreateJsonStringContent(parsedQuery);
            Uri uri = new Uri(DefaultGqlRequestUrl);

            try
            {
                var response = await _networkClient.PostAsync(uri, stringContent, token);
                if (response == null || !response.IsSuccessStatusCode)
                {
                    LogHttpFailure(response).DoNotAwait();
                    return ApiResult<IEnumerable<ApiStop>>.Fail;
                }

                IEnumerable<ApiStop> result = await UnwrapGqlResposne<IEnumerable<ApiStop>>(response);

                if (!result.Any())
                {
                    LogLogicFailure(FailureReason.NoResults);
                    return ApiResult<IEnumerable<ApiStop>>.FailWithReason(FailureReason.NoResults);
                }

                return new ApiResult<IEnumerable<ApiStop>>(result);
            }
            catch (Exception ex) when (ex is HttpRequestException || ex is COMException || ex is OperationCanceledException)
            {
                if (ex is OperationCanceledException)
                {
                    return ApiResult<IEnumerable<ApiStop>>.FailWithReason(FailureReason.Canceled);
                }
                else
                {
                    LogException(ex);
                    return ApiResult<IEnumerable<ApiStop>>.FailWithReason(FailureReason.NoConnection);
                }
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

        private async Task LogHttpFailure(HttpResponseMessage response, [CallerMemberName] string callerMethod = "Unknown Method")
        {
            if (response.Content != null)
            {
                string errorResponse = await response.Content?.ReadAsStringAsync();
                _logger.Error(
                    $"{callerMethod} call failed. Response failed: Error code: {response.StatusCode}. Response message:\n{errorResponse}");
            }
            else
            {
                _logger.Error($"{callerMethod} call failed: Error code: {response.StatusCode}. Did not receive a response message.");
            }
        }

        private void LogLogicFailure(FailureReason reason, [CallerMemberName]string callerMethod = "Unknown method()")
        {
            _logger.Error($"{callerMethod} call failed. Reason: {reason}.");
        }

        private void LogException(Exception ex, [CallerMemberName]string caller = "Unknown")
        {
            _logger.Error($"{caller} threw exception: ", ex);
        }
    }
}
