using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
using ST.Notifications.Abstractions;

namespace ST.MultiTenant.Razor.Controllers
{
    [Authorize(Roles = "Company Administrator")]
    public class CompanyManageController : BaseCrudController<ApplicationDbContext, ApplicationUser, ApplicationDbContext, EntitiesDbContext, ApplicationUser, ApplicationRole, Tenant, INotify<ApplicationRole>>
    {
        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;

        public CompanyManageController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ICacheService cacheService, ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<ApplicationRole> notify, IDataFilter dataFilter, IOrganizationService<Tenant> organizationService) : base(userManager, roleManager, cacheService, applicationDbContext, context, notify, dataFilter)
        {
            _organizationService = organizationService;
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
                    return Json(new DTResult<ApplicationUser>
                    {
                        Draw = param.Draw,
                        Data = new List<ApplicationUser>(),
                        RecordsFiltered = 0,
                        RecordsTotal = 0
                    });
            }

            var filtered = ApplicationDbContext.Filter<ApplicationUser>(param.Search.Value, param.SortOrder,
                param.Start,
                param.Length,
                out var totalCount, x => !x.IsDeleted && x.TenantId == currentUser.TenantId).ToList();

            var finalResult = new DTResult<ApplicationUser>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return Json(finalResult);
        }
    }
}
