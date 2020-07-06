using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using GR.Identity.Abstractions;
using GR.MultiTenant.Abstractions;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.TagHelpers.TagHelperViewModels.ListTagHelperViewModels;
using GR.Entities.Abstractions;
using GR.Identity.Abstractions.Extensions;
using GR.Identity.Abstractions.Helpers;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Permissions.Abstractions;
using GR.MultiTenant.Abstractions.Events;
using GR.MultiTenant.Abstractions.Events.EventArgs;
using GR.MultiTenant.Abstractions.Helpers;
using GR.MultiTenant.Abstractions.ViewModels;
using GR.MultiTenant.Razor.Helpers;

namespace GR.MultiTenant.Razor.Controllers
{
    [Authorize(Roles = MultiTenantResources.Roles.COMPANY_ADMINISTRATOR)]
    public class CompanyManageController : BaseGearController
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
        private readonly IEntityService _service;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject sign in manager
        /// </summary>
        private readonly SignInManager<GearUser> _signInManager;

        /// <summary>
        /// Inject permission service
        /// </summary>
        private readonly IPermissionService _permissionService;

        #endregion

        public CompanyManageController(IOrganizationService<Tenant> organizationService, IEntityService service, IUserManager<GearUser> userManager1, SignInManager<GearUser> signInManager, IPermissionService permissionService)
        {
            _organizationService = organizationService;
            _service = service;
            _userManager = userManager1;
            _signInManager = signInManager;
            _permissionService = permissionService;
            _listSettings = new MultiTenantListSettings();
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = (await _userManager.GetCurrentUserAsync()).Result;
            var roles = await _userManager.UserManager.GetRolesAsync(user);
            ViewBag.UserRoles = string.Join(", ", roles
            );
            ViewBag.User = user;
            var hasAccess = await _permissionService.HasPermissionAsync(new List<string> { UserPermissions.UserCreate });
            var listSettings = _listSettings.GetCompanyUserListSettings();
            if (!hasAccess) listSettings.HeadButtons = new List<UrlTagHelperViewModel>();
            ViewBag.UsersListSettings = listSettings;
            ViewBag.Organization = _organizationService.GetUserOrganization(user);
            ViewBag.Countries = _organizationService.GetCountrySelectListAsync().GetAwaiter().GetResult();
            return View();
        }

        /// <summary>
        /// Load page items
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadPageItems(DTParameters param)
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
        public async Task<JsonResult> GetRoles() => Json(await _organizationService.GetRolesAsync());

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
                resultModel.AttachModelState(ModelState);
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
            if (User.IsAuthenticated()) return Redirect($"{HttpContext.GetAppBaseUrl()}/home");

            var model = new RegisterCompanyViewModel
            {
                CountrySelectListItems = await _organizationService.GetCountrySelectListAsync()
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
            data.UserName = data.Email;
            //if (data.Email.Contains("@") && data.Email.IndexOf("@", StringComparison.Ordinal) > -1)
            //{
            //    .Substring(0, data.Email.IndexOf("@", StringComparison.Ordinal));
            //}

            if (User.IsAuthenticated()) return Redirect($"{HttpContext.GetAppBaseUrl()}/home");

            if (!ModelState.IsValid)
            {
                data.CountrySelectListItems = await _organizationService.GetCountrySelectListAsync();
                return View(data);
            }

            var userNameExist = await _userManager.UserManager.FindByNameAsync(data.UserName);
            var userEmailExist = await _userManager.UserManager.FindByEmailAsync(data.Email);

            if (userEmailExist != null)
            {
                data.CountrySelectListItems = await _organizationService.GetCountrySelectListAsync();
                ModelState.AddModelError(string.Empty, "Email address is used!");
                return View(data);
            }

            if (userNameExist != null)
            {
                data.CountrySelectListItems = await _organizationService.GetCountrySelectListAsync();
                ModelState.AddModelError(string.Empty, "UserName is used!");
                return View(data);
            }
            var newCompanyOwner = new GearUser
            {
                Email = data.Email,
                UserName = data.UserName,
                FirstName = data.FirstName,
                LastName = data.LastName,
                AuthenticationType = IdentityResources.LocalAuthenticationType,
                EmailConfirmed = false,
                IsEditable = true
            };

            //create new user
            var usrReq = await _userManager.UserManager.CreateAsync(newCompanyOwner, data.Password);
            if (!usrReq.Succeeded)
            {
                ModelState.AppendIdentityResult(usrReq);
                data.CountrySelectListItems = await _organizationService.GetCountrySelectListAsync();
                return View(data);
            }

            var reqTenant = await _organizationService.CreateOrganizationAsync(data);

            if (reqTenant.IsSuccess)
            {
                var claim = new Claim(nameof(Tenant).ToLowerInvariant(), newCompanyOwner.TenantId.ToString());
                newCompanyOwner.TenantId = reqTenant.Result.Id;
                await _userManager.UserManager.UpdateAsync(newCompanyOwner);
                await _userManager.UserManager.AddClaimAsync(newCompanyOwner, claim);

                var generateResult = await _service.GenerateTablesForTenantAsync(reqTenant.Result);
                if (!generateResult.IsSuccess)
                {
                    ModelState.AppendResultModelErrors(generateResult.Errors);
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

                //send confirm email request
                await _organizationService.SendConfirmEmailRequestAsync(newCompanyOwner);

                var roleReq = await _userManager.AddToRolesAsync(newCompanyOwner, new List<string> { MultiTenantResources.Roles.COMPANY_ADMINISTRATOR });

                if (!roleReq.IsSuccess)
                {
                    ModelState.AppendResultModelErrors(roleReq.Errors);
                    return View(reqTenant.Result.Adapt<RegisterCompanyViewModel>());
                }

                //sing in new created
                var signResult =
                    await _signInManager.PasswordSignInAsync(data.UserName, data.Password, true, false);

                if (signResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError(string.Empty, "Fail to sign in");

                return View(reqTenant.Result.Adapt<RegisterCompanyViewModel>());
            }

            await _userManager.UserManager.DeleteAsync(newCompanyOwner);

            ModelState.AppendResultModelErrors(reqTenant.Errors);

            return View(reqTenant.Result.Adapt<RegisterCompanyViewModel>());
        }

        /// <summary>
        /// Check user name if exist
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post"), AllowAnonymous]
        public async Task<IActionResult> CheckUserNameIfExist(string userName)
        {
            if (userName.IsNullOrEmpty()) return Json(false);
            var userNameExist = await _userManager.UserManager.FindByNameAsync(userName);
            return userNameExist != null ? Json($"The username {userName} is already used") : Json(true);
        }

        /// <summary>
        /// Check email if exist
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post"), AllowAnonymous]
        public async Task<IActionResult> CheckEmailIfExist(string email)
        {
            var userNameExist = await _userManager.UserManager.FindByEmailAsync(email);
            return userNameExist != null ? Json($"The email {email} is already used") : Json(true);
        }

        /// <summary>
        /// Check if tenant name is used
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post"), AllowAnonymous]
        public async Task<IActionResult> CheckTenantIfExist(string name)
        {
            var isUsed = await _organizationService.IsTenantNameUsedAsync(name);
            return !isUsed ? Json(true) : Json($"The company name {name} is already used");
        }

        /// <summary>
        /// Delete user from company
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DeleteUser(Guid? userId)
        {
            var deleteResult = await _organizationService.DeleteUserPermanentAsync(userId);
            if (deleteResult.IsSuccess)
            {
                return RedirectToAction(nameof(Index));
            }

            return BadRequest();
        }
    }
}