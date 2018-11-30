using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using FakeItEasy;
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
            var config = new PactConfig();            
            var builder = new PactBuilder(config);
            var (client, _) =
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
            var builder = new PactBuilder();
            var (client, verify) =
                builder
                    .ServiceConsumer("Me")
                    .HasPactWith("Someone")
                    .Interaction()
                    .With(new ProviderServiceRequest
                        {
                            Method = HttpVerb.post,
                            Path = "/api/test/something",
                            Headers = new Dictionary<string, string>
                            {
                                { "Authorization", "Bearer accesstoken" }
                            },
                            Body = new { SomeProperty = "test" }
                        })
                    .WillRespondWith(new ProviderServiceResponse
                        {
                            Status = HttpStatusCode.Created,
                            Headers = new Dictionary<string, string>
                            {
                                { "Content-Type", "application/json" }
                            }
                        })
                    .Client();                    
            Assert.NotNull(client);
            var r = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "/api/test/something");
            
            r.Headers.Authorization =  new AuthenticationHeaderValue("Bearer", "accesstoken");
            r.Content = new StringContent(JsonConvert.SerializeObject(new { SomeProperty = "test" }), Encoding.UTF8, "application/json");
            var response = await client.SendAsync(r);
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            verify();
        }

        [Fact]
        public async Task PostThrowsIfBodyIsDifferent()
        {
            var builder = new PactBuilder();
            var (client, verify) =
                builder
                    .ServiceConsumer("Me")
                    .HasPactWith("Someone")
                    .Interaction()
                    .With(new ProviderServiceRequest
                        {
                            Method = HttpVerb.post,
                            Path = "/api/test/something",
                            Body = new { SomeProperty = "test" }
                        })
                    .WillRespondWith(new ProviderServiceResponse
                        {
                            Status = HttpStatusCode.Created
                        })
                    .Client();                    
            Assert.NotNull(client);
            var r = new HttpRequestMessage(HttpMethod.Post, client.BaseAddress + "/api/test/something");
            r.Content = new StringContent(JsonConvert.SerializeObject(new { SomeOtherProperty = "test" }), Encoding.UTF8, "application/json");
            var response = await client.SendAsync(r);
            Assert.Throws<ExpectationException>(() => verify());
        }

        [Fact]
        public async Task ClientRespondsSuccessToGet()
        {
            var builder = new PactBuilder();
            var (client, verify) =
                builder
                    .ServiceConsumer("Me")
                    .HasPactWith("Someone")
                    .Interaction()
                    .With(new ProviderServiceRequest
                        {
                            Method = HttpVerb.get,
                            Path = "/api/test/something",
                            Headers = new Dictionary<string, string>
                            {
                                { "Authorization", "Bearer accesstoken" }
                            }
                        })
                    .WillRespondWith(new ProviderServiceResponse
                        {
                            Status = HttpStatusCode.OK,
                            Headers = new Dictionary<string, string>
                            {
                                { "Content-Type", "application/json; charset=utf-8" }
                            },
                            Body = new { SomeProperty = "test" }
                        })
                    .Client();                    
            Assert.NotNull(client);
            client.SetBearerToken("accesstoken");        
            var response = await client.GetAsync("/api/test/something");
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var json = await response.Content.ReadAsStringAsync();
            Assert.NotNull(json);
            var result = JsonConvert.DeserializeObject<dynamic>(json);
            Assert.Equal("test", (string)result.SomeProperty);

            verify();
        }

        [Fact]
        public async Task ThrowsIfQueryIsDifferent()
        {
            var builder = new PactBuilder();
            var (client, verify) =
                builder
                    .ServiceConsumer("Me")
                    .HasPactWith("Someone")
                    .Interaction()
                    .With(new ProviderServiceRequest
                        {
                            Method = HttpVerb.get,
                            Path = "/api/test/something",
                            Query = "a=b"
                        })
                    .WillRespondWith(new ProviderServiceResponse
                        {
                            Status = HttpStatusCode.OK                            
                        })
                    .Client();                    
            var response = await client.GetAsync("/api/test/something?a=c");
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.Throws<ExpectationException>(() => verify());
        }

        [Fact]
        public async Task QueryCanContainMultipleSegments()
        {
            var builder = new PactBuilder();
            var (client, verify) =
                builder
                    .ServiceConsumer("Me")
                    .HasPactWith("Someone")
                    .Interaction()
                    .Given("something given")
                    .UponReceiving("upon receiving stuff")
                    .With(new ProviderServiceRequest
                        {
                            Method = HttpVerb.get,
                            Path = "/api/test/something",
                            Query = "a=b&test=quest"
                        })
                    .WillRespondWith(new ProviderServiceResponse
                        {
                            Status = HttpStatusCode.OK
                        })
                    .Client();                    
            var response = await client.GetAsync("/api/test/something?a=b&test=quest");
            Assert.NotNull(response);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            verify();
        }

        [Fact]
        public void BuildDoesNotThrow()
        {
            var builder = new PactBuilder();
            
            builder
                .ServiceConsumer("Me")
                .HasPactWith("Someone")
                .Interaction()
                .Given("something given")
                .UponReceiving("upon receiving stuff")
                .With(new ProviderServiceRequest
                    {
                        Method = HttpVerb.get,
                        Path = "/api/test/something",
                        Query = "a=b",
                        Headers = new Dictionary<string, string>
                        {
                            { "Authorization", "Bearer accesstoken" }
                        },
                        Body = new { SomeProperty = "test" }
                    })
                .WillRespondWith(new ProviderServiceResponse
                    {
                        Status = HttpStatusCode.OK,
                        Headers = new Dictionary<string, string>
                        {
                            { "Content-Type", "application/json; charset=utf-8" }
                        },                           
                        Body = new { SomeProperty = "test" }                            
                    })
                .Client();                    
            builder.Build();
        }

        [Fact]
        public void UseBaseUrlIfset()
        {
            var builder = new PactBuilder(new PactConfig("/api"));
            
            var (client, verify) = builder
                .ServiceConsumer("Me")
                .HasPactWith("Someone")
                .Interaction()
                .Given("something given")
                .UponReceiving("upon receiving stuff")
                .With(new ProviderServiceRequest
                    {
                        Method = HttpVerb.get,
                        Path = "/api/test/something",
                        Query = "a=b",
                    })
                .WillRespondWith(new ProviderServiceResponse
                    {
                        Status = HttpStatusCode.OK,
                        Headers = new Dictionary<string, string>
                        {
                            { "Content-Type", "application/json; charset=utf-8" }
                        },                           
                        Body = new { SomeProperty = "test" }                            
                    })
                .Client();   
            var request = new HttpRequestMessage(HttpMethod.Get, "test/something?a=b");
            client.SendAsync(request);
            verify();
        }
    }
}
