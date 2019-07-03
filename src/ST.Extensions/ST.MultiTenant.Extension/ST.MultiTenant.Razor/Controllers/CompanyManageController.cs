﻿using System.Collections.Generic;
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
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.Entities.Data;
using ST.Identity.Data;
using ST.MultiTenant.Razor.Settings;
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

        private readonly MultiTenantListSettings _listSettings;

        public CompanyManageController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ICacheService cacheService, ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<ApplicationRole> notify, IDataFilter dataFilter, IOrganizationService<Tenant> organizationService, IStringLocalizer localizer) : base(userManager, roleManager, cacheService, applicationDbContext, context, notify, dataFilter, localizer)
        {
            _organizationService = organizationService;
            _listSettings = new MultiTenantListSettings();
        }

        public override IActionResult Index()
        {
            var user = GetCurrentUser();
            ViewBag.UserRoles = string.Join(", ", UserManager.GetRolesAsync(user).GetAwaiter().GetResult());
            ViewBag.User = user;
            ViewBag.UsersListSettings = _listSettings.GetCompanyUserListSettings();
            return base.Index();
        }

        public virtual IActionResult Users()
        {
            return View(_listSettings.GetCompanyUserListSettings());
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
