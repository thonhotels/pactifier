namespace Pactifier.Core.Serialization
{
    public class Specification
    {
        public Specification(string version)
        {
            Version = version;
        }
        public string Version { get; set; }
    }
}