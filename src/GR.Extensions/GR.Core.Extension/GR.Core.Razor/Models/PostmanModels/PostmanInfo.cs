using System;
using Newtonsoft.Json;

namespace GR.Core.Razor.Models.PostmanModels
{
    public class PostmanInfo
    {
        /// <summary>
        ///     Id of collection
        /// </summary>
        [JsonProperty(PropertyName = "_postman_id")]
        public Guid Id { get; set; }

        /// <summary>
        ///     Name of collection
        /// </summary>
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        /// <summary>
        ///     Description of collection
        /// </summary>
        [JsonProperty(PropertyName = "description")]
        public string Description { get; set; }

        /// <summary>
        /// Schema
        /// </summary>
        public string Schema => "https://schema.getpostman.com/json/collection/v2.0.0/collection.json";
    }
}
