using System.Collections.Generic;

namespace GR.Core.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels
{
    public class ListApiConfigurationViewModel
    {
        /// <summary>
        /// Url for load data in list
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Access type
        /// </summary>
        public ApiType ApiType { get; set; } = ApiType.Post;

        /// <summary>
        /// Url parameters
        /// </summary>
        public Dictionary<string, string> Parameters { get; set; } = new Dictionary<string, string>();
    }

    public enum ApiType
    {
        Post,
        Get,
        Delete,
        Put
    }
}
