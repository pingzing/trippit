using DigiTransit10.ExtensionMethods;
using DigiTransit10.GraphQL;
using DigiTransit10.Helpers;
using DigiTransit10.Models;
using DigiTransit10.Models.ApiModels;
using DigiTransit10.Models.Geocoding;
using DigiTransit10.Services.SettingsServices;
using MetroLog;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Web.Http;
using Windows.Web.Http.Headers;
using static DigiTransit10.Helpers.Enums;
using HttpResponseMessage = Windows.Web.Http.HttpResponseMessage;
using UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding;

namespace DigiTransit10.Services
{
    public interface INetworkService
    {
        string DefaultGqlRequestUrl { get; }
        string DefaultGeocodingRequestUrl { get; }

        Task<ApiResult<GeocodingResponse>> SearchAddressAsync(string searchString, CancellationToken token = default(CancellationToken));

        Task<ApiResult<IEnumerable<TransitStop>>> GetStopsAsync(string searchString, CancellationToken token = default(CancellationToken));
        Task<ApiResult<TripPlan>> PlanTripAsync(TripQueryDetails details, CancellationToken token = default(CancellationToken));
        Task<ApiResult<IEnumerable<TransitLine>>> GetLinesAsync(string searchString, CancellationToken token = default(CancellationToken));
        Task<ApiResult<IEnumerable<TransitLine>>> GetLinesAsync(IEnumerable<string> gtfsIds, CancellationToken token = default(CancellationToken));
        Task<ApiResult<IEnumerable<TransitStop>>> GetStopsByBoundingRadius(float lat, float lon, int radiusMeters, CancellationToken token = default(CancellationToken));        
        Task<ApiResult<TransitStopDetails>> GetStopDetails(string stopId, DateTime forDate, CancellationToken token = default(CancellationToken));
        Task<ApiResult<IEnumerable<TransitTrafficAlert>>> GetTrafficAlertsAsync(CancellationToken token = default(CancellationToken));
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
                    if (response?.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        return ApiResult<GeocodingResponse>.FailWithReason(FailureReason.ServerDown);
                    }
                    else
                    {
                        return ApiResult<GeocodingResponse>.Fail;
                    }
                }

                GeocodingResponse geoResponse = (await response.Content.ReadAsInputStreamAsync())
                    .AsStreamForRead()
                    .DeseriaizeJsonFromStream<GeocodingResponse>();

                if (!geoResponse.Features.Any())
                {
                    return ApiResult<GeocodingResponse>.FailWithReason(FailureReason.NoResults);
                }

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

        public async Task<ApiResult<IEnumerable<TransitStop>>> GetStopsAsync(string searchString, CancellationToken token = default(CancellationToken))
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
            var response = await GetGraphQLAsync<List<ApiStop>>(query);
            if(!response.HasResult)
            {
                return ApiResult<IEnumerable<TransitStop>>.FailWithReason(response.Failure.Reason);
            }
            if(response.HasResult && !response.Result.Any())
            {
                LogLogicFailure(FailureReason.NoResults);
                return ApiResult<IEnumerable<TransitStop>>.FailWithReason(FailureReason.NoResults);
            }

