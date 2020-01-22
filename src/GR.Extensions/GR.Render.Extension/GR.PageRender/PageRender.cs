using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GR.Cache.Abstractions;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Filters;
using GR.Entities.Abstractions.Constants;
using GR.Entities.Data;
using GR.Entities.Security.Abstractions;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.MultiTenant.Abstractions;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Constants;
using GR.PageRender.Abstractions.Enums;
using GR.PageRender.Abstractions.Events;
using GR.PageRender.Abstractions.Events.EventArgs;
using GR.PageRender.Abstractions.Helpers;
using GR.PageRender.Abstractions.Models.Pages;
using GR.PageRender.Abstractions.Models.ViewModels;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace GR.PageRender
{
    public class PageRender : IPageRender
    {
        private const string BasePath = "Templates/";

        #region Injectable
        /// <summary>
        /// Context
        /// </summary>
        private readonly EntitiesDbContext _context;

        /// <summary>
        /// Inject page context
        /// </summary>
        private readonly IDynamicPagesContext _pagesContext;

        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;

        /// <summary>
        /// Inject cache service
        /// </summary>
        private readonly ICacheService _cacheService;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly UserManager<GearUser> _userManager;

        /// <summary>
        /// Inject http context
        /// </summary>
        private readonly IHttpContextAccessor _contextAccessor;

        /// <summary>
        /// Inject app provider
        /// </summary>
        private readonly IAppProvider _appProvider;

        /// <summary>
        /// Inject role access manager
        /// </summary>
        private readonly IEntityRoleAccessService _entityRoleAccessService;
        #endregion

        public PageRender(EntitiesDbContext context, ICacheService cacheService, UserManager<GearUser> userManager, IHttpContextAccessor contextAccessor, IDynamicPagesContext pagesContext, IOrganizationService<Tenant> organizationService, IAppProvider appProvider, IEntityRoleAccessService entityRoleAccessService)
        {
            _context = context;
            _cacheService = cacheService;
            _userManager = userManager;
            _contextAccessor = contextAccessor;
            _pagesContext = pagesContext;
            _organizationService = organizationService;
            _appProvider = appProvider;
            _entityRoleAccessService = entityRoleAccessService;
        }

        private async Task<GearUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(_contextAccessor.HttpContext.User);
        }

        /// <summary>
        /// Get Layout html
        /// </summary>
        /// <returns></returns>
        public virtual async Task<(HtmlString, HtmlString)> GetLayoutHtml(Guid? layoutId = null)
        {
            var (code, _) = await GetLayoutCode(PageContentType.Html, "layout", layoutId);
            if (string.IsNullOrEmpty(code))
            {
                return (new HtmlString("<h1 style=\"color: red\">Layout not configured!</h1>"), new HtmlString(""));
            }
            if (!code.Contains("@RenderBody()"))
            {
                return (new HtmlString("<h1 style=\"color: red\">Layout must have @RenderBody() section</h1>"), new HtmlString(""));
            }
            var routeData = _contextAccessor.HttpContext.GetRouteData();
            var data = new Dictionary<string, string>
            {
                { "AppName", await _appProvider.GetAppName("core")},
                { "SystemYear", DateTime.Now.Year.ToString() },
                { "RouteController", routeData.Values["controller"].ToString() },
                { "RouteView", routeData.Values["action"].ToString() }
            };
            var user = await GetCurrentUserAsync();
            if (user != null)
            {
                var organization = _organizationService.GetUserOrganization(user);
                if (organization != null)
                {
                    data.Add("Organization", organization.Name);
                }
                data.Add("UserName", user.UserName);
                data.Add("UserEmail", user.Email);
                data.Add("UserImagePath", $"/Users/GetImage?id={user.Id}");
            }


            code = code.Inject(data);
            var arr = code.Split("@RenderBody()");
            return (new HtmlString(arr[0]), new HtmlString(arr[1]));
        }

        /// <summary>
        /// Get Layout css
        /// </summary>
        /// <returns></returns>
        public virtual async Task<string> GetLayoutCss(Guid? layoutId = null)
        {
            var (content, _) = await GetLayoutCode(PageContentType.Css, "layout", layoutId);
            return content;
        }

        /// <summary>
        /// Get layout js
        /// </summary>
        /// <returns></returns>
        public virtual async Task<string> GetLayoutJavaScript(Guid? layoutId = null)
        {
            var (content, _) = await GetLayoutCode(PageContentType.Js, "layout", layoutId);
            return content;
        }

        /// <summary>
        /// Get html of page
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public virtual async Task<HtmlString> GetPageHtml(string pageName)
        {
            var (content, _) = await GetLayoutCode(PageContentType.Html, pageName);
            return new HtmlString(content);
        }
        /// <summary>
        /// Get css of page
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public virtual async Task<string> GetPageCss(string pageName)
        {
            var (content, _) = await GetLayoutCode(PageContentType.Css, pageName);
            return content;
        }

        /// <summary>
        /// Get js scripts of page
        /// </summary>
        /// <param name="pageName"></param>
        /// <returns></returns>
        public virtual async Task<string> GetPageJavaScript(string pageName)
        {
            var (content, _) = await GetLayoutCode(PageContentType.Js, pageName);
            return content;
        }

        /// <summary>
        /// Save page data
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="html"></param>
        /// <param name="css"></param>
        /// <param name="js"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SavePageContent(Guid pageId, string html, string css, string js = null)
        {
            var result = new ResultModel();
            if (pageId == Guid.Empty) return result;
            var page = await GetPageAsync(pageId);
            if (page == null) return result;
            page.Settings.CssCode = css;
            page.Settings.HtmlCode = html;
            try
            {
                _pagesContext.Pages.Update(page);
                _pagesContext.SaveChanges();
                await _cacheService.RemoveAsync($"{PageRenderConstants.PageCacheIdentifier}{pageId}");
                result.IsSuccess = true;
                DynamicUiEvents.Pages.PageUpdated(new PageCreatedEventArgs
                {
                    PageId = pageId,
                    PageName = page.Settings.Name
                });
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
        public virtual async Task<ResultModel<IEnumerable<PageScript>>> GetPageScripts(Guid pageId)
        {
            var page = await GetPageAsync(pageId);
            if (page == null) return new ResultModel<IEnumerable<PageScript>>();
            var scrips = page.PageScripts?.OrderBy(x => x.Order).ToList();
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
        public virtual async Task<ResultModel<IEnumerable<PageStyle>>> GetPageStyles(Guid pageId)
        {
            var page = await GetPageAsync(pageId);
            if (page == null) return new ResultModel<IEnumerable<PageStyle>>();
            var scrips = page.PageStyles?.OrderBy(x => x.Order).ToList();
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
        private async Task<(string, Page)> GetLayoutCode(PageContentType type, string pageName = "layout", Guid? pageId = null)
        {
            try
            {
                var layout = pageId == null
                    ? _pagesContext.Pages.Include(x => x.Settings).FirstOrDefault(x => x.Settings.Name == pageName)
                    : await GetPageAsync(pageId.Value);

                if (layout == null) return (string.Empty, null);
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

                return (code, layout);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return (string.Empty, null);
        }

        /// <summary>
        /// Generate list page type
        /// </summary>
        /// <param name="name"></param>
        /// <param name="path"></param>
        /// <param name="viewModelId"></param>
        /// <param name="addPath"></param>
        /// <param name="editPath"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> GenerateListPageType([Required] string name, string path,
            [Required] Guid viewModelId, string addPath = "#", string editPath = "#")
        {
            if (string.IsNullOrEmpty(name) || viewModelId.Equals(Guid.Empty)) return default;
            var match = _pagesContext.Pages.Include(x => x.Settings)
                .FirstOrDefault(x => x.Path.ToLower().Equals($"/{path}".ToLower()));
            var viewModel = _pagesContext.ViewModels
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
                PageTypeId = PageSeeder.PageTypes[1].Id,
                LayoutId = PageSeeder.Layouts.DefaultCosmoLayout,
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
                _pagesContext.Pages.Add(page);
                _pagesContext.SaveChanges();
                var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, $"{BasePath}/listDefaultTemplate.html"), FileMode.Open);
                var reader = new StreamReader(fileStream);
                var listId = Guid.NewGuid();

                var tableHead = new StringBuilder();

                foreach (var line in viewModel.ViewModelFields.ToList().OrderBy(x => x.Order))
                    tableHead.AppendLine($"<th translate='{line.Translate}'>{line.Name}</th>");
                tableHead.AppendLine("<th>Actions</th>");
                var dictData = new Dictionary<string, string>
                {
                    { "Title", name },
                    { "SubTitle", name },
                    { "EntityName", viewModel.TableModel.Name },
                    { "ViewModelId", viewModel.Id.ToString() },
                    { "ListId", listId.ToString() },
                    { "AddPagePath", addPath },
                    { "EditPagePath", editPath },
                    { "TableHead", tableHead.ToString() }
                };

                var template = (await reader.ReadToEndAsync()).Inject(dictData);
                reader.Close();
                fileStream.Close();
                await SavePageContent(pageId, template, string.Empty, string.Empty);
                DynamicUiEvents.Pages.PageCreated(new PageCreatedEventArgs
                {
                    PageId = pageId,
                    PageName = page.Settings.Name
                });
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                return default;
            }

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
            var fileStream = new FileStream(Path.Combine(AppContext.BaseDirectory, $"{BasePath}/formDefaultTemplate.html"), FileMode.Open);
            var reader = new StreamReader(fileStream);

            var dictData = new Dictionary<string, string>
            {
                { "FormId", formId.ToString() }
            };
            var template = (await reader.ReadToEndAsync()).Inject(dictData);
            reader.Close();
            fileStream.Close();
            var page = new Page
            {
                Created = DateTime.Now,
                Changed = DateTime.Now,
                PageTypeId = PageSeeder.PageTypes[1].Id,
                LayoutId = PageSeeder.Layouts.DefaultCosmoLayout,
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
                _pagesContext.Pages.Add(page);
                _pagesContext.SaveChanges();
                await SavePageContent(page.Id, template, string.Empty, string.Empty);
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
        /// Get page type
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public virtual async Task<Page> GetPageAsync(Guid? pageId)
        {
            if (pageId == null) return null;
            var cachedPage = await _cacheService.GetAsync<Page>($"{PageRenderConstants.PageCacheIdentifier}{pageId}");
            if (cachedPage != null) return cachedPage;
            var page = await _pagesContext.Pages
                .Include(x => x.PageScripts)
                .Include(x => x.PageStyles)
                .Include(x => x.PageType)
                .Include(x => x.Layout)
                .Include(x => x.Settings)
                .Include(x => x.RolePagesAcls)
                .FirstOrDefaultAsync(x => x.Id.Equals(pageId));
            await _cacheService.SetAsync($"{PageRenderConstants.PageCacheIdentifier}{pageId}", page);
            return page;
        }

        /// <summary>
        /// Generate view model
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> GenerateViewModel(Guid entityId)
        {
            var result = new ResultModel<Guid>();
            var table = _context.Table.Include(x => x.TableFields).FirstOrDefault(x => x.Id.Equals(entityId));
            if (table == null) return result;
            var id = Guid.NewGuid();
            var fields = new List<ViewModelFields>();
            var model = new ViewModel
            {
                Id = id,
                Name = $"{table.Name}_{id}",
                TableModelId = table.Id
            };
            var c = 0;

            fields.AddRange(table.TableFields.Select(x => new ViewModelFields
            {
                Order = c++,
                Name = x.Name,
                TableModelFieldsId = x.Id,
                Template = GetTemplate(x.Name, x.DataType)
            }));

            var props = typeof(BaseModel).GetProperties().ToList();

            fields.AddRange(props.Select(x => new ViewModelFields
            {
                Order = c++,
                Name = x.Name,
                Template = GetTemplate(x.Name, x.PropertyType.Name)
            }));

            model.ViewModelFields = fields;
            await _pagesContext.ViewModels.AddAsync(model);
            try
            {
                await _pagesContext.SaveChangesAsync();
                result.IsSuccess = true;
                result.Result = id;
                return result;
            }
            catch (Exception ex)
            {
                result.Errors.Add(new ErrorModel("throw", ex.Message));
                return result;
            }
        }

        /// <summary>
        /// Get template
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static string GetTemplate(string name, string type)
        {
            var parsedField = $"{name[0].ToString().ToLower()}{name.Substring(1, name.Length - 1)}";
            var result = $"`${{row.{parsedField}}}`";
            switch (type)
            {
                case TableFieldDataType.Boolean:
                    {
                        result = $"`${{row.{parsedField}?`<i class='fa fa-check'></i>`:`<i class='fa fa-minus'></i>`}}`";
                    }
                    break;
            }
            return result;
        }


        public async Task<DTResult<object>> FilterJqueryDataTableRequestAsync(DTParameters param, Guid? viewModelId, ICollection<Filter> filters)
        {
            var defaultResult = new DTResult<object>
            {
                Data = new List<object>()
            };

            if (viewModelId == Guid.Empty) return defaultResult;
            var viewModel = await _pagesContext.ViewModels
                .Include(x => x.TableModel)
                .ThenInclude(x => x.TableFields)
                .Include(x => x.ViewModelFields)
                .ThenInclude(x => x.TableModelFields)
                .Include(x => x.ViewModelFields)
                .ThenInclude(x => x.Configurations)
                .FirstOrDefaultAsync(x => x.Id.Equals(viewModelId));

            if (viewModel == null) return defaultResult;
            if (!await _entityRoleAccessService.HaveReadAccessAsync(viewModel.TableModelId)) return defaultResult;

            filters?.ToList().ForEach(x =>
            {
                x.SetValue();
                x.AdaptTypes();
            });




            return defaultResult;
        }
    }
}
