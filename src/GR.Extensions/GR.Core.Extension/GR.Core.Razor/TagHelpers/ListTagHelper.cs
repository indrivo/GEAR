using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using GR.Core.Extensions;
using GR.Core.Helpers.Templates;
using GR.Core.Helpers.Templates.Enums;
using GR.Core.Razor.Enums;
using GR.Core.Razor.Helpers;
using GR.Core.Razor.TagHelpersStructures;

namespace GR.Core.Razor.TagHelpers
{
    [HtmlTargetElement("JList")]
    public class ListTagHelper : TagHelper
    {
        public ListTagHelperModel AspFor { get; set; }

        #region Inject
        /// <summary>
        /// Inject localizer
        /// </summary>
        private readonly IStringLocalizer _localizer;
        #endregion

        public ListTagHelper(IStringLocalizer localizer)
        {
            _localizer = localizer;
        }

        /// <inheritdoc />
        /// <summary>
        /// Render content
        /// </summary>
        /// <param name="context"></param>
        /// <param name="output"></param>
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            string docInfo = null;
            var addInfo = new StringBuilder();
            var template = TemplateManager.GetTemplateBody(Templates.ListTemplate);
            var addTemplate = TemplateManager.GetTemplateBody(Templates.ListAddSectionTemplate);
            if (addTemplate.IsSuccess)
                AspFor.HeadButtons.ToList().ForEach(button =>
                {
                    var bAttrs = string.Join(" ", button.HtmlAttributes.Select(x => x.ToString()));
                    addInfo.Append(addTemplate.Result
                        .Inject(button)
                        .Inject(new Dictionary<string, string>
                        {
                            {"Attrs", bAttrs},
                            {"ButtonBootstrapStyle",  button.BootstrapButton.ToString().ToLower()}
                        }));
                });

            if (!string.IsNullOrEmpty(AspFor.Documentation))
            {
                var docTemplate = TemplateManager.GetTemplateBody(Templates.ListDocumentationTemplate);
                if (docTemplate.IsSuccess)
                {
                    docTemplate.Result = docTemplate.Result.Inject(new Dictionary<string, string>
                    {
                        { "Documentation", AspFor.Documentation }
                    });
                    docInfo = docTemplate.Result;
                }
            }

            var tInlineStyles = string.Join(" ", AspFor.StyleAttributes.Select(x => x.ToString()));
            var content = template.Result.Inject(AspFor).Inject(new Dictionary<string, string>
            {
                { "ColumnsContainer", GetHeadColumns() },
                { "JsScript", GetJsScript() },
                { "DocumentationContainer", docInfo ?? string.Empty },
                { "AddSectionContainer", addInfo.ToString() },
                { "TableInlineStyles", tInlineStyles}
            });
            output.Content.SetHtmlContent(content);
        }

        /// <summary>
        /// Get js script
        /// </summary>
        /// <returns></returns>
        protected virtual string GetJsScript()
        {
            var container = new StringBuilder();
            var template = TemplateManager
                .GetTemplateBody(Templates.ListJsScriptTemplate,
                    TemplateType.Js);
            if (!template.IsSuccess) return container.ToString();
            var actions = AspFor.ListActionDrawer.Render(AspFor);
            var renderColumns = new StringBuilder();
            foreach (var renderItem in AspFor.RenderColumns)
            {
                if (renderItem.BodySystemTemplate != RenderCellBodySystemTemplate.Undefined)
                {
                    switch (renderItem.BodySystemTemplate)
                    {
                        case RenderCellBodySystemTemplate.Boolean:
                            var boolTemplate = TemplateManager.GetTemplateBody(Templates.ListBooleanRenderJsTemplate, TemplateType.Js);
                            if (boolTemplate.IsSuccess)
                            {
                                renderColumns.AppendLine(boolTemplate.Result.Inject(new Dictionary<string, string>
                                {
                                    { nameof(renderItem.ColumnName), renderItem.ApiIdentifier},
                                    { nameof(renderItem.ApiIdentifier), renderItem.ApiIdentifier }
                                }));
                            }
                            break;
                    }
                }
                else
                {
                    renderColumns.AppendLine(renderItem.HasTemplate
                        ? renderItem.Template
                        : $"{{ data : \"{renderItem.ApiIdentifier}\"}},");
                }
            }

            if (AspFor.Api.Parameters.Any())
            {
                AspFor.Api.Url = $"{AspFor.Api.Url}?{string.Join("&", AspFor.Api.Parameters.Select((x) => x.Key + "=" + x.Value.ToString()))}";
            }
            var content = template.Result.Inject(AspFor).Inject(new Dictionary<string, object>
            {
                { "DataApi", AspFor.Api.Url },
                { "Method", AspFor.Api.ApiType.ToString().ToLower() },
                { "ListActionsContainer", actions },
                { "RenderColumnsContainer", renderColumns.ToString() }
            });
            container.Append(content);
            return container.ToString();
        }

        /// <summary>
        /// Get head columns
        /// </summary>
        /// <returns></returns>
        protected virtual string GetHeadColumns()
        {
            var container = new StringBuilder();
            var thTemplate = TemplateManager.GetTemplateBody(Templates.ThTemplate);
            var inlineCss = new StringBuilder();
            var colAttrs = new StringBuilder();
            foreach (var column in AspFor.RenderColumns)
            {
                if (column.StyleAttributes.Any())
                {
                    column.StyleAttributes.ToList().ForEach(x => { inlineCss.AppendLine(x.ToString()); });
                }

                if (column.HtmlAttributes.Any())
                {
                    column.HtmlAttributes.ToList().ForEach(x => { colAttrs.AppendLine($"{x} "); });
                }
                container.AppendLine(thTemplate.Result.Inject(new Dictionary<string, string> {
                    { "ColumnName", column.ColumnName },
                    { "Attrs", colAttrs.ToString() },
                    { "InlineStyle", inlineCss.ToString() }
                }));
            }

            if (AspFor.HasActions)
            {
                container.AppendLine(thTemplate.Result.Inject(new Dictionary<string, string> {
                    { "ColumnName", _localizer["list_actions"] },
                    { "Attrs", "" },
                    { "InlineStyle", "" }
                }));
            }
            return container.ToString();
        }
    }
}