            return new ApiResult<IEnumerable<TransitStop>>(response.Result.Select(x => new TransitStop
            {
                Name = x.Name,
                Code = x.Code,
                Coords = BasicGeopositionExtensions.Create(0.0, x.Lon, x.Lat),
                GtfsId = x.GtfsId,
                Id = Guid.NewGuid()
            }));

        }

        /// <summary>
        /// Returns a travel plan.
        /// </summary>
        /// <param name="details"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ApiResult<TripPlan>> PlanTripAsync(TripQueryDetails details, CancellationToken token = default(CancellationToken))
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
                    new GqlParameter(ApiGqlMembers.modes, details.TransitModes),
                    new GqlParameter(ApiGqlMembers.walkSpeed, details.WalkSpeed.UnderlyingMetersPerSecond),
                    new GqlParameter(ApiGqlMembers.walkReluctance, details.WalkAmount.UnderlyingWalkReluctance)
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
                                new GqlReturnValue(ApiGqlMembers.gtfsId),
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

            var response = await GetGraphQLAsync<ApiPlan>(query, token);
            if (!response.HasResult)
            {
                return ApiResult<TripPlan>.FailWithReason(response.Failure.Reason);
            }
            if (response.HasResult && response.Result?.Itineraries.Any() != true)
            {
                LogLogicFailure(FailureReason.NoResults);
                return ApiResult<TripPlan>.FailWithReason(FailureReason.NoResults);
            }

            return new ApiResult<TripPlan>(new TripPlan(response.Result, details.FromPlaceString, details.ToPlaceString));
        }

        public async Task<ApiResult<IEnumerable<TransitLine>>> GetLinesAsync(string searchString, CancellationToken token = default(CancellationToken))
        {
            GqlQuery query = new GqlQuery(ApiGqlMembers.routes)
                .WithParameters(new GqlParameter(ApiGqlMembers.name, searchString))
                .WithReturnValues(
                    new GqlReturnValue(ApiGqlMembers.shortName),
                    new GqlReturnValue(ApiGqlMembers.longName),
                    new GqlReturnValue(ApiGqlMembers.mode),
                    new GqlReturnValue(ApiGqlMembers.patterns,
                        new GqlReturnValue(ApiGqlMembers.stops,
                            new GqlReturnValue(ApiGqlMembers.gtfsId),
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

            ApiResult<IEnumerable<ApiRoute>> response = await GetGraphQLAsync<IEnumerable<ApiRoute>>(query, token);
            if(!response.HasResult)
            {
                return ApiResult<IEnumerable<TransitLine>>.FailWithReason(response.Failure.Reason);
            }
            if (response.HasResult && !response.Result.Any())
            {
                LogLogicFailure(FailureReason.NoResults);
                return ApiResult<IEnumerable<TransitLine>>.FailWithReason(FailureReason.NoResults);
            }

            return new ApiResult<IEnumerable<TransitLine>>(response.Result.Select(x => new TransitLine(x)));
        }

        public async Task<ApiResult<IEnumerable<TransitLine>>> GetLinesAsync(IEnumerable<string> gtfsIds, CancellationToken token = default(CancellationToken))
        {
            GqlQuery query = new GqlQuery(ApiGqlMembers.routes)
                .WithParameters(new GqlParameter(ApiGqlMembers.ids, gtfsIds))
                .WithReturnValues(
                    new GqlReturnValue(ApiGqlMembers.shortName),
                    new GqlReturnValue(ApiGqlMembers.longName),
                    new GqlReturnValue(ApiGqlMembers.mode),
                    new GqlReturnValue(ApiGqlMembers.patterns,
                        new GqlReturnValue(ApiGqlMembers.stops,
                            new GqlReturnValue(ApiGqlMembers.gtfsId),
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

            ApiResult<IEnumerable<ApiRoute>> response = await GetGraphQLAsync<IEnumerable<ApiRoute>>(query, token);
            if (!response.HasResult)
            {
                return ApiResult<IEnumerable<TransitLine>>.FailWithReason(response.Failure.Reason);
            }
            if (response.HasResult && !response.Result.Any())
            {
                LogLogicFailure(FailureReason.NoResults);
                return ApiResult<IEnumerable<TransitLine>>.FailWithReason(FailureReason.NoResults);
            }

            return new ApiResult<IEnumerable<TransitLine>>(response.Result.Select(x => new TransitLine(x)));
        }

        public async Task<ApiResult<IEnumerable<TransitStop>>> GetStopsByBoundingRadius(float lat, float lon, int radiusMeters,
            CancellationToken token = default(CancellationToken))
        {
            GqlQuery query = new GqlQuery(ApiGqlMembers.stopsByRadius)
                .WithParameters(
                    new GqlParameter(ApiGqlMembers.lat, lat),
                    new GqlParameter(ApiGqlMembers.lon, lon),
                    new GqlParameter(ApiGqlMembers.radius, radiusMeters)
                )
                .WithReturnValues(
                    new GqlReturnValue(ApiGqlMembers.edges,
                        new GqlReturnValue(ApiGqlMembers.node,
                            new GqlReturnValue(ApiGqlMembers.stop,
                                new GqlReturnValue(ApiGqlMembers.gtfsId),
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
                            )
                        )
                    )
                );

            var response = await GetGraphQLAsync<ApiStopAtDistanceConnection>(query, token).ConfigureAwait(false);
            if(response.HasResult)
            {
                IEnumerable<TransitStop> stops = response.Result.Edges?.Select(x => new TransitStop
                {
                    GtfsId = x.Node.Stop.GtfsId,
                    Coords = BasicGeopositionExtensions.Create(0.0, x.Node.Stop.Lon, x.Node.Stop.Lat),
                    Code = x.Node.Stop.Code,
                    Name = x.Node.Stop.Name,
                    Id = Guid.NewGuid()
                });
                if(stops == null)
                {
                    LogLogicFailure(FailureReason.Unspecified);
                    return ApiResult<IEnumerable<TransitStop>>.Fail;
                }
                if(!stops.Any())
                {
                    LogLogicFailure(FailureReason.NoResults);
                    return ApiResult<IEnumerable<TransitStop>>.FailWithReason(FailureReason.NoResults);
                }                
                return new ApiResult<IEnumerable<TransitStop>>(stops);
            }
            else
            {
                return ApiResult<IEnumerable<TransitStop>>.FailWithReason(FailureReason.Unspecified);
            }
        }

        public async Task<ApiResult<TransitStopDetails>> GetStopDetails(string stopId, DateTime forDate, CancellationToken token = default(CancellationToken))
        {
            GqlQuery query = new GqlQuery(ApiGqlMembers.stop)
                .WithParameters(
                    new GqlParameter(ApiGqlMembers.id, stopId)
                )
                .WithReturnValues(
                    new GqlReturnValue(ApiGqlMembers.gtfsId),
                    new GqlReturnValue(ApiGqlMembers.name),
                    new GqlInlineMethodReturnValue(ApiGqlMembers.stoptimesForServiceDate, new List<GqlParameter> { new GqlParameter(ApiGqlMembers.date, forDate) },
                        new GqlReturnValue(ApiGqlMembers.pattern,
                            new GqlReturnValue(ApiGqlMembers.route,
                                new GqlReturnValue(ApiGqlMembers.gtfsId),
                                new GqlReturnValue(ApiGqlMembers.mode),
                                new GqlReturnValue(ApiGqlMembers.shortName),
                                new GqlReturnValue(ApiGqlMembers.longName)
                            )
                        ),
                        new GqlReturnValue(ApiGqlMembers.stoptimes,
                            new GqlReturnValue(ApiGqlMembers.realtimeState),
                            new GqlReturnValue(ApiGqlMembers.scheduledArrival),
                            new GqlReturnValue(ApiGqlMembers.scheduledDeparture),
                            new GqlReturnValue(ApiGqlMembers.realtimeArrival),
                            new GqlReturnValue(ApiGqlMembers.realtimeDeparture),
                            new GqlReturnValue(ApiGqlMembers.realtime),
                            new GqlReturnValue(ApiGqlMembers.stopHeadsign)
                        )
                    )
                );

            ApiResult<ApiStop> response = await GetGraphQLAsync<ApiStop>(query, token);
            if(!response.HasResult)
            {
                return ApiResult<TransitStopDetails>.FailWithReason(response.Failure.Reason);
            }

            return new ApiResult<TransitStopDetails>(new TransitStopDetails(response.Result, forDate));
        }

        // Some debug stuff for testing out alerts. HSL doesn't publish alerts very often,
        // and also doesn't have a Test Alerts endpoint, so we can just stub out the real call
        // with some pregenerated data.
        //private const string hardcodedAlertsTestResponse = "{\"data\":{\"alerts\":[{\"id\": \"1\", \"alertHeaderTextTranslations\":[],\"alertDescriptionTextTranslations\":[{\"text\":\"Raitiolinja 1A KÃ¤pylÃ¤n suuntaan , myÃ¶hÃ¤ssÃ¤. Syy: tekninen vika. Arvioitu kesto: 17:28 - 17:40.\",\"language\":\"fi\"},{\"text\":\"SpÃ¥rvÃ¤gslinje 1A mot Kottby , fÃ¶rsenad. Orsak: tekniska problem. BerÃ¤knad tid: 17:28 - 17:40.\",\"language\":\"sv\"},{\"text\":\"Tram 1A to KÃ¤pylÃ¤ , delayed. Cause: technical problems. Estimated time: 17:28 - 17:40.\",\"language\":\"en\"}],\"alertUrl\":null,\"effectiveStartDate\":1494944983,\"effectiveEndDate\":1494945600,\"route\":{\"gtfsId\":\"HSL:1001A\"},\"stop\":null},{\"id\": \"2\", \"alertHeaderTextTranslations\":[],\"alertDescriptionTextTranslations\":[{\"text\":\"Helsingin sisÃ¤isen liikenteen linja 67 Rautatientorilta, klo 17:00 peruttu. Syy: tekninen vika.\",\"language\":\"fi\"},{\"text\":\"Helsingfors lokaltrafik, linje 67 frÃ¥n JÃ¤rnvÃ¤gstorget, kl. 17:00 instÃ¤lld. Orsak: tekniska problem.\",\"language\":\"sv\"},{\"text\":\"Helsinki local traffic, line 67 from Rautatientori, 17:00 cancelled.Cause: technical problems.\",\"language\":\"en\"}],\"alertUrl\":null,\"effectiveStartDate\":1494943500,\"effectiveEndDate\":1494946800,\"route\":{\"gtfsId\":\"HSL:1067\"},\"stop\":null}]}}";
        //new ApiResult<IEnumerable<ApiAlert>>(
        //        JsonConvert.DeserializeObject<ApiDataContainer>(hardcodedAlertsTestResponse)
        //        .Data
        //        .First
        //        .First
        //        .ToObject<IEnumerable<ApiAlert>>());
        public async Task<ApiResult<IEnumerable<TransitTrafficAlert>>> GetTrafficAlertsAsync(CancellationToken token = default(CancellationToken))
        {
            GqlQuery query = new GqlQuery(ApiGqlMembers.alerts)
                .WithReturnValues(
                    new GqlReturnValue(ApiGqlMembers.id),
                    new GqlReturnValue(ApiGqlMembers.alertHeaderTextTranslations,
                        new GqlReturnValue(ApiGqlMembers.text),
                        new GqlReturnValue(ApiGqlMembers.language)
                    ),
                    new GqlReturnValue(ApiGqlMembers.alertDescriptionTextTranslations,
                        new GqlReturnValue(ApiGqlMembers.text),
                        new GqlReturnValue(ApiGqlMembers.language)
                    ),
                    new GqlReturnValue(ApiGqlMembers.alertUrl),
                    new GqlReturnValue(ApiGqlMembers.effectiveStartDate),
                    new GqlReturnValue(ApiGqlMembers.effectiveEndDate),
                    new GqlReturnValue(ApiGqlMembers.route,
                        new GqlReturnValue(ApiGqlMembers.gtfsId),
                        new GqlReturnValue(ApiGqlMembers.shortName),
                        new GqlReturnValue(ApiGqlMembers.longName),
                        new GqlReturnValue(ApiGqlMembers.mode)
                    ),
                    new GqlReturnValue(ApiGqlMembers.stop,
                        new GqlReturnValue(ApiGqlMembers.gtfsId)
                    )                    
                );

             

            ApiResult<IEnumerable<ApiAlert>> response = await GetGraphQLAsync<IEnumerable<ApiAlert>>(query, token);

            if (!response.HasResult)
            {
                return ApiResult<IEnumerable<TransitTrafficAlert>>.FailWithReason(response.Failure.Reason);
            }

            return new ApiResult<IEnumerable<TransitTrafficAlert>>(response.Result.Select(x => new TransitTrafficAlert(x, _settingsService.CurrentLanguage)));
        }

        private async Task<ApiResult<T>> GetGraphQLAsync<T>(GqlQuery query, CancellationToken token = default(CancellationToken))
        {
            string parsedQuery = query.ParseToJsonString(DateParsingStrategy.NoSeparators);
            HttpStringContent stringContent = CreateJsonStringContent(parsedQuery);
            Uri uri = new Uri(DefaultGqlRequestUrl);

            try
            {
                HttpResponseMessage response = await _networkClient.PostAsync(uri, stringContent, token).ConfigureAwait(false);
                if(response == null || !response.IsSuccessStatusCode)
                {
                    LogHttpFailure(response).DoNotAwait();

                    if (response?.StatusCode == HttpStatusCode.ServiceUnavailable)
                    {
                        return ApiResult<T>.FailWithReason(FailureReason.ServerDown);
                    }
                    else
                    {
                        return ApiResult<T>.Fail;
                    }
                }

                T result = await UnwrapGqlResposne<T>(response).ConfigureAwait(false);
                return new ApiResult<T>(result);
            }
            catch(Exception ex) when (ex is HttpRequestException || ex is COMException || ex is OperationCanceledException)
            {
                if(ex is OperationCanceledException)
                {
                    return ApiResult<T>.FailWithReason(FailureReason.Canceled);
                }
                else
                {
                    LogException(ex);
                    return ApiResult<T>.FailWithReason(FailureReason.NoConnection);
                }
            }
        }

        private HttpStringContent CreateJsonStringContent(string requestString)
        {
            return new HttpStringContent(requestString, UnicodeEncoding.Utf8, "application/json");
        }

        private async Task<T> UnwrapGqlResposne<T>(HttpResponseMessage response)
        {
            return (await response.Content.ReadAsInputStreamAsync().AsTask().ConfigureAwait(false))
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
