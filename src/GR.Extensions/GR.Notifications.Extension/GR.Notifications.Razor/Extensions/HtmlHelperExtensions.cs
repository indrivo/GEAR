using Microsoft.AspNetCore.Mvc.Rendering;
using GR.Core.Razor.Extensions;
using Microsoft.AspNetCore.Html;

namespace GR.Notifications.Razor.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Include notification plugin
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static HtmlString IncludeNotificationJavascriptPlugins(this IHtmlHelper htmlHelper)
        {
            var script = htmlHelper.GetScriptDeclaration("/assets/notification-plugins/notification-plugin-v.0.1.js");
            return script;
        }
    }
}
