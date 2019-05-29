namespace ST.CORE.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels
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
    }

    public enum ApiType
    {
        Post,
        Get,
        Delete,
        Put
    }
}
