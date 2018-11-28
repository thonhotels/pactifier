using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Pactifier.Core.Comparers;

namespace Pactifier.Core
{
    public class FakeHttpHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public static Action Verify(FakeHttpHandler handler, ProviderServiceRequest r, RequestComparer comparer)
        {
            return () => A.CallTo(() => handler.SendAsync(A<HttpRequestMessage>.That.Matches(actual => comparer.Execute(r, actual)), A<CancellationToken>._))
                .MustHaveHappenedOnceExactly();
        }

        internal void ConfigureResponse(HttpResponseMessage httpResponseMessage)
        {
            A.CallTo(() => SendAsync(A<HttpRequestMessage>._, A<CancellationToken>._)).Returns(Task.FromResult(httpResponseMessage));
        }
    }
    
    public static class HttpClientExtensions
    {
        public static void SetBearerToken(this HttpClient client, string token)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
    }
}