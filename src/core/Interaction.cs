using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FakeItEasy;
using Pactifier.Core.Comparers;
using Xunit;

namespace Pactifier.Core
{
    public class Interaction
    {
        private ProviderServiceResponse Response { get; set; }
        private ProviderServiceRequest Request { get; set; }
        private string Description { get; set; }
        private string ProviderState { get; set; }
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
            A.CallTo(() => client.SendAsync(A<HttpRequestMessage>._)).Returns(Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created)));
            return (client, Verify(client, Request, Comparer));
        }

        private static Action Verify(IHttpClientBase client, ProviderServiceRequest r, RequestComparer comparer)
        {
            return () => A.CallTo(() => client.SendAsync(A<HttpRequestMessage>.That.Matches(actual => comparer.Execute(r, actual))))
                .MustHaveHappenedOnceExactly();
        }

        private static bool RequestsAreSame(ProviderServiceRequest expected, HttpRequestMessage actual)
        {
            return true;
        }
    }
}