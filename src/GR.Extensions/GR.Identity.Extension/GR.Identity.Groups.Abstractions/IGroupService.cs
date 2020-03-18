using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Groups.Abstractions.Models;
using GR.Identity.Groups.Abstractions.ViewModels;

namespace GR.Identity.Groups.Abstractions
{
    public interface IGroupService<TUser> where TUser : GearUser
    {
        /// <summary>
        /// Create new group
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        Task<ResultModel> CreateGroupAsync(CreateGroupViewModel group);

        /// <summary>
        /// Update group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateGroupAsync(UpdateGroupViewModel model);

        /// <summary>
        /// Add user to group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task<ResultModel> AddUserToGroupAsync(TUser user, string groupName);

        /// <summary>
        /// Add user to group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task<ResultModel> AddUserToGroupAsync(ClaimsPrincipal user, string groupName);

        /// <summary>
        /// Remove user from group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveUserFromGroupAsync(ClaimsPrincipal user, string groupName);

        /// <summary>
        /// Remove user from group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveUserFromGroupAsync(TUser user, string groupName);

        /// <summary>
        /// Is user in group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task<bool> UserIsInGroupAsync(TUser user, string groupName);

        /// <summary>
        /// Is user in group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        Task<bool> UserIsInGroupAsync(ClaimsPrincipal user, string groupName);

        /// <summary>
        /// Get groups with paginated
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<Group>> GetGroupsPaginatedAsync(DTParameters parameters);

        /// <summary>
        /// Get group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<ResultModel<Group>> GetGroupByIdAsync(Guid? id);

        /// <summary>
        /// Exist group with name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Task<bool> ExistGroupWithNameAsync(string name);

        /// <summary>
        /// Get users for group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<TUser>>> GetUsersForGroupAsync(Guid? groupId);

        /// <summary>
        /// Remove permanently
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<ResultModel> RemoveGroupPermanentlyAsync(Guid? groupId);

        /// <summary>
        /// Disable group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<ResultModel> DisableGroupAsync(Guid? groupId);

        /// <summary>
        /// Enable group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        Task<ResultModel> EnableGroupAsync(Guid? groupId);
    }
}