using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using  Newtonsoft.Json.Serialization;

namespace Pactifier.Core
{
    public class ProviderServiceRequest
    {
        public ProviderServiceRequest()
        {
            Headers = new Dictionary<string, string>();
        }
        
        [JsonProperty(PropertyName = "method", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))] 
        public HttpVerb Method { get; set; }

        [JsonProperty(PropertyName = "path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        [JsonProperty(PropertyName = "query", NullValueHandling = NullValueHandling.Ignore)]
        public string Query { get; set; }

        [JsonProperty(PropertyName = "headers", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Headers { get; set; }
        
        [JsonProperty(PropertyName = "body", NullValueHandling = NullValueHandling.Ignore)]
        public dynamic Body { get; set; }
    }
}