namespace ST.CORE.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels
{
    public class UrlTagHelperViewModel : TagHelperBaseModel
    {
        /// <summary>
        /// Controller to redirect on  click button
        /// </summary>
        public string AspController { get; set; }

        /// <summary>
        /// Action to redirect on click button
        /// </summary>
        public string AspAction { get; set; } = "Create";

        /// <summary>
        /// Store create button data
        /// </summary>
        public string ButtonName { get; set; } = "Add";

        /// <summary>
        /// Description for add button
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Url
        /// </summary>
        public string Url => $"/{AspController}/{AspAction}";
    }
}
