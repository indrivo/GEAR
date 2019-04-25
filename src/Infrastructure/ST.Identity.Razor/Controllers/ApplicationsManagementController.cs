using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ST.Identity.Abstractions;
using ST.Identity.Data;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Extensions;
using ST.Identity.Razor.ViewModels.ApiClientViewModels;
using ST.Identity.Razor.ViewModels.PermissionViewModels;
using ST.Identity.Razor.ViewModels.RoleViewModels;
using ST.Notifications.Abstractions;
using ST.Shared;
using Client = IdentityServer4.EntityFramework.Entities.Client;

namespace ST.Identity.Razor.Controllers
{
    [Authorize]
    public class ApplicationsManagementController : Controller
    {
        private ConfigurationDbContext ConfigurationDbContext { get; }
        private PersistedGrantDbContext GrantDbContext { get; }

        private ApplicationDbContext ApplicationDbContext { get; }
        private RoleManager<ApplicationRole> RoleManager { get; }

        /// <summary>
        /// Inject Logger
        /// </summary>
        private readonly ILogger<ApplicationsManagementController> _logger;

        /// <summary>
        /// Inject permission Service
        /// </summary>
        private readonly IPermissionService _permissionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="configurationDbContext"></param>
        /// <param name="applicationDbContext"></param>
        /// <param name="roleManager"></param>
        /// <param name="logger"></param>
        /// <param name="permissionService"></param>
        /// <param name="grantDbContext"></param>
        public ApplicationsManagementController(ConfigurationDbContext configurationDbContext,
            ApplicationDbContext applicationDbContext, RoleManager<ApplicationRole> roleManager,
            ILogger<ApplicationsManagementController> logger, IPermissionService permissionService, PersistedGrantDbContext grantDbContext)
        {
            ConfigurationDbContext = configurationDbContext;
            ApplicationDbContext = applicationDbContext;
            RoleManager = roleManager;
            _logger = logger;
            _permissionService = permissionService;
            GrantDbContext = grantDbContext;
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
        public JsonResult ApplicationList(DTParameters param)
        {
            var filtered = GetApplicationClientsFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount);
            var finalResult = new DTResult<Client>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return Json(finalResult);
        }

