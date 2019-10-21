using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GR.Core;
using GR.Core.Helpers;
using GR.Entities.Security.Abstractions;
using GR.Entities.Security.Razor.ViewModels;

namespace GR.Entities.Security.Razor.Controllers
{
    [Authorize(Roles = GlobalResources.Roles.ADMINISTRATOR)]
    public class EntitySecurityController : Controller
    {
        /// <summary>
        /// Inject role access manager
        /// </summary>
        private readonly IEntityRoleAccessManager _accessManager;

        public EntitySecurityController(IEntityRoleAccessManager accessManager)
        {
            _accessManager = accessManager;
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
        /// Save entity permissions
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveEntityMappedPermissions([Required]EntityRolesPermissionsViewModel model)
        {
            var result = new ResultModel();
            if (!ModelState.IsValid) return Json(result);
            var serviceResult =
                await _accessManager.SetPermissionsForRoleOnEntityAsync(model.EntityId, model.RoleId,
                    model.Permissions);
            return Json(serviceResult);
        }
    }
}
