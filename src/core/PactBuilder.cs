using System.Collections.Generic;
using Pactifier.Core.Comparers;

namespace Pactifier.Core
{
    public class PactBuilder 
    {
        private PactConfig Config { get; }
        private List<Interaction> Interactions { get; }

        
        public string ConsumerName { get; private set; }
        public string ProviderName { get; private set; }
        public string BaseUrl { get; }

        public PactBuilder() : this(new PactConfig())
        {
        }

        public PactBuilder(PactConfig config)
        {
            Config = config;
            Interactions = new List<Interaction>();
            BaseUrl = "http://localhost";

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

        public Interaction Interaction()
        {
            var i = new Interaction(BaseUrl, new RequestComparer(BaseUrl, new HeaderComparer()));
            Interactions.Add(i);
            return i;
        }

        public void Build()
        {

        }

        public void ClearInteractions()
        {
            Interactions.Clear();
        }    
    }
}