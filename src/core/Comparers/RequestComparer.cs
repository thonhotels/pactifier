using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

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
            string RemoveLeadingQuestionMark(string q) => string.IsNullOrEmpty(q) ? q : q.Substring(1);

            if (!CompareHttpVerb(expected.Method, actual.Method))
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

        private bool CompareHttpVerb(HttpVerb expected, HttpMethod actual)
        {
            switch (expected)
            {
                case HttpVerb.delete:
                    return actual == HttpMethod.Delete;
                case HttpVerb.get:
                    return actual == HttpMethod.Get;
                case HttpVerb.head:
                    return actual == HttpMethod.Head;
                case HttpVerb.options:
                    return actual == HttpMethod.Options;
                case HttpVerb.post:
                    return actual == HttpMethod.Post;
                case HttpVerb.put:
                    return actual == HttpMethod.Put;
                case HttpVerb.trace:
                    return actual == HttpMethod.Trace;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}