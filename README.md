# pactifier
Pure .NET Core implementation of [Pact.Net](https://github.com/pact-foundation/pact-net)

The main difference between our Pactifier and the [Pact.Net](https://github.com/pact-foundation/pact-net) implementation is that we do everything inside a single .NET Core process. We have replaced the calls to an external Ruby web server by an in-memory HttpClient fake. We believe this will make the code both faster and more stable.

As the Pactifier API is based on the API of [Pact.Net](https://github.com/pact-foundation/pact-net) parts of this readme is copied from their readme.

Both Pactifier and [Pact.Net](https://github.com/pact-foundation/pact-net) are implementations of the Ruby consumer driven contract library, [Pact](https://github.com/realestate-com-au/pact).  
Pact is based off the specification found at https://github.com/pact-foundation/pact-specification.  

Pactifier primarily provides a fluent .NET DSL for describing HTTP requests that will be made to a service provider and the HTTP responses the consumer expects back to function correctly.  
In documenting the consumer interactions, we can replay them on the provider and ensure the provider responds as expected. This basically gives us complete test symmetry and removes the basic need for integrated tests.  

Pactifier is Version 2.0 compliant.  

From the [Pact Specification repo](https://github.com/pact-foundation/pact-specification)

> "Pact" is an implementation of "consumer driven contract" testing that allows mocking of responses in the consumer codebase, and verification of the interactions in the provider codebase. The initial implementation was written in Ruby for Rack apps, however a consumer and provider may be implemented in different programming languages, so the "mocking" and the "verifying" steps would be best supported by libraries in their respective project's native languages. Given that the pact file is written in JSON, it should be straightforward to implement a pact library in any language, however, to get the best experience and most reliability of out mixing pact libraries, the matching logic for the requests and responses needs to be identical. There is little confidence to be gained in having your pacts "pass" if the logic used to verify a "pass" is inconsistent between implementations.

Read more about Pact and the problems it solves at https://docs.pact.io/

Please feel free to contribute, we do accept pull requests. This solution has been built using Visual Studio Code, but Visual Studio 2017 should work fine too.

## Install
dotnet add package pactifier

## Usage


In the following examples we will be using xUnit, but NUnit could be used as well.

### Service Consumer

#### 1. Build your client
Which may look something like this.

```c#

public class SomethingApiClient
{
  private HttpClient Client { get; };

  public SomethingApiClient(HttpClient client)
  {
    Client = client;
  }


  public Something GetSomething(string id)
  {
    var request = new HttpRequestMessage(HttpMethod.Get, "/somethings/" + id);
    request.Headers.Add("Accept", "application/json");

    var response = Client.SendAsync(request);
    ...
    return something;
  }
}
```

#### 2. Declare an xunit CollectionDefinition
We need to run the tests for a single Provider in serial and we need to share state as we need to write result of all the tests to a single file (the pact file).
In xUnit we use a CollectionDefinition to group a number of tests into a Collection.

```c#
 [CollectionDefinition("SomeProviderPactBuilderCollection")]
public class SomeProviderPactBuilderCollection : ICollectionFixture<SomeProviderPactFixture>
{
    // This class has no code, and is never created. Its purpose is simply
    // to be the place to apply [CollectionDefinition] and all the
    // ICollectionFixture<> interfaces.
}
```

#### 3. Create an xunit CollectionFixture
Then we create a fixture for the collection that contains setup and tear down of the fixture used for all the tests in a collection.
This means that the PactBuilder will be shared by all the tests in the collection.
All tests for a Provider-Consumer pair should belong to the same collection.
```c#
public class SomeProviderPactFixture : IDisposable
{
    public PactBuilder PactBuilder { get; private set; }

    public SomeProviderPactFixture()
    {
        // Using Spec version 2.0.0 more details at https://goo.gl/UrBSRc
        var pactConfig = new PactConfig()
        {
            SpecificationVersion = "2.0.0",
            PactDir = @"../../../../pacts",
            LogDir = @"./pact_logs",
        };

        PactBuilder = new PactBuilder(pactConfig);

        PactBuilder.ServiceConsumer("unique.name.of.my.consumer")
                    .HasPactWith("unique.name.of.the.provider");
    }

    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // This will save the pact file once finished.
                PactBuilder.Build();
            }

            disposedValue = true;
        }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        Dispose(true);
    }
}
```

#### 4. Write your test
Create a new test case and implement it.

```c#
[Collection("SomeProviderPactBuilderCollection")]
public class SomethingApiConsumerTests
{
    private PactBuilder PactBuilder { get; }

    public SomethingApiConsumerTests(SomeProviderPactFixture fixture)
    {
        PactBuilder = fixture.PactBuilder;
    }

    [Fact]
    public async Task GetSomething_WhenTheTesterSomethingExists_ReturnsTheSomething()
    {
        var (client, verify) =
            PactBuilder
                .Given("There is a something with id 'tester'")
                .UponReceiving("A GET request to retrieve the something")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.get,
                    Path = "/somethings/tester",
                    Headers = new Dictionary<string, string>
                    {
                        { "Accept", "application/json" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = HttpStatusCode.OK,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    },
                    Body = new 
                    {
                        Id = "tester",
                        FirstName = "Totally",
                        LastName = "Awesome"
                    }
                }) //NOTE: WillRespondWith call must come last as it will register the interaction
                .Client();  // Client returns a tuple, containing a HttpClient and a verify function
        var consumer = new SomethingApiClient(client);

        //Act
        var result = consumer.GetSomething("tester");

        //Assert
        Assert.Equal("tester", result.id);

        verify(); //NOTE: Verifies that the interaction registered on the mock provider is called exactly once
    }
}
```

#### 4. Run the test
Everything should be green and the pact file is generated in the folder you specified in PactConfig.PactDir
