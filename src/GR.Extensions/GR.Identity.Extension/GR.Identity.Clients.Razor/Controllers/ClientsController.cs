using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Identity.Abstractions;
using GR.Identity.Clients.Abstractions.ViewModels.ApiClientViewModels;
using GR.Identity.Clients.Abstractions.ViewModels.PermissionViewModels;
using GR.Identity.Permissions.Abstractions;
using GR.Identity.Permissions.Abstractions.Extensions;
using GR.Identity.Permissions.Abstractions.Permissions;
using GR.Identity.Roles.Razor.ViewModels.RoleViewModels;
using GR.Notifications.Abstractions;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GR.Identity.Clients.Razor.Controllers
{
    [Authorize]
    public sealed class ClientsController : Controller
    {

        #region Injectable

        /// <summary>
        /// Inject configuration context
        /// </summary>
        private readonly ConfigurationDbContext _configurationDbContext;

        /// <summary>
        /// Inject identity context
        /// </summary>
        private readonly IPermissionsContext _applicationDbContext;

        /// <summary>
        /// Inject role manager
        /// </summary>
        private readonly RoleManager<GearRole> _roleManager;

        /// <summary>
        /// Inject Logger
        /// </summary>
        private readonly ILogger<ClientsController> _logger;

        /// <summary>
        /// Inject permission Service
        /// </summary>
        private readonly IPermissionService _permissionService;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationDbContext"></param>
        /// <param name="applicationDbContext"></param>
        /// <param name="roleManager"></param>
        /// <param name="logger"></param>
        /// <param name="permissionService"></param>
        public ClientsController(ConfigurationDbContext configurationDbContext,
            IPermissionsContext applicationDbContext, RoleManager<GearRole> roleManager,
            ILogger<ClientsController> logger, IPermissionService permissionService)
        {
            _configurationDbContext = configurationDbContext;
            _applicationDbContext = applicationDbContext;
            _roleManager = roleManager;
            _logger = logger;
            _permissionService = permissionService;
        }

        /// <summary>
        /// Index
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get application list for jQuery grid
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> ApplicationList(DTParameters param)
        {
            var result = await _configurationDbContext.Clients
                .AsNoTracking()
                .GetPagedAsDtResultAsync(param);
            return Json(result);
        }

        /// <summary>
        /// Create client
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ApiClientCreateViewModel { AvailableApiScopes = GetAvailableApiScopes() };
            return View(model);
        }

        /// <summary>
        /// Get available api scopes
        /// </summary>
        /// <returns></returns>
        [NonAction]
        private IEnumerable<SelectListItem> GetAvailableApiScopes()
        {
            var ls = new List<SelectListItem>();

            ls.AddRange(_configurationDbContext.ApiResources.Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Name
            }));

            ls.AddRange(_configurationDbContext.IdentityResources.Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Name
            }));

            return ls;
        }

        /// <summary>
        /// Create application
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ApiClientCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.AvailableApiScopes = GetAvailableApiScopes();
                return View();
            }

            try
            {
                var cl = new Client
                {
                    ClientName = model.ClientName,
                    ClientId = model.ClientId,
                    AllowedGrantTypes = GetGrantTypesFromViewModel(model),
                    ClientUri = model.ClientUri,
                    AllowOfflineAccess = true,
                    ClientSecrets = new List<Secret>
                    {
                        new Secret("very_secret".Sha256())
                    }
                };
                var parsed = cl.ToEntity();
                parsed.Id = await _configurationDbContext.Clients.MaxAsync(x => x.Id) + 1;
                foreach (var item in parsed.AllowedGrantTypes)
                {
                    item.Id = new Random().Next(1, 9999);
                }

                _configurationDbContext.Clients.Add(parsed);
                _configurationDbContext.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is SqlException exception)
                {
                    switch (exception.Number)
                    {
                        case 2601:
                            ModelState.AddModelError(string.Empty, "The API Client Already Exists");
                            //_logger.LogError(exception, "The API Client already exists");
                            break;

                        default:
                            ModelState.AddModelError(string.Empty, "An unknown error occured");
                            //_logger.LogError(exception, "Unknown sql error");
                            break;
                    }
                }
                else
                {
                    //_logger.LogError(ex, "A db error occured");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
            }

            model.AvailableApiScopes = GetAvailableApiScopes();
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var app = await _configurationDbContext.Clients.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            if (app == null)
            {
                return NotFound();
            }

            ViewBag.Id = app.Id;
            return View();
        }

        /// <summary>
        /// GetGrantTypesFromViewModel
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private static ICollection<string> GetGrantTypesFromViewModel(ApiClientCreateViewModel model)
        {
            var result = new Collection<string>();
            if (model.AuthorizationCodeGrantType)
                result.Add(GrantType.AuthorizationCode);
            if (model.ImplicitGrantType)
                result.Add(GrantType.Implicit);
            if (model.ClientCredentialsGrantType)
                result.Add(GrantType.ClientCredentials);
            if (model.HybridGrantType)
                result.Add(GrantType.Hybrid);
            if (model.ResourceOwnerPasswordGrantType)
                result.Add(GrantType.ResourceOwnerPassword);
            return result;
        }

        /// <summary>
        /// Get general info about current application
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> GeneralInfoPartialView(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var appClient = await _configurationDbContext.Clients.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            var model = new ApiClientGeneralViewModel
            {
                Id = appClient.Id,
                ClientName = appClient.ClientName,
                Description = appClient.Description,
                //ClientId = appClient.ClientId,
                ClientUri = appClient.ClientUri
            };

            return PartialView("Partial/GeneralInfoPartialView", model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> GeneralInfoPartialView(int id, ApiClientGeneralViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(false);
            }

            var client = _configurationDbContext.Clients.Find(id);
            if (client == null)
            {
                return Json(false);
            }

            try
            {
                _configurationDbContext.Entry(client).CurrentValues.SetValues(model);
                await _configurationDbContext.SaveChangesAsync();
                return Json(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Json(false);
            }
        }

        public IActionResult RolesPartialView(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            ViewBag.ApplicationId = id;
            return PartialView("Partial/RolesPartialView");
        }

        [HttpPost]
        public JsonResult RolesList(int? id, DTParameters param)
        {
            var filtered = GetRolesFiltered(id, param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount);

            var finalResult = new DTResult<GearRole>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return Json(finalResult);
        }

        private List<GearRole> GetRolesFiltered(int? id, string search, string sortOrder, int start, int length,
            out int totalCount)
        {
            var result = _applicationDbContext.Roles.AsNoTracking().ToList().Where(x =>
                x.ClientId == id && (search == null || x.Name != null &&
                                     x.Name.ToLower().Contains(search.ToLower()) || x.Title != null &&
                                     x.Title.ToLower().Contains(search.ToLower()))).ToList();
            totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();
            switch (sortOrder)
            {
                case "name":
                    result = result.OrderBy(a => a.Name).ToList();
                    break;

                case "title":
                    result = result.OrderBy(a => a.Title).ToList();
                    break;

                case "name DESC":
                    result = result.OrderByDescending(a => a.Name).ToList();
                    break;

                case "title DESC":
                    result = result.OrderByDescending(a => a.Title).ToList();
                    break;

                default:
                    result = result.AsQueryable().ToList();
                    break;
            }

            return result.ToList();
        }

        /// <summary>
        /// Return view with list of permissions
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult PermissionsPartialView(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var result = _applicationDbContext.Permissions.AsNoTracking().Where(x => x.ClientId == id).ToList();
            var resultList = new List<ApiClientPermissions>();
            foreach (var item in result)
            {
                var permission = new ApiClientPermissions
                {
                    PermissionName = item.PermissionName,
                    ClientId = item.ClientId,
                    PermissionId = item.Id
                };
                resultList.Add(permission);
            }

            ViewBag.ApplicationId = id;
            return PartialView("Partial/PermissionsPartialView", resultList);
        }

        public IActionResult CreateRolePartialView(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var model = new CreateRoleViewModel
            {
                PermissionsList = _applicationDbContext.Permissions.AsNoTracking().Where(x => x.ClientId == id)
                    .AsEnumerable()
            };
            ViewBag.ClientId = id;
            return PartialView("Partial/CreateRolePartialView", model);
        }

        [HttpPost]
        public async Task<JsonResult> CreateRolePartialView(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model is not valid" });
            }

            var applicationRole = new GearRole
            {
                Name = model.Name,
                Title = model.Title,
                IsDeleted = model.IsDeleted,
                Created = DateTime.Now,
                Author = User.Identity.Name,
                ClientId = model.ClientId,
            };
            var result = await _roleManager.CreateAsync(applicationRole);
            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Error on create role" });
            }


            var roleId = await _applicationDbContext.Roles.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Name == model.Name);

            if (roleId == null)
            {
                return Json(new { success = false, message = "Role not found!!!" });
            }

            if (model.SelectedPermissionId.Any())
            {
                foreach (var _ in model.SelectedPermissionId)
                {
                    var permission =
                        await _applicationDbContext.Permissions.SingleOrDefaultAsync(x => x.Id == Guid.Parse(_));
                    if (permission != null)
                    {
                        var newRolePermission = new RolePermission
                        {
                            RoleId = roleId.Id,
                            PermissionId = permission.Id
                        };
                        try
                        {
                            await _applicationDbContext.RolePermissions.AddAsync(newRolePermission);
                            await _applicationDbContext.SaveChangesAsync();
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.Message);
                            return Json(new { success = false, message = "Error on save!" });
                        }
                    }
                }
            }

            return Json(new { success = true, message = "Save success!!!" });
        }

        public async Task<IActionResult> EditRolePartialView(Guid? id)
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

            var rolePermissionId = await _applicationDbContext.Set<RolePermission>().Where(x => x.RoleId == id)
                .Select(x => x.PermissionId.ToString()).ToListAsync();

            var model = new UpdateRoleViewModel
            {
                Id = applicationRole.Id,
                Name = applicationRole.Name,
                Title = applicationRole.Title,
                IsDeleted = applicationRole.IsDeleted,
                IsNoEditable = applicationRole.IsNoEditable,
                Permissions = await _applicationDbContext.Permissions.AsNoTracking()
                    .Where(x => x.ClientId == applicationRole.ClientId).ToListAsync(),
                SelectedPermissionId = rolePermissionId
            };
            return PartialView("Partial/EditRolePartialView", model);
        }

        [HttpPost]
        public async Task<JsonResult> EditRolePartialView([FromServices] SignInManager<GearUser> signInManager,
            [FromServices] ICommunicationHub hub, Guid id, UpdateRoleViewModel model)
        {
            if (id != model.Id)
            {
                return Json(new { success = false, message = "Not found" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model is not valid" });
            }

            var applicationRole = await _roleManager.FindByIdAsync(id.ToString());
            applicationRole.Name = model.Name;
            applicationRole.Title = model.Title;
            applicationRole.IsDeleted = model.IsDeleted;
            var result = await _roleManager.UpdateAsync(applicationRole);
            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Cant update role" });
            }

            var role = await _applicationDbContext.Roles.SingleOrDefaultAsync(m => m.Name == model.Name);
            if (role != null)
            {
                //Delete previous permissions
                var rolePermissionId = _applicationDbContext.Set<RolePermission>()
                    .Where(x => x.RoleId == applicationRole.Id);
                _applicationDbContext.RolePermissions.RemoveRange(rolePermissionId);
                await _applicationDbContext.SaveChangesAsync();
                if (model.SelectedPermissionId != null)
                {
                    foreach (var _ in model.SelectedPermissionId)
                    {
                        var permission =
                            await _applicationDbContext.Permissions.SingleOrDefaultAsync(x => x.Id == Guid.Parse(_));
                        if (permission != null)
                        {
                            var newRolePermission = new RolePermission
                            {
                                RoleId = id,
                                PermissionId = permission.Id
                            };
                            await _applicationDbContext.RolePermissions.AddAsync(newRolePermission);
                            await _applicationDbContext.SaveChangesAsync();
                        }
                    }
                }

                var onlineUsers = hub.GetOnlineUsers();
                await User.RefreshOnlineUsersClaims(_applicationDbContext, signInManager, onlineUsers);
            }

            return Json(new { success = true, message = "Save success!!" });
        }

        [HttpPost]
        public JsonResult PermissionList(int? id, DTParameters param)
        {
            var filtered = GetPermissionsFiltered(id, param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount);

            var permissionList = filtered.Select(x => new PermissionListItemViewModel
            {
                Id = x.Id,
                ClientName = _configurationDbContext.Clients.AsNoTracking()
                    .SingleOrDefaultAsync(z => z.Id.Equals(x.ClientId)).Result.ClientName,
                PermissionDescription = x.Description,
                PermissionKey = x.PermissionKey,
                PermissionName = x.PermissionName
            });

            var finalResult = new DTResult<PermissionListItemViewModel>
            {
                Draw = param.Draw,
                Data = permissionList.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return Json(finalResult);
        }

        /// <summary>
        /// Get permissions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private List<Permission> GetPermissionsFiltered(int? id, string search, string sortOrder, int start, int length,
            out int totalCount)
        {
            var result = _applicationDbContext.Permissions.AsNoTracking().ToList().Where(x =>
                x.ClientId == id && (search == null || x.PermissionName != null &&
                                     x.PermissionName.ToLower().Contains(search.ToLower()) || x.Author != null &&
                                     x.Author.ToLower().Contains(search.ToLower()) ||
                                     x.Description != null && x.Description.ToLower().Contains(search) ||
                                     x.PermissionKey != null && x.PermissionKey.Contains(search))).ToList();
            totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();
            switch (sortOrder)
            {
                case "clientName":
                    result = result.OrderBy(a => a.ClientId).ToList();
                    break;

                case "permissionName":
                    result = result.OrderBy(a => a.PermissionName).ToList();
                    break;

                case "author":
                    result = result.OrderBy(a => a.Author).ToList();
                    break;

                case "isDeleted":
                    result = result.OrderBy(a => a.IsDeleted).ToList();
                    break;

                case "permissionDescription":
                    result = result.OrderBy(a => a.Description).ToList();
                    break;

                case "permissionKey":
                    result = result.OrderBy(a => a.PermissionKey).ToList();
                    break;

                case "id DESC":
                    result = result.OrderByDescending(a => a.Id).ToList();
                    break;

                case "clientName DESC":
                    result = result.OrderByDescending(a => a.ClientId).ToList();
                    break;

                case "permissionName DESC":
                    result = result.OrderByDescending(a => a.PermissionName).ToList();
                    break;

                case "author DESC":
                    result = result.OrderByDescending(a => a.Author).ToList();
                    break;

                case "isDeleted DESC":
                    result = result.OrderByDescending(a => a.IsDeleted).ToList();
                    break;

                case "permissionDescription DESC":
                    result = result.OrderByDescending(a => a.Description).ToList();
                    break;

                case "permissionKey DESC":
                    result = result.OrderByDescending(a => a.PermissionKey).ToList();
                    break;

                default:
                    result = result.AsQueryable().ToList();
                    break;
            }

            return result.ToList();
        }

        /// <summary>
        /// Get permission for update
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult EditPermissionPartialView(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var permission = _applicationDbContext.Permissions.AsNoTracking().FirstOrDefault(x => x.Id == id);
            if (permission == null)
            {
                return NotFound();
            }

            var model = new EditPermissionViewModel
            {
                Description = permission.Description,
                PermissionName = permission.PermissionName,
                PermissionId = permission.Id
            };
            return PartialView("Partial/EditPermissionPartialView", model);
        }

        [HttpPost]
        public async Task<JsonResult> EditPermissionPartialView(EditPermissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model is not valid" });
            }

            var entity = _applicationDbContext.Permissions.SingleOrDefault(x => x.Id == model.PermissionId);
            if (entity == null)
            {
                return Json(new { success = false, message = "Permission not found" });
            }

            entity.Changed = DateTime.Now;
            entity.Description = model.Description;
            entity.PermissionName = model.PermissionName;

            try
            {
                await _applicationDbContext.SaveChangesAsync();
                //var onlineUsers = hub.GetOnlineUsers();
                //await User.RefreshOnlineUsersClaims(ApplicationDbContext, signInManager, onlineUsers);
                return Json(new { success = true, message = "Save successful " });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Delete permission
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> DeleteRole(Guid? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id not found" });
            }

            var isUsed = _applicationDbContext.UserRoles.Any(x => x.RoleId == id);
            if (isUsed)
            {
                return Json(new { success = false, message = "Role is used!" });
            }

            var applicationRole = await _roleManager.FindByIdAsync(id.ToString());
            if (applicationRole == null)
            {
                return Json(new { success = false, message = "Role not found!" });
            }

            try
            {
                await _roleManager.DeleteAsync(applicationRole);
                await _applicationDbContext.SaveChangesAsync();
                await _permissionService.RefreshCacheByRoleAsync(applicationRole.Name, true);
                return Json(new { success = true, message = "Delete success" });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Json(new { success = false, message = "Error on delete!!!" });
            }
        }

        /// <summary>
        /// Delete application
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteApplication(int? id)
        {
            if (!id.HasValue)
            {
                return Json(false);
            }

            var client = await _configurationDbContext.Clients.SingleOrDefaultAsync(x => x.Id == id);
            var permissionList = await _applicationDbContext.Permissions.Where(x => x.ClientId == id).ToListAsync();
            if (client == null)
            {
                return Json(new { message = "Client not found!", success = false });
            }

            if (permissionList != null)
            {
                try
                {
                    _applicationDbContext.Permissions.RemoveRange(permissionList);
                    await _applicationDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return Json(new { message = e.ToString(), success = false });
                }
            }

            try
            {
                _configurationDbContext.Clients.Remove(client);
                await _configurationDbContext.SaveChangesAsync();
                await _permissionService.SetOrResetPermissionsOnCacheAsync();
                return Json(new { message = "Deleted", success = true });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Json(new { message = e.ToString(), success = false });
            }
        }

        /// <summary>
        /// Check if permission exist
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool IsExist(CreatePermissionViewModel model)
        {
            var result = _applicationDbContext.Permissions.Where(x => x.ClientId == model.ClientId).FirstOrDefault(x =>
                x.PermissionName.ToLower().Equals(model.PermissionName.ToLower()) ||
                x.PermissionKey.ToLower().Equals(model.PermissionKey.ToLower()));
            return result != null;
        }
    }
}