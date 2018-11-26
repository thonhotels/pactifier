using System.Collections.Generic;
using Newtonsoft.Json;

namespace Pactifier.Core
{
    public class ProviderServiceResponse
    {

        [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
        public int Status { get; set; }
        [JsonProperty(PropertyName = "headers", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, object> Headers { get; set; }
        [JsonProperty(PropertyName = "body")]
        public dynamic Body { get; set; }
    }
}