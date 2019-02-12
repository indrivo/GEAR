using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using ST.BaseRepository;
using ApplicationUser = ST.Identity.Data.UserProfiles.ApplicationUser;
using AuthGroup = ST.Identity.Data.Permissions.AuthGroup;
using UserGroup = ST.Identity.Data.Permissions.UserGroup;

namespace ST.Identity.Data.Groups
{
	public class GroupRepository : Repository<ApplicationDbContext>, IGroupRepository<ApplicationDbContext, UserProfiles.ApplicationUser>
	{
		private readonly UserManager<ApplicationUser> _userManager;

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

		public GroupRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : base(context)
		{
			_emptyGroupNameResult = GroupResult.Failed(_emptyGroupError);
			_emptyUserResult = GroupResult.Failed(_emptyUserError);
			_userManager = userManager;
		}

		public GroupResult AddUserToGroup(ApplicationUser user, string groupName)
		{
			var validationResult = ValidateUserAndGroupInput(user, groupName);
			if (!validationResult.Succeeded)
				return validationResult;

			var group = GetSingle<AuthGroup>(gr => gr.Name == groupName);
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
				var alreadyExists = context.UserGroups
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
					context.UserGroups.Add(groupUserPair);
					context.SaveChanges();
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

		public GroupResult RemoveUserFromGroup(ApplicationUser user, string groupName)
		{
			throw new NotImplementedException();
		}

		public bool UserIsInGroup(ApplicationUser user, string groupName)
		{
			var group = GetSingle<AuthGroup>(ag => ag.Name == groupName);
			return context.UserGroups.Any(ug => ug.UserId == user.Id && ug.AuthGroupId == group.Id);
		}

		public bool UserIsInGroup(ClaimsPrincipal user, string groupName)
		{
			var appUser = _userManager.GetUserAsync(user).Result;
			return UserIsInGroup(appUser, groupName);
		}

		private GroupResult ValidateUserAndGroupInput(ApplicationUser user, string groupName, bool isValidatedAlready = false)
		{
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