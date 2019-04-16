using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ST.Entities.Data;
using ST.Entities.Extensions;
using ST.Entities.Models.Pages;

namespace ST.Configuration.Seed
{
    public static class PageManager
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
        public static readonly List<Guid> Layouts = new List<Guid> {
            Guid.Parse("D12BDEB9-EC63-4AD6-A9AA-F47D8F1DEE55".ToLower())
        };

        /// <summary>
        /// Default Page Sync
        /// </summary>
        public static async Task SyncWebPagesAsync()
        {
            var context = IoC.Resolve<EntitiesDbContext>();
            //Add page types
            if (!await context.PageTypes.AnyAsync())
            {
                await context.PageTypes.AddRangeAsync(PageTypes);
                context.SaveChanges();
            }

            //Add pages
            if (context.Pages.Any()) return;
            try
            {
                var baseDirectory = AppContext.BaseDirectory;
                var path = Path.Combine(baseDirectory, "Static/Pages");

                var exists = Directory.Exists(path);
                if (!exists) return;
                var directories = Directory.GetDirectories(path);
                foreach (var dir in directories)
                {
                    var pagePath = Path.Combine(dir, "settings.json");
                    var page = ReadPageSettings(pagePath);
                    if (page == null) continue;
                    try
                    {
                        page.Settings.CssCode = await File.ReadAllTextAsync(Path.Combine(dir, $"{page.Settings.Identifier}.css"));
                        page.Settings.JsCode = await File.ReadAllTextAsync(Path.Combine(dir, $"{page.Settings.Identifier}.js"));
                        page.Settings.HtmlCode = await File.ReadAllTextAsync(Path.Combine(dir, $"{page.Settings.Identifier}.html"));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    page.PageTypeId = page.IsLayout ? PageTypes.First().Id : PageTypes[1].Id;
                    page.Created = DateTime.Now;
                    page.Changed = DateTime.Now;
                    context.Pages.Add(page);
                }
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }


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
        /// <summary>
        /// Read page settings
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static Page ReadPageSettings(string path)
        {
            try
            {
                using (Stream str = new FileStream(path, FileMode.Open, FileAccess.Read,
                    FileShare.ReadWrite))
                using (var sReader = new StreamReader(str))
                using (var reader = new JsonTextReader(sReader))
                {
                    var fileObj = JObject.Load(reader);
                    return fileObj.ToObject<Page>();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
            }

            return default;
        }
    }
}
