using System;
using System.IO;
using System.Text;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Helpers.Compilers;
using GR.Dashboard.Abstractions.Models.WidgetTypes;

namespace GR.Dashboard.Renders
{
    public class ReportWidgetRender : IWidgetRenderer<ReportWidget>
    {
        //TODO: Investigate why embedded resource is not discovered
        /// <summary>
        /// Cache template
        /// </summary>
        /// YourAssembly.NamespaceOfRootType.Templates.View.cshtml
        private const string ReportTemplateCacheKey = "GR.Dashboard.Templates.Templates.ReportTemplate.cshtml";

        /// <inheritdoc />
        /// <summary>
        /// Render report widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public virtual string Render(ReportWidget widget)
        {
            Arg.NotNull(widget, nameof(ReportWidgetRender));
            var builder = new StringBuilder();
            var cacheResult = RazorCompilerEngine.Compiler.TemplateCache.RetrieveTemplate(ReportTemplateCacheKey);
            var htmlTemplate = cacheResult.Success
                ? RazorCompilerEngine.Compiler.RenderTemplateAsync(cacheResult.Template.TemplatePageFactory(), widget).ExecuteAsync()
                : RazorCompilerEngine.Compiler.CompileRenderAsync(ReportTemplateCacheKey,
                    RazorCompilerEngine.ReadTemplateFromFile(Path.Combine(AppContext.BaseDirectory, "Templates/ReportTemplate.cshtml")), widget).ExecuteAsync();

            builder.AppendLine(htmlTemplate);

            return builder.ToString();
        }
    }
}
