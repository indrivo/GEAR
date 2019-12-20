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
    }
}
