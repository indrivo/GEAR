using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GR.Core.Razor.Extensions
{
    public static class PartialViewStylesExtensions
    {
        private const string StylesKey = "DelayedStyles";

        public static IDisposable BeginPartialViewStyles(this IHtmlHelper helper)
        {
            return new StyleBlock(helper.ViewContext);
        }

        public static HtmlString PartialViewStyles(this IHtmlHelper helper)
        {
            return new HtmlString(string.Join(Environment.NewLine, GetPageStylesList(helper.ViewContext.HttpContext)));
        }

        private static List<string> GetPageStylesList(HttpContext httpContext)
        {
            var pageScripts = (List<string>)httpContext.Items[StylesKey];
            if (pageScripts == null)
            {
                pageScripts = new List<string>();
                httpContext.Items[StylesKey] = pageScripts;
            }
            return pageScripts;
        }

        private class StyleBlock : IDisposable
        {
            private readonly TextWriter _originalWriter;
            private readonly StringWriter _scriptsWriter;

            private readonly ViewContext _viewContext;

            public StyleBlock(ViewContext viewContext)
            {
                _viewContext = viewContext;
                _originalWriter = _viewContext.Writer;
                _viewContext.Writer = _scriptsWriter = new StringWriter();
            }

            public void Dispose()
            {
                _viewContext.Writer = _originalWriter;
                var pageScripts = GetPageStylesList(_viewContext.HttpContext);
                pageScripts.Add(_scriptsWriter.ToString());
            }
        }
    }
}
