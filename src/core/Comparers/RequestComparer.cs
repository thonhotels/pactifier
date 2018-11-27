using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            string RemoveLeadingQuestionMark(string q) => string.IsNullOrEmpty(q) ? q : q.Substring(1);

            if (actual.Method != expected.Method)
                return false;

            if ( expected
                    .Headers
                    .Keys
                    .Any(k => !actual.Headers.Contains(k) || !HeaderComparer.Execute(expected.Headers[k], actual.Headers.GetValues(k))))
                return false;
            if (actual.RequestUri.LocalPath.Replace("//", "/") != expected.Path)
                return false;
            if (!string.IsNullOrEmpty(expected.Query) && RemoveLeadingQuestionMark(actual.RequestUri.Query) != expected.Query)
                return false;
            return CompareContent(expected, actual.Content);
        }

        private bool CompareContent(ProviderServiceRequest expected, HttpContent actualContent)
        {
            string actualStringContent = "";
            if (ReferenceEquals(null, expected.Body))
                return true;
            Task.Run(async () => actualStringContent = await actualContent.ReadAsStringAsync()).Wait();
            var expectedContent = JsonConvert.SerializeObject(expected.Body);
            return (actualStringContent == expectedContent);
        }
    }
}