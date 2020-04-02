using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Identity.Groups.Abstractions;
using GR.Identity.Groups.Abstractions.Models;
using GR.Identity.Groups.Abstractions.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Groups.Infrastructure
{
    [Author(Authors.LUPEI_NICOLAE, 1.1)]
    public class GroupService : IGroupService<GearUser>
    {
        #region Injectable
        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly UserManager<GearUser> _userManager;

        /// <summary>
        /// Inject group context 
        /// </summary>
        private readonly IGroupContext _context;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="context"></param>
        public GroupService(UserManager<GearUser> userManager, IGroupContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Create new group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> CreateGroupAsync(CreateGroupViewModel model)
        {
            if (model == null) return new InvalidParametersResultModel();
            var group = new Group(model.Name);
            await _context.Groups.AddAsync(group);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Update group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateGroupAsync(UpdateGroupViewModel model)
        {
            if (model == null || model.Id == Guid.Empty) return new InvalidParametersResultModel();
            var group = await _context.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(x =>
                x.Name.Equals(model.Name) && x.Id.Equals(model.Id));
            if (group == null) return new NotFoundResultModel();
            group.Name = model.Name;
            _context.Groups.Update(group);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Add user to group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddUserToGroupAsync(GearUser user, string groupName)
        {
            var response = new ResultModel();
            var group = await _context.Groups.FirstOrDefaultAsync(x => x.Name == groupName);
            if (group == null)
            {
                response.Errors.Add(new ErrorModel("", $"The group {groupName} does not exist"));
                return response;
            }

            var alreadyExists = await _context.UserGroups
                .AnyAsync(ug => ug.UserId == user.Id
                           && ug.GroupId == group.Id);

            if (alreadyExists)
            {
                response.Errors.Add(new ErrorModel("", $"The user: {user.UserName} is already part of '{groupName}' group"));
                return response;
            }

            var groupUserPair = new UserGroup
            {
                UserId = user.Id,
                GroupId = group.Id
            };

            _context.UserGroups.Add(groupUserPair);
            var dbRequest = await _context.PushAsync();
            if (dbRequest.IsSuccess)
            {
                response.IsSuccess = true;
                return response;
            }

            response.Errors.Add(new ErrorModel("AddToGroupFail", $"Adding user: {user.UserName} to group {groupName} failed: " +
                                                                 $"{dbRequest.Errors.FirstOrDefault()?.Message}"));
            return response;
        }

        /// <summary>
        /// Add user to group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> AddUserToGroupAsync(ClaimsPrincipal user, string groupName)
        {
            var appUser = await _userManager.GetUserAsync(user);
            if (appUser == null)
            {
                return new NotFoundResultModel();
            }

            return await AddUserToGroupAsync(appUser, groupName);
        }

        /// <summary>
        /// Remove user from group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveUserFromGroupAsync(ClaimsPrincipal user, string groupName)
        {
            var appUser = await _userManager.GetUserAsync(user);
            if (appUser == null) return new InvalidParametersResultModel();
            return await RemoveUserFromGroupAsync(appUser, groupName);
        }

        /// <summary>
        /// Remove user from group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveUserFromGroupAsync(GearUser user, string groupName)
        {
            if (user == null) return new InvalidParametersResultModel();
            var userGroup = await _context.UserGroups
                .AsNoTracking()
                .Include(x => x.Group)
                .FirstOrDefaultAsync(x => x.UserId.Equals(user.Id) && x.Group.Name.Equals(groupName));
            if (userGroup == null)
                return new NotFoundResultModel();
            _context.UserGroups.Remove(userGroup);
            return await _context.PushAsync();
        }

        /// <summary>
        /// Is user in group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public virtual async Task<bool> UserIsInGroupAsync(GearUser user, string groupName)
        {
            var group = await _context.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name == groupName);
            return _context.UserGroups.Any(ug => ug.UserId == user.Id && ug.GroupId == group.Id);
        }

        /// <summary>
        /// Is user in group
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public virtual async Task<bool> UserIsInGroupAsync(ClaimsPrincipal user, string groupName)
        {
            var appUser = await _userManager.GetUserAsync(user);
            return await UserIsInGroupAsync(appUser, groupName);
        }

        /// <summary>
        /// Get groups with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual async Task<DTResult<Group>> GetGroupsPaginatedAsync(DTParameters parameters)
        {
            var data = await _context.Groups.GetPagedAsDtResultAsync(parameters);
            return data;
        }

        /// <summary>
        /// Get group by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Group>> GetGroupByIdAsync(Guid? id)
        {
            if (id == null) return new InvalidParametersResultModel<Group>();
            var group = await _context.Groups
                .AsNoTracking()
                .Include(x => x.UserGroups)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (group == null) return new NotFoundResultModel<Group>();

            return new SuccessResultModel<Group>(group);
        }

        /// <summary>
        /// Check if exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual async Task<bool> ExistGroupWithNameAsync(string name)
        {
            if (name.IsNullOrEmpty()) return false;
            var group = await _context.Groups
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name.Equals(name));
            return group != null;
        }

        /// <summary>
        /// Get users for group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<GearUser>>> GetUsersForGroupAsync(Guid? groupId)
        {
            var userGroups = await _context.UserGroups
                .AsNoTracking()
                .Include(x => x.User)
                .Where(x => x.GroupId.Equals(groupId))
                .ToListAsync();

            var users = userGroups.Select(x => x.User).ToList();
            return new SuccessResultModel<IEnumerable<GearUser>>(users);
        }

        /// <summary>
        /// Remove group    
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> RemoveGroupPermanentlyAsync(Guid? groupId) => await _context.RemovePermanentRecordAsync<Group>(groupId);

        /// <summary>
        /// Disable group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DisableGroupAsync(Guid? groupId) => await _context.DisableRecordAsync<Group>(groupId);

        /// <summary>
        /// Enable group
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> EnableGroupAsync(Guid? groupId) => await _context.ActivateRecordAsync<Group>(groupId);
    }
}