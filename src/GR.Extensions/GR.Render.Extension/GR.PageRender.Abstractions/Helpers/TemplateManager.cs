using System;
using System.IO;
using System.Threading.Tasks;
using GR.Cache.Abstractions;
using GR.Core;
using GR.Core.Helpers;
using GR.PageRender.Abstractions.Models.RenderTemplates;
using Microsoft.EntityFrameworkCore;

namespace GR.PageRender.Abstractions.Helpers
{
    public static class TemplateManager
    {
        /// <summary>
        /// Seed templates into database and into cache
        /// </summary>
        /// <returns></returns>
        public static async Task SeedAsync()
        {
            var context = IoC.Resolve<IDynamicPagesContext>();
            var cache = IoC.Resolve<ICacheService>();
            if (await context.Templates.AnyAsync()) return;
            var files = Directory.GetFiles(Path.Combine(AppContext.BaseDirectory, "Static/Templates/"));
            foreach (var file in files)
            {
                var content = File.ReadAllText(file);
                var info = new FileInfo(file);
                var template = new Template
                {
                    Name = info.Name,
                    TenantId = GearSettings.TenantId,
                    Author = "system",
                    ModifiedBy = "system",
                    IdentifierName = $"template_{info.Name}",
                    Value = content
                };
                await context.Templates.AddAsync(template);
                await cache.SetAsync(template.IdentifierName, new TemplateCacheModel
                {
                    Identifier = template.IdentifierName,
                    Value = template.Value
                });
            }

            try
            {
                await context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
