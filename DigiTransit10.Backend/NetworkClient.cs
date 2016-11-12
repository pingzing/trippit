using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.Web.Http;
using Windows.Web.Http.Headers;

namespace DigiTransit10.Backend
{
    public interface INetworkClient
    {
        HttpRequestHeaderCollection DefaultHeaders { get; }

        Task<HttpResponseMessage> GetAsync(Uri uri,
            HttpRequestHeaderCollection headers = default(HttpRequestHeaderCollection),
            CancellationToken token = default(CancellationToken));

        Task<HttpResponseMessage> GetAsync(Uri uri,            
            CancellationToken token = default(CancellationToken));

        Task<HttpResponseMessage> PostAsync(Uri uri,
            IHttpContent postContent,
            CancellationToken token = default(CancellationToken));

        Task<HttpResponseMessage> PostAsync(Uri uri,
            IHttpContent postContent,
            HttpRequestHeaderCollection headers = default(HttpRequestHeaderCollection),
            CancellationToken token = default(CancellationToken));

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message,
            HttpRequestHeaderCollection headers = default(HttpRequestHeaderCollection),
            CancellationToken token = default(CancellationToken));

        Task<HttpResponseMessage> SendAsync(HttpRequestMessage message,
            IHttpContent content,
            HttpRequestHeaderCollection headers = default(HttpRequestHeaderCollection),
            CancellationToken token = default(CancellationToken));
    }

    public class NetworkClient : INetworkClient
    {
        readonly HttpClient _client;

        public HttpRequestHeaderCollection DefaultHeaders => _client.DefaultRequestHeaders;

        public NetworkClient()
        {
            //HttpBaseProtocolFilter filter = new HttpBaseProtocolFilter();            //leaving this here as a note for myself in case I need it...
            _client = new HttpClient();
        }

        public async Task<HttpResponseMessage> GetAsync(Uri uri,
            HttpRequestHeaderCollection headers = default(HttpRequestHeaderCollection),
            CancellationToken token = default(CancellationToken))
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, uri);
            return await SendAsync(message, headers, token);
        }

        public async Task<HttpResponseMessage> GetAsync(Uri uri,            
            CancellationToken token = default(CancellationToken))
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpRequestHeaderCollection headers = DefaultHeaders;
            return await SendAsync(message, headers, token);
        }

        public async Task<HttpResponseMessage> PostAsync(Uri uri, IHttpContent postContent, 
            CancellationToken token = new CancellationToken())
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = postContent
            };
            HttpRequestHeaderCollection headers = DefaultHeaders;
            return await SendAsync(message, postContent, headers, token);
        }

        public async Task<HttpResponseMessage> PostAsync(Uri uri,
            IHttpContent postContent,
            HttpRequestHeaderCollection headers = default(HttpRequestHeaderCollection),
            CancellationToken token = default(CancellationToken))
        {
            HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, uri)
            {
                Content = postContent
            };
            return await SendAsync(message, postContent, headers, token);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message,
            HttpRequestHeaderCollection headers = default(HttpRequestHeaderCollection),
            CancellationToken token = default(CancellationToken))
        {
            return await _client.SendRequestAsync(message, HttpCompletionOption.ResponseHeadersRead).AsTask(token);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage message, 
            IHttpContent content, 
            HttpRequestHeaderCollection headers = null,
            CancellationToken token = default(CancellationToken))
        {
            return await _client.SendRequestAsync(message, HttpCompletionOption.ResponseHeadersRead).AsTask(token);
        }
    }
}
