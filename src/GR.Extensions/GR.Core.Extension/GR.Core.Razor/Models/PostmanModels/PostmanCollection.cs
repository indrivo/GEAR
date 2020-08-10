using System.Collections.Generic;
using Newtonsoft.Json;

namespace GR.Core.Razor.Models.PostmanModels
{
    /// <summary>
    ///     [Postman](http://getpostman.com) collection representation
    /// </summary>
    public class PostmanCollection
    {
        /// <summary>
        /// Info
        /// </summary>
        public PostmanInfo Info { get; set; }

        /// <summary>
        ///     folders within the collection
        /// </summary>
        [JsonProperty(PropertyName = "item")]
        public ICollection<PostmanFolder> Folders { get; set; }
    }
}