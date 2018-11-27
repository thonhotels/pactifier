using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Pactifier.Core.Serialization
{

    public class PactSerializer
    {
        [JsonProperty(Order = 1)]
        public Party Consumer { get; set; }

        [JsonProperty(Order = 2)]
        public Party Provider { get; set; }

        [JsonProperty(Order = 3)]
        public IEnumerable<Interaction> Interactions { get; set; }

        [JsonProperty(Order = 4)]
        public Metadata Metadata { get; set; }

        private string PactDir { get; }

        public PactSerializer(string consumerName, string providerName, IEnumerable<Interaction> interactions, PactConfig config)
        {
            Consumer = new Party(consumerName);
            Provider = new Party(providerName);
            Interactions = interactions;
            Metadata = new Metadata(config.SpecificationVersion);
            PactDir = config.PactDir;
        }

        public void Execute()
        {
            var json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
                                {
                                    ContractResolver = new DefaultContractResolver
                                                            {
                                                                NamingStrategy = new CamelCaseNamingStrategy()
                                                            },
                                    Formatting = Formatting.Indented
                                });
            
            File.WriteAllText(GetFileName(), json);
        }

        private string GetFileName()
        {
            return Path.Combine(Path.GetFullPath(PactDir), Consumer.Name + "-" + Provider.Name + ".json");
        }
    }
}