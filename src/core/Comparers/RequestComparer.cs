using System.Linq;
using System.Net.Http;

namespace Pactifier.Core.Comparers
{
    public class RequestComparer
    {
        private HeaderComparer HeaderComparer { get; }
        private string BaseUrl { get; }

        public RequestComparer(string baseUrl, HeaderComparer headerComparer)
        {
            HeaderComparer = headerComparer;
            BaseUrl = baseUrl;
        }

        public bool Execute(ProviderServiceRequest expected, HttpRequestMessage actual)
        {
            if ( expected
                    .Headers
                    .Keys
                    .Any(k => !actual.Headers.Contains(k) || !HeaderComparer.Execute(expected.Headers[k], actual.Headers.GetValues(k))))
                return false;
            if (actual.RequestUri.LocalPath.Replace("//", "/") != expected.Path)
                return false;
            return true;
        }
    }
}