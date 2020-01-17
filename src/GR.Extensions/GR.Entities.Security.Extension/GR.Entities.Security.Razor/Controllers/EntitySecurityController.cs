using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using GR.Core;
using GR.Core.BaseControllers;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Entities.Security.Abstractions;
using GR.Entities.Security.Abstractions.Helpers;
using GR.Entities.Security.Razor.ViewModels;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Helpers.Attributes;

namespace GR.Entities.Security.Razor.Controllers
{
    [Authorize]
    [Roles(GlobalResources.Roles.ADMINISTRATOR)]
    public class EntitySecurityController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject role access manager
        /// </summary>
        private readonly IEntityRoleAccessService _accessService;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        #endregion

        public EntitySecurityController(IEntityRoleAccessService accessService, IUserManager<GearUser> userManager)
        {
            _accessService = accessService;
            _userManager = userManager;
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
                await _accessService.SetPermissionsForRoleOnEntityAsync(model.EntityId, model.RoleId,
                    model.Permissions);
            return Json(serviceResult);
        }

        #region Api's


        /// <summary>
        /// Get entity permissions for current user
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        [HttpGet]
        [Roles(GlobalResources.Roles.USER)]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(ResultModel<IEnumerable<string>>))]
        public async Task<JsonResult> GetEntityPermissionsForCurrentUser([Required]Guid entityId)
        {
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return Json(new AccessDeniedResult<object>());
            var user = userRequest.Result;
            var permissions = (await _accessService.GetPermissionsAsync(user, entityId)).Select(x => x.ToString());
            return Json(new SuccessResultModel<IEnumerable<string>>(permissions));
        }

        #endregion
    }
}
