using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Mapster;
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
using ST.Identity.Abstractions.Enums;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.Identity.Data;
using ST.MultiTenant.Abstractions.Events;
using ST.MultiTenant.Abstractions.Events.EventArgs;
using ST.MultiTenant.Abstractions.Helpers;
using ST.MultiTenant.Abstractions.ViewModels;
using ST.MultiTenant.Razor.Helpers;
using ST.Notifications.Abstractions;
using ST.Notifications.Abstractions.Models.Notifications;

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

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Inject sign in manager
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Inject notifier
        /// </summary>
        private readonly INotify<ApplicationRole> _notify;

        #endregion

        public CompanyManageController(UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, ICacheService cacheService,
            ApplicationDbContext applicationDbContext, EntitiesDbContext context, INotify<ApplicationRole> notify,
            IDataFilter dataFilter, IOrganizationService<Tenant> organizationService, IStringLocalizer localizer, IEntityRepository service, IUserManager<ApplicationUser> userManager1, SignInManager<ApplicationUser> signInManager) :
            base(userManager, roleManager, cacheService, applicationDbContext, context, notify, dataFilter, localizer)
        {
            _notify = notify;
            _organizationService = organizationService;
            _service = service;
            _userManager = userManager1;
            _signInManager = signInManager;
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
            var model = new RegisterCompanyViewModel
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
        public async Task<IActionResult> RegisterCompany(RegisterCompanyViewModel data)
        {
            if (!ModelState.IsValid)
            {
                data.CountrySelectListItems = await _organizationService.GetCountrySelectList();
                return View(data);
            }

            //check if exist
            var userNameExist = await _userManager.UserManager.FindByNameAsync(data.UserName);
            var userEmailExist = await _userManager.UserManager.FindByNameAsync(data.Email);

            if (userEmailExist != null || userNameExist != null)
            {
                data.CountrySelectListItems = await _organizationService.GetCountrySelectList();
                ModelState.AddModelError(string.Empty, "User or email are used!");
                return View(data);
            }

            var reqTenant = await _organizationService.CreateOrganizationAsync(data);

            if (reqTenant.IsSuccess)
            {
                var generateResult = await _service.GenerateTablesForTenantAsync(reqTenant.Result);
                if (!generateResult.IsSuccess)
                {
                    ModelState.AppendResultModelErrors(generateResult.Errors);
                    return View(reqTenant.Result.Adapt<RegisterCompanyViewModel>());
                }

                var newCompanyOwner = new ApplicationUser
                {
                    Email = data.Email,
                    UserName = data.UserName,
                    UserFirstName = data.FirstName,
                    UserLastName = data.LastName,
                    AuthenticationType = AuthenticationType.Local,
                    EmailConfirmed = false,
                    TenantId = reqTenant.Result.Id
                };

                var usrReq = await _userManager.UserManager.CreateAsync(newCompanyOwner, data.Password);

                if (!usrReq.Succeeded)
                {
                    ModelState.AddModelError(string.Empty, "Fail to create user!");
                    return View(reqTenant.Result.Adapt<RegisterCompanyViewModel>());
                }

                //Trigger event
                TenantEvents.Company.CompanyRegistered(new CompanyRegisterEventArgs
                {
                    UserName = newCompanyOwner.UserName,
                    UserId = newCompanyOwner.Id,
                    CompanyName = reqTenant.Result.Name,
                    UserEmail = newCompanyOwner.Email
                });

                var roleReq = await _userManager.AddToRolesAsync(newCompanyOwner, new List<string> { Resources.Roles.COMPANY_ADMINISTRATOR });

                if (!roleReq.IsSuccess)
                {
                    ModelState.AppendResultModelErrors(roleReq.Errors);
                    return View(reqTenant.Result.Adapt<RegisterCompanyViewModel>());
                }

                var claim = new Claim(nameof(Tenant).ToLowerInvariant(), newCompanyOwner.TenantId.ToString());

                await _userManager.UserManager.AddClaimAsync(newCompanyOwner, claim);

                var signResult =
                    await _signInManager.PasswordSignInAsync(data.UserName, data.Password, true, false);

                if (signResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Fail to sign in");

                return View(reqTenant.Result.Adapt<RegisterCompanyViewModel>());
            }

            ModelState.AppendResultModelErrors(reqTenant.Errors);

            return View(reqTenant.Result.Adapt<RegisterCompanyViewModel>());
        }
    }
}