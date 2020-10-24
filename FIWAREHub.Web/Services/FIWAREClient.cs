using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace FIWAREHub.Web.Services
{
    public class FIWAREClient : HttpClient
    {
        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>
        {
            {"fiware-service", "epu_ntua"},
            {"fiware-servicepath", "/"}
        };

        /// <summary>
        /// Adds appropriate headers for JSON Posting to FIWARE IoT Agents
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendJson(HttpMethod method, string url, dynamic obj)
        {
            var serializedContent = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.None,
                new Newtonsoft.Json.JsonSerializerSettings
                {
                    NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore
                });

            var requestMessage = new HttpRequestMessage(method, url);
            requestMessage.Content = new StringContent(serializedContent, Encoding.UTF8);
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            foreach (var (key, value) in _headers) requestMessage.Content.Headers.Add(key, value);

            return await this.SendAsync(requestMessage);
        }

        /// <summary>
        /// Adds appropriate headers for UltraLigth Posting to FIWARE IoT Agents
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task<HttpResponseMessage> SendUltraLight(HttpMethod method, string url, string content)
        {
            var requestMessage = new HttpRequestMessage(method, url);
            requestMessage.Content = new StringContent(content, Encoding.UTF8);
            requestMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("text/plain");
            foreach (var (key, value) in _headers) requestMessage.Content.Headers.Add(key, value);

            return await this.SendAsync(requestMessage);

        }
    }
}
