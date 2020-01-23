using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GR.Core;
using GR.Core.Extensions;
using GR.Entities.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Data;
using GR.Identity.Data.Permissions;
using GR.Identity.Permissions.Abstractions.Attributes;
using GR.MultiTenant.Abstractions;
using GR.MultiTenant.Abstractions.ViewModels;

namespace GR.MultiTenant.Razor.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Tenant manipulation
    /// </summary>
    [Authorize]
    public class TenantController : Controller
    {
        #region Services

        /// <summary>
        /// Inject context
        /// </summary>
        private ApplicationDbContext Context { get; }

        /// <summary>
        /// Inject dynamic service
        /// </summary>
        private readonly IEntityService _service;

        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="service"></param>
        /// <param name="organizationService"></param>
        public TenantController(
            ApplicationDbContext context, IEntityService service, IOrganizationService<Tenant> organizationService)
        {
            Context = context;
            _service = service;
            _organizationService = organizationService;
        }

        /// <summary>
        /// List with tenants
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityRead)]
        public IActionResult Index() => View();

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrderList(DTParameters param) => Json(_organizationService.GetFilteredList(param));

        /// <summary>
        /// View for create a tenant
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityCreate)]
        public async Task<IActionResult> Create()
        {
            var model = new CreateTenantViewModel
            {
                CountrySelectListItems = await _organizationService.GetCountrySelectListAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Add new tenant
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityCreate)]
        public async Task<IActionResult> Create([Required]CreateTenantViewModel data)
        {
            if (!ModelState.IsValid)
            {
                data.CountrySelectListItems = await _organizationService.GetCountrySelectListAsync();
                return View(data);
            }
            var reqTenant = await _organizationService.CreateOrganizationAsync(data);

            if (reqTenant.IsSuccess)
            {
                var generateResult = await _service.GenerateTablesForTenantAsync(reqTenant.Result);
                if (generateResult.IsSuccess) return RedirectToAction(nameof(Index));
                ModelState.AppendResultModelErrors(generateResult.Errors);

                return View(reqTenant.Result);
            }

            ModelState.AppendResultModelErrors(reqTenant.Errors);

            return View(reqTenant.Result);
        }

        /// <summary>
        /// Get tenant by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityUpdate)]
        public async Task<IActionResult> Edit(Guid id)
        {
            var response = Context.Tenants.FirstOrDefault(x => x.Id == id);
            if (response == null) return RedirectToAction(nameof(Index), "Tenant");
            var model = new EditTenantViewModel(response)
            {
                CountrySelectListItems = await _organizationService.GetCountrySelectListAsync()
            };
            return View(model);
        }

        /// <summary>
        /// Update tenant model
        /// </summary>
        /// <param name="model"></param>
        /// <param name="callBack"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityUpdate)]
        public async Task<IActionResult> Edit(EditTenantViewModel model, string callBack = null)
        {
            if (!ModelState.IsValid) return View(model);
            var dbTenant = _organizationService.GetTenantById(model.Id);
            var updateModel = model.GetBase();
            if (model.OrganizationLogoFormFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.OrganizationLogoFormFile.CopyToAsync(memoryStream);
                    updateModel.OrganizationLogo = memoryStream.ToArray();
                }
            }
            else
            {
                updateModel.OrganizationLogo = dbTenant.OrganizationLogo;
                updateModel.MachineName = dbTenant.MachineName;
            }

            Context.Tenants.Update(updateModel);

            var dbResult = await Context.SaveAsync();
            if (dbResult.IsSuccess && !string.IsNullOrEmpty(callBack))
            {
                return Redirect(callBack);
            }

            if (dbResult.IsSuccess) return RedirectToAction(nameof(Index), "Tenant");
            ModelState.AppendResultModelErrors(dbResult.Errors);
            return View(model);
        }

        /// <summary>
        /// Get company image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public virtual IActionResult GetImage(Guid id)
        {
            if (id == Guid.Empty)
            {
                return File(_organizationService.GetDefaultImage(), "image/png");
            }

            var photo = _organizationService.GetTenantById(id);
            return File(photo?.OrganizationLogo ?? _organizationService.GetDefaultImage(), "image/png");
        }
    }
}