using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using FakeItEasy;
using Pactifier.Core.Comparers;
using Newtonsoft.Json;
using System.Text;

namespace Pactifier.Core
{
    public class Interaction
    {
        [JsonProperty(Order = 1)]
        private string Description { get; set; }

        [JsonProperty(Order = 2)]
        private string ProviderState { get; set; }
        
        [JsonProperty(Order = 3)]
        private ProviderServiceRequest Request { get; set; }

        [JsonProperty(Order = 4)]
        private ProviderServiceResponse Response { get; set; }

        
        private RequestComparer Comparer { get; }

        public Interaction(RequestComparer comparer)
        {
            Comparer = comparer;
        }

        public Interaction WillRespondWith(ProviderServiceResponse response)
        {
            Response = response;
            return this;
        }

        public Interaction With(ProviderServiceRequest request)
        {
            Request = request;
            return this;
        }

        public Interaction Given(string providerState)
        {
            ProviderState = providerState;
            return this;
        }

        public Interaction UponReceiving(string description)
        {
            Description = description;
            return this;
        }
        public (IHttpClientBase, Action) Client()
        {
            var client = A.Fake<IHttpClientBase>();
            client.BaseAddress = new Uri("http://localhost");
            A.CallTo(() => client.SendAsync(A<HttpRequestMessage>._)).Returns(Task.FromResult(CreateResponseMessage()));
            var clientBase = new HttpClientBase(client);
            return (clientBase, Verify(client, Request, Comparer));
        }

        private static Action Verify(IHttpClientBase client, ProviderServiceRequest r, RequestComparer comparer)
        {
            return () => A.CallTo(() => client.SendAsync(A<HttpRequestMessage>.That.Matches(actual => comparer.Execute(r, actual))))
                .MustHaveHappenedOnceExactly();
        }

        private HttpResponseMessage CreateResponseMessage()
        {
            var result = new HttpResponseMessage(Response.Status);
            result.Headers.Clear();
            Response
                .Headers
                .Where(h => h.Key != "Content-Type")
                .ToList()
                .ForEach(kv => result.Headers.Add(kv.Key, kv.Value));

            if (!ReferenceEquals(null, Response.Body))
                result.Content = new StringContent(JsonConvert.SerializeObject(Response.Body), Encoding.UTF8, "application/json");
            return result;
        }
    }
}