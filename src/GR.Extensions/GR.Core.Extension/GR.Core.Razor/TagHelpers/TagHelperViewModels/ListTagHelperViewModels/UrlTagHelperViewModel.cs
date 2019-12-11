using System.Collections.Generic;
using GR.Core.Razor.TagHelpersStructures;

namespace GR.Core.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels
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
        public string Url
        {
            get
            {
                if (string.IsNullOrEmpty(AspAction) || string.IsNullOrEmpty(AspController)) return "#";
                return $"/{AspController}/{AspAction}";
            }
        }

        /// <summary>
        /// Bootstrap button
        /// </summary>
        public BootstrapButton BootstrapButton { get; set; } = BootstrapButton.Info;

        /// <summary>
        /// Html attributes
        /// </summary>
        public ICollection<HtmlAttribute> HtmlAttributes { get; set; } = new List<HtmlAttribute>();
    }
}
