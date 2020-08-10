using System;
using System.Linq;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GR.Core;
using GR.Core.Extensions;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Helpers;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.TableTypes;
using GR.Identity.Permissions.Abstractions.Attributes;

namespace GR.Entities.Razor.Controllers.Entity
{
    /// <inheritdoc />
    /// <summary>
    /// Schema manipulation
    /// </summary>
    [Authorize]
    public class EntityTypeController : Controller
    {
        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IEntityContext _context;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<EntityTypeController> _logger;

        public EntityTypeController(IEntityContext context, ILogger<EntityTypeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// List with schemas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(EntityTypePermissions.EntityTypeRead)]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> OrderList(DTParameters param)
        {
            var filtered = await _context.EntityTypes.GetPagedAsDtResultAsync(param);
            return Json(filtered);
        }


        /// <summary>
        /// Check if schema exist or not
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult CheckEnitityType(string type)
        {
            if (type == null) return Json(null);
            var result = _context.EntityTypes.FirstOrDefault(x => x.Name == type);
            return Json(result != null);
        }

        /// <summary>
        /// View for create a schema
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(EntityTypePermissions.EntityTypeCreate)]
        public IActionResult Create() => View();

        /// <summary>
        /// Add new schema
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(EntityTypePermissions.EntityTypeCreate)]
        public IActionResult Create(EntityTypeCreateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                _context.EntityTypes.Add(model.Adapt<EntityType>());
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), "EntityType");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("fail", e.Message);
            }

            return View(model);
        }

        /// <summary>
        /// Get schema by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(EntityTypePermissions.EntityTypeUpdate)]
        public IActionResult Edit(Guid id)
        {
            var response = _context.EntityTypes.FirstOrDefault(x => x.Id == id);
            if (response != null)
            {
                return View(response.Adapt<EntityTypeUpdateViewModel>());
            }

            return RedirectToAction(nameof(Index), "EntityType");
        }

        /// <summary>
        /// Update schema model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(EntityTypePermissions.EntityTypeUpdate)]
        public IActionResult Edit(EntityTypeUpdateViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            try
            {
                _context.EntityTypes.Update(model.Adapt<EntityType>());
                _context.SaveChanges();
                return RedirectToAction(nameof(Index), "EntityType");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("fail", e.Message);
            }

            return View(model);
        }


        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        [AuthorizePermission(EntityTypePermissions.EntityTypeDelete)]
        public JsonResult Delete(Guid? id)
        {
            if (!id.HasValue)
            {
                return Json(new { success = false, message = "Id not found" });
            }

            var typeName = _context.EntityTypes.AsNoTracking().SingleOrDefault(x => x.Id == id);
            if (typeName == null)
            {
                return Json(new { success = false, message = "EntityTypes not found" });
            }

            var isUsed = _context.Table.Any(x => x.EntityType == typeName.Name);
            if (isUsed)
            {
                return Json(new { success = false, message = "EntityTypes is used" });
            }

            try
            {
                _context.EntityTypes.Remove(typeName);
                _context.SaveChanges();
                return Json(new { success = true, message = "EntityType deleted !" });
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return Json(new { success = false, message = "Error on save in DB" });
            }
        }
    }
}