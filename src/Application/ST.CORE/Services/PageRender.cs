using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.EntityFrameworkCore;
using ST.Audit.Models;
using ST.BaseBusinessRepository;
using ST.Configuration.Seed;
using ST.CORE.Services.Abstraction;
using ST.Entities.Data;
using ST.Entities.Models.Notifications;
using ST.Entities.Models.Pages;
using ST.Entities.Models.ViewModels;
using ST.Identity.Data.Permissions;
using ST.Identity.Services.Abstractions;
using ST.Notifications.Abstraction;

namespace ST.CORE.Services
{
	public class PageRender : IPageRender
	{
		private const string BasePath = "Static/Templates/";
		/// <summary>
		/// Context
		/// </summary>
		private readonly EntitiesDbContext _context;

		private readonly INotify<ApplicationRole> _notify;

		private readonly IHostingEnvironment _env;

		/// <summary>
		/// Inject cache service
		/// </summary>
		private ICacheService _cacheService;

		public PageRender(EntitiesDbContext context, ICacheService cacheService, INotify<ApplicationRole> notify, IHostingEnvironment env)
		{
			_context = context;
			_cacheService = cacheService;
			_notify = notify;
			_env = env;
		}

		/// <summary>
		/// Get Layout html
		/// </summary>
		/// <returns></returns>
		public (HtmlString, HtmlString) GetLayoutHtml(Guid? layoutId = null)
		{
			var code = GetLayoutCode(PageContentType.Html, "layout", layoutId);
			var arr = code.Split("@RenderBody()");
			return (new HtmlString(arr[0]), new HtmlString(arr[1]));
		}

		/// <summary>
		/// Get Layout css
		/// </summary>
		/// <returns></returns>
		public string GetLayoutCss(Guid? layoutId = null)
		{
			return GetLayoutCode(PageContentType.Css, "layout", layoutId);
		}

		/// <summary>
		/// Get layout js
		/// </summary>
		/// <returns></returns>
		public string GetLayoutJavaScript(Guid? layoutId = null)
		{
			return GetLayoutCode(PageContentType.Js, "layout", layoutId);
		}

		/// <summary>
		/// Get html of page
		/// </summary>
		/// <param name="pageName"></param>
		/// <returns></returns>
		public virtual HtmlString GetPageHtml(string pageName)
		{
			return new HtmlString(GetLayoutCode(PageContentType.Html, pageName));
		}
		/// <summary>
		/// Get css of page
		/// </summary>
		/// <param name="pageName"></param>
		/// <returns></returns>
		public virtual string GetPageCss(string pageName)
		{
			return GetLayoutCode(PageContentType.Css, pageName);
		}

		/// <summary>
		/// Get js scripts of page
		/// </summary>
		/// <param name="pageName"></param>
		/// <returns></returns>
		public virtual string GetPageJavaScript(string pageName)
		{
			return GetLayoutCode(PageContentType.Js, pageName);
		}

		/// <summary>
		/// Save page data
		/// </summary>
		/// <param name="pageId"></param>
		/// <param name="html"></param>
		/// <param name="css"></param>
		/// <param name="js"></param>
		/// <returns></returns>
		public virtual ResultModel SavePageContent(Guid pageId, string html, string css, string js = null)
		{
			var result = new ResultModel();
			if (pageId == Guid.Empty) return result;
			var page = _context.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Id == pageId);
			if (page == null) return result;
			page.Settings.CssCode = css;
			page.Settings.JsCode = js;
			page.Settings.HtmlCode = html;
			try
			{
				_context.Pages.Update(page);
				_context.SaveChanges();
				result.IsSuccess = true;
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
		public virtual ResultModel<IEnumerable<PageScript>> GetPageScripts(Guid pageId)
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
		public virtual ResultModel<IEnumerable<PageStyle>> GetPageStyles(Guid pageId)
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
		private string GetLayoutCode(PageContentType type, string pageName = "layout", Guid? pageId = null)
		{
			try
			{
				var layout = pageId == null
					? _context.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Settings.Name == pageName)
					: _context.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Id.Equals(pageId));

				if (layout == null) return string.Empty;
				var code = string.Empty;
				switch (type)
				{
					case PageContentType.Html:
						code = layout.Settings.HtmlCode;
						break;
					case PageContentType.Js:
						code = layout.Settings.JsCode;
						break;
					case PageContentType.Css:
						code = layout.Settings.CssCode;
						break;
				}

				return code;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			return string.Empty;
		}

