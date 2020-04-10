using GR.Core;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.Abstractions.ViewModels.RoleViewModels;
using GR.Identity.Abstractions.ViewModels.UserViewModels;
using GR.Identity.Permissions.Abstractions;
using GR.Identity.Permissions.Abstractions.Attributes;
using GR.Identity.Roles.Razor.ViewModels.RoleViewModels;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions.Helpers;
using GR.Identity.Clients.Abstractions;
using GR.Identity.Permissions.Abstractions.Permissions;

namespace GR.Identity.Roles.Razor.Controllers
{
    public class RolesController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject configuration db context
        /// </summary>
        private IClientsContext ConfigurationDbContext { get; }

        /// <summary>
        /// Inject sign in manager
        /// </summary>
        private readonly SignInManager<GearUser> _signInManager;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<RolesController> _logger;

        /// <summary>
        /// Inject permission dataService
        /// </summary>
        private readonly IPermissionService _permissionService;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject identity context
        /// </summary>
        private readonly IIdentityContext _applicationDbContext;

        /// <summary>
        /// Inject permissions context
        /// </summary>
        private readonly IPermissionsContext _permissionsContext;

        /// <summary>
        /// Inject role manager
        /// </summary>
        private readonly RoleManager<GearRole> _roleManager;

        /// <summary>
        /// Inject notifier
        /// </summary>
        private readonly INotify<GearRole> _notify;

        #endregion

        public RolesController(SignInManager<GearUser> signInManager, ILogger<RolesController> logger, IPermissionService permissionService, IClientsContext configurationDbContext, IIdentityContext applicationDbContext, RoleManager<GearRole> roleManager, IUserManager<GearUser> userManager, INotify<GearRole> notify, IPermissionsContext permissionsContext)
        {
            _signInManager = signInManager;
            _logger = logger;
            _permissionService = permissionService;
            ConfigurationDbContext = configurationDbContext;
            _applicationDbContext = applicationDbContext;
            _roleManager = roleManager;
            _userManager = userManager;
            _notify = notify;
            _permissionsContext = permissionsContext;
        }

        /// <summary>
        /// RoleProfile / Add
        /// </summary>
        /// <returns></returns>
        [AuthorizePermission(RolePermissions.CreateRole)]
        public async Task<IActionResult> Create()
        {
            var model = new CreateRoleViewModel
            {
                Clients = await ConfigurationDbContext.Clients.AsNoTracking().ToListAsync()
            };

            return View(model);
        }

