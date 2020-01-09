using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.PageRender.Abstractions.Models.Pages;
using Microsoft.EntityFrameworkCore;

namespace GR.PageRender.Abstractions.Helpers
{
    public static class PageSeeder
    {
        /// <summary>
        /// page types
        /// </summary>
        public static readonly List<PageType> PageTypes = new List<PageType>()
                {
                    new PageType
                    {
                        Id = Guid.Parse("cb37c370-9f05-4e4b-947c-c972f73d5bc8"),
                        Name = "Layout Page",
                        Description = "Layout Description"
                    },
                    new PageType
                    {
                        Id = Guid.Parse("16116510-71f7-422f-9993-6cbcf2a697fe"),
                        Name = "Content Page",
                        Description = "Layout Description"
                    }
                };


        /// <summary>
        /// Layouts
        /// </summary>
        public static class Layouts
        {
            public static Guid DefaultCosmoLayout = Guid.Parse("587592ed-cd11-432e-b689-a5fce2a9859c");
        }

        /// <summary>
        /// Default Page Sync
        /// </summary>
        public static async Task SyncWebPagesAsync()
        {
            var context = IoC.Resolve<IDynamicPagesContext>();
            //Add page types
            if (!await context.PageTypes.AnyAsync())
            {
                await context.PageTypes.AddRangeAsync(PageTypes);
                await context.PushAsync();
            }

            //Add pages
            if (context.Pages.Any()) return;
            var baseDirectory = AppContext.BaseDirectory;
            var path = Path.Combine(baseDirectory, "Static/Pages");

            var exists = Directory.Exists(path);
            if (!exists) return;
            var directories = Directory.GetDirectories(path);
            foreach (var dir in directories)
            {
                var pagePath = Path.Combine(dir, "settings.json");
                var page = JsonParser.ReadObjectDataFromJsonFile<Page>(pagePath);
                if (page == null) continue;
                try
                {
                    page.Settings.CssCode = File.ReadAllText(Path.Combine(dir, $"{page.Settings.Identifier}.css"));
                    page.Settings.JsCode = File.ReadAllText(Path.Combine(dir, $"{page.Settings.Identifier}.js"));
                    page.Settings.HtmlCode = File.ReadAllText(Path.Combine(dir, $"{page.Settings.Identifier}.html"));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
                page.PageTypeId = page.IsLayout ? PageTypes.First().Id : PageTypes[1].Id;
                page.Created = DateTime.Now;
                page.Changed = DateTime.Now;
                await context.Pages.AddAsync(page);
            }
            await context.PushAsync();


            //Add block categories
            if (!await context.BlockCategories.AnyAsync())
            {
                var blockCategories = new List<string> { "Basic", "Extra", "Forms", "Dynamic Entities", "Bootstrap" };

                await context.BlockCategories.AddRangeAsync(blockCategories.Select(categoryName => new BlockCategory
                {
                    Created = DateTime.Now,
                    Description = $"{categoryName} description",
                    Name = categoryName,
                    Author = "system",
                    Changed = DateTime.Now,
                    ModifiedBy = "system"
                }));

                await context.PushAsync();
            }
        }
    }
}
