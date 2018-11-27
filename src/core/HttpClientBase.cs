using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pactifier.Core
{
    public class HttpClientBase : IHttpClientBase
    {        
        public Uri BaseAddress { get => InnerClient.BaseAddress; set => throw new NotImplementedException(); }
        public TimeSpan Timeout { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        private IHttpClientBase InnerClient { get; }
        private string Token { get; set; }

        public HttpClientBase(IHttpClientBase client)
        {
            InnerClient = client;
        }

        public Task<HttpResponseMessage> GetAsync(string requestUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, InnerClient.BaseAddress + requestUri);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, requestUri);
            return SendAsync(request, content);
        }

        public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri(requestUri, UriKind.RelativeOrAbsolute));
            return SendAsync(request, content);
        }

        private Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpContent content)
        {
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
            request.Content = content;
            return SendAsync(request);
        }

        public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request) 
        {
            return InnerClient.SendAsync(request);
        } 

        public void SetBearerToken(string token)
        {
            Token = token;
        }
    }
}