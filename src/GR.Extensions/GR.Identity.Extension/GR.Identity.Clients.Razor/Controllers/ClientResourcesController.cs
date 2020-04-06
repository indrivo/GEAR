using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Identity.Clients.Abstractions;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Clients.Razor.Controllers
{
    [Authorize]
    public class ClientResourcesController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly IClientsContext _context;

        /// <summary>
        /// Inject clients service
        /// </summary>
        private readonly IClientsService _clientsService;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public ClientResourcesController(IClientsContext context, IClientsService clientsService)
        {
            _context = context;
            _clientsService = clientsService;
        }

        /// <summary>
        /// List with tenants
        /// </summary>
        /// <returns></returns>
        [HttpGet]

        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> OrderList(DTParameters param)
        {
            var filtered = await _context.ApiResources.GetPagedAsDtResultAsync(param);
            return Json(filtered);
        }

        /// <summary>
        /// View for create new api resource
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Create() => View();

        /// <summary>
        /// Add new api resource
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Create(ApiResource model)
        {
            if (!ModelState.IsValid) return View(model);
            if (_context.ApiResources.Any(x => x.Name == model.Name))
            {
                ModelState.AddModelError(string.Empty, "Name can't be used because is used by another api resource");
            }

            model.Id = await _context.ApiResources.MaxAsync(x => x.Id) + 1;
            try
            {
                await _context.ApiResources.AddAsync(model);
                await _context.SaveChangesAsync();
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
        public async Task<IActionResult> Edit(int id)
        {
            var response = await _clientsService.FindApiResourceByIdAsync(id);
            if (!response.IsSuccess) return NotFound();
            return View(response.Result);
        }

        /// <summary>
        /// Update tenant model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Edit(ApiResource model)
        {
            if (!ModelState.IsValid) return View(model);
            var updateResult = await _clientsService.UpdateApiResourceAsync(model);
            if (updateResult.IsSuccess) return RedirectToAction(nameof(Index));
            ModelState.AppendResultModelErrors(updateResult.Errors);
            return View(model);
        }

        /// <summary>
        /// Delete role
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<JsonResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return Json(new { success = false, message = "Id not found" });
            }

            var tenant = _context.ApiResources.AsNoTracking().SingleOrDefault(x => x.Id == id);
            if (tenant == null)
            {
                return Json(new { success = false, message = "Api resource not found" });
            }

            _context.ApiResources.Remove(tenant);
            var dbResult = await _context.PushAsync();
            return Json(dbResult.IsSuccess
                ? new { success = true, message = "Api resource deleted !" }
                : new { success = false, message = "Error on save in DB" });
        }
    }
}