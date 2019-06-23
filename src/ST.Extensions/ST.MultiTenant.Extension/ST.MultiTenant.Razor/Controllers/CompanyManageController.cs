using System.Collections.Generic;
using System.Linq;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ST.Identity.Abstractions;
using ST.Identity.Data.MultiTenants;
using ST.MultiTenant.Abstractions;
using ST.Cache.Abstractions;
using ST.Core;
using ST.Core.Abstractions;
using ST.Core.BaseControllers;
using ST.Core.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels;
using ST.Core.Razor.TagHelpersStructures;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.Entities.Data;
using ST.Identity.Data;
using ST.MultiTenant.Razor.ViewModels;
using ST.Notifications.Abstractions;

namespace ST.MultiTenant.Razor.Controllers
{
    [Authorize(Roles = "Company Administrator")]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class CompanyManageController : BaseCrudController<ApplicationDbContext, ApplicationUser, ApplicationDbContext, EntitiesDbContext, ApplicationUser, ApplicationRole, Tenant, INotify<ApplicationRole>>
    {
        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;

        public CompanyManageController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ICacheService cacheService, ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<ApplicationRole> notify, IDataFilter dataFilter, IOrganizationService<Tenant> organizationService, IStringLocalizer localizer) : base(userManager, roleManager, cacheService, applicationDbContext, context, notify, dataFilter, localizer)
        {
            _organizationService = organizationService;
        }

        public override IActionResult Index()
        {
            return base.Index();
        }

        public virtual IActionResult Users()
        {
            var model = new ListTagHelperModel
            {
                Title = "Manage my company users",
                SubTitle = "This is the list of your company users",
                ListIdentifier = "manageCompanyUsers",
                Api = new ListApiConfigurationViewModel
                {
                    Url = Url.Action("LoadPageItems")
                },
                RenderColumns = new List<ListRenderColumn> {
            new ListRenderColumn(Localizer["name"], "userName"),
            new ListRenderColumn(Localizer["roles"], "roles")
            {
                StyleAttributes = new List<InlineStyleAttribute>
                {
                    new InlineStyleAttribute("width", "300px")
                }
            },
            new ListRenderColumn(Localizer["created"], "created"),
            new ListRenderColumn(Localizer["author"], "author")
        },
                HeadButtons = new List<UrlTagHelperViewModel> {
            new UrlTagHelperViewModel
            {
                AspAction = "Create",
                AspController = "CompanyManage",
                ButtonName = "Add new user to company",
                Description = "New user will be added to company"
            }
        },
                HasActions = true,
                ListActions = new List<ListActionViewModel>{
             new ListActionViewModel
             {
                 HasIcon = false,
                 Name = Localizer["edit"],
                 Url = Url.Action("LoadPageItems"),
                 ButtonType = BootstrapButton.Primary
             },
            new ListActionViewModel
            {
                HasIcon = false,
                Name = Localizer["system_user_change_password"],
                Url = "/Users/ChangeUserPassword",
                ActionParameters = new List<ActionParameter>
                {
                    new ActionParameter("userId", "id"),
                    new ActionParameter("callBackUrl", "/CompanyManage/Users")
                    {
                        IsCustomValue = true
                    }
                },
                ButtonType = BootstrapButton.Danger
            }
         },
                Documentation = "This page allow to manage only your company users"
            };
            return View(model);
        }

        /// <inheritdoc />
        /// <summary>
        /// Load page items
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public override JsonResult LoadPageItems(DTParameters param)
        {
            var currentUser = GetCurrentUser();
            if (currentUser.TenantId != null)
            {
                var tenant = _organizationService.GetTenantById(currentUser.TenantId.Value);
                if (tenant == null)
                    return Json(new DTResult<CompanyUsersViewModel>
                    {
                        Draw = param.Draw,
                        Data = new List<CompanyUsersViewModel>(),
                        RecordsFiltered = 0,
                        RecordsTotal = 0
                    });
            }

            var filtered = ApplicationDbContext.Filter<ApplicationUser>(param.Search.Value, param.SortOrder,
                param.Start,
                param.Length,
                out var totalCount, x => !x.IsDeleted && x.TenantId == currentUser.TenantId).ToList();

            var rs = filtered.Select(async x =>
            {
                var u = x.Adapt<CompanyUsersViewModel>();
                u.Roles = await UserManager.GetRolesAsync(x);
                return u;
            }).Select(x => x.Result);

            var finalResult = new DTResult<CompanyUsersViewModel>
            {
                Draw = param.Draw,
                Data = rs.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return Json(finalResult);
        }
    }
}
