using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using GR.Core.Attributes;

namespace GR.Core.Razor.TagHelpers
{
    [HtmlTargetElement("JTranslateLabel")]
    public class TranslateLabelTagHelper : TagHelper
    {
        #region Services

        private readonly IStringLocalizer _localizer;

        #endregion

        public TranslateLabelTagHelper(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// Bind property
        /// </summary>
        public ModelExpression AspFor { get; set; }

        /// <summary>
        /// Classes
        /// </summary>
        public string Class { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Render content
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "label";
            if (AspFor == null)
            {
                output.Content.SetContent("Bad configuration");
            }
            else
            {
                output.Attributes.Add("for", AspFor.Name);
                output.Attributes.Add("class", Class);
                output.TagMode = TagMode.StartTagAndEndTag;
                var childs = await output.GetChildContentAsync();
                var childsBody = childs.GetContent();
                var body = new StringBuilder();
                body.Append(string.IsNullOrEmpty(childsBody) ? GetTranslatedKey() : childsBody);
                body.Append(childsBody);
                output.Content.SetContent(body.ToString());
            }
        }

        /// <summary>
        /// Get translated key
        /// </summary>
        /// <returns></returns>
        public string GetTranslatedKey()
        {
            var prop = AspFor.Metadata.ContainerType?.GetProperty(AspFor.Name);
            var displayTranslate = prop?.GetCustomAttribute<DisplayTranslateAttribute>();
            return displayTranslate == null ? AspFor.Metadata.DisplayName ?? AspFor.Name : _localizer[displayTranslate.Key];
        }
    }
}
