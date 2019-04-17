using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ST.BaseBusinessRepository;
using ST.DynamicEntityStorage.Abstractions;
using ST.Entities.Data;
using ST.Identity.Attributes;
using ST.Identity.Data;
using ST.Identity.Data.MultiTenants;
using ST.Identity.Data.Permissions;
using ST.Organization.Utils;
using ST.Organization.ViewModels;
using ST.Shared;

namespace ST.Identity.Razor.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Tenant manipulation
    /// </summary>
    [Authorize]
    public class TenantController : Controller
    {
        /// <summary>
        /// Inject context
        /// </summary>
        private ApplicationDbContext Context { get; }

        /// <summary>
        /// Inject entities db context
        /// </summary>
        private readonly EntitiesDbContext _entitiesDbContext;

        /// <summary>
        /// Inject Business repository
        /// </summary>
        private IBaseBusinessRepository<ApplicationDbContext> Repository { get; }

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<TenantController> _logger;

        /// <summary>
        /// Inject dynamic service
        /// </summary>
        private readonly IDynamicService _service;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="configuration"></param>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="entitiesDbContext"></param>
        public TenantController(IBaseBusinessRepository<ApplicationDbContext> repository, IConfiguration configuration,
            ApplicationDbContext context, ILogger<TenantController> logger, EntitiesDbContext entitiesDbContext, IDynamicService service)
        {
            Repository = repository;
            Context = context;
            _logger = logger;
            _entitiesDbContext = entitiesDbContext;
            _service = service;
        }

        /// <summary>
        /// List with tenants
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityRead)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get ordered list
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        private List<Tenant> GetOrderFiltered(string search, string sortOrder, int start, int length,
            out int totalCount)
        {
            var result = Context.Tenants.Where(p =>
                search == null || p.Name != null &&
                p.Name.ToLower().Contains(search.ToLower()) || p.Description != null &&
                p.Description.ToLower().Contains(search.ToLower()) || p.Author != null &&
                p.Author.ToString().ToLower().Contains(search.ToLower()) || p.ModifiedBy != null &&
                p.ModifiedBy.ToString().ToLower().Contains(search.ToLower())).ToList();
            totalCount = result.Count;

            result = result.Skip(start).Take(length).ToList();
            switch (sortOrder)
            {
                case "id":
                    result = result.OrderBy(a => a.Id).ToList();
                    break;
                case "name":
                    result = result.OrderBy(a => a.Name).ToList();
                    break;
                case "description":
                    result = result.OrderBy(a => a.Description).ToList();
                    break;
                case "created":
                    result = result.OrderBy(a => a.Created).ToList();
                    break;
                case "modifiedBy":
                    result = result.OrderBy(a => a.ModifiedBy).ToList();
                    break;
                case "isDeleted":
                    result = result.OrderBy(a => a.IsDeleted).ToList();
                    break;
                case "id DESC":
                    result = result.OrderByDescending(a => a.Id).ToList();
                    break;
                case "name DESC":
                    result = result.OrderByDescending(a => a.Name).ToList();
                    break;
                case "description DESC":
                    result = result.OrderByDescending(a => a.Description).ToList();
                    break;
                case "created DESC":
                    result = result.OrderByDescending(a => a.Created).ToList();
                    break;
                case "modifiedBy DESC":
                    result = result.OrderByDescending(a => a.ModifiedBy).ToList();
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
        /// Get list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrderList(DTParameters param)
        {
            var filtered = GetOrderFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
                out var totalCount);

            var finalResult = new DTResult<Tenant>
            {
                draw = param.Draw,
                data = filtered.ToList(),
                recordsFiltered = totalCount,
                recordsTotal = filtered.Count
            };

            return Json(finalResult);
        }

        /// <summary>
        /// View for create a tenant
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityCreate)]
        public IActionResult Create() => View();

        /// <summary>
        /// Add new tenant
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityCreate)]
        public async Task<IActionResult> Create(CreateTenantViewModel data)
        {
            if (!ModelState.IsValid) return View(data);
            var tenantMachineName = TenantUtils.GetTenantMachineName(data.Name);
            if (string.IsNullOrEmpty(tenantMachineName))
            {
                ModelState.AddModelError(string.Empty, "Invalid name for tenant");
                return View(data);
            }
            var model = data.Adapt<Tenant>();
            model.MachineName = tenantMachineName;
            var check = Context.Tenants.FirstOrDefault(x => x.MachineName == tenantMachineName);
            if (check != null)
            {
                ModelState.AddModelError(string.Empty, "Tenant exists");
                return View(data);
            }
            var response = Repository.Add<Tenant, Tenant>(model);
            if (response.IsSuccess)
            {
                if (!_entitiesDbContext.EntityTypes.Any(x => x.MachineName == tenantMachineName))
                {
                    _entitiesDbContext.EntityTypes.Add(new Entities.Models.Tables.EntityType
                    {
                        MachineName = tenantMachineName,
                        Author = "System",
                        Created = DateTime.Now,
                        Changed = DateTime.Now,
                        Name = tenantMachineName,
                        Description = $"Generated schema on created {data.Name} tenant"
                    });
                    _entitiesDbContext.SaveChanges();
                }
                await _service.CreateDynamicTables(model.Id, model.MachineName);

                return RedirectToAction(nameof(Index), "Tenant");
            }
            else
            {
                foreach (var e in response.Errors)
                {
                    ModelState.AddModelError(e.Key, e.Message);
                }

                return View(data);
            }
        }

        /// <summary>
        /// Get tenant by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityUpdate)]
        public IActionResult Edit(Guid id)
        {
            var response = Repository.GetById<Tenant, Tenant>(id);
            if (response.IsSuccess)
            {
                return View(response.Result);
            }

            return RedirectToAction(nameof(Index), "Tenant");
        }

        /// <summary>
        /// Update tenant model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityUpdate)]
        public IActionResult Edit(Tenant model)
        {
            if (!ModelState.IsValid) return View(model);
            var response = Repository.Refresh<Tenant, Tenant>(model);
            if (response.IsSuccess)
            {
                return RedirectToAction(nameof(Index), "Tenant");
            }
            else
            {
                foreach (var e in response.Errors)
                {
                    ModelState.AddModelError(e.Key, e.Message);
                }

                return View(model);
            }
        }


        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityDelete)]
        public JsonResult Delete(Guid? id)
        {
            if (!id.HasValue)
            {
                return Json(new { success = false, message = "Id not found" });
            }

            var tenant = Context.Tenants.AsNoTracking().SingleOrDefault(x => x.Id == id);
            if (tenant == null)
            {
                return Json(new { success = false, message = "Tenant not found" });
            }

            try
            {
                Context.Tenants.Remove(tenant);
                Context.SaveChanges();
                return Json(new { success = true, message = "Tenant deleted !" });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Json(new { success = false, message = "Error on save in DB" });
            }
        }
    }
}