        /// <summary>
        /// Get application list filtered
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private List<Client> GetApplicationClientsFiltered(string search, string sortOrder, int start, int length,
            out int totalCount)
        {
            var result = ConfigurationDbContext.Clients.AsNoTracking()
                .Where(p =>
                    search == null || p.ClientName != null &&
                    p.ClientName.ToLower().Contains(search.ToLower()) || p.Description != null &&
                    p.Description.ToLower().Contains(search.ToLower()) || p.RedirectUris != null &&
                    p.RedirectUris.ToString().ToLower().Contains(search.ToLower()) || p.ProtocolType != null &&
                    p.ProtocolType.ToString().ToLower().Contains(search.ToLower())).ToList();
            totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();
            switch (sortOrder)
            {
                case "clientName":
                    result = result.OrderBy(a => a.ClientName).ToList();
                    break;
                case "clientId":
                    result = result.OrderBy(a => a.ClientId).ToList();
                    break;
                case "allowOfflineAccess":
                    result = result.OrderBy(a => a.AllowOfflineAccess).ToList();
                    break;
                case "allowedGrantTypes":
                    result = result.OrderBy(a => a.AllowedGrantTypes).ToList();
                    break;
                case "allowedScopes":
                    result = result.OrderBy(a => a.AllowedScopes).ToList();
                    break;
                case "clientUri":
                    result = result.OrderBy(a => a.ClientUri).ToList();
                    break;
                case "clientName DESC":
                    result = result.OrderByDescending(a => a.ClientName).ToList();
                    break;
                case "clientId DESC":
                    result = result.OrderByDescending(a => a.ClientId).ToList();
                    break;
                case "allowOfflineAccess DESC":
                    result = result.OrderByDescending(a => a.AllowOfflineAccess).ToList();
                    break;
                case "allowedGrantTypes DESC":
                    result = result.OrderByDescending(a => a.AllowedGrantTypes).ToList();
                    break;
                case "allowedScopes DESC":
                    result = result.OrderByDescending(a => a.AllowedScopes).ToList();
                    break;
                case "clientUri DESC":
                    result = result.OrderByDescending(a => a.ClientUri).ToList();
                    break;
                default:
                    result = result.AsQueryable().ToList();
                    break;
            }

            return result.ToList();
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

            ls.AddRange(ConfigurationDbContext.ApiResources.Select(f => new SelectListItem
            {
                Text = f.Name,
                Value = f.Name
            }));

            ls.AddRange(ConfigurationDbContext.IdentityResources.Select(f => new SelectListItem
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
                var cl = new IdentityServer4.Models.Client
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
                parsed.Id = await ConfigurationDbContext.Clients.MaxAsync(x => x.Id) + 1;
                foreach (var item in parsed.AllowedGrantTypes)
                {
                    item.Id = new Random().Next(1, 9999);
                }

                ConfigurationDbContext.Clients.Add(parsed);
                ConfigurationDbContext.SaveChanges();
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

            var app = await ConfigurationDbContext.Clients.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
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
        public async Task<IActionResult> _GeneralInfo(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var appClient = await ConfigurationDbContext.Clients.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
            var model = new ApiClientGeneralViewModel
            {
                Id = appClient.Id,
                ClientName = appClient.ClientName,
                Description = appClient.Description,
                //ClientId = appClient.ClientId,
                ClientUri = appClient.ClientUri
            };

            return PartialView(model);
        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> _GeneralInfo(int id, ApiClientGeneralViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var client = ConfigurationDbContext.Clients.Find(id);
            if (client == null)
            {
                return Json(false);
            }

            try
            {
                ConfigurationDbContext.Entry(client).CurrentValues.SetValues(model);
                await ConfigurationDbContext.SaveChangesAsync();
                return Json(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Json(false);
            }
        }

        public IActionResult _Roles(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            ViewBag.ApplicationId = id;
            return PartialView();
        }

        [HttpPost]
        public JsonResult RolesList(int? id, DTParameters param)
        {
            var filtered = GetRolesFiltered(id, param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount);

            var finalResult = new DTResult<ApplicationRole>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return Json(finalResult);
        }

        private List<ApplicationRole> GetRolesFiltered(int? id, string search, string sortOrder, int start, int length,
            out int totalCount)
        {
            var result = ApplicationDbContext.Roles.AsNoTracking().ToList().Where(x =>
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
        public IActionResult _Permissions(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var result = ApplicationDbContext.Permissions.AsNoTracking().Where(x => x.ClientId == id).ToList();
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
            return PartialView(resultList);
        }


        public IActionResult _CreateRole(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var model = new CreateRoleViewModel
            {
                PermissionsList = ApplicationDbContext.Permissions.AsNoTracking().Where(x => x.ClientId == id)
                    .AsEnumerable(),
                Profiles = ApplicationDbContext.Profiles.AsNoTracking().Where(x => x.IsDeleted == false)
            };
            ViewBag.ClientId = id;
            return PartialView(model);
        }

        [HttpPost]
        public async Task<JsonResult> _CreateRole(CreateRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model is not valid" });
            }

            var applicationRole = new ApplicationRole
            {
                Name = model.Name,
                Title = model.Title,
                IsDeleted = model.IsDeleted,
                Created = DateTime.Now,
                Author = User.Identity.Name,
                ClientId = model.ClientId,
            };
            var result = await RoleManager.CreateAsync(applicationRole);
            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Error on create role" });
            }

            if (model.SelectedProfileId == null)
            {
                return Json(new { success = false, message = "Select profile" });
            }

            var roleId = await ApplicationDbContext.Roles.AsNoTracking()
                .SingleOrDefaultAsync(x => x.Name == model.Name);

            if (roleId == null)
            {
                return Json(new { success = false, message = "Role not found!!!" });
            }

            foreach (var _ in model.SelectedProfileId)
            {
                var newRoleProfile = new RoleProfile
                {
                    ApplicationRoleId = roleId.Id,
                    ProfileId = Guid.Parse(_)
                };
                try
                {
                    await ApplicationDbContext.AddAsync(newRoleProfile);
                    await ApplicationDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return Json(new { success = false, message = "Error on save!" });
                }
            }

            if (model.SelectedPermissionId.Any())
            {
                foreach (var _ in model.SelectedPermissionId)
                {
                    var permission =
                        await ApplicationDbContext.Permissions.SingleOrDefaultAsync(x => x.Id == Guid.Parse(_));
                    if (permission != null)
                    {
                        var newRolePermission = new RolePermission
                        {
                            PermissionCode = permission.PermissionKey,
                            RoleId = roleId.Id,
                            PermissionId = permission.Id
                        };
                        try
                        {
                            await ApplicationDbContext.AddAsync(newRolePermission);
                            await ApplicationDbContext.SaveChangesAsync();
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

        public async Task<IActionResult> _EditRole(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var applicationRole = await RoleManager.FindByIdAsync(id);
            if (applicationRole == null)
            {
                return NotFound();
            }

            var roleProfilesId = await ApplicationDbContext.Set<RoleProfile>()
                .Where(x => x.ApplicationRoleId == applicationRole.Id).Select(x => x.ProfileId.ToString())
                .ToListAsync();
            var rolePermissionId = await ApplicationDbContext.Set<RolePermission>().Where(x => x.RoleId == id)
                .Select(x => x.PermissionId.ToString()).ToListAsync();

            var model = new UpdateRoleViewModel
            {
                Profiles = await ApplicationDbContext.Profiles.Where(x => x.IsDeleted == false)
                    .ToListAsync(), // ApplicationDbContext.GetAll<Profile>(x => x.IsDeleted == false),
                Id = applicationRole.Id,
                Name = applicationRole.Name,
                SelectedProfileId = roleProfilesId,
                Title = applicationRole.Title,
                IsDeleted = applicationRole.IsDeleted,
                IsNoEditable = applicationRole.IsNoEditable,
                Permissions = await ApplicationDbContext.Permissions.AsNoTracking()
                    .Where(x => x.ClientId == applicationRole.ClientId).ToListAsync(),
                SelectedPermissionId = rolePermissionId
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<JsonResult> _EditRole([FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] INotificationHub hub, string id, UpdateRoleViewModel model)
        {
            if (id != model.Id)
            {
                return Json(new { success = false, message = "Not found" });
            }

            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model is not valid" });
            }

            var applicationRole = await RoleManager.FindByIdAsync(id);
            applicationRole.Name = model.Name;
            applicationRole.Title = model.Title;
            applicationRole.IsDeleted = model.IsDeleted;
            var result = await RoleManager.UpdateAsync(applicationRole);
            if (!result.Succeeded)
            {
                return Json(new { success = false, message = "Cant update role" });
            }

            var roleProfilesId = ApplicationDbContext.Set<RoleProfile>()
                .Where(x => x.ApplicationRoleId == applicationRole.Id);
            ApplicationDbContext.RemoveRange(roleProfilesId);
            await ApplicationDbContext.SaveChangesAsync();
            if (model.SelectedProfileId == null)
            {
                return Json(new { success = false, message = "Select profile" });
            }

            var role = await ApplicationDbContext.Roles.SingleOrDefaultAsync(m => m.Name == model.Name);
            if (role != null)
            {
                foreach (var _ in model.SelectedProfileId)
                {
                    var newRoleProfile = new RoleProfile
                    {
                        ApplicationRoleId = role.Id,
                        ProfileId = Guid.Parse(_)
                    };
                    await ApplicationDbContext.AddAsync(newRoleProfile);
                    await ApplicationDbContext.SaveChangesAsync();
                }

                //Delete previous permissions
                var rolePermissionId = ApplicationDbContext.Set<RolePermission>()
                    .Where(x => x.RoleId == applicationRole.Id);
                ApplicationDbContext.RemoveRange(rolePermissionId);
                await ApplicationDbContext.SaveChangesAsync();
                if (model.SelectedPermissionId != null)
                {
                    foreach (var _ in model.SelectedPermissionId)
                    {
                        var permission =
                            await ApplicationDbContext.Permissions.SingleOrDefaultAsync(x => x.Id == Guid.Parse(_));
                        if (permission != null)
                        {
                            var newRolePermission = new RolePermission
                            {
                                PermissionCode = permission.PermissionKey,
                                RoleId = id,
                                PermissionId = permission.Id
                            };
                            await ApplicationDbContext.AddAsync(newRolePermission);
                            await ApplicationDbContext.SaveChangesAsync();
                        }
                    }
                }

                var onlineUsers = hub.GetOnlineUsers();
                await User.RefreshOnlineUsersClaims(ApplicationDbContext, signInManager, onlineUsers);
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
                ClientName = ConfigurationDbContext.Clients.AsNoTracking()
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
        /// Get permmisions
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
            var result = ApplicationDbContext.Permissions.AsNoTracking().ToList().Where(x =>
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
        public IActionResult _EditPermission(Guid? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var permission = ApplicationDbContext.Permissions.AsNoTracking().FirstOrDefault(x => x.Id == id);
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
            return PartialView(model);
        }

        [HttpPost]
        public async Task<JsonResult> _EditPermission([FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] INotificationHub hub, EditPermissionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model is not valid" });
            }

            var entity = ApplicationDbContext.Permissions.SingleOrDefault(x => x.Id == model.PermissionId);
            if (entity == null)
            {
                return Json(new { success = false, message = "Permission not found" });
            }

            entity.Changed = DateTime.Now;
            entity.Description = model.Description;
            entity.PermissionName = model.PermissionName;

            try
            {
                await ApplicationDbContext.SaveChangesAsync();
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
        public async Task<JsonResult> DeleteRole(string id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Id not found" });
            }

            var isUsed = ApplicationDbContext.UserRoles.Any(x => x.RoleId == id);
            if (isUsed)
            {
                return Json(new { success = false, message = "Role is used!" });
            }

            var applicationRole = await RoleManager.FindByIdAsync(id);
            if (applicationRole == null)
            {
                return Json(new { success = false, message = "Role not found!" });
            }

            try
            {
                await RoleManager.DeleteAsync(applicationRole);
                var roleProfilesId = ApplicationDbContext.Set<RoleProfile>()
                    .Where(x => x.ApplicationRoleId == applicationRole.Id);
                ApplicationDbContext.RemoveRange(roleProfilesId);
                await ApplicationDbContext.SaveChangesAsync();
                await _permissionService.RefreshCacheByRole(applicationRole.Name, true);
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

            var client = await ConfigurationDbContext.Clients.SingleOrDefaultAsync(x => x.Id == id);
            var permissionList = await ApplicationDbContext.Permissions.Where(x => x.ClientId == id).ToListAsync();
            if (client == null)
            {
                return Json(new { message = "Client not found!", success = false });
            }

            if (permissionList != null)
            {
                try
                {
                    ApplicationDbContext.Permissions.RemoveRange(permissionList);
                    await ApplicationDbContext.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    return Json(new { message = e.ToString(), success = false });
                }
            }

            try
            {
                ConfigurationDbContext.Remove(client);
                await ConfigurationDbContext.SaveChangesAsync();
                await _permissionService.RefreshCache();
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
            var result = ApplicationDbContext.Permissions.Where(x => x.ClientId == model.ClientId).FirstOrDefault(x =>
                x.PermissionName.ToLower().Equals(model.PermissionName.ToLower()) ||
                x.PermissionKey.ToLower().Equals(model.PermissionKey.ToLower()));
            return result != null;
        }
    }
}