        /// <summary>
        ///     Create role
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(RolePermissions.CreateRole)]
        public async Task<IActionResult> Create(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Clients = await ConfigurationDbContext.Clients.AsNoTracking().ToListAsync();
                return View(model);
            }

            if (ApplicationRoleExists(model.Name))
            {
                model.Clients = await ConfigurationDbContext.Clients.AsNoTracking().ToListAsync();
                ModelState.AddModelError("", "Role with same name exist!");
                return View(model);
            }

            var applicationRole = new GearRole
            {
                Name = model.Name,
                Title = model.Title,
                IsDeleted = model.IsDeleted,
                ClientId = model.ClientId,
            };
            var result = await _roleManager.CreateAsync(applicationRole);
            var user = await _signInManager.UserManager.GetUserAsync(User);
            var client = ConfigurationDbContext.Clients.AsNoTracking().FirstOrDefault(x => x.Id.Equals(model.ClientId))
                ?.ClientName;
            await _notify.SendNotificationAsync(new Notification
            {
                Content = $"{user?.UserName} created the role {applicationRole.Name} for {client}",
                Subject = "Info",
                NotificationTypeId = NotificationType.Info
            });
            if (!result.Succeeded)
            {
                model.Clients = await ConfigurationDbContext.Clients.AsNoTracking().ToListAsync();
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }
            else
            {
                var role = await _applicationDbContext.Roles.AsNoTracking().SingleOrDefaultAsync(m => m.Name == model.Name);
                if (role == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                if (model.SelectedPermissionId.Any())
                {
                    var listOfRolePermission = new List<RolePermission>();
                    foreach (var _ in model.SelectedPermissionId)
                    {
                        var permission = await _permissionsContext.Permissions.AsNoTracking()
                            .SingleOrDefaultAsync(x => x.Id == Guid.Parse(_));
                        if (permission != null)
                        {
                            listOfRolePermission.Add(new RolePermission
                            {
                                PermissionId = permission.Id,
                                RoleId = role.Id
                            });
                        }
                    }

                    await _permissionsContext.RolePermissions.AddRangeAsync(listOfRolePermission);
                }

                try
                {
                    await _permissionsContext.SaveChangesAsync();
                    await _permissionService.RefreshCacheByRoleAsync(applicationRole.Name);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// <summary>
        /// POST: Roles/Delete
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(RolePermissions.DeleteRole)]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            if (id == null)
            {
                return Json(new { message = "Not found!", success = false });
            }

            var applicationRole = await _roleManager.FindByIdAsync(id.ToString());
            if (applicationRole == null)
            {
                return Json(new { message = "Role not found!", success = false });
            }

            if (applicationRole.IsNoEditable)
            {
                return Json(new { message = "Is system role!", success = false });
            }

            if (_applicationDbContext.UserRoles.Any(x => x.RoleId == id))
            {
                return Json(new { message = "Role is used!", success = false });
            }

            var rolePermissionsList =
                _permissionsContext.RolePermissions.AsNoTracking().Where(x => x.RoleId.Equals(applicationRole.Id));
            if (await rolePermissionsList.AnyAsync())
            {
                try
                {
                    _permissionsContext.RolePermissions.RemoveRange(rolePermissionsList);
                    await _permissionsContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return Json(new { message = "Error on delete role permissions!", success = false });
                }
            }

            try
            {
                await _roleManager.DeleteAsync(applicationRole);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Json(new { message = "Error on delete role !", success = false });
            }

            await _notify.SendNotificationToSystemAdminsAsync(new Notification
            {
                Content = $"{User.Identity.Name} deleted the role {applicationRole.Name}",
                Subject = "Info",
                NotificationTypeId = NotificationType.Info
            });
            await _permissionService.RefreshCacheByRoleAsync(applicationRole.Name, true);
            return Json(new { message = "Role was delete with success!", success = true });
        }

        /// <summary>
        /// GET: Roles/Edit
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(RolePermissions.EditRole)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationRole = await _roleManager.FindByIdAsync(id.ToString());
            if (applicationRole == null)
            {
                return NotFound();
            }

            var rolePermissionId = await _permissionsContext.RolePermissions.Where(x => x.RoleId == id)
                .Select(x => x.PermissionId.ToString()).ToListAsync();

            var model = new UpdateRoleViewModel
            {
                Id = applicationRole.Id,
                ClientName = ConfigurationDbContext.Clients.FirstOrDefault(x => x.Id.Equals(applicationRole.ClientId))
                    ?.ClientName,
                Name = applicationRole.Name,
                Title = applicationRole.Title,
                IsDeleted = applicationRole.IsDeleted,
                IsNoEditable = applicationRole.IsNoEditable,
                Permissions = await _permissionsContext.Permissions.AsNoTracking()
                    .Where(x => x.ClientId == applicationRole.ClientId).ToListAsync(),
                SelectedPermissionId = rolePermissionId,
                TenantId = applicationRole.TenantId
            };
            return View(model);
        }

        /// <summary>
        /// POST: Roles/Edit
        /// </summary>
        /// <param name="hub"></param>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(RolePermissions.EditRole)]
        public async Task<IActionResult> Edit([FromServices] ICommunicationHub hub, Guid? id, UpdateRoleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var applicationRole = await _roleManager.FindByIdAsync(id.ToString());
            applicationRole.Name = model.Name;
            applicationRole.Title = model.Title;
            applicationRole.IsNoEditable = model.IsNoEditable;
            applicationRole.IsDeleted = model.IsDeleted;
            applicationRole.ModifiedBy = User.Identity.Name;
            applicationRole.Changed = DateTime.Now;
            applicationRole.TenantId = model.TenantId;

            model.Permissions =
                await _permissionsContext.Permissions.Where(x => x.ClientId == applicationRole.ClientId).ToListAsync();
            var result = await _roleManager.UpdateAsync(applicationRole);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                var role = await _applicationDbContext.Roles.SingleOrDefaultAsync(m => m.Name == model.Name);
                if (role == null)
                {
                    return RedirectToAction(nameof(Index));
                }

                //Delete previous permissions
                var rolePermissionId =
                    _permissionsContext.RolePermissions.Where(x => x.RoleId == applicationRole.Id);
                if (await rolePermissionId.AnyAsync())
                {
                    _permissionsContext.RolePermissions.RemoveRange(rolePermissionId);
                    await _permissionsContext.SaveChangesAsync();
                }

                var rolePermissionList = new List<RolePermission>();
                foreach (var _ in model.SelectedPermissionId)
                {
                    var permission = await _permissionsContext.Permissions.SingleOrDefaultAsync(x => x.Id == Guid.Parse(_));
                    if (permission == null) continue;
                    rolePermissionList.Add(new RolePermission
                    {
                        RoleId = id.Value,
                        PermissionId = permission.Id
                    });
                }

                var user = await _signInManager.UserManager.GetUserAsync(User);

                try
                {
                    await _permissionsContext.RolePermissions.AddRangeAsync(rolePermissionList);
                    await _permissionsContext.SaveChangesAsync();
                    await _permissionService.RefreshCacheByRoleAsync(applicationRole.Name);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    ModelState.AddModelError("", "Error on add role permission!");
                    return View(model);
                }

                //var onlineUsers = hub.GetOnlineUsers();
                //await User.RefreshOnlineUsersClaims(Context, _signInManager, onlineUsers);
                await _notify.SendNotificationToSystemAdminsAsync(new Notification
                {
                    Content = $"{user.UserName} edited the role {applicationRole.Name}",
                    Subject = "Info",
                    NotificationTypeId = NotificationType.Info
                });
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        /// <summary>
        /// Get list of roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(RolePermissions.ReadRole)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get application roles list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ApplicationRolesList(DTParameters param)
        {
            var filtered = GetApplicationRoleFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount);
            var list = filtered.Adapt<List<RoleListViewModel>>();
            foreach (var role in list)
            {
                role.ClientName = ConfigurationDbContext.Clients.FirstOrDefault(x => x.Id.Equals(role.ClientId))
                    ?.ClientName;
            }

            var finalResult = new DTResult<RoleListViewModel>
            {
                Draw = param.Draw,
                Data = list,
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return Json(finalResult);
        }

        /// <summary>
        /// Get application role filtered
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private List<GearRole> GetApplicationRoleFiltered(string search, string sortOrder, int start, int length,
            out int totalCount)
        {
            var result = _applicationDbContext.Roles.Where(p =>
                search == null || p.Name != null &&
                p.Name.ToLower().Contains(search.ToLower()) || p.Author != null &&
                p.Author.ToLower().Contains(search.ToLower()) || p.ModifiedBy != null &&
                p.ModifiedBy.ToString().ToLower().Contains(search.ToLower()) || p.Created != null &&
                p.Created.ToString(CultureInfo.InvariantCulture).ToLower().Contains(search.ToLower())).ToList();
            totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();
            switch (sortOrder)
            {
                case "name":
                    result = result.OrderBy(a => a.Name).ToList();
                    break;

                case "created":
                    result = result.OrderBy(a => a.Created).ToList();
                    break;

                case "author":
                    result = result.OrderBy(a => a.Author).ToList();
                    break;

                case "modifiedBy":
                    result = result.OrderBy(a => a.ModifiedBy).ToList();
                    break;

                case "changed":
                    result = result.OrderBy(a => a.Changed).ToList();
                    break;

                case "isDeleted":
                    result = result.OrderBy(a => a.IsDeleted).ToList();
                    break;

                case "name DESC":
                    result = result.OrderByDescending(a => a.Name).ToList();
                    break;

                case "created DESC":
                    result = result.OrderByDescending(a => a.Created).ToList();
                    break;

                case "author DESC":
                    result = result.OrderByDescending(a => a.Author).ToList();
                    break;

                case "modifiedBy DESC":
                    result = result.OrderByDescending(a => a.ModifiedBy).ToList();
                    break;

                case "changed DESC":
                    result = result.OrderByDescending(a => a.Changed).ToList();
                    break;

                case "isDeleted DESC":
                    result = result.OrderByDescending(a => a.IsDeleted).ToList();
                    break;

                default:
                    result = result.AsQueryable().ToList();
                    break;
            }

            return result.ToList();
        }

        /// <summary>
        /// Check if Application Role exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private bool ApplicationRoleExists(string name)
        {
            return _applicationDbContext.Roles.Any(e => e.Name == name);
        }

        /// <summary>
        /// Check role name
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> CheckRoleName(string roleName)
        {
            if (roleName == null)
            {
                return Json(null);
            }

            var result = await _applicationDbContext.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
            return Json(result != null);
        }

        /// <summary>
        /// Get permissions by client
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult GetPermissionsByClient(int? id)
        {
            if (!id.HasValue)
            {
                return Json(true);
            }

            return Json(_permissionsContext.Permissions.Where(x => x.ClientId == id).Select(x => new
            {
                x.Id,
                x.PermissionName
            }).ToList());
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        [Route("api/[controller]/[action]")]
        [HttpGet]
        [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
        public async Task<IActionResult> RefreshCachedPermissionsForEachRole()
        {
            await _permissionService.SetOrResetPermissionsOnCacheAsync();
            return StatusCode(200);
        }

        /// <summary>
        /// Get users in role for current company
        /// </summary>
        /// <param name="roleName"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<SampleGetUserViewModel>>))]
        [Route("api/[controller]/[action]")]
        public async Task<JsonResult> GetUsersInRoleForCurrentCompany([Required] string roleName) => Json(await _userManager.GetUsersInRoleForCurrentCompanyAsync(roleName));

        /// <summary>
        /// Change user roles
        /// </summary>
        /// <returns></returns>
        [GearAuthorize(GlobalResources.Roles.ADMINISTRATOR, "Company Administrator")]
        [HttpPost]
        [Produces("application/json", Type = typeof(ResultModel))]
        [Route("api/[controller]/[action]")]
        public async Task<JsonResult> ChangeUserRoles(Guid? userId, IEnumerable<Guid> roles)
            => await JsonAsync(_userManager.ChangeUserRolesAsync(userId, roles));

        /// <summary>
        /// Get current user roles
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<SampleGetUserViewModel>>))]
        [Route("api/[controller]/[action]")]
        public async Task<JsonResult> GetUserRoles([Required]string userId)
        {
            var user = await _userManager.UserManager.FindByIdAsync(userId);
            if (user == null) return Json(new NotFoundResultModel());
            var roles = (await _userManager.GetUserRolesAsync(user)).Select(x => new BaseRoleViewModel
            {
                Id = x.Id,
                Name = x.Name
            });

            return Json(new SuccessResultModel<IEnumerable<BaseRoleViewModel>>(roles));
        }

        /// <summary>
        /// Get  user roles
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<Dictionary<string, object>>))]
        [Route("api/[controller]/[action]")]
        public async Task<JsonResult> GetUserRolesWithAllRoles([Required]string userId)
        {
            var user = await _userManager.UserManager.FindByIdAsync(userId);
            if (user == null) return Json(new NotFoundResultModel());
            var roles = (await _userManager.GetUserRolesAsync(user)).Select(x => new BaseRoleViewModel
            {
                Id = x.Id,
                Name = x.Name
            });

            var dict = new Dictionary<string, object>
            {
                { "UserRoles", roles },
                { "AllRoles",  _roleManager.Roles.Select(x => new BaseRoleViewModel
                {
                    Id = x.Id,
                    Name = x.Name
                })}
            };

            return Json(new SuccessResultModel<Dictionary<string, object>>(dict));
        }

        /// <summary>
        /// Get all roles
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<SampleGetUserViewModel>>))]
        [Route("api/[controller]/[action]")]
        public JsonResult GetAllRolesAsync()
            => Json(new SuccessResultModel<IEnumerable<BaseRoleViewModel>>(_roleManager.Roles.Select(x => new BaseRoleViewModel
            {
                Id = x.Id,
                Name = x.Name
            })));
    }
}