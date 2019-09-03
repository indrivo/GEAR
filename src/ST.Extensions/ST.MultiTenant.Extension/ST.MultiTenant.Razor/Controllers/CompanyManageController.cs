using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ST.Identity.Abstractions;
using ST.MultiTenant.Abstractions;
using ST.Cache.Abstractions;
using ST.Core;
using ST.Core.Abstractions;
using ST.Core.BaseControllers;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Entities.Abstractions;
using ST.Entities.Data;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.Identity.Data;
using ST.MultiTenant.Abstractions.Helpers;
using ST.MultiTenant.Abstractions.ViewModels;
using ST.MultiTenant.Razor.Helpers;
using ST.Notifications.Abstractions;

namespace ST.MultiTenant.Razor.Controllers
{
    [Authorize(Roles = Resources.Roles.COMPANY_ADMINISTRATOR)]
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class CompanyManageController : BaseCrudController<ApplicationDbContext, ApplicationUser,
        ApplicationDbContext, EntitiesDbContext, ApplicationUser, ApplicationRole, Tenant, INotify<ApplicationRole>>
    {
        #region Injectable

        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;

        /// <summary>
        /// Users settings
        /// </summary>
        private readonly MultiTenantListSettings _listSettings;


        /// <summary>
        /// Inject dynamic service
        /// </summary>
        private readonly IEntityRepository _service;

        #endregion

        public CompanyManageController(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, ICacheService cacheService,
            ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<ApplicationRole> notify,
            IDataFilter dataFilter, IOrganizationService<Tenant> organizationService, IStringLocalizer localizer, IEntityRepository service) :
            base(userManager, roleManager, cacheService, applicationDbContext, context, notify, dataFilter, localizer)
        {
            _organizationService = organizationService;
            _service = service;
            _listSettings = new MultiTenantListSettings();
        }

        /// <summary>
        /// Index view
        /// </summary>
        /// <returns></returns>
        public override IActionResult Index()
        {
            var user = GetCurrentUser();
            ViewBag.UserRoles = string.Join(", ", UserManager.GetRolesAsync(user).GetAwaiter().GetResult());
            ViewBag.User = user;
            ViewBag.UsersListSettings = _listSettings.GetCompanyUserListSettings();
            ViewBag.Organization = _organizationService.GetUserOrganization(user);
            ViewBag.Countries = _organizationService.GetCountrySelectList().GetAwaiter().GetResult();
            return base.Index();
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
            var listObj = _organizationService
                .LoadFilteredListCompanyUsersAsync(param)
                .ExecuteAsync();
            return Json(listObj);
        }

        /// <summary>
        /// Get system roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> GetRoles() => Json(await _organizationService.GetRoles());

        /// <summary>
        /// Invite new user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> InviteNewUserAsync([FromBody] InviteNewUserViewModel model)
        {
            var resultModel = new ResultModel();
            if (!ModelState.IsValid)
            {
                resultModel.Errors.Add(new ErrorModel
                {
                    Key = string.Empty,
                    Message = "Invalid model"
                });
                return Json(resultModel);
            }

            resultModel = await _organizationService.InviteNewUserByEmailAsync(model);

            return Json(resultModel);
        }

        /// <summary>
        /// Register company
        /// </summary>
        /// <returns></returns>
        [HttpGet("/register-company"), AllowAnonymous]
        public async Task<IActionResult> RegisterCompany()
        {
            var model = new CreateTenantViewModel
            {
                CountrySelectListItems = await _organizationService.GetCountrySelectList()
            };
            return View(model);
        }

        /// <summary>
        /// Register company
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost("/register-company"), AllowAnonymous]
        public async Task<IActionResult> RegisterCompany(CreateTenantViewModel data)
        {
            if (!ModelState.IsValid)
            {
                data.CountrySelectListItems = await _organizationService.GetCountrySelectList();
                return View(data);
            }
            var reqTenant = await _organizationService.CreateOrganizationAsync(data);

            if (reqTenant.IsSuccess)
            {
                var generateResult = await _service.GenerateTablesForTenantAsync(reqTenant.Result);
                if (generateResult.IsSuccess)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AppendResultModelErrors(generateResult.Errors);

                return View(reqTenant.Result);
            }

            ModelState.AppendResultModelErrors(reqTenant.Errors);

            return View(reqTenant.Result);
        }
    }
}