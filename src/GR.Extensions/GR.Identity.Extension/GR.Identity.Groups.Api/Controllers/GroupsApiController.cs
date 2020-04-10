using System;
using GR.Core;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions;
using GR.Identity.Groups.Abstractions;
using GR.Identity.Groups.Abstractions.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Identity.Groups.Abstractions.Helpers;
using GR.Identity.Groups.Abstractions.ViewModels;
using GR.Identity.Permissions.Abstractions.Attributes;

namespace GR.Identity.Groups.Api.Controllers
{
    [GearAuthorize(GearAuthenticationScheme.Bearer | GearAuthenticationScheme.Identity)]
    [Route("api/groups/[action]")]
    public class GroupsApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject user group service
        /// </summary>
        private readonly IGroupService<GearUser> _groupService;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="groupService"></param>
        public GroupsApiController(IGroupService<GearUser> groupService)
        {
            _groupService = groupService;
        }

        /// <summary>
        /// Create group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Produces(ContentType.ApplicationJson, Type = typeof(ResultModel))]
        [AuthorizePermission(GroupPermissions.GroupCreate)]
        public async Task<JsonResult> CreateGroup([Required] CreateGroupViewModel model)
        {
            if (!ModelState.IsValid) return JsonModelStateErrors();
            return await JsonAsync(_groupService.CreateGroupAsync(model));
        }

        /// <summary>
        /// Get group list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces(ContentType.ApplicationJson, Type = typeof(DTResult<Group>))]
        [AuthorizePermission(GroupPermissions.GroupRead)]
        public async Task<JsonResult> GetGroupList([Required]DTParameters param)
            => await JsonAsync(_groupService.GetGroupsPaginatedAsync(param));


        /// <summary>
        /// Remove group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(GroupPermissions.GroupDelete)]
        public async Task<JsonResult> RemoveGroupPermanently(Guid? id)
            => await JsonAsync(_groupService.RemoveGroupPermanentlyAsync(id));
    }
}
