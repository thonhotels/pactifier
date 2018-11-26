using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Pactifier.Core
{
    public interface IHttpClientBase
    {
        Uri BaseAddress { get; set; }
        TimeSpan Timeout { get; set; }
        Task<HttpResponseMessage> GetAsync(string requestUri);
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
        Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request);
        void SetBearerToken(string token);
    } 
}