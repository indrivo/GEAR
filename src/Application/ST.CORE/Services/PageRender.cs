using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using ST.BaseBusinessRepository;
using ST.CORE.Services.Abstraction;
using ST.Entities.Data;
using ST.Entities.Models.Pages;

namespace ST.CORE.Services
{
	public class PageRender : IPageRender
	{
		/// <summary>
		/// Pages path
		/// </summary>
		private static string DirectoryPath
		{
			get
			{
				var baseDirectory = AppContext.BaseDirectory;
				var path = Path.Combine(baseDirectory, "static\\pages");
				return path;
			}
		}
		/// <summary>
		/// Context
		/// </summary>
		private readonly EntitiesDbContext _context;

		public PageRender(EntitiesDbContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Get Layout html
		/// </summary>
		/// <returns></returns>
		public (HtmlString, HtmlString) GetLayoutHtml(Guid? layoutId = null)
		{
			var code = GetLayoutCode("html", "layout", layoutId);
			var arr = code.Split("@RenderBody()");
			return (new HtmlString(arr[0]), new HtmlString(arr[1]));
		}

		/// <summary>
		/// Get Layout css
		/// </summary>
		/// <returns></returns>
		public string GetLayoutCss(Guid? layoutId = null)
		{
			return GetLayoutCode("css", "layout", layoutId);
		}

		/// <summary>
		/// Get layout js
		/// </summary>
		/// <returns></returns>
		public string GetLayoutJavaScript(Guid? layoutId = null)
		{
			return GetLayoutCode("js", "layout", layoutId);
		}

		/// <summary>
		/// Create page
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public ResultModel<string> CreatePage(string name)
		{
			if (string.IsNullOrEmpty(name)) return default;
			var dPath = Path.Combine(DirectoryPath, name);
			if (Directory.Exists(dPath)) return default;

			var d = Directory.CreateDirectory(dPath);
			if (d == null) return default;
			try
			{
				File.Create(Path.Combine(dPath, $"{name}.html")).Close();
				File.Create(Path.Combine(dPath, $"{name}.css")).Close();
				File.Create(Path.Combine(dPath, $"{name}.js")).Close();

				return new ResultModel<string>
				{
					IsSuccess = true,
					Result = dPath
				};
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return default;
			}
		}
		/// <summary>
		/// Update page name 
		/// </summary>
		/// <param name="path"></param>
		/// <param name="oldName"></param>
		/// <param name="newName"></param>
		/// <returns></returns>
		public ResultModel UpdatePageName(string path, string oldName, string newName)
		{
			if (string.IsNullOrEmpty(path)) return default;
			if (oldName.ToLower().Equals(newName)) return new ResultModel { IsSuccess = true };
			try
			{
				var parent = new DirectoryInfo(path).Parent?.FullName;
				var pathNew = Path.Combine(parent, newName);
				Directory.Move(path, pathNew);
				var files = Directory.GetFiles(pathNew);
				foreach (var file in files)
				{
					var oldFile = new FileInfo(file);
					var newFile = oldFile.Directory?.FullName;
					File.Move(file, Path.Combine(newFile, oldFile.Name));
				}
				return new ResultModel { IsSuccess = true };
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return default;
		}

		/// <summary>
		/// Delete page
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public ResultModel DeletePage(string path)
		{
			if (string.IsNullOrEmpty(path)) return default;
			try
			{
				var files = Directory.GetFiles(path);
				if (files.Any())
				{
					foreach (var file in files)
					{
						File.Delete(file);
					}
				}

				Directory.Delete(path, true);
				return new ResultModel
				{
					IsSuccess = true
				};
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return default;
			}
		}

		/// <summary>
		/// Get html of page
		/// </summary>
		/// <param name="pageName"></param>
		/// <returns></returns>
		public HtmlString GetPageHtml(string pageName)
		{
			return new HtmlString(GetLayoutCode("html", pageName));
		}
		/// <summary>
		/// Get css of page
		/// </summary>
		/// <param name="pageName"></param>
		/// <returns></returns>
		public string GetPageCss(string pageName)
		{
			return GetLayoutCode("css", pageName);
		}

		/// <summary>
		/// Get js scripts of page
		/// </summary>
		/// <param name="pageName"></param>
		/// <returns></returns>
		public string GetPageJavaScript(string pageName)
		{
			return GetLayoutCode("js", pageName);
		}

		/// <summary>
		/// Save page data
		/// </summary>
		/// <param name="pageId"></param>
		/// <param name="html"></param>
		/// <param name="css"></param>
		/// <returns></returns>
		public ResultModel SavePageContent(Guid pageId, string html, string css, string js = null)
		{
			var result = new ResultModel();
			if (pageId == Guid.Empty) return result;
			var page = _context.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Id == pageId);
			if (page == null) return result;
			if (!Directory.Exists(page.Settings.PhysicPath)) return result;

			var cssPath = Path.Combine(page.Settings.PhysicPath, $"{page.Settings.Name}.css");
			var htmlPath = Path.Combine(page.Settings.PhysicPath, $"{page.Settings.Name}.html");
			var jsPath = Path.Combine(page.Settings.PhysicPath, $"{page.Settings.Name}.js");

			try
			{
				File.WriteAllText(cssPath, css);
				File.WriteAllText(htmlPath, html);
				if (!string.IsNullOrEmpty(js))
				{
					File.WriteAllText(jsPath, js);
				}
				result.IsSuccess = true;
				return result;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return result;
		}

		/// <summary>
		/// Get page scripts
		/// </summary>
		/// <param name="pageId"></param>
		/// <returns></returns>
		public ResultModel<IEnumerable<PageScript>> GetPageScripts(Guid pageId)
		{
			var page = _context.Pages.FirstOrDefault(x => x.Id.Equals(pageId));
			if (page == null) return default;
			var scrips = _context.PageScripts.Where(x => x.PageId.Equals(pageId)).OrderBy(x => x.Order).ToList();
			return new ResultModel<IEnumerable<PageScript>>
			{
				IsSuccess = true,
				Result = scrips
			};
		}

		/// <summary>
		/// Get Page styles
		/// </summary>
		/// <param name="pageId"></param>
		/// <returns></returns>
		public ResultModel<IEnumerable<PageStyle>> GetPageStyles(Guid pageId)
		{
			var page = _context.Pages.FirstOrDefault(x => x.Id.Equals(pageId));
			if (page == null) return default;
			var scrips = _context.PageStyles.Where(x => x.PageId.Equals(pageId)).OrderBy(x => x.Order).ToList();
			return new ResultModel<IEnumerable<PageStyle>>
			{
				IsSuccess = true,
				Result = scrips
			};
		}


		/// <summary>
		/// Get code by type from layout
		/// </summary>
		/// <param name="type"></param>
		/// <param name="pageName"></param>
		/// <param name="pageId"></param>
		/// <returns></returns>
		private string GetLayoutCode(string type, string pageName = "layout", Guid? pageId = null)
		{
			try
			{
				var layout = pageId == null
					? _context.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Settings.Name == pageName)
					: _context.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Id.Equals(pageId));
				if (layout == null) return string.Empty;
				var file = layout.Settings.PhysicPath.Split("\\").LastOrDefault();
				var path = Path.Combine(layout.Settings.PhysicPath, $"{file}.{type}");
				var code = File.ReadAllText(path);
				return code;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return string.Empty;
		}
	}
}
