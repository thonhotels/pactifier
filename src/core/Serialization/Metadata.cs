namespace Pactifier.Core.Serialization
{
    public class Metadata
    {
        public Metadata(string version)
        {
            PactSpecification = new Specification(version);
        }
        public Specification PactSpecification { get; set; }
    }
}