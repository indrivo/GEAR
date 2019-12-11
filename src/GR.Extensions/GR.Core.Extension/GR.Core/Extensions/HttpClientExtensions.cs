using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace GR.Core.Extensions
{
    public static class HttpClientExtensions
    {
        /// <summary>
        /// Settings
        /// </summary>
        private static readonly JsonSerializer JsonSerializer = new JsonSerializer();


        /// <summary>
        /// Get and parse
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="httpContent"></param>
        /// <returns></returns>
        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent httpContent)
        {
            using (var stream = await httpContent.ReadAsStreamAsync())
            {
                var jsonReader = new JsonTextReader(new StreamReader(stream));

                return JsonSerializer.Deserialize<T>(jsonReader);
            }
        }

        /// <summary>
        /// Post
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PostJsonAsync<T>(this HttpClient client, string url, T value)
        {
            return SendJsonAsync<T>(client, HttpMethod.Post, url, value);
        }

        /// <summary>
        /// Put
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> PutJsonAsync<T>(this HttpClient client, string url, T value)
        {
            return SendJsonAsync<T>(client, HttpMethod.Put, url, value);
        }


        /// <summary>
        /// Send
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Task<HttpResponseMessage> SendJsonAsync<T>(this HttpClient client, HttpMethod method, string url,
            T value)
        {
            var stream = new MemoryStream();
            var jsonWriter = new JsonTextWriter(new StreamWriter(stream));
            JsonSerializer.Serialize(jsonWriter, value);
            jsonWriter.Flush();
            stream.Position = 0;
            var request = new HttpRequestMessage(method, url)
            {
                Content = new StreamContent(stream)
            };

            request.Content.Headers.TryAddWithoutValidation("Content-Type", "application/json");
            return client.SendAsync(request);
        }
    }
}