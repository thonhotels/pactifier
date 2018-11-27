using System;

namespace Pactifier.Core
{
    public class PactConfig
    {
        public PactConfig()
        {
            PactDir = "../../../pacts";
            SpecificationVersion = "2.0.0";
            LogDir = "./logs";
        }

        public string PactDir { get; set; }
        public string LogDir { get; set; }
        public string SpecificationVersion { get; set; }
    }
}
