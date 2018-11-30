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

        public PactBuilder() : this(new PactConfig())
        {
        }

        public PactBuilder(PactConfig config)
        {
            Config = config;
            Interactions = new List<Interaction>();
            BaseUrl = config.BaseUrl;
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

        public Interaction Interaction(string baseUrl = null)
        {
            var i = new Interaction(baseUrl ?? BaseUrl, new RequestComparer(new HeaderComparer()));
            Interactions.Add(i);
            return i;
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