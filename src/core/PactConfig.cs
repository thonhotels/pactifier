using System;

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
        }

        public string PactDir { get; set; }
        public string LogDir { get; set; }
        public string BaseUrl { get; }
        public string SpecificationVersion { get; set; }
    }
}
