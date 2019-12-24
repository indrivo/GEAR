using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GR.Core.Razor.Extensions
{
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Get script declaration
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="path"></param>
        /// <param name="appendNewVersion"></param>
        /// <returns></returns>
        public static HtmlString GetScriptDeclaration(this IHtmlHelper htmlHelper, string path, bool appendNewVersion = true)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("<script src=\"{0}\" asp-append-version=\"{1}\"></script>", path, appendNewVersion.ToString());
            return new HtmlString(builder.ToString());
        }

        /// <summary>
        /// Get link declaration
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="path"></param>
        /// <param name="appendNewVersion"></param>
        /// <returns></returns>
        public static HtmlString GetLinkDeclaration(this IHtmlHelper htmlHelper, string path, bool appendNewVersion = true)
        {
            var builder = new StringBuilder();
            builder.AppendFormat("<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" asp-append-version=\"{1}\"/>", path, appendNewVersion.ToString());
            return new HtmlString(builder.ToString());
        }

        /// <summary>
        /// Include core scripts
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static HtmlString IncludeCoreJavascriptPlugins(this IHtmlHelper htmlHelper)
        {
            var builder = new StringBuilder();
            var coreScript = htmlHelper.GetScriptDeclaration("/assets/js/core.js");
            var cookieScript = htmlHelper.GetScriptDeclaration("/assets/js/cookie-plugin-v.0.1.js");
            builder.AppendLine(coreScript.Value);
            builder.AppendLine(cookieScript.Value);
            return new HtmlString(builder.ToString());
        }
    }
}
