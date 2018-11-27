using System.Linq;
using System.Net.Http;

namespace Pactifier.Core.Comparers
{
    public class RequestComparer
    {
        private HeaderComparer HeaderComparer { get; }

        public RequestComparer(HeaderComparer headerComparer)
        {
            HeaderComparer = headerComparer;
        }

        public bool Execute(ProviderServiceRequest expected, HttpRequestMessage actual)
        {
            if ( expected
                    .Headers
                    .Keys
                    .Any(k => !actual.Headers.Contains(k) || !HeaderComparer.Execute(expected.Headers[k], actual.Headers.GetValues(k))))
                return false;
            return true;
        }
    }
}