using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Pactifier.Core.Comparers;
using System.Linq;
using Newtonsoft.Json;

namespace Pactifier.Core
{
    public class FakeHttpHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Action Verify(FakeHttpHandler handler, ProviderServiceRequest r, RequestComparer comparer, Action<string> output)
        {
            return () => 
                {
                    var requests = new List<HttpRequestMessage>();
                    try
                    {
                        A.CallTo(() => handler.SendAsync(A<HttpRequestMessage>.That.Matches(actual => comparer.Execute(r, actual)), A<CancellationToken>._))
                            .MustHaveHappenedOnceExactly();
                    }
                    catch (ExpectationException)
                    {
                        if (output != null && r != null)
                            DisplayRequest(output, r);
                        throw;
                    }
                };
        }

        internal void ConfigureResponse(HttpResponseMessage httpResponseMessage)
        {
            A.CallTo(() => SendAsync(A<HttpRequestMessage>._, A<CancellationToken>._)).Returns(Task.FromResult(httpResponseMessage));
        }

        private static void DisplayRequest(Action<string> output, ProviderServiceRequest r)
        {
            var message = 
    @"Actual request was:
      Method: {0}
      Path:   {1}
      Query:  {2}
      Body:   {3}
      Headers:{4}";
            output(string.Format(message, 
                r.Method, 
                r.Path, 
                r.Query, 
                r.Body != null ? JsonConvert.SerializeObject(r.Body) : "", 
                HeadersToDisplayString(r.Headers)));
        }

        private static string HeadersToDisplayString(IDictionary<string, string> h)
        {
            if (!h.Any()) return "";
            var result = h
                .ToList()
                .Select(kv => "        " + kv.Key + " : " + kv.Value + Environment.NewLine)
                .Aggregate((a,b) => a + b);
            return Environment.NewLine + result;
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