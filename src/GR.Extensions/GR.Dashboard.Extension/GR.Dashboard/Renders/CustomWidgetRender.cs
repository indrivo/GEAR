using System.Text;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Helpers.Compilers;
using GR.Dashboard.Abstractions.Helpers.Enums;
using GR.Dashboard.Abstractions.Models.WidgetTypes;

namespace GR.Dashboard.Renders
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