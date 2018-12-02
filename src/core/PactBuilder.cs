using System;
using System.Collections.Generic;
using Pactifier.Core.Comparers;
using Pactifier.Core.Serialization;

namespace Pactifier.Core
{
    public class PactBuilder 
    {
        private PactConfig Config { get; }


        public string ConsumerName { get; private set; }

        public string ProviderName { get; private set; }

        private List<Interaction> Interactions { get; }
        private string BaseUrl { get; }
        public Interaction InteractionUnderConstruction { get; private set; }

        public PactBuilder() : this(new PactConfig())
        {
        }

        public PactBuilder(PactConfig config)
        {
            Config = config;
            Interactions = new List<Interaction>();
            BaseUrl = config.BaseUrl;

            InteractionUnderConstruction =
                new Interaction(BaseUrl, new RequestComparer(new HeaderComparer()));
        }

        public PactBuilder HasPactWith(string providerName)
        {
            ProviderName = providerName;
            return this;
        }

        public PactBuilder ServiceConsumer(string consumerName)
        {
            ConsumerName = consumerName;
            return this;
        }

        [Obsolete("No need to call this anymore")]
        public PactBuilder Interaction(string baseUrl = null)
        {
            InteractionUnderConstruction = 
                 new Interaction(baseUrl ?? BaseUrl, new RequestComparer(new HeaderComparer()));
            return this;
        }

        public PactBuilder Given(string providerState)
        {
            InteractionUnderConstruction =
                InteractionUnderConstruction.Given(providerState);
            return this;
        }

        public PactBuilder UponReceiving(string description)
        {
            InteractionUnderConstruction =
                InteractionUnderConstruction.UponReceiving(description);
            return this;
        }

        public PactBuilder With(ProviderServiceRequest request)
        {
            InteractionUnderConstruction =
                InteractionUnderConstruction.With(request);
            return this;
        }

        public Interaction WillRespondWith(ProviderServiceResponse response)
        {
            var result =
                InteractionUnderConstruction.WillRespondWith(response);
            Interactions.Add(InteractionUnderConstruction);
            InteractionUnderConstruction = new Interaction(BaseUrl, new RequestComparer(new HeaderComparer())); 
            return result;
        }

        public void Build()
        {
            var serializer = new PactSerializer(ConsumerName, ProviderName, Interactions, Config);
            serializer.Execute();
        }

        public void ClearInteractions()
        {
            Interactions.Clear();
        }    
    }
}