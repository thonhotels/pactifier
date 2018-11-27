using System.Collections.Generic;
using System.Net.Http;
using Newtonsoft.Json;
using  Newtonsoft.Json.Serialization;

namespace Pactifier.Core
{
    public class ProviderServiceRequest
    {
        [JsonProperty(PropertyName = "method", NullValueHandling = NullValueHandling.Ignore)]
        public HttpMethod Method { get; set; }

        [JsonProperty(PropertyName = "path", NullValueHandling = NullValueHandling.Ignore)]
        public object Path { get; set; }

        [JsonProperty(PropertyName = "query", NullValueHandling = NullValueHandling.Ignore)]
        public object Query { get; set; }

        [JsonProperty(PropertyName = "headers", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Headers { get; set; }
        
        [JsonProperty(PropertyName = "body")]
        public dynamic Body { get; set; }
    }
}