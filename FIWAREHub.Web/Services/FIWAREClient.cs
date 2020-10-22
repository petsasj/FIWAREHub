using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FIWAREHub.Web.Services
{
    public class FIWAREClient : HttpClient
    {
        private List<(string headerKey, string headerValue)> headers = new List<(string headerKey, string headerValue)>
        {
            ("fiware-service", "epu_ntua"),
            ("fiware-servicepath", "/")
        };

        public async Task<HttpResponseMessage> SendJson(HttpMethod method, string url, dynamic obj)
        {
            var serializedContent = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                });

            var requestMessage = new HttpRequestMessage(method, url);
            requestMessage.Content = new StringContent(serializedContent);
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            headers.ForEach(h => requestMessage.Content.Headers.Add(h.headerKey, h.headerValue));

            return await this.SendAsync(requestMessage);
        }

        public async Task<HttpResponseMessage> SendUltraLight(HttpMethod method, string url, string content)
        {


            var requestMessage = new HttpRequestMessage(method, url);
            requestMessage.Content = new StringContent(content);
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            headers.ForEach(h => requestMessage.Content.Headers.Add(h.headerKey, h.headerValue));

            return await this.SendAsync(requestMessage);

        }
    }
}
