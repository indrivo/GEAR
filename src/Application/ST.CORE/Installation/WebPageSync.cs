using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ST.Entities.Data;
using ST.Entities.Models.Pages;

namespace ST.CORE.Installation
{
	public static class WebPageSync
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
					},
					new PageType
					{
						Id = Guid.Parse("19b130c5-c798-4a52-b65d-d3dbd51d708e"),
						Name = "Detail Page",
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
		/// <param name="context"></param>
		public static void SyncWebPages(EntitiesDbContext context)
		{
			//Add page types
			if (!context.PageTypes.Any())
			{
				context.PageTypes.AddRange(PageTypes);
				context.SaveChanges();
			}

			//Add pages
			if (context.Pages.Any()) return;
			try
			{
				var baseDirectory = AppContext.BaseDirectory;
				var path = Path.Combine(baseDirectory, "static\\pages");

				var exists = Directory.Exists(path);
				if (!exists) return;
				var directories = Directory.GetDirectories(path);
				foreach (var dir in directories)
				{
					var pagePath = Path.Combine(dir, "settings.json");
					var page = ReadPageSettings(pagePath);
					if (page == null) continue;
					page.Settings.PhysicPath = dir;
					page.PageTypeId = page.IsLayout ? PageTypes.First().Id : PageTypes[1].Id;
					page.Created = DateTime.Now;
					page.Changed = DateTime.Now;
					context.Pages.Add(page);
				}
				context.SaveChanges();

				var pages = context.Pages.ToList();
				var layout = pages.FirstOrDefault(x => x.IsLayout);
				if (layout == null) return;
				{
					foreach (var page in pages)
					{
						if (page.IsLayout) continue;
						page.Layout = layout;
						context.Update(page);
					}

					context.SaveChanges();
				}
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
			}


			//Add block categories
			if (!context.BlockCategories.Any())
			{
				var blockCategories = new List<string>() { "Basic", "Extra", "Forms", "Dynamic Entities", "Bootstrap" };

				context.BlockCategories.AddRange(blockCategories.Select(categoryName => new BlockCategory
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
					context.SaveChanges();
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
