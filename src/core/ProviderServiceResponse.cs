using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;

namespace Pactifier.Core
{
    public class ProviderServiceResponse
    {
        public ProviderServiceResponse()
        {
            Headers = new Dictionary<string, string>();
        }

        [JsonProperty(PropertyName = "status", NullValueHandling = NullValueHandling.Ignore)]
        public HttpStatusCode Status { get; set; }
        [JsonProperty(PropertyName = "headers", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Headers { get; set; }
        [JsonProperty(PropertyName = "body")]
        public dynamic Body { get; set; }
    }
}