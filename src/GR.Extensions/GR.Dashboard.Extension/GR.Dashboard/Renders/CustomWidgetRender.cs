using System;
using System.Dynamic;
using System.Text;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Helpers.Compilers;
using GR.Dashboard.Abstractions.Helpers.Enums;
using GR.Dashboard.Abstractions.Models.WidgetTypes;
using GR.Identity.Abstractions;

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
            const string errorTemplateMessage = "<h2 style='color: red;'>Something didn't work</h2>";
            Arg.NotNull(widget, nameof(ReportWidgetRender));
            var builder = new StringBuilder();
            if (widget.WidgetTemplateType.Equals(WidgetTemplateType.Razor))
            {
                var services = IoC.Container;
                if (widget.AllowCache)
                {
                    try
                    {
                        var cacheResult = RazorCompilerEngine.Compiler.TemplateCache.RetrieveTemplate(widget.Id.ToString());
                        var htmlTemplate = cacheResult.Success
                            ? RazorCompilerEngine.Compiler.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), services).ExecuteAsync()
                            : RazorCompilerEngine.Compiler.CompileRenderAsync(widget.Id.ToString(),
                                widget.Template, services).ExecuteAsync();
                        builder.AppendLine(htmlTemplate);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        builder.AppendLine(errorTemplateMessage);
                    }
                }
                else
                {
                    try
                    {
                        RazorCompilerEngine.Compiler.TemplateCache.Remove(widget.Id.ToString());
                        var htmlTemplate = RazorCompilerEngine.Compiler.CompileRenderAsync(widget.Id.ToString(),
                            widget.Template, services).ExecuteAsync();
                        builder.AppendLine(htmlTemplate);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        builder.AppendLine(errorTemplateMessage);
                    }
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