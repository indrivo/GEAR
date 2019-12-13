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
    [Author(Authors.LUPEI_NICOLAE, 1.1, "Add drawer for actions on dropdown")]
    public class DropDownListActionDrawer : BaseListActionDrawer
    {
        /// <summary>
        /// Actions name
        /// </summary>
        private readonly string _name = "${window.translate(\"list_actions\")}";

        public DropDownListActionDrawer()
        {

        }

        public DropDownListActionDrawer(string actionsName)
        {
            _name = actionsName;
        }


        public override string Render(ListTagHelperModel model)
        {
            var actions = new StringBuilder();
            if (!model.HasActions || !model.ListActions.Any()) return actions.ToString();
            var renderColumnTemplate = TemplateManager.GetTemplateBody(Templates.ListButtonDropdownRenderJsTemplate, TemplateType.Js);
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

                    foreach (var attr in action.Attributes) attrs.Append(attr);
                    if (!action.Attributes.Select(x => x.Name).Contains("class"))
                    {
                        attrs.Append(new HtmlAttribute("class", "dropdown-item"));
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
                .Inject(new Dictionary<string, string>
                {
                    { "Buttons", buttons.ToString() },
                    { "Name" , _name }
                }));

            return actions.ToString();
        }
    }
}
