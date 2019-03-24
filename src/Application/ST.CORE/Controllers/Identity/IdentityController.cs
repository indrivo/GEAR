using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ST.BaseBusinessRepository;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Data;

namespace ST.CORE.Controllers.Identity
{
	[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
	[Route("api/[controller]")]
	public class IdentityController : Controller
	{
		private IBaseBusinessRepository<ApplicationDbContext> Repository { get; }

		private UserManager<ApplicationUser> UserManager { get; }

		public IdentityController(UserManager<ApplicationUser> userManager,
			IBaseBusinessRepository<ApplicationDbContext> repository)
		{
			UserManager = userManager;
			Repository = repository;
		}

		/// <summary>
		///     Get user group
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet("[action]")]
		[Produces("application/json", Type = typeof(ResultModel))]
		public async Task<IActionResult> GetUserGroup(string userId)
		{
			var user = await UserManager.FindByIdAsync(userId);
			if (user == null)
				return Json(new ResultModel
				{
					IsSuccess = false,
					Result = string.Empty
				});
			var courtId = Repository.GetAll<UserGroup>(s => s.UserId == user.Id).Select(s => s.AuthGroupId)
				.FirstOrDefault();
			return Json(new ResultModel
			{
				IsSuccess = true,
				Result = courtId
			});
		}

		/// <summary>
		///     Get User role
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet("[action]")]
		[Produces("application/json", Type = typeof(ResultModel))]
		public async Task<IActionResult> GetUserRole(string userId)
		{
			var user = await UserManager.FindByIdAsync(userId);

			if (user == null)
				return Json(new ResultModel
				{
					IsSuccess = true,
					Result = string.Empty
				});
			var roles = await UserManager.GetRolesAsync(user);
			if (roles == null || roles.Count <= 0)
				return Json(new ResultModel
				{
					IsSuccess = true,
					Result = string.Empty
				});
			var result = BuildSuccess(string.Join(',', roles));
			return Json(result);
		}

		private static ResultModel BuildSuccess(object data)
		{
			return new ResultModel
			{
				IsSuccess = true,
				Result = data
			};
		}
	}
}