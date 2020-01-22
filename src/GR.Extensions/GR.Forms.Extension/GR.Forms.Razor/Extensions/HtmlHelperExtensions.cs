using System.Text;
using GR.Core.Razor.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GR.Forms.Razor.Extensions
{
    public static class HtmlHelperExtensions
    {
        private static HtmlString _formJsScripts;

        private static HtmlString _formCssScripts;

        /// <summary>
        /// Include form js plugins
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static HtmlString IncludeFormsJavascriptPlugins(this IHtmlHelper htmlHelper)
        {
            if (_formJsScripts != null) return _formJsScripts;
            var builder = new StringBuilder();
            var formeoScript = htmlHelper.GetScriptDeclaration("/lib/formeo/dist/formeo.min.js");
            var customScript = htmlHelper.GetScriptDeclaration("/assets/FormModulePlugins/js/st-form-prototype-library-v.0.1.js");
            builder.AppendLine(formeoScript.Value);
            builder.AppendLine(customScript.Value);
            return _formJsScripts = new HtmlString(builder.ToString());
        }

        /// <summary>
        /// Get forms styles plugins
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static HtmlString IncludeFormsStylePlugins(this IHtmlHelper htmlHelper)
        {
            if (_formCssScripts != null) return _formCssScripts;
            var builder = new StringBuilder();
            var formeoScript = htmlHelper.GetLinkDeclaration("/assets/FormModulePlugins/css/formeo.css");
            builder.AppendLine(formeoScript.Value);
            return _formCssScripts = new HtmlString(builder.ToString());
        }
    }
}