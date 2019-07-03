﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Localization;
using ST.Core.Razor.Enums;
using ST.Core.Razor.Extensions;
using ST.Core.Razor.Helpers;
using ST.Core.Razor.TagHelpersStructures;

namespace ST.Core.Razor.TagHelpers
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
            //output.TagName = "div";
            string docInfo = null;
            var addInfo = new StringBuilder();
            var template = TemplateManager.GetTagHelperTemplate(Templates.ListTemplate);
            var addTemplate = TemplateManager.GetTagHelperTemplate(Templates.ListAddSectionTemplate);
            if (addTemplate.IsSuccess)
                AspFor.HeadButtons.ToList().ForEach(button =>
                {
                    var bAttrs = string.Join(" ", button.HtmlAttributes.Select(x => x.ToString()));
                    addInfo.Append(addTemplate.Result
                        .Inject(button)
                        .Inject(new Dictionary<string, string>
                        {
                            {"Attrs", bAttrs}
                        }));
                });

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

            var tInlineStyles = string.Join(" " , AspFor.StyleAttributes.Select(x => x.ToString()));
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
                                {
                                    attrs.Append(new HtmlAttribute(action.ButtonEvent.GetEvent, action.ButtonEvent.JsEventHandler));
                                }
                            }

                            if (action.ActionParameters.Any())
                            {
                                var actionsParams = new StringBuilder();
                                action.ActionParameters.ToList().ForEach(x => { actionsParams.AppendLine($"{x.ToString()}&"); });
                                action.Url = $"{action.Url}?{actionsParams}";
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

            if (AspFor.Api.Parameters.Any())
            {
                AspFor.Api.Url = $"{AspFor.Api.Url}?{string.Join("&", AspFor.Api.Parameters.Select((x) => x.Key + "=" + x.Value.ToString()))}";
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
                    column.HtmlAttributes.ToList().ForEach(x => { colAttrs.AppendLine($"{x.ToString()} "); });
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
