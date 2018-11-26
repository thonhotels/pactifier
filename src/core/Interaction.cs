using System;
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
            return client;
        }
    }
}