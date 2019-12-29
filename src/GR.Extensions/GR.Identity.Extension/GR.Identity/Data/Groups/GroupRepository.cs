using GR.Identity.Abstractions;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using GR.Core.Helpers;
using GR.Identity.Abstractions.Models.GroupModels;
using UserGroup = GR.Identity.Abstractions.UserGroup;

namespace GR.Identity.Data.Groups
{
    public class GroupRepository<TContext> : IGroupRepository<TContext, GearUser> where TContext : ApplicationDbContext
    {
        private readonly UserManager<GearUser> _userManager;
        private readonly TContext _context;

        #region Predefined GroupResults and Errros

        private readonly GroupActionError _emptyGroupError = new GroupActionError
        {
            Code = "InvalidGroupName",
            Description = "Please select a group to add the user to."
        };

        private readonly GroupResult _emptyGroupNameResult;

        private readonly GroupActionError _emptyUserError = new GroupActionError
        {
            Code = "InvalidUser",
            Description = "The supplied user is empty"
        };

        private readonly GroupResult _emptyUserResult;

        #endregion Predefined GroupResults and Errros

        public GroupRepository(UserManager<GearUser> userManager, TContext context)
        {
            _emptyGroupNameResult = GroupResult.Failed(_emptyGroupError);
            _emptyUserResult = GroupResult.Failed(_emptyUserError);
            _userManager = userManager;
            _context = context;
        }

        public GroupResult AddUserToGroup(GearUser user, string groupName)
        {
            var validationResult = ValidateUserAndGroupInput(user, groupName);
            if (!validationResult.Succeeded)
                return validationResult;

            var group = _context.AuthGroups.FirstOrDefault(x => x.Name == groupName);
            if (group == null)
            {
                return GroupResult.Failed(new GroupActionError
                {
                    Code = "NullGroup",
                    Description = $"The group {groupName} does not exist"
                });
            }
            else
            {
                var alreadyExists = _context.UserGroups
                    .Any(ug => ug.UserId == user.Id
                        && ug.AuthGroupId == group.Id);

                if (alreadyExists)
                {
                    return GroupResult.Failed(new GroupActionError
                    {
                        Code = "AlreadyExists",
                        Description = $"The user: {user.UserName} is already part of '{groupName}' group"
                    });
                }

                var groupUserPair = new UserGroup
                {
                    UserId = user.Id,
                    AuthGroupId = group.Id
                };

                try
                {
                    _context.UserGroups.Add(groupUserPair);
                    _context.SaveChanges();
                    return GroupResult.Success;
                }
                catch (Exception ex)
                {
                    return GroupResult.Failed(new GroupActionError
                    {
                        Code = "AddToGroupFail",
                        Description = $"Adding user: {user.UserName} to group {groupName} failed: {ex.Message}"
                    });
                }
            }
        }

        public GroupResult AddUserToGroup(ClaimsPrincipal user, string groupName)
        {
            try
            {
                var appUser = _userManager.GetUserAsync(user).Result;
                return AddUserToGroup(appUser, groupName);
            }
            catch (NullReferenceException)
            {
                return _emptyUserResult;
            }
        }

        public GroupResult RemoveUserFromGroup(ClaimsPrincipal user, string groupName)
        {
            throw new NotImplementedException();
        }

        public GroupResult RemoveUserFromGroup(GearUser user, string groupName)
        {
            throw new NotImplementedException();
        }

        public bool UserIsInGroup(GearUser user, string groupName)
        {
            var group = _context.AuthGroups.FirstOrDefault(x => x.Name == groupName);
            return _context.UserGroups.Any(ug => ug.UserId == user.Id && ug.AuthGroupId == group.Id);
        }

        public bool UserIsInGroup(ClaimsPrincipal user, string groupName)
        {
            var appUser = _userManager.GetUserAsync(user).Result;
            return UserIsInGroup(appUser, groupName);
        }

        private GroupResult ValidateUserAndGroupInput(GearUser user, string groupName, bool isValidatedAlready = false)
        {
            Arg.NotNull(user, nameof(ValidateUserAndGroupInput));
            if (isValidatedAlready) return GroupResult.Success;
            var userProvided = user != null || !string.IsNullOrEmpty(user.Id);
            var groupNameProvided = !string.IsNullOrEmpty(groupName);

            if (!userProvided && !groupNameProvided)
                return GroupResult.Failed(_emptyUserError, _emptyGroupError);

            if (!userProvided)
                return _emptyUserResult;

            return !groupNameProvided ? _emptyGroupNameResult : GroupResult.Success;
        }
    }
}