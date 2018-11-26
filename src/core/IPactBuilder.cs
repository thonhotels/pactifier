namespace Pactifier.Core
{
    public interface IPactBuilder
    {
        IPactBuilder HasPactWith(string providerName);
        IPactBuilder ServiceConsumer(string consumerName);
        void Build();
    }
}