		/// <summary>
		/// Generate list page type
		/// </summary>
		/// <param name="name"></param>
		/// <param name="path"></param>
		/// <param name="viewModelId"></param>
		/// <returns></returns>
		public virtual async Task<ResultModel<Guid>> GenerateListPageType([Required] string name, string path, [Required] Guid viewModelId)
		{
			if (string.IsNullOrEmpty(name) || viewModelId.Equals(Guid.Empty)) return default;
			var match = _context.Pages.Include(x => x.Settings)
				.FirstOrDefault(x => x.Settings.Name.ToLower().Equals(name.ToLower()));
			var viewModel = _context.ViewModels
					.Include(x => x.TableModel)
					.Include(x => x.ViewModelFields)
					.FirstOrDefault(x => x.Id.Equals(viewModelId));
			if (viewModel == null) return default;

			if (match != null)
			{
				return default;
			}

			var pageId = Guid.NewGuid();

			var page = new Page
			{
				Id = pageId,
				Created = DateTime.Now,
				Changed = DateTime.Now,
				PageTypeId = PageManager.PageTypes[1].Id,
				LayoutId = PageManager.Layouts[0],
				Path = $"/{path}",
				Settings = new PageSettings
				{
					Name = name,
					Description = "Generated page",
					Title = name
				},
				IsLayout = false
			};

			try
			{
				_context.Pages.Add(page);
				_context.SaveChanges();

				var fileInfo = _env.ContentRootFileProvider.GetFileInfo($"{BasePath}/listDefaultTemplate.html");
				var reader = new StreamReader(fileInfo.CreateReadStream());
				var template = await reader.ReadToEndAsync();
				var listId = Guid.NewGuid();
				template = template.Replace("#Title", name);
				template = template.Replace("#SubTitle", name);
				template = template.Replace("#EntityName", viewModel.TableModel.Name);
				template = template.Replace("#ViewModelId", viewModel.Id.ToString());
				template = template.Replace("#ListId", listId.ToString());

				var tableHead = new StringBuilder();

				foreach (var line in viewModel.ViewModelFields.ToList().OrderBy(x => x.Order))
					tableHead.AppendLine($"<th translate='{line.Translate}'>{line.Name}</th>");
				tableHead.AppendLine("<th>Actions</th>");

				template = template.Replace("#TableHead", tableHead.ToString());

				SavePageContent(pageId, template, "", string.Empty);
			}
			catch (Exception e)
			{
				Debug.WriteLine(e);
				return default;
			}

			await _notify.SendNotificationAsync(new SystemNotifications
			{
				Content = $"New page generated with name {page.Settings.Name}  and route {page.Path}",
				Subject = "Info",
				NotificationTypeId = NotificationType.Info
			});

			return new ResultModel<Guid>
			{
				IsSuccess = true,
				Result = pageId
			};
		}

		/// <summary>
		/// Generate form page
		/// </summary>
		/// <param name="formId"></param>
		/// <param name="path"></param>
		/// <param name="pageName"></param>
		/// <returns></returns>
		public virtual async Task<ResultModel> GenerateFormPage(Guid formId, string path, string pageName)
		{
			var fileInfo = _env.ContentRootFileProvider.GetFileInfo($"{BasePath}/formDefaultTemplate.html");
			var reader = new StreamReader(fileInfo.CreateReadStream());
			var template = await reader.ReadToEndAsync();
			template = template.Replace("#FormId", formId.ToString());
			var page = new Page
			{
				Created = DateTime.Now,
				Changed = DateTime.Now,
				PageTypeId = PageManager.PageTypes[1].Id,
				LayoutId = PageManager.Layouts[0],
				Path = path,
				Settings = new PageSettings
				{
					Name = pageName,
					Description = "Generated page",
					Title = pageName
				},
				IsLayout = false
			};
			try
			{
				_context.Pages.Add(page);
				_context.SaveChanges();
				SavePageContent(page.Id, template, string.Empty, string.Empty);
				return new ResultModel
				{
					IsSuccess = true
				};
			}
			catch (Exception e)
			{
				return new ResultModel
				{
					Errors = new List<IErrorModel>
					{
						new ErrorModel(string.Empty, e.ToString())
					}
				};
			}
		}

		/// <summary>
		/// Generate view model
		/// </summary>
		/// <param name="entityId"></param>
		/// <returns></returns>
		public virtual async Task<ResultModel<Guid>> GenerateViewModel(Guid entityId)
		{
			var table = _context.Table.Include(x => x.TableFields).FirstOrDefault(x => x.Id.Equals(entityId));
			if (table == null) return default;
			var id = Guid.NewGuid();
			var fields = new List<ViewModelFields>();
			var model = new ViewModel
			{
				Id = id,
				Name = $"{table.Name}_{id}",
				TableModel = table
			};
			var c = 0;
			var props = typeof(ExtendedModel).GetProperties().ToList();

			fields.AddRange(props.Select(x => new ViewModelFields
			{
				Order = c++,
				Name = x.Name,
				Template = $"`${{row.{x.Name[0].ToString().ToLower()}{x.Name.Substring(1, x.Name.Length - 1)}}}`"
			}));

			fields.AddRange(table.TableFields.Select(x => new ViewModelFields
			{
				Order = c++,
				Name = x.Name,
				TableModelFields = x,
				Template = $"`${{row.{x.Name[0].ToString().ToLower()}{x.Name.Substring(1, x.Name.Length - 1)}}}`"
			}));

			model.ViewModelFields = fields;
			await _context.ViewModels.AddAsync(model);
			try
			{
				await _context.SaveChangesAsync();
				return new ResultModel<Guid> { IsSuccess = true, Result = id };
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex);
				return default;
			}
		}
	}

	public enum PageContentType
	{
		Html, Js, Css
	}
}
