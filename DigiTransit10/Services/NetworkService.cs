using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace DigiTransit10.Services
{
    public interface INetworkService
    {
        Task<string> GetStop(int stopId, CancellationToken token = default(CancellationToken));
    }

    public class NetworkService : INetworkService
    {
        private readonly Backend.INetworkClient _networkClient;
        private HttpRequestHeaderCollection _defaultHeaders = null;

        public NetworkService(Backend.INetworkClient networkClient)
        {
            _networkClient = networkClient;
            _defaultHeaders = _networkClient.DefaultHeaders;
        }

        public async Task<string> GetStop(int stopId, CancellationToken token = default(CancellationToken))
        {
            Uri uri = new Uri("https://api.digitransit.fi/routing/v1/routers/hsl/index/graphql");                        
            HttpStringContent stringContent = new HttpStringContent("{\"query\": \"{stop(id: \\\"HSL:1173210\\\") {name lat lon wheelchairBoarding}}\"}", 
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
    }
}
