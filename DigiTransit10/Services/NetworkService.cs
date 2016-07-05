using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
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
using DigiTransit10.Models.Geocoding;

namespace DigiTransit10.Services
{
    public interface INetworkService
    {
        string DefaultGqlRequestUrl { get; }
        string DefaultGeocodingRequestUrl { get; }

        Task<GeocodingResponse> SearchAddress(string searchString, CancellationToken token = default(CancellationToken));

        Task<List<ApiStop>> GetStops(string searchString, CancellationToken token = default(CancellationToken));
        Task<ApiPlan> PlanTrip(BasicTripDetails details, CancellationToken token = default(CancellationToken));
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

        public async Task<GeocodingResponse> SearchAddress(string searchString, CancellationToken token = default(CancellationToken))
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
                LogFailure(response).DoNotAwait();
                return null;
            }

            GeocodingResponse geoResponse = (await response.Content.ReadAsInputStreamAsync())
                .AsStreamForRead()
                .DeseriaizeJsonFromStream<GeocodingResponse>();

            return geoResponse;
        }

        //---GRAPHQL REQUESTS---

        public async Task<List<ApiStop>> GetStops(string searchString, CancellationToken token = default(CancellationToken))
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
            var response = await _networkClient.PostAsync(uri, stringContent, token);
            if(!response.IsSuccessStatusCode)
            {
                LogFailure(response).DoNotAwait();
                return null;
            }

            return await UnwrapGqlResposne<List<ApiStop>>(response);
        }        

        /// <summary>
        /// Returns a travel plan, or null on failure.
        /// </summary>
        /// <param name="details"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<ApiPlan> PlanTrip(BasicTripDetails details, CancellationToken token = default(CancellationToken))
        {
            Uri uri = new Uri(DefaultGqlRequestUrl);

            GqlQuery query = new GqlQuery(ApiGqlMembers.plan)
                .WithParameters(
                    new GqlParameter(ApiGqlMembers.from, new GqlTuple { { ApiGqlMembers.lat, details.FromPlaceCoords.Lat}, { ApiGqlMembers.lon, details.FromPlaceCoords.Lon } }),
                    new GqlParameter(ApiGqlMembers.to, new GqlTuple { { ApiGqlMembers.lat, details.ToPlaceCoordinates.Lat}, { ApiGqlMembers.lon, details.ToPlaceCoordinates.Lon} }),
                    new GqlParameter(ApiGqlMembers.numItineraries, 5),
                    new GqlParameter(ApiGqlMembers.time, details.Time),
                    new GqlParameter(ApiGqlMembers.date, details.Date),
                    new GqlParameter(ApiGqlMembers.arriveBy, details.IsTimeTypeArrival)
                )
                .WithReturnValues(
                    new GqlReturnValue(ApiGqlMembers.itineraries, 
                        new GqlReturnValue(ApiGqlMembers.legs,
                            new GqlReturnValue(ApiGqlMembers.startTime), 
                            new GqlReturnValue(ApiGqlMembers.endTime),
                            new GqlReturnValue(ApiGqlMembers.mode),
                            new GqlReturnValue(ApiGqlMembers.duration),
                            new GqlReturnValue(ApiGqlMembers.realTime),
                            new GqlReturnValue(ApiGqlMembers.distance),
                            new GqlReturnValue(ApiGqlMembers.transitLeg)
                        )
                    )
                );
            string parsedQuery = query.ParseToJsonString();           

            HttpStringContent stringContent = CreateJsonStringContent(parsedQuery);
            //todo: this needs to be wrapped in a try/catch with a generic handler, and a way to override/bypass that handler
            var response = await _networkClient.PostAsync(uri, stringContent, token);
            if (!response.IsSuccessStatusCode)
            {
                LogFailure(response).DoNotAwait();
                return null;
            }

            return await UnwrapGqlResposne<ApiPlan>(response);
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
