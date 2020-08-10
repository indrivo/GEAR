using System.Collections.Generic;
using Newtonsoft.Json;

namespace GR.Core.Razor.Models.PostmanModels
{
    /// <summary>
    ///     [Postman](http://getpostman.com) request object
    /// </summary>
    public class PostmanRequest
    {
        /// <summary>
        ///     request description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        ///     headers associated with the request
        /// </summary>
        [JsonProperty(PropertyName = "header")]
        public IEnumerable<PostmanHeader> Headers { get; set; } = new List<PostmanHeader>();

        /// <summary>
        ///     url of the request
        /// </summary>
        [JsonProperty(PropertyName = "url")]
        public PostmanRequestUrl Url { get; set; }

        /// <summary>
        ///     method of request
        /// </summary>
        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; }

        /// <summary>
        /// Body
        /// </summary>
        public virtual PostmanBodyRequest Body { get; set; }

        /// <summary>
        ///     request response
        /// </summary>
        [JsonProperty(PropertyName = "responses")]
        public ICollection<object> Responses { get; set; }
    }
}