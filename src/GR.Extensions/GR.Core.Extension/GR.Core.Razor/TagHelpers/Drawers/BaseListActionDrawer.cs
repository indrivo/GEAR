using System.Collections.Generic;
using System.Linq;
using System.Text;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Templates;
using GR.Core.Helpers.Templates.Enums;
using GR.Core.Razor.Helpers;
using GR.Core.Razor.TagHelpersStructures;

namespace GR.Core.Razor.TagHelpers.Drawers
{
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Sample actions inline")]
    public class BaseListActionDrawer
    {
        /// <summary>
        /// Render list action
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual string Render(ListTagHelperModel model)
        {
            var actions = new StringBuilder();
            if (!model.HasActions || !model.ListActions.Any()) return actions.ToString();
            var renderColumnTemplate = TemplateManager.GetTemplateBody(Templates.ListButtonGroupRenderJsTemplate, TemplateType.Js);
            if (!renderColumnTemplate.IsSuccess) return actions.ToString();
            var buttons = new StringBuilder();
            var buttonTemplate = TemplateManager.GetTemplateBody(Templates.ATemplate);
            if (buttonTemplate.IsSuccess)
            {
                foreach (var action in model.ListActions)
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
                        action.ActionParameters.ToList().ForEach(x => { actionsParams.AppendLine($"{x}&"); });
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

            return actions.ToString();
        }
    }
}
