using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
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

        [Fact]
        public async Task ClientRespondsCreatedToPost()
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
                            Method = HttpMethod.Post,
                            Path = "/api/test/something",
                            Headers = new Dictionary<string, object>
                            {
                                { "Authorization", "Bearer accesstoken" },
                                { "Content-Type", "application/json; charset=utf-8" }
                            },
                            Body = new { SomeProperty = "test" }
                        })
                    .WillRespondWith(new ProviderServiceResponse
                        {
                            Status = 201,
                            Headers = new Dictionary<string, object>
                            {
                                { "Content-Type", "application/json" }
                            }
                        })
                    .Client();                    
            Assert.NotNull(client);
            var r = new HttpRequestMessage(HttpMethod.Post, "/api/test/something");
            
            r.Headers.Authorization =  new AuthenticationHeaderValue("Bearer", "accesstoken");
            r.Content = new StringContent(JsonConvert.SerializeObject(new { SomeProperty = "test" }), Encoding.UTF8, "application/json");
            var response = await client.SendAsync(r);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}
