using System;
using Newtonsoft.Json;

namespace Pactifier.Core
{
    public class PactConfig
    {
        public PactConfig(string baseUrl = "")
        {
            PactDir = "../../../pacts";
            SpecificationVersion = "2.0.0";
            LogDir = "./logs";
            BaseUrl = baseUrl;
            JsonConverters = new JsonConverter[0];
        }

        public string PactDir { get; set; }
        public string LogDir { get; set; }
        public string BaseUrl { get; }
        public string SpecificationVersion { get; set; }

        public JsonConverter[] JsonConverters { get; internal set; }
    }
}
