using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using ST.CORE.Razor.Enums;
using ST.CORE.Razor.Extensions;
using ST.CORE.Razor.Helpers;
using ST.CORE.Razor.TagHelpersStructures;

namespace ST.CORE.Razor.TagHelpers
{
    [HtmlTargetElement("admin-list")]
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
            //output.TagName = "div";
            string docInfo = null;
            var addInfo = new StringBuilder();
            var template = TemplateManager.GetTagHelperTemplate(Templates.ListTemplate);

            foreach (var button in AspFor.HeadButtons)
            {
                var addTemplate = TemplateManager.GetTagHelperTemplate(Templates.ListAddSectionTemplate);
                if (addTemplate.IsSuccess)
                {
                    addInfo.Append(addTemplate.Result.Inject(button));
                }
            }

            if (!string.IsNullOrEmpty(AspFor.Documentation))
            {
                var docTemplate = TemplateManager.GetTagHelperTemplate(Templates.ListDocumentationTemplate);
                if (docTemplate.IsSuccess)
                {
                    docTemplate.Result = docTemplate.Result.Inject(new Dictionary<string, string>
                    {
                        { "Documentation", AspFor.Documentation }
                    });
                    docInfo = docTemplate.Result;
                }
            }
            var content = template.Result.Inject(AspFor).Inject(new Dictionary<string, string>
            {
                { "ColumnsContainer", GetHeadColumns() },
                { "JsScript", GetJsScript() },
                { "DocumentationContainer", docInfo ?? string.Empty },
                { "AddSectionContainer", addInfo.ToString() }
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
                .GetTagHelperTemplate(Templates.ListJsScriptTemplate,
                    TemplateType.Js);
            if (!template.IsSuccess) return container.ToString();
            var actions = new StringBuilder();
            if (AspFor.HasActions && AspFor.ListActions.Any())
            {
                var renderColumnTemplate = TemplateManager.GetTagHelperTemplate(Templates.ListRenderColumnJsTemplate, TemplateType.Js);
                if (renderColumnTemplate.IsSuccess)
                {
                    var buttons = new StringBuilder();
                    var buttonTemplate = TemplateManager.GetTagHelperTemplate(Templates.ATemplate);
                    if (buttonTemplate.IsSuccess)
                    {
                        foreach (var action in AspFor.ListActions)
                        {
                            var attrs = new StringBuilder();
                            if (action.IsJsEvent)
                            {
                                if (action.ButtonEvent != null)
                                    attrs.Append($" {action.ButtonEvent.GetEvent}=\"{action.ButtonEvent.JsEventHandler}\" ");
                            }
                            var actionContent = buttonTemplate.Result
                                .Inject(new Dictionary<string, string>
                                {
                                    { "ButtonBootstrapStyle", action.ButtonType.ToString().ToLower() },
                                    { "Attributes", attrs.ToString() }
                                })
                                .Inject(action);
                            buttons.AppendLine(actionContent);
                        }
                    }

                    actions.AppendLine(renderColumnTemplate
                        .Result
                        .Inject(new Dictionary<string, string> { { "Buttons", buttons.ToString() } }));
                }
            }
            var renderColumns = new StringBuilder();
            foreach (var renderItem in AspFor.RenderColumns)
            {
                if (renderItem.HasTemplate)
                {
                    renderColumns.AppendLine($"{{ data : \"{renderItem.ApiIdentifier}\"}},");
                }
                else
                {
                    renderColumns.AppendLine($"{{ data : \"{renderItem.ApiIdentifier}\"}},");
                }
            }
            var content = template.Result.Inject(AspFor).Inject(new Dictionary<string, object>
            {
                { "DataApi", AspFor.Api.Url },
                { "Method", AspFor.Api.ApiType.ToString().ToLower() },
                { "ListActionsContainer", actions.ToString() },
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
            var thTemplate = TemplateManager.GetTagHelperTemplate(Templates.ThTemplate);
            foreach (var column in AspFor.Columns)
            {
                container.AppendLine(thTemplate.Result.Inject(new Dictionary<string, string> { { "ColumnName", column } }));
            }

            if (AspFor.HasActions)
            {
                container.AppendLine(thTemplate.Result.Inject(new Dictionary<string, string> { { "ColumnName", _localizer["list_actions"] } }));
            }
            return container.ToString();
        }
    }
}
