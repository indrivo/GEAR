using GR.Identity.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Groups.Abstractions;
using GR.Identity.Groups.Abstractions.Helpers;
using GR.Identity.Groups.Abstractions.ViewModels;
using GR.Identity.Permissions.Abstractions.Attributes;

namespace GR.Identity.Groups.Razor.Controllers
{
    [Authorize]
    public class GroupsController : BaseGearController
    {

        #region Injectable

        /// <summary>
        /// Inject group service
        /// </summary>
        private readonly IGroupService<GearUser> _groupService;

        #endregion

        public GroupsController(IGroupService<GearUser> groupService)
        {
            _groupService = groupService;
        }

        /// <summary>
        /// Get list of groups
        /// </summary>
        /// <returns></returns>
        [AuthorizePermission(GroupPermissions.GroupRead)]
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// View for create new group
        /// </summary>
        /// <returns></returns>
        [AuthorizePermission(GroupPermissions.GroupCreate)]
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Create new group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(GroupPermissions.GroupCreate)]
        public async Task<IActionResult> Create(CreateGroupViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var addRequest = await _groupService.CreateGroupAsync(model);
            if (addRequest.IsSuccess) return Redirect(nameof(Index));

            ModelState.AppendResultModelErrors(addRequest.Errors);
            return View(model);
        }

        /// <summary>
        /// View for edit group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [AuthorizePermission(GroupPermissions.GroupUpdate)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            var groupRequest = await _groupService.GetGroupByIdAsync(id);
            if (!groupRequest.IsSuccess) return NotFound();
            var model = new UpdateGroupViewModel
            {
                Id = groupRequest.Result.Id,
                Name = groupRequest.Result.Name
            };

            return View(model);
        }

        /// <summary>
        /// Edit group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AuthorizePermission(GroupPermissions.GroupUpdate)]
        public async Task<IActionResult> Edit([Required]UpdateGroupViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var updateRequest = await _groupService.UpdateGroupAsync(model);
            if (updateRequest.IsSuccess) return RedirectToAction("Index");
            ModelState.AppendResultModelErrors(updateRequest.Errors);
            return View(model);
        }

        /// <summary>
        /// Check group name
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<JsonResult> CheckGroupName(string groupName)
        {
            return await JsonAsync(_groupService.ExistGroupWithNameAsync(groupName));
        }
    }
}