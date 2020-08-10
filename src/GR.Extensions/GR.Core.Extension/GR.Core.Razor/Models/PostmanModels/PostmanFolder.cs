using System.Collections.Generic;
using Newtonsoft.Json;

namespace GR.Core.Razor.Models.PostmanModels
{
    /// <summary>
    ///     Object that describes a [Postman](http://getpostman.com) folder
    /// </summary>
    public class PostmanFolder
    {
        /// <summary>
        ///     folder name
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        ///     folder description
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        ///     Requests associated with the collection
        /// </summary>
        [JsonProperty(PropertyName = "item")]
        public ICollection<PostmanFolderRequest> FolderRequests { get; set; }
    }
}
