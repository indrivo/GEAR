using System.Text;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Dashboard.Abstractions;
using ST.Dashboard.Abstractions.Helpers.Compilers;
using ST.Dashboard.Abstractions.Helpers.Enums;
using ST.Dashboard.Abstractions.Models.WidgetTypes;

namespace ST.Dashboard.Renders
{
    public class CustomWidgetRender : IWidgetRenderer<CustomWidget>
    {
        /// <inheritdoc />
        /// <summary>
        /// Render custom widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public string Render(CustomWidget widget)
        {
            Arg.NotNull(widget, nameof(ReportWidgetRender));
            var builder = new StringBuilder();
            if (widget.WidgetTemplateType.Equals(WidgetTemplateType.Razor))
            {
                if (widget.AllowCache)
                {
                    var cacheResult = RazorCompilerEngine.Compiler.TemplateCache.RetrieveTemplate(widget.Id.ToString());
                    var htmlTemplate = cacheResult.Success
                        ? RazorCompilerEngine.Compiler.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), widget).ExecuteAsync()
                        : RazorCompilerEngine.Compiler.CompileRenderAsync(widget.Id.ToString(),
                            widget.Template, widget).ExecuteAsync();
                    builder.AppendLine(htmlTemplate);
                }
                else
                {
                    var htmlTemplate = RazorCompilerEngine.Compiler.CompileRenderAsync(widget.Id.ToString(),
                        widget.Template, widget).ExecuteAsync();
                    builder.AppendLine(htmlTemplate);
                }
            }
            else
            {
                builder.AppendLine(widget.Template);
            }

            return builder.ToString();
        }
    }
}