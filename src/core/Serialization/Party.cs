namespace Pactifier.Core.Serialization
{
    public class Party
    {
        public string Name { get; set; }

        public Party(string consumerName)
        {
            Name = consumerName;
        }
    }
}