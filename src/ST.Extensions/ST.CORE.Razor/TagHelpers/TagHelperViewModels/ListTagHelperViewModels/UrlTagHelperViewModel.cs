using System.Collections.Generic;
using ST.Core.Razor.TagHelpersStructures;

namespace ST.Core.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels
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

        /// <summary>
        /// Html attributes
        /// </summary>
        public ICollection<HtmlAttribute> HtmlAttributes { get; set; } = new List<HtmlAttribute>();
    }
}
