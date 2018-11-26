using System;
using Pactifier.Core;
using Xunit;

namespace CoreTests
{
    public class PactBuilderTests
    {
        [Fact]
        public void PactBuilderCreatesHttpClient()
        {
            var config = new PactConfig
            {
                PactDir = "./pacts",
                SpecificationVersion = "2.0"
            };
            var builder = new PactBuilder(config);
            var client =
                builder
                    .ServiceConsumer("Me")
                    .HasPactWith("Someone")
                    .Interaction()
                    .With(new ProviderServiceRequest
                        {

                        })
                    .WillRespondWith(new ProviderServiceResponse
                        {

                        })
                    .Client();
            Assert.NotNull(client);
        }
    }
}
