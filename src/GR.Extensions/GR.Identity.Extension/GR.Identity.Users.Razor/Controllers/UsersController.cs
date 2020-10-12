using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers.ErrorCodes;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Events;
using GR.Identity.Abstractions.Events.EventArgs.Users;
using GR.Identity.Abstractions.Helpers;
using GR.Identity.Permissions.Abstractions.Attributes;
using GR.Identity.Users.Razor.ViewModels.UserViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace GR.Identity.Users.Razor.Controllers
{
    [Authorize]
    public class UsersController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Logger
        /// </summary>
        private ILogger<UsersController> Logger { get; }

        /// <summary>
        /// Inject localizer
        /// </summary>
        private readonly IStringLocalizer _localizer;

        /// <summary>
        /// Inject role manager
        /// </summary>
        private readonly RoleManager<GearRole> _roleManager;


        /// <summary>
        /// Inject identity context
        /// </summary>
        private readonly IIdentityContext _identityContext;

        /// <summary>
        /// Inject custom user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        #endregion

        public UsersController(IUserManager<GearUser> userManager, RoleManager<GearRole> roleManager,
            ILogger<UsersController> logger, IStringLocalizer localizer, IIdentityContext identityContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            Logger = logger;
            _localizer = localizer;
            _identityContext = identityContext;
        }

        /// <summary>
        /// User list for admin visualization
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(UserPermissions.UserRead)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <returns></returns>
        [AuthorizePermission(UserPermissions.UserCreate)]
        public virtual async Task<IActionResult> Create()
        {
            var model = new CreateUserViewModel
            {
                Roles = await GetRoleSelectListItemAsync(),
                Tenants = await GetTenantsSelectListItemAsync()
            };

            return View(model);
        }

        /// <summary>
        /// Apply user create
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(UserPermissions.UserCreate)]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = await GetRoleSelectListItemAsync();
                model.Tenants = await GetTenantsSelectListItemAsync();
                return View(model);
            }

            var user = new GearUser
            {
                UserName = model.UserName,
                Email = model.Email,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                IsDeleted = model.IsDeleted,
                Author = User.Identity.Name,
                AuthenticationType = model.AuthenticationType,
                IsEditable = true,
                TenantId = model.TenantId,
                LastPasswordChanged = DateTime.Now,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Birthday = model.Birthday ?? DateTime.MinValue,
                AboutMe = model.AboutMe,
            };

            if (model.UserPhoto != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.UserPhoto.CopyToAsync(memoryStream);
                    user.UserPhoto = memoryStream.ToArray();
                }
            }

            var result = await _userManager.UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                model.Roles = await GetRoleSelectListItemAsync();
                model.Tenants = await GetTenantsSelectListItemAsync();
                return View(model);
            }

            Logger.LogInformation("User {0} created successfully", user.UserName);

            if (model.SelectedRoleId != null && model.SelectedRoleId.Any())
            {
                var rolesNameList = await _roleManager.Roles.Where(x => model.SelectedRoleId.Contains(x.Id))
                    .Select(x => x.Name).ToListAsync();
                var roleAddResult = await _userManager.UserManager.AddToRolesAsync(user, rolesNameList);
                if (!roleAddResult.Succeeded)
                {
                    foreach (var error in roleAddResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    model.Roles = await GetRoleSelectListItemAsync();
                    model.Tenants = await GetTenantsSelectListItemAsync();
                    return View(model);
                }
            }

            var dbResult = await _identityContext.PushAsync();
            if (!dbResult.IsSuccess)
            {
                ModelState.AppendResultModelErrors(dbResult.Errors);
                model.Roles = await GetRoleSelectListItemAsync();
                model.Tenants = await GetTenantsSelectListItemAsync();
                return View(model);
            }

            IdentityEvents.Users.UserCreated(new UserCreatedEventArgs
            {
                Email = user.Email,
                UserName = user.UserName,
                UserId = user.Id
            });

            return RedirectToAction(nameof(Index), "Users");
        }

        /// <summary>
        /// Delete user form DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(UserPermissions.UserDelete)]
        public async Task<JsonResult> Delete(Guid? id)
        {
            var deleteRequest = await _userManager.DeleteUserPermanently(id);
            return Json(deleteRequest);
        }

        /// <summary>
        ///  Edit user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(UserPermissions.UserUpdate)]
        public async Task<IActionResult> Edit(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            var applicationUser = await _identityContext.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            var roles = _roleManager.Roles.ToList();
            var userRolesNames = await _userManager.UserManager.GetRolesAsync(applicationUser);
            var userRoles = userRolesNames.Select(item => roles.FirstOrDefault(x => x.Name == item)?.Id.ToString())
                .ToList();

            var model = new UpdateUserViewModel
            {
                Id = applicationUser.Id,
                Email = applicationUser.Email,
                IsDeleted = applicationUser.IsDeleted,
                Password = applicationUser.PasswordHash,
                RepeatPassword = applicationUser.PasswordHash,
                UserName = applicationUser.UserName,
                UserNameOld = applicationUser.UserName,
                Roles = roles,
                SelectedRoleId = userRoles,
                UserPhoto = applicationUser.UserPhoto,
                AuthenticationType = applicationUser.AuthenticationType,
                TenantId = applicationUser.TenantId,
                Tenants = _identityContext.Tenants.AsNoTracking().Where(x => !x.IsDeleted).ToList(),
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName
            };
            return View(model);
        }

        /// <summary>
        ///     Save user data
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(UserPermissions.UserUpdate)]
        public virtual async Task<IActionResult> Edit(Guid id, UpdateUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var applicationUser = await _identityContext.Users.SingleOrDefaultAsync(m => m.Id == id);
            var roles = _roleManager.Roles.ToList();
            var userRolesNames = await _userManager.UserManager.GetRolesAsync(applicationUser);
            var userRoleList = userRolesNames
                .Select(item => roles.FirstOrDefault(x => x.Name == item)?.Id.ToString())
                .ToList();

            model.Roles = roles;
            model.Tenants = _identityContext.Tenants.Where(x => !x.IsDeleted).ToList();

            if (!ModelState.IsValid)
            {
                model.SelectedRoleId = userRoleList;
                foreach (var error in ViewData.ModelState.Values.SelectMany(stateValue => stateValue.Errors))
                {
                    ModelState.AddModelError(string.Empty, error.ErrorMessage);
                }

                return View(model);
            }

            // Update User Data
            var user = await _userManager.UserManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            user.IsDeleted = model.IsDeleted;
            user.Changed = DateTime.Now;
            user.Email = model.Email;
            user.ModifiedBy = User.Identity.Name;
            user.UserName = model.UserName;
            user.TenantId = model.TenantId;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            if (model.UserPhotoUpdateFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.UserPhotoUpdateFile.CopyToAsync(memoryStream);
                    user.UserPhoto = memoryStream.ToArray();
                }
            }

            var result = await _userManager.UserManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                model.SelectedRoleId = userRoleList;
                foreach (var _ in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, _.Description);
                }

                return View(model);
            }

            // REFRESH USER ROLES
            var userRoles = await _identityContext.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();
            var rolesList = new List<string>();
            foreach (var _ in userRoles)
            {
                var role = await _roleManager.FindByIdAsync(_.RoleId.ToString());
                rolesList.Add(role.Name);
            }

            await _userManager.UserManager.RemoveFromRolesAsync(user, rolesList);

            var roleNameList = new List<string>();
            foreach (var _ in model.SelectedRoleId)
            {
                var role = await _roleManager.FindByIdAsync(_);
                roleNameList.Add(role.Name);
            }

            await _userManager.AddToRolesAsync(user, roleNameList);

            await _userManager.UserManager.UpdateSecurityStampAsync(user);

            //Refresh user claims for this user
            //await user.RefreshClaims(Context, signInManager);
            IdentityEvents.Users.UserUpdated(new UserUpdatedEventArgs
            {
                Email = user.Email,
                UserName = user.UserName,
                UserId = user.Id
            });
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Get view for change user password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="callBackUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChangeUserPassword([Required] Guid? userId, string callBackUrl)
        {
            if (userId == null) return NotFound();
            var user = await _userManager.UserManager.FindByIdAsync(userId.Value.ToString());
            if (user == null) return NotFound();
            return View(new ChangeUserPasswordViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                AuthenticationType = user.AuthenticationType,
                UserId = user.Id,
                CallBackUrl = callBackUrl
            });
        }

        /// <summary>
        /// Apply new password
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public virtual async Task<IActionResult> ChangeUserPassword([Required] ChangeUserPasswordViewModel model)
        {
            var user = await _userManager.UserManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                ModelState.AddModelError("", "The user is no longer in the system");
                return View(model);
            }

            var hasher = new PasswordHasher<GearUser>();
            var hashedPassword = hasher.HashPassword(user, model.Password);
            user.PasswordHash = hashedPassword;
            user.LastPasswordChanged = DateTime.Now;
            var result = await _userManager.UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                IdentityEvents.Users.UserPasswordChange(new UserChangePasswordEventArgs
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    UserId = user.Id,
                    Password = model.Password
                });
                return Redirect(model.CallBackUrl);
            }

            foreach (var _ in result.Errors)
            {
                ModelState.AddModelError(string.Empty, _.Description);
            }

            return View(model);
        }

        /// <summary>
        /// Get user image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [ResponseCache(Duration = 120 /*2 minutes*/, Location = ResponseCacheLocation.Any, NoStore = false, VaryByQueryKeys = new[] { "id" })]
        public async Task<IActionResult> GetImage(Guid id)
        {
            var imageRequest = await _userManager.GetUserImageAsync(id);
            if (imageRequest.IsSuccess)
            {
                return File(imageRequest.Result, "image/jpg");
            }

            if (imageRequest.HasErrorCode(ResultModelCodes.NotFound))
            {
                return NotFound();
            }

            return BadRequest();
        }

        /// <summary>
        /// Validate user name
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userNameOld"></param>
        /// <returns></returns>
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> VerifyName(string userName, string userNameOld)
        {
            if (userNameOld != null && userName.ToLower().Equals(userNameOld.ToLower()))
            {
                return Json(true);
            }

            if (await _identityContext.Users
                .AsNoTracking()
                .AnyAsync(x => x.UserName.ToLower().Equals(userName.ToLower())))
            {
                return Json($"User name {userName} is already in use.");
            }

            return Json(true);
        }

        /// <summary>
        /// Return roles select list items
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<SelectListItem>> GetRoleSelectListItemAsync()
        {
            var roles = await _roleManager.Roles
                .AsNoTracking()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();
            roles.Insert(0, new SelectListItem(_localizer["sel_role"], string.Empty));

            return roles;
        }

        /// <summary>
        /// Return tenants select list items
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<SelectListItem>> GetTenantsSelectListItemAsync()
        {
            var tenants = await _identityContext.Tenants
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();
            tenants.Insert(0, new SelectListItem("Select tenant", string.Empty));

            return tenants;
        }
    }
}