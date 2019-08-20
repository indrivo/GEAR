using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Castle.Core.Internal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ST.Cache.Abstractions;
using ST.Entities.Data;
using ST.Identity.Attributes;
using ST.Identity.Data;
using ST.Identity.Data.Permissions;
using ST.Identity.Razor.Users.ViewModels.UserViewModels;
using ST.Notifications.Abstractions;
using ST.Notifications.Abstractions.Models.Notifications;
using ST.Core;
using ST.Core.Attributes;
using ST.Core.BaseControllers;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Entities.Abstractions.ViewModels.DynamicEntities;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Enums;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.Identity.LdapAuth.Abstractions;
using ST.MultiTenant.Abstractions;

namespace ST.Identity.Razor.Users.Controllers
{
    public class UsersController : BaseController<ApplicationDbContext, EntitiesDbContext, ApplicationUser,
        ApplicationRole, Tenant, INotify<ApplicationRole>>
    {
        #region Injections

        public UsersController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager,
            ICacheService cacheService, ApplicationDbContext applicationDbContext, EntitiesDbContext context,
            INotify<ApplicationRole> notify, BaseLdapUserManager<ApplicationUser> ldapUserManager,
            ILogger<UsersController> logger) : base(userManager, roleManager, cacheService,
            applicationDbContext,
            context, notify)
        {
            _ldapUserManager = ldapUserManager;
            Logger = logger;
        }

        /// <summary>
        /// Logger
        /// </summary>
        private ILogger<UsersController> Logger { get; }

        /// <summary>
        /// Inject Ldap User Manager
        /// </summary>
        private readonly BaseLdapUserManager<ApplicationUser> _ldapUserManager;

        #endregion


        /// <summary>
        /// User list for admin visualization
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmUserRead)]
        public virtual IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Create new user
        /// </summary>
        /// <returns></returns>
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmUserCreate)]
        public virtual IActionResult Create()
        {
            var model = new CreateUserViewModel
            {
                Roles = RoleManager.Roles.AsEnumerable(),
                Groups = ApplicationDbContext.AuthGroups.AsEnumerable(),
                Tenants = ApplicationDbContext.Tenants.AsEnumerable()
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
        public virtual async Task<IActionResult> Create(CreateUserViewModel model)
        {
            model.Roles = RoleManager.Roles.AsEnumerable();
            model.Groups = await ApplicationDbContext.AuthGroups.ToListAsync();
            model.Profiles = new List<EntityViewModel>();
            model.Tenants = ApplicationDbContext.Tenants.AsEnumerable();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
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
                LastPasswordChanged = DateTime.Now
            };

            if (model.UserPhoto != null)
            {
                using (var _ = new MemoryStream())
                {
                    await model.UserPhoto.CopyToAsync(_);
                    user.UserPhoto = _.ToArray();
                }
            }

            var hasher = new PasswordHasher<ApplicationUser>();
            var hashedPassword = hasher.HashPassword(user, model.Password);
            user.PasswordHash = hashedPassword;
            var result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                foreach (var _ in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, _.Description);
                }

                return View(model);
            }

            Logger.LogInformation("User {0} created successfully", user.UserName);
            var roleNameList = new List<string>();
            foreach (var _ in model.SelectedRoleId)
            {
                var role = await RoleManager.FindByIdAsync(_);
                if (role == null)
                {
                    Logger.LogWarning(
                        "The user has sent an invalid roleId which is bizarre since the role is selected from the html select tag, possible attack");
                    ModelState.AddModelError(string.Empty, "The role you've selected does not exist");
                    return View(model);
                }

                roleNameList.Add(role.Name);
            }

            var roleAddResult = await UserManager.AddToRolesAsync(user, roleNameList);
            if (!roleAddResult.Succeeded)
            {
                foreach (var _ in roleAddResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, _.Description);
                }

                return View(model);
            }

            if (model.SelectedGroupId != null && model.SelectedGroupId.Any())
            {
                var userGroupList = new List<UserGroup>();
                foreach (var _ in model.SelectedGroupId)
                {
                    userGroupList.Add(new UserGroup
                    {
                        AuthGroupId = Guid.Parse(_),
                        UserId = user.Id
                    });
                }

                await ApplicationDbContext.UserGroups.AddRangeAsync(userGroupList);
                await Context.SaveChangesAsync();
            }
            //ToDO: Modify letter !!!
            else
            {
                var groupId = await ApplicationDbContext.AuthGroups.FirstOrDefaultAsync();
                if (groupId != null)
                {
                    ApplicationDbContext.UserGroups.Add(new UserGroup
                    {
                        AuthGroupId = groupId.Id,
                        UserId = user.Id
                    });
                }

                await Context.SaveChangesAsync();
            }


            try
            {
                await Notify.SendNotificationToSystemAdminsAsync(new SystemNotifications
                {
                    Content = $"{user.UserName} was created by {User.Identity.Name}",
                    Subject = "Info",
                    NotificationTypeId = NotificationType.Info
                });
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                ModelState.AddModelError(string.Empty, "Error on save User Groups!");
                return View(model);
            }

            return RedirectToAction(nameof(Index), "Users");
        }

        /// <summary>
        /// Get Ad users
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AjaxOnly]
        public virtual JsonResult GetAdUsers()
        {
            var result = new ResultModel<IEnumerable<ApplicationUser>>();
            var addedUsers = ApplicationDbContext.Users.Where(x => x.AuthenticationType.Equals(AuthenticationType.Ad))
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

            var exists = await UserManager.FindByNameAsync(userName);
            if (exists != null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"UserName {userName} exists!"));
                return Json(result);
            }

            var user = new ApplicationUser();

            var ldapUser = await _ldapUserManager.FindByNameAsync(userName);
            if (ldapUser == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"There is no AD user with this username : {userName}"));
                return Json(result);
            }

            user.Id = Guid.NewGuid().ToString();
            user.UserName = ldapUser.SamAccountName;
            user.Email = ldapUser.EmailAddress;
            user.AuthenticationType = AuthenticationType.Ad;
            user.Created = DateTime.Now;
            user.Author = User.Identity.Name;
            user.Changed = DateTime.Now;
            user.TenantId = CurrentUserTenantId;
            var hasher = new PasswordHasher<ApplicationUser>();
            var hashedPassword = hasher.HashPassword(user, "ldap_default_password");
            user.PasswordHash = hashedPassword;
            result.IsSuccess = true;
            var req = await UserManager.CreateAsync(user);
            if (!req.Succeeded)
            {
                result.Errors.Add(new ErrorModel(string.Empty, $"Fail to add user : {userName}"));
                result.IsSuccess = false;
            }
            else
            {
                result.Result = Guid.Parse(user.Id);
            }

            return Json(result);
        }

        /// <summary>
        /// Delete user by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await ApplicationDbContext.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            return View(applicationUser);
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
        public virtual async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return Json(new {success = false, message = "Id is null"});
            }

            if (IsCurrentUser(id))
            {
                return Json(new {success = false, message = "You can't delete current user"});
            }

            var applicationUser = await ApplicationDbContext.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return Json(new {success = false, message = "User not found"});
            }

            if (applicationUser.IsEditable == false)
            {
                return Json(new {succsess = false, message = "Is system user!!!"});
            }

            try
            {
                await UserManager.UpdateSecurityStampAsync(applicationUser);
                await UserManager.DeleteAsync(applicationUser);
                await Notify.SendNotificationToSystemAdminsAsync(new SystemNotifications
                {
                    Content = $"{applicationUser.UserName} was deleted by {User.Identity.Name}",
                    Subject = "Info",
                    NotificationTypeId = NotificationType.Info
                });
                return Json(new {success = true, message = "Delete success"});
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                return Json(new {success = false, message = "Error on delete!!!"});
            }
        }

        /// <summary>
        ///     Edit user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmUserUpdate)]
        public virtual async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationUser = await ApplicationDbContext.Users.SingleOrDefaultAsync(m => m.Id == id);
            if (applicationUser == null)
            {
                return NotFound();
            }

            var roles = RoleManager.Roles.AsEnumerable();
            var groups = ApplicationDbContext.AuthGroups.AsEnumerable();
            var userGroup = ApplicationDbContext.UserGroups.Where(x => x.UserId == applicationUser.Id).ToList()
                .Select(s => s.AuthGroupId.ToString()).ToList();
            var userRolesNames = await UserManager.GetRolesAsync(applicationUser);
            var userRoles = userRolesNames.Select(item => roles.FirstOrDefault(x => x.Name == item)?.Id.ToString())
                .ToList();

            var model = new UpdateUserViewModel
            {
                Id = applicationUser.Id.ToGuid(),
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
                Tenants = ApplicationDbContext.Tenants.Where(x => !x.IsDeleted).ToList()
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
        public virtual async Task<IActionResult> Edit(string id,
            UpdateUserViewModel model)
        {
            if (Guid.Parse(id) != model.Id)
            {
                return NotFound();
            }

            var applicationUser = await ApplicationDbContext.Users.SingleOrDefaultAsync(m => m.Id == id);
            var roles = RoleManager.Roles.AsEnumerable();
            var groupsList = ApplicationDbContext.AuthGroups.AsEnumerable();
            var userGroupListList = ApplicationDbContext.UserGroups.Where(x => x.UserId == applicationUser.Id).ToList()
                .Select(s => s.AuthGroupId.ToString()).ToList();
            var userRolesNames = await UserManager.GetRolesAsync(applicationUser);
            var userRoleList = userRolesNames
                .Select(item => roles.FirstOrDefault(x => x.Name == item)?.Id.ToString())
                .ToList();

            model.Roles = roles;
            model.Groups = groupsList;
            model.SelectedGroupId = userGroupListList;
            model.Tenants = ApplicationDbContext.Tenants.Where(x => !x.IsDeleted).ToList();

            if (!ModelState.IsValid)
            {
                model.SelectedRoleId = userRoleList;
                foreach (var _ in ViewData.ModelState.Values)
                {
                    foreach (var error in _.Errors)
                    {
                        ModelState.AddModelError("", error.ErrorMessage);
                    }
                }

                return View(model);
            }

            // Update User Data
            var user = await UserManager.FindByIdAsync(id);
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

            if (model.UserPhotoUpdateFile != null)
            {
                using (var _ = new MemoryStream())
                {
                    await model.UserPhotoUpdateFile.CopyToAsync(_);
                    user.UserPhoto = _.ToArray();
                }
            }

            var result = await UserManager.UpdateAsync(user);
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
            var userRoles = await ApplicationDbContext.UserRoles.Where(x => x.UserId == user.Id).ToListAsync();
            var rolesList = new List<string>();
            foreach (var _ in userRoles)
            {
                var role = await RoleManager.FindByIdAsync(_.RoleId);
                rolesList.Add(role.Name);
            }

            await UserManager.RemoveFromRolesAsync(user, rolesList);

            var roleNameList = new List<string>();
            foreach (var _ in model.SelectedRoleId)
            {
                var role = await RoleManager.FindByIdAsync(_);
                roleNameList.Add(role.Name);
            }

            await UserManager.AddToRolesAsync(user, roleNameList);

            if (model.Groups != null && model.Groups.Any())
            {
                //Refresh groups
                var currentGroupsList =
                    await ApplicationDbContext.UserGroups.Where(x => x.UserId == user.Id).ToListAsync();
                ApplicationDbContext.UserGroups.RemoveRange(currentGroupsList);


                var userGroupList = model.SelectedGroupId
                    .Select(groupId => new UserGroup {UserId = user.Id, AuthGroupId = Guid.Parse(groupId)}).ToList();
                await ApplicationDbContext.UserGroups.AddRangeAsync(userGroupList);
            }

            try
            {
                Context.SaveChanges();
                await UserManager.UpdateSecurityStampAsync(user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            //Refresh user claims for this user
            //await user.RefreshClaims(Context, signInManager);
            await Notify.SendNotificationToSystemAdminsAsync(new SystemNotifications
            {
                Content = $"{user.UserName} was edited by {User.Identity.Name}",
                Subject = "Info",
                NotificationTypeId = NotificationType.Info
            });
            return RedirectToAction(nameof(Index));
        }


        /// <summary>
        /// User profile info
        /// </summary>
        /// <param name="organizationService"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Profile([FromServices] IOrganizationService<Tenant> organizationService)
        {
            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                return NotFound();
            }

            var model = new UserProfileViewModel
            {
                UserId = currentUser.Id.ToGuid(),
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Tenant = organizationService.GetUserOrganization(currentUser),
                Roles = await UserManager.GetRolesAsync(currentUser),
                Groups = await ApplicationDbContext.UserGroups
                    .Include(x => x.AuthGroup)
                    .Where(x => x.UserId.Equals(currentUser.Id))
                    .Select(x => x.AuthGroup.Name)
                    .ToListAsync()
            };
            return View(model);
        }


        /// <summary>
        /// Get view for change user password
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="callBackUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual async Task<IActionResult> ChangeUserPassword([Required] Guid? userId, string callBackUrl)
        {
            if (userId == null) return NotFound();
            var user = await UserManager.FindByIdAsync(userId.Value.ToString());
            if (user == null) return NotFound();
            return View(new ChangeUserPasswordViewModel
            {
                Email = user.Email,
                UserName = user.UserName,
                AuthenticationType = user.AuthenticationType,
                UserId = user.Id.ToGuid(),
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

            var user = await UserManager.FindByIdAsync(model.UserId.ToString());
            if (user == null)
            {
                ModelState.AddModelError("", "The user is no longer in the system");
                return View(model);
            }

            var hasher = new PasswordHasher<ApplicationUser>();
            var hashedPassword = hasher.HashPassword(user, model.Password);
            user.PasswordHash = hashedPassword;
            user.LastPasswordChanged = DateTime.Now;
            var result = await UserManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                await Notify.SendNotificationAsync(new List<Guid> {user.Id.ToGuid()}, new SystemNotifications
                {
                    Content = $"Your password was changed to : {model.Password}",
                    Subject = "Password changed",
                    NotificationTypeId = NotificationType.Info
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
        public virtual JsonResult LoadUsers([FromServices] INotificationHub hub, DTParameters param)
        {
            var filtered = GetUsersFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount);

            var usersList = filtered.Select(async o =>
            {
                var sessions = hub.GetSessionsCountByUserId(Guid.Parse(o.Id));
                var roles = await UserManager.GetRolesAsync(o);
                var org = await ApplicationDbContext.Tenants.FirstOrDefaultAsync(x => x.Id == o.TenantId);
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
        protected virtual List<ApplicationUser> GetUsersFiltered(string search, string sortOrder, int start, int length,
            out int totalCount)
        {
            var result = ApplicationDbContext.Users.AsNoTracking()
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
        public virtual JsonResult GetUserById([Required] Guid userId)
        {
            var user = ApplicationDbContext.Users.FirstOrDefault(x => x.Id == userId.ToString());
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
        protected virtual bool IsCurrentUser(string id)
        {
            return id.Equals(User.Identity.Name);
        }

        /// <summary>
        /// Get user image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public virtual IActionResult GetImage(string id)
        {
            if (id.IsNullOrEmpty())
            {
                return NotFound();
            }

            try
            {
                var photo = ApplicationDbContext.Users.SingleOrDefault(x => x.Id == id);
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
        protected virtual byte[] GetDefaultImage()
        {
            var path = Path.Combine(AppContext.BaseDirectory, "Static/Embedded Resources/user.jpg");
            if (!System.IO.File.Exists(path))
                return default;

            try
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var binary = new BinaryReader(stream))
                {
                    var data = binary.ReadBytes((int) stream.Length);
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
        public virtual async Task<IActionResult> VerifyName(string userName, string userNameOld)
        {
            if (userNameOld != null && userName.ToLower().Equals(userNameOld.ToLower()))
            {
                return Json(true);
            }

            if (await ApplicationDbContext.Users
                .AsNoTracking()
                .AnyAsync(x => x.UserName.ToLower().Equals(userName.ToLower())))
            {
                return Json($"User name {userName} is already in use.");
            }

            return Json(true);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<JsonResult> UserPasswordChange(ChangePasswordViewModel model)
        {
            var resultModel = new ResultModel();
            if (!ModelState.IsValid)
            {
                resultModel.IsSuccess = false;
                resultModel.Errors.Add(new ErrorModel {Key = string.Empty, Message = "Invalid model"});
                return Json(resultModel);
            }

            var currentUser = await GetCurrentUserAsync();
            if (currentUser == null)
            {
                resultModel.IsSuccess = false;
                resultModel.Errors.Add(new ErrorModel {Key = string.Empty, Message = "User not found"});
                return Json(resultModel);
            }

            var result = await UserManager.ChangePasswordAsync(currentUser, model.CurrentPassword, model.Password);
            if (result.Succeeded)
            {
                resultModel.IsSuccess = true;
                return Json(resultModel);
            }

            resultModel.Errors.Add(new ErrorModel {Key = string.Empty, Message = "Error on change password"});
            return Json(resultModel);
        }
    }
}