using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using ST.Core;
using ST.Core.Extensions;
using ST.DynamicEntityStorage.Abstractions.Extensions;
using ST.Entities.Abstractions;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Data;
using ST.Identity.Abstractions.Models.MultiTenants;
using ST.Identity.Data;
using ST.Identity.Data.Permissions;
using ST.Identity.Permissions.Abstractions.Attributes;
using ST.MultiTenant.Abstractions;
using ST.MultiTenant.Helpers;
using ST.MultiTenant.Razor.ViewModels;
using ST.MultiTenant.ViewModels;

namespace ST.MultiTenant.Razor.Controllers
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
        /// Inject entities db context
        /// </summary>
        private readonly EntitiesDbContext _entitiesDbContext;

        /// <summary>
        /// Inject logger
        /// </summary>
        private readonly ILogger<TenantController> _logger;

        /// <summary>
        /// Inject dynamic service
        /// </summary>
        private readonly IEntityRepository _service;

        /// <summary>
        /// Inject localizer
        /// </summary>
        private readonly IStringLocalizer _localizer;

        /// <summary>
        /// Inject organization service
        /// </summary>
        private readonly IOrganizationService<Tenant> _organizationService;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="logger"></param>
        /// <param name="entitiesDbContext"></param>
        /// <param name="service"></param>
        /// <param name="localizer"></param>
        /// <param name="organizationService"></param>
        public TenantController(
            ApplicationDbContext context, ILogger<TenantController> logger, EntitiesDbContext entitiesDbContext, IEntityRepository service, IStringLocalizer localizer, IOrganizationService<Tenant> organizationService)
        {
            Context = context;
            _logger = logger;
            _entitiesDbContext = entitiesDbContext;
            _service = service;
            _localizer = localizer;
            _organizationService = organizationService;
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
        /// Get list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult OrderList(DTParameters param)
        {
            var filtered = Context.Filter<Tenant>(param.Search.Value, param.SortOrder, param.Start,
                param.Length,
                out var totalCount);

            var list = filtered.Select(x => new OrganizationListViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Created = x.Created,
                Changed = x.Changed,
                ModifiedBy = x.ModifiedBy,
                Author = x.Author,
                Users = _organizationService.GetUsersByOrganization(x).Count()
            });

            var finalResult = new DTResult<OrganizationListViewModel>
            {
                Draw = param.Draw,
                Data = list.ToList(),
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };

            return Json(finalResult);
        }

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
                CountrySelectListItems = await GetCountrySelectList()
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
        public async Task<IActionResult> Create(CreateTenantViewModel data)
        {
            if (!ModelState.IsValid) return View(data);
            var tenantMachineName = TenantUtils.GetTenantMachineName(data.Name);
            if (string.IsNullOrEmpty(tenantMachineName))
            {
                ModelState.AddModelError(string.Empty, "Invalid name for tenant");
                data.CountrySelectListItems = await GetCountrySelectList();
                return View(data);
            }

            var model = data.GetBase();
            model.MachineName = tenantMachineName;
            var check = Context.Tenants.FirstOrDefault(x => x.MachineName == tenantMachineName);
            if (check != null)
            {
                ModelState.AddModelError(string.Empty, "Tenant exists");
                data.CountrySelectListItems = await GetCountrySelectList();
                return View(data);
            }

            if (data.OrganizationLogoFormFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await data.OrganizationLogoFormFile.CopyToAsync(memoryStream);
                    model.OrganizationLogo = memoryStream.ToArray();
                }
            }

            Context.Tenants.Add(model);

            var dbResult = await Context.SaveAsync();
            if (dbResult.IsSuccess)
            {
                if (!_entitiesDbContext.EntityTypes.Any(x => x.MachineName == tenantMachineName))
                {
                    _entitiesDbContext.EntityTypes.Add(new EntityType
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
                await _service.CreateDynamicTablesByReplicateSchema(model.Id, model.MachineName);

                return RedirectToAction(nameof(Index), "Tenant");
            }

            ModelState.AddModelError("", "Fail to save");
            data.CountrySelectListItems = await GetCountrySelectList();

            return View(data);

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
                CountrySelectListItems = await GetCountrySelectList()
            };
            return View(model);

        }

        /// <summary>
        /// Update tenant model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityUpdate)]
        public async Task<IActionResult> Edit(EditTenantViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var updateModel = model.GetBase();
            if (model.OrganizationLogoFormFile != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.OrganizationLogoFormFile.CopyToAsync(memoryStream);
                    updateModel.OrganizationLogo = memoryStream.ToArray();
                }
            }
            Context.Tenants.Update(updateModel);

            var dbResult = await Context.SaveAsync();
            if (dbResult.IsSuccess) return RedirectToAction(nameof(Index), "Tenant");
            ModelState.AppendResultModelErrors(dbResult.Errors);
            return View(model);
        }

        public virtual IActionResult GetImage(Guid id)
        {
            try
            {
                var photo = _organizationService.GetTenantById(id);
                if (photo?.OrganizationLogo != null) return File(photo.OrganizationLogo, "image/png");
                var def = _organizationService.GetDefaultImage();
                if (def == null) return NotFound();
                return File(def, "image/png");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return NotFound();
        }

        /// <summary>
        /// Get countries
        /// </summary>
        /// <returns></returns>
        protected virtual async Task<IEnumerable<SelectListItem>> GetCountrySelectList()
        {
            var countrySelectList = await Context.Countries
                .AsNoTracking()
                .Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id
                }).ToListAsync();

            countrySelectList.Insert(0, new SelectListItem(_localizer["system_select_country"], string.Empty));

            return countrySelectList;
        }
    }
}