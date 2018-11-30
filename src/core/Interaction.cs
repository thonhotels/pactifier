using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using FakeItEasy;
using Pactifier.Core.Comparers;
using Newtonsoft.Json;
using System.Text;
using System.Threading;

namespace Pactifier.Core
{
    public class Interaction
    {
        [JsonProperty(Order = 1, NullValueHandling = NullValueHandling.Ignore)]
        private string Description { get; set; }

        [JsonProperty(Order = 2, NullValueHandling = NullValueHandling.Ignore)]
        private string ProviderState { get; set; }
        
        [JsonProperty(Order = 3)]
        private ProviderServiceRequest Request { get; set; }

        [JsonProperty(Order = 4)]
        private ProviderServiceResponse Response { get; set; }
        private string BaseUrl { get; }

        private RequestComparer Comparer { get; }

        public Interaction(string baseUrl, RequestComparer comparer)
        {
            string EnsureEndsWithSlash(string path) => string.IsNullOrEmpty(path) || path.EndsWith("/") ? path : path + "/";

            BaseUrl = EnsureEndsWithSlash(baseUrl);
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
        public (HttpClient, Action) Client()
        {
            var handler = A.Fake<FakeHttpHandler>();
            var client = new HttpClient(handler);
            client.BaseAddress = new Uri("http://localhost" + BaseUrl);
            handler.ConfigureResponse(CreateResponseMessage());
            
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "");
            return (client, FakeHttpHandler.Verify(handler, Request, Comparer));
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