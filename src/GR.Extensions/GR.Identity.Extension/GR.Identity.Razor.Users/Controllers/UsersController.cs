using GR.Core;
using GR.Core.Attributes;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Enums;
using GR.Identity.Abstractions.Events;
using GR.Identity.Abstractions.Events.EventArgs.Users;
using GR.Identity.Abstractions.Models.AddressModels;
using GR.Identity.Abstractions.ViewModels.UserProfileAddress;
using GR.Identity.Data.Permissions;
using GR.Identity.LdapAuth.Abstractions;
using GR.Identity.LdapAuth.Abstractions.Models;
using GR.Identity.Permissions.Abstractions.Attributes;
using GR.Identity.Razor.Users.ViewModels.UserProfileViewModels;
using GR.Identity.Razor.Users.ViewModels.UserViewModels;
using GR.Notifications.Abstractions;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Razor.BaseControllers;
using UserProfileViewModel = GR.Identity.Razor.Users.ViewModels.UserProfileViewModels.UserProfileViewModel;

namespace GR.Identity.Razor.Users.Controllers
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
        /// Inject Ldap User Manager
        /// </summary>
        private readonly BaseLdapUserManager<LdapUser> _ldapUserManager;

        /// <summary>
        /// Inject localizer
        /// </summary>
        private readonly IStringLocalizer _localizer;

        private readonly RoleManager<GearRole> _roleManager;

        /// <summary>
        /// Inject identity user manager
        /// </summary>
        private readonly UserManager<GearUser> _userManager;

        /// <summary>
        /// Inject identity context
        /// </summary>
        private readonly IIdentityContext _identityContext;

        /// <summary>
        /// Inject custom user manager
        /// </summary>
        private readonly IUserManager<GearUser> _customUserManager;

        #endregion

        public UsersController(UserManager<GearUser> userManager, RoleManager<GearRole> roleManager,
             BaseLdapUserManager<LdapUser> ldapUserManager,
            ILogger<UsersController> logger, IStringLocalizer localizer, IIdentityContext identityContext, IUserManager<GearUser> customUserManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _ldapUserManager = ldapUserManager;
            Logger = logger;
            _localizer = localizer;
            _identityContext = identityContext;
            _customUserManager = customUserManager;
        }

        /// <summary>
        /// User list for admin visualization
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmUserRead)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <returns></returns>
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmUserCreate)]
        public virtual async Task<IActionResult> Create()
        {
            var model = new CreateUserViewModel
            {
                Roles = await GetRoleSelectListItemAsync(),
                Groups = await GetAuthGroupSelectListItemAsync(),
                Tenants = await GetTenantsSelectListItemAsync(),
                CountrySelectListItems = await GetCountrySelectList()
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
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmUserCreate)]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Roles = await GetRoleSelectListItemAsync();
                model.Groups = await GetAuthGroupSelectListItemAsync();
                model.Tenants = await GetTenantsSelectListItemAsync();
                model.CountrySelectListItems = await GetCountrySelectList();
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
                UserFirstName = model.FirstName,
                UserLastName = model.LastName,
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

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                model.Roles = await GetRoleSelectListItemAsync();
                model.Groups = await GetAuthGroupSelectListItemAsync();
                model.Tenants = await GetTenantsSelectListItemAsync();
                model.CountrySelectListItems = await GetCountrySelectList();
                return View(model);
            }

            Logger.LogInformation("User {0} created successfully", user.UserName);

            if (model.SelectedRoleId != null && model.SelectedRoleId.Any())
            {
                var rolesNameList = await _roleManager.Roles.Where(x => model.SelectedRoleId.Contains(x.Id))
                    .Select(x => x.Name).ToListAsync();
                var roleAddResult = await _userManager.AddToRolesAsync(user, rolesNameList);
                if (!roleAddResult.Succeeded)
                {
                    foreach (var error in roleAddResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }

                    model.Roles = await GetRoleSelectListItemAsync();
                    model.Groups = await GetAuthGroupSelectListItemAsync();
                    model.Tenants = await GetTenantsSelectListItemAsync();
                    model.CountrySelectListItems = await GetCountrySelectList();
                    return View(model);
                }
            }

            if (model.SelectedGroupId != null && model.SelectedGroupId.Any())
            {
                var userGroupList = model.SelectedGroupId
                    .Select(_ => new UserGroup { AuthGroupId = Guid.Parse(_), UserId = user.Id }).ToList();

                await _identityContext.UserGroups.AddRangeAsync(userGroupList);
            }
            else
            {
                var groupId = await _identityContext.AuthGroups.FirstOrDefaultAsync();
                if (groupId != null)
                {
                    _identityContext.UserGroups.Add(new UserGroup
                    {
                        AuthGroupId = groupId.Id,
                        UserId = user.Id
                    });
                }
            }

            var dbResult = await _identityContext.PushAsync();
            if (!dbResult.IsSuccess)
            {
                ModelState.AppendResultModelErrors(dbResult.Errors);
                model.Roles = await GetRoleSelectListItemAsync();
                model.Groups = await GetAuthGroupSelectListItemAsync();
                model.Tenants = await GetTenantsSelectListItemAsync();
                model.CountrySelectListItems = await GetCountrySelectList();
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
        /// Get Ad users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AjaxOnly]
        public JsonResult GetAdUsers()
        {
            var result = new ResultModel<IEnumerable<LdapUser>>();
            var addedUsers = _identityContext.Users.Where(x => x.AuthenticationType.Equals(AuthenticationType.Ad)).Adapt<IEnumerable<LdapUser>>()
                .ToList();
            var users = _ldapUserManager.Users;
            if (addedUsers.Any())
            {
                users = users.Except(addedUsers);
            }

            result.IsSuccess = true;
            result.Result = users;
            return Json(users);
        }

        /// <summary>
        /// Add Ad user
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public virtual async Task<JsonResult> AddAdUser([Required] string userName)
        {
            var result = new ResultModel<Guid>();
            if (string.IsNullOrEmpty(userName))
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"Invalid username : {userName}"));
                return Json(result);
            }

            var exists = await _userManager.FindByNameAsync(userName);
            if (exists != null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"UserName {userName} exists!"));
                return Json(result);
            }

            var user = new GearUser();

            var ldapUser = await _ldapUserManager.FindByNameAsync(userName);
            if (ldapUser == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"There is no AD user with this username : {userName}"));
                return Json(result);
            }

            user.Id = Guid.NewGuid();
            user.UserName = ldapUser.SamAccountName;
            user.Email = ldapUser.EmailAddress;
            user.AuthenticationType = AuthenticationType.Ad;
            result.IsSuccess = true;
            var req = await _userManager.CreateAsync(user, "ldap_default_password");
            if (!req.Succeeded)
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"Fail to add user : {userName}"));
                result.IsSuccess = false;
            }
            else
            {
                IdentityEvents.Users.UserCreated(new UserCreatedEventArgs
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    UserId = user.Id
                });
                result.Result = user.Id;
            }

            return Json(result);
        }

        /// <summary>
        /// Delete user form DB
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmUserDelete)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (id == Guid.Empty)
            {
                return Json(new { success = false, message = "Id is null" });
            }

            if (IsCurrentUser(id))
            {
                return Json(new { success = false, message = "You can't delete current user" });
            }

            var applicationUser = await _identityContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return Json(new { success = false, message = "User not found" });
            }

            if (applicationUser.IsEditable == false)
            {
                return Json(new { succsess = false, message = "Is system user!!!" });
            }

            var deleteResult = await _userManager.DeleteAsync(applicationUser);
            if (deleteResult.Succeeded)
            {
                IdentityEvents.Users.UserDelete(new UserDeleteEventArgs
                {
                    Email = applicationUser.Email,
                    UserName = applicationUser.UserName,
                    UserId = applicationUser.Id
                });
                return Json(new { success = true, message = "Delete success" });
            }
            else
            {
                return Json(new { success = false, message = deleteResult.Errors.FirstOrDefault()?.Description });
            }
        }

        /// <summary>
        ///  Edit user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmUserUpdate)]
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

            var roles = _roleManager.Roles.AsEnumerable();
            var groups = _identityContext.AuthGroups.AsEnumerable();
            var userGroup = _identityContext.UserGroups.Where(x => x.UserId == applicationUser.Id).ToList()
                .Select(s => s.AuthGroupId.ToString()).ToList();
            var userRolesNames = await _userManager.GetRolesAsync(applicationUser);
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
                Groups = groups,
                SelectedGroupId = userGroup,
                SelectedRoleId = userRoles,
                UserPhoto = applicationUser.UserPhoto,
                AuthenticationType = applicationUser.AuthenticationType,
                TenantId = applicationUser.TenantId,
                Tenants = _identityContext.Tenants.AsNoTracking().Where(x => !x.IsDeleted).ToList(),
                FirstName = applicationUser.UserFirstName,
                LastName = applicationUser.UserLastName
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
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmUserUpdate)]
        public virtual async Task<IActionResult> Edit(Guid id, UpdateUserViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            var applicationUser = await _identityContext.Users.SingleOrDefaultAsync(m => m.Id == id);
            var roles = _roleManager.Roles.AsEnumerable();
            var groupsList = _identityContext.AuthGroups.AsEnumerable();
            var userGroupListList = _identityContext.UserGroups.Where(x => x.UserId == applicationUser.Id).ToList()
                .Select(s => s.AuthGroupId.ToString()).ToList();
            var userRolesNames = await _userManager.GetRolesAsync(applicationUser);
            var userRoleList = userRolesNames
                .Select(item => roles.FirstOrDefault(x => x.Name == item)?.Id.ToString())
                .ToList();

            model.Roles = roles;
            model.Groups = groupsList;
            model.SelectedGroupId = userGroupListList;
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
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            if (model.AuthenticationType.Equals(AuthenticationType.Ad))
            {
                var ldapUser = await _ldapUserManager.FindByNameAsync(model.UserName);
                if (ldapUser == null)
                {
                    model.SelectedRoleId = userRoleList;
                    ModelState.AddModelError("", $"There is no AD user with this username : {model.UserName}");
                    return View(model);
                }

                user.UserName = ldapUser.SamAccountName;
                user.Email = ldapUser.EmailAddress;
            }

            user.IsDeleted = model.IsDeleted;
            user.Changed = DateTime.Now;
            user.Email = model.Email;
            user.ModifiedBy = User.Identity.Name;
            user.UserName = model.UserName;
            user.TenantId = model.TenantId;
            user.UserFirstName = model.FirstName;
            user.UserLastName = model.LastName;

            if (model.UserPhotoUpdateFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.UserPhotoUpdateFile.CopyToAsync(memoryStream);
                    user.UserPhoto = memoryStream.ToArray();
                }
            }

            var result = await _userManager.UpdateAsync(user);
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

            await _userManager.RemoveFromRolesAsync(user, rolesList);

            var roleNameList = new List<string>();
            foreach (var _ in model.SelectedRoleId)
            {
                var role = await _roleManager.FindByIdAsync(_);
                roleNameList.Add(role.Name);
            }

            await _userManager.AddToRolesAsync(user, roleNameList);

            if (model.Groups != null && model.Groups.Any())
            {
                //Refresh groups
                var currentGroupsList =
                    await _identityContext.UserGroups.Where(x => x.UserId == user.Id).ToListAsync();
                _identityContext.UserGroups.RemoveRange(currentGroupsList);

                var userGroupList = model.SelectedGroupId
                    .Select(groupId => new UserGroup { UserId = user.Id, AuthGroupId = Guid.Parse(groupId) }).ToList();
                await _identityContext.UserGroups.AddRangeAsync(userGroupList);
            }

            await _userManager.UpdateSecurityStampAsync(user);

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
        /// Return list of State Or Provinces by country id
        /// </summary>
        /// <param name="countryId"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        public virtual JsonResult GetCityByCountryId([Required] string countryId)
        {
            var resultModel = new ResultModel<IEnumerable<SelectListItem>>();
            if (string.IsNullOrEmpty(countryId))
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Country id is null"));
                return Json(resultModel);
            }

            var citySelectList = _identityContext.StateOrProvinces
                .AsNoTracking()
                .Where(x => x.CountryId.Equals(countryId))
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToList();
            citySelectList.Insert(0, new SelectListItem("Select city", string.Empty));

            resultModel.Result = citySelectList;
            resultModel.IsSuccess = true;
            return Json(resultModel);
        }

        /// <summary>
        /// User profile info
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> Profile()
        {
            var currentUser = (await _customUserManager.GetCurrentUserAsync()).Result;
            if (currentUser == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                UserId = currentUser.Id,
                TenantId = currentUser.TenantId ?? Guid.Empty,
                UserName = currentUser.UserName,
                UserFirstName = currentUser.UserFirstName,
                UserLastName = currentUser.UserLastName,
                UserPhoneNumber = currentUser.PhoneNumber,
                AboutMe = currentUser.AboutMe,
                Birthday = currentUser.Birthday,
                Email = currentUser.Email,
                Roles = await _userManager.GetRolesAsync(currentUser),
                Groups = await _identityContext.UserGroups
                    .Include(x => x.AuthGroup)
                    .Where(x => x.UserId.Equals(currentUser.Id))
                    .Select(x => x.AuthGroup.Name)
                    .ToListAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Get view for edit profile info
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> EditProfile(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return NotFound();
            }

            var currentUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId));
            if (currentUser == null)
            {
                return NotFound();
            }

            var model = new UserProfileEditViewModel
            {
                Id = currentUser.Id,
                UserFirstName = currentUser.UserFirstName,
                UserLastName = currentUser.UserLastName,
                Birthday = currentUser.Birthday,
                AboutMe = currentUser.AboutMe,
                UserPhoneNumber = currentUser.PhoneNumber,
            };
            return PartialView("Partial/_EditProfilePartial", model);
        }

        /// <summary>
        /// Update user profile info
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> EditProfile(UserProfileEditViewModel model)
        {
            var resultModel = new ResultModel();
            if (!ModelState.IsValid)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid model"));
                return Json(resultModel);
            }

            var currentUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
            if (currentUser == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "User not found!"));
                return Json(resultModel);
            }

            currentUser.UserFirstName = model.UserFirstName;
            currentUser.UserLastName = model.UserLastName;
            currentUser.Birthday = model.Birthday;
            currentUser.AboutMe = model.AboutMe;
            currentUser.PhoneNumber = model.UserPhoneNumber;

            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                return Json(resultModel);
            }

            foreach (var identityError in result.Errors)
            {
                resultModel.Errors.Add(new ErrorModel(identityError.Code, identityError.Description));
            }

            return Json(resultModel);
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
            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
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
            if (model.AuthenticationType.Equals(AuthenticationType.Ad))
            {
                var ldapUser = await _ldapUserManager.FindByNameAsync(model.UserName);

                var bind = await _ldapUserManager.CheckPasswordAsync(ldapUser, model.Password);
                if (!bind)
                {
                    ModelState.AddModelError("", $"Invalid credentials for AD authentication");
                    return View(model);
                }
            }

            var user = await _userManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                ModelState.AddModelError("", "The user is no longer in the system");
                return View(model);
            }

            var hasher = new PasswordHasher<GearUser>();
            var hashedPassword = hasher.HashPassword(user, model.Password);
            user.PasswordHash = hashedPassword;
            user.LastPasswordChanged = DateTime.Now;
            var result = await _userManager.UpdateAsync(user);
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
        /// Load user with ajax
        /// </summary>
        /// <param name="hub"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [AjaxOnly]
        public JsonResult LoadUsers([FromServices] ICommunicationHub hub, DTParameters param)
        {
            var filtered = GetUsersFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount);

            var usersList = filtered.Select(async o =>
            {
                var sessions = hub.GetSessionsCountByUserId(o.Id);
                var roles = await _userManager.GetRolesAsync(o);
                var org = await _identityContext.Tenants.FirstOrDefaultAsync(x => x.Id == o.TenantId);
                return new UserListItemViewModel
                {
                    Id = o.Id,
                    UserName = o.UserName,
                    CreatedDate = o.Created.ToShortDateString(),
                    CreatedBy = o.Author,
                    ModifiedBy = o.ModifiedBy,
                    Changed = o.Changed.ToShortDateString(),
                    Roles = roles,
                    Sessions = sessions,
                    AuthenticationType = o.AuthenticationType.ToString(),
                    LastLogin = o.LastLogin,
                    Organization = org?.Name
                };
            }).Select(x => x.Result);

            var finalResult = new DTResult<UserListItemViewModel>
            {
                Draw = param.Draw,
                Data = usersList.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return Json(finalResult);
        }

        /// <summary>
        /// Get application users list filtered
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        [NonAction]
        private List<GearUser> GetUsersFiltered(string search, string sortOrder, int start, int length,
            out int totalCount)
        {
            var result = _identityContext.Users.AsNoTracking()
                .Where(p =>
                    search == null || p.Email != null &&
                    p.Email.ToLower().Contains(search.ToLower()) || p.UserName != null &&
                    p.UserName.ToLower().Contains(search.ToLower()) ||
                    p.ModifiedBy != null && p.ModifiedBy.ToLower().Contains(search.ToLower())).ToList();
            totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();
            switch (sortOrder)
            {
                case "email":
                    result = result.OrderBy(a => a.Email).ToList();
                    break;

                case "created":
                    result = result.OrderBy(a => a.Created).ToList();
                    break;

                case "userName":
                    result = result.OrderBy(a => a.UserName).ToList();
                    break;

                case "author":
                    result = result.OrderBy(a => a.Author).ToList();
                    break;

                case "changed":
                    result = result.OrderBy(a => a.Changed).ToList();
                    break;

                case "email DESC":
                    result = result.OrderByDescending(a => a.Email).ToList();
                    break;

                case "created DESC":
                    result = result.OrderByDescending(a => a.Created).ToList();
                    break;

                case "userName DESC":
                    result = result.OrderByDescending(a => a.UserName).ToList();
                    break;

                case "author DESC":
                    result = result.OrderByDescending(a => a.Author).ToList();
                    break;

                case "changed DESC":
                    result = result.OrderByDescending(a => a.Changed).ToList();
                    break;

                default:
                    result = result.AsQueryable().ToList();
                    break;
            }

            return result.ToList();
        }

        /// <summary>
        ///     Get user by id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel))]
        public JsonResult GetUserById([Required] Guid userId)
        {
            var user = _identityContext.Users.FirstOrDefault(x => x.Id == userId);
            return Json(new ResultModel
            {
                IsSuccess = true,
                Result = user
            });
        }

        /// <summary>
        /// Check if is current user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private bool IsCurrentUser(Guid id)
        {
            return id.Equals(User.Identity.Name.ToGuid());
        }

        /// <summary>
        /// Get user image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult GetImage(Guid id)
        {
            if (id == Guid.Empty)
            {
                return NotFound();
            }

            try
            {
                var photo = _identityContext.Users.SingleOrDefault(x => x.Id == id);
                if (photo?.UserPhoto != null) return File(photo.UserPhoto, "image/jpg");
                var def = GetDefaultImage();
                if (def == null) return NotFound();
                return File(def, "image/jpg");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return NotFound();
        }

        /// <summary>
        /// Get default user image
        /// </summary>
        /// <returns></returns>
        private static byte[] GetDefaultImage()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Static/Embedded Resources/user.jpg");
            if (!System.IO.File.Exists(path))
                return default;

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var binary = new BinaryReader(stream))
                {
                    var data = binary.ReadBytes((int)stream.Length);
                    return data;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return default;
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

        [HttpPost]
        public virtual async Task<JsonResult> DeleteUserAddress(Guid? id)
        {
            var resultModel = new ResultModel();
            if (!id.HasValue)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Null id"));
                return Json(resultModel);
            }

            var currentAddress = await _identityContext.Addresses.FindAsync(id.Value);
            if (currentAddress == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Address not found"));
                return Json(resultModel);
            }

            currentAddress.IsDeleted = true;
            var result = await _identityContext.PushAsync();
            if (!result.IsSuccess)
            {
                foreach (var error in result.Errors)
                {
                    resultModel.Errors.Add(new ErrorModel(error.Key, error.Message));
                }

                return Json(resultModel);
            }

            resultModel.IsSuccess = true;
            return Json(resultModel);
        }

        [HttpGet]
        public virtual PartialViewResult UserPasswordChange()
        {
            return PartialView("Partial/_ChangePassword");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> UserPasswordChange(ChangePasswordViewModel model)
        {
            var resultModel = new ResultModel();
            if (!ModelState.IsValid)
            {
                resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "Invalid model" });
                return Json(resultModel);
            }

            var currentUser = (await _customUserManager.GetCurrentUserAsync()).Result;
            if (currentUser == null)
            {
                resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "User not found" });
                return Json(resultModel);
            }

            var result = await _userManager.ChangePasswordAsync(currentUser, model.CurrentPassword, model.Password);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                IdentityEvents.Users.UserPasswordChange(new UserChangePasswordEventArgs
                {
                    Email = currentUser.Email,
                    UserName = currentUser.UserName,
                    UserId = currentUser.Id,
                    Password = model.Password
                });
                return Json(resultModel);
            }

            resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "Error on change password" });
            return Json(resultModel);
        }

        #region Partial Views

        [HttpGet]
        public virtual async Task<IActionResult> UserOrganizationPartial(Guid? tenantId)
        {
            if (!tenantId.HasValue)
            {
                return NotFound();
            }

            var tenant = await _identityContext.Tenants.FindAsync(tenantId);
            if (tenant == null)
            {
                return NotFound();
            }

            var model = new UserProfileTenantViewModel
            {
                Name = tenant.Name,
                TenantId = tenant.TenantId,
                Description = tenant.Description,
                Address = tenant.Address,
                SiteWeb = tenant.SiteWeb
            };
            return PartialView("Partial/_OrganizationPartial", model);
        }

        [HttpGet]
        public virtual IActionResult UserAddressPartial(Guid? userId)
        {
            if (!userId.HasValue)
            {
                return NotFound();
            }

            var addressList = _identityContext.Addresses
                .AsNoTracking()
                .Where(x => x.ApplicationUserId.Equals(userId.Value) && x.IsDeleted == false)
                .Include(x => x.Country)
                .Include(x => x.StateOrProvince)
                .Include(x => x.District)
                .Select(address => new UserProfileAddressViewModel
                {
                    Id = address.Id,
                    AddressLine1 = address.AddressLine1,
                    AddressLine2 = address.AddressLine2,
                    Phone = address.Phone,
                    ContactName = address.ContactName,
                    District = address.District.Name,
                    Country = address.Country.Name,
                    City = address.StateOrProvince.Name,
                    IsPrimary = address.IsDefault,
                    ZipCode = address.ZipCode,
                })
                .ToList();

            return PartialView("Partial/_AddressListPartial", addressList);
        }

        [HttpGet]
        public virtual PartialViewResult ChangeUserPasswordPartial()
        {
            return PartialView("Partial/_ChangePasswordPartial");
        }

        [HttpGet]
        public virtual async Task<IActionResult> AddUserProfileAddress()
        {
            var model = new AddUserProfileAddressViewModel
            {
                CountrySelectListItems = await GetCountrySelectList()
            };
            return PartialView("Partial/_AddUserProfileAddress", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> AddUserProfileAddress(AddUserProfileAddressViewModel model)
        {
            var resultModel = new ResultModel();

            if (!ModelState.IsValid)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid model"));
                return Json(resultModel);
            }

            var currentUser = (await _customUserManager.GetCurrentUserAsync()).Result;
            if (currentUser == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "User not found"));
                return Json(resultModel);
            }

            var address = new Address
            {
                AddressLine1 = model.AddressLine1,
                AddressLine2 = model.AddressLine2,
                Created = DateTime.Now,
                ContactName = model.ContactName,
                ZipCode = model.ZipCode,
                Phone = model.Phone,
                CountryId = model.SelectedCountryId,
                StateOrProvinceId = model.SelectedStateOrProvinceId,
                ApplicationUser = currentUser,
                IsDefault = model.IsDefault
            };

            if (model.IsDefault)
            {
                _identityContext.Addresses
                    .Where(x => x.ApplicationUserId.Equals(currentUser.Id))
                    .ToList().ForEach(b => b.IsDefault = false);
            }

            await _identityContext.Addresses.AddAsync(address);
            var result = await _identityContext.PushAsync();
            if (!result.IsSuccess)
            {
                foreach (var resultError in result.Errors)
                {
                    resultModel.Errors.Add(new ErrorModel(resultError.Key, resultError.Message));
                }

                return Json(resultModel);
            }

            resultModel.IsSuccess = true;
            return Json(resultModel);
        }

        [HttpGet]
        public virtual async Task<IActionResult> EditUserProfileAddress(Guid? addressId)
        {
            if (!addressId.HasValue)
            {
                return NotFound();
            }

            var currentAddress = await _identityContext.Addresses
                .FirstOrDefaultAsync(x => x.Id.Equals(addressId.Value));
            var cityBySelectedCountry = await _identityContext.StateOrProvinces
                .AsNoTracking()
                .Where(x => x.CountryId.Equals(currentAddress.CountryId))
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();
            if (currentAddress == null)
            {
                return NotFound();
            }

            var model = new EditUserProfileAddressViewModel
            {
                Id = currentAddress.Id,
                CountrySelectListItems = await GetCountrySelectList(),
                AddressLine1 = currentAddress.AddressLine1,
                AddressLine2 = currentAddress.AddressLine2,
                Phone = currentAddress.Phone,
                ContactName = currentAddress.ContactName,
                ZipCode = currentAddress.ZipCode,
                SelectedCountryId = currentAddress.CountryId,
                SelectedStateOrProvinceId = currentAddress.StateOrProvinceId,
                SelectedStateOrProvinceSelectListItems = cityBySelectedCountry,
                IsDefault = currentAddress.IsDefault
            };
            return PartialView("Partial/_EditUserProfileAddress", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> EditUserProfileAddress(EditUserProfileAddressViewModel model)
        {
            var resultModel = new ResultModel();

            if (!ModelState.IsValid)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Invalid model"));
                return Json(resultModel);
            }

            var currentAddress = await _identityContext.Addresses.FirstOrDefaultAsync(x => x.Id.Equals(model.Id));
            if (currentAddress == null)
            {
                resultModel.Errors.Add(new ErrorModel(string.Empty, "Address not found"));
                return Json(resultModel);
            }

            if (model.IsDefault)
            {
                _identityContext.Addresses
                    .Where(x => x.ApplicationUserId.Equals(currentAddress.ApplicationUserId))
                    .ToList().ForEach(b => b.IsDefault = false);
            }

            currentAddress.CountryId = model.SelectedCountryId;
            currentAddress.StateOrProvinceId = model.SelectedStateOrProvinceId;
            currentAddress.AddressLine1 = model.AddressLine1;
            currentAddress.AddressLine2 = model.AddressLine2;
            currentAddress.ContactName = model.ContactName;
            currentAddress.Phone = model.Phone;
            currentAddress.ZipCode = model.ZipCode;
            currentAddress.IsDefault = model.IsDefault;
            currentAddress.Changed = DateTime.Now;

            _identityContext.Update(currentAddress);
            var result = await _identityContext.PushAsync();
            if (!result.IsSuccess)
            {
                foreach (var resultError in result.Errors)
                {
                    resultModel.Errors.Add(new ErrorModel(resultError.Key, resultError.Message));
                }

                return Json(resultModel);
            }

            resultModel.IsSuccess = true;
            return Json(resultModel);
        }

        #endregion Partial Views

        [HttpPost]
        public virtual async Task<JsonResult> UploadUserPhoto(IFormFile file)
        {
            var resultModel = new ResultModel();
            if (file == null || file.Length == 0)
            {
                resultModel.IsSuccess = false;
                resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "Image not found" });
                return Json(resultModel);
            }

            var currentUser = (await _customUserManager.GetCurrentUserAsync()).Result;
            if (currentUser == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Errors.Add(new ErrorModel { Key = string.Empty, Message = "User not found" });
                return Json(resultModel);
            }

            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                currentUser.UserPhoto = memoryStream.ToArray();
            }

            var result = await _userManager.UpdateAsync(currentUser);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                return Json(resultModel);
            }

            resultModel.IsSuccess = false;
            foreach (var error in result.Errors)
            {
                resultModel.Errors.Add(new ErrorModel { Key = error.Code, Message = error.Description });
            }

            return Json(resultModel);
        }

        protected virtual async Task<IEnumerable<SelectListItem>> GetCountrySelectList()
        {
            var countrySelectList = await _identityContext.Countries
                .AsNoTracking()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                }).ToListAsync();

            countrySelectList.Insert(0, new SelectListItem(_localizer["system_select_country"], string.Empty));

            return countrySelectList;
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
        /// Return Auth Group select list items
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<SelectListItem>> GetAuthGroupSelectListItemAsync()
        {
            var authGroups = await _identityContext.AuthGroups
                .AsNoTracking()
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name
                }).ToListAsync();
            authGroups.Insert(0, new SelectListItem(_localizer["sel_group"], string.Empty));

            return authGroups;
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