using GR.Core;
using GR.Identity.Data.Permissions;
using GR.Identity.Permissions.Abstractions.Attributes;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Identity.Razor.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Tenant manipulation
    /// </summary>
    [Authorize]
    public class ApiResourceController : Controller
    {
        /// <summary>
        /// Inject context
        /// </summary>
        private ConfigurationDbContext Context { get; }

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<ApiResourceController> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        public ApiResourceController(ConfigurationDbContext context, ILogger<ApiResourceController> logger)
        {
            Context = context;
            _logger = logger;
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
        private List<ApiResource> GetOrderFiltered(string search, string sortOrder, int start, int length,
            out int totalCount)
        {
            var result = Context.ApiResources.Where(p =>
                search == null || p.Name != null &&
                p.Name.ToLower().Contains(search.ToLower()) || p.Description != null &&
                p.Description.ToLower().Contains(search.ToLower())).ToList();
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

                case "enabled":
                    result = result.OrderBy(a => a.Enabled).ToList();
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

            var finalResult = new DTResult<ApiResource>
            {
                Draw = param.Draw,
                Data = filtered.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return Json(finalResult);
        }

        /// <summary>
        /// View for create new api resource
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityCreate)]
        public IActionResult Create() => View();

        /// <summary>
        /// Add new api resource
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityCreate)]
        public async Task<IActionResult> Create(ApiResource model)
        {
            if (!ModelState.IsValid) return View(model);
            if (Context.ApiResources.Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError(string.Empty, "Name can't be used because is used by another api resource");
            }

            model.Id = await Context.ApiResources.MaxAsync(x => x.Id) + 1;
            try
            {
                await Context.ApiResources.AddAsync(model);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                ModelState.AddModelError(string.Empty, "Fail to add new api resource, try contact administrator!");
                return View(model);
            }
        }

        /// <summary>
        /// Get api resource by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityUpdate)]
        public async Task<IActionResult> Edit(int id)
        {
            var response = await Context.ApiResources.FirstOrDefaultAsync(x => x.Id == id);
            if (response == null)
            {
                return NotFound();
            }

            return View(response);
        }

        /// <summary>
        /// Update tenant model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityUpdate)]
        public async Task<IActionResult> Edit(ApiResource model)
        {
            if (!ModelState.IsValid) return View(model);
            var data = await Context.ApiResources.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (data == null)
            {
                ModelState.AddModelError("fail", "No api resource found!");
                return View(model);
            }

            data.Name = model.Name;
            data.Enabled = model.Enabled;
            data.Description = model.Description;
            try
            {
                Context.ApiResources.Update(data);
                await Context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                ModelState.AddModelError("fail", e.ToString());
            }

            return View(model);
        }

        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityDelete)]
        public JsonResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return Json(new { success = false, message = "Id not found" });
            }

            var tenant = Context.ApiResources.AsNoTracking().SingleOrDefault(x => x.Id == id);
            if (tenant == null)
            {
                return Json(new { success = false, message = "Api resource not found" });
            }

            try
            {
                Context.ApiResources.Remove(tenant);
                Context.SaveChanges();
                return Json(new { success = true, message = "Api resource deleted !" });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Json(new { success = false, message = "Error on save in DB" });
            }
        }
    }
}