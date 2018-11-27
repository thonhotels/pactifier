using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeItEasy;

namespace Pactifier.Core
{
    public class Interaction
    {
        private ProviderServiceResponse Response { get; set; }
        private ProviderServiceRequest Request { get; set; }

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

        public IHttpClientBase Client()
        {
            var client = A.Fake<IHttpClientBase>();
            A.CallTo(() => client.SendAsync(A<HttpRequestMessage>._)).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created)));
            return client;
        }
    }
}