using Newtonsoft.Json;

namespace GR.Core.Razor.Models.PostmanModels
{
    public class PostmanFolderRequest
    {
        /// <summary>
        ///     name of request
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Request
        /// </summary>
        [JsonProperty("request")]
        public PostmanRequest Request { get; set; }
    }
}
