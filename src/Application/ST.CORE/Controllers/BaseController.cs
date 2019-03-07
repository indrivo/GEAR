using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.Entities.Data;
using ST.Identity.Data;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Organization.Models;

namespace ST.CORE.Controllers
{
	[Authorize]
	public class BaseController : Controller
	{
		/// <summary>
		/// Entity DbContext
		/// </summary>
		protected readonly EntitiesDbContext Context;

		/// <summary>
		/// Application DbContext 
		/// </summary>
		protected readonly ApplicationDbContext ApplicationDbContext;

		/// <summary>
		/// Inject UserManager
		/// </summary>
		protected readonly UserManager<ApplicationUser> UserManager;

		/// <summary>
		/// Inject RoleManager
		/// </summary>
		protected readonly RoleManager<ApplicationRole> RoleManager;

		public BaseController(EntitiesDbContext context, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
		{
			Context = context;
			ApplicationDbContext = applicationDbContext;
			UserManager = userManager;
			RoleManager = roleManager;
		}


		/// <summary>
		/// Get Current User async
		/// </summary>
		/// <returns></returns>
		protected async Task<ApplicationUser> GetCurrentUserAsync()
		{
			return await UserManager.GetUserAsync(User);
		}

		/// <summary>
		/// Get current user
		/// </summary>
		/// <returns></returns>
		protected ApplicationUser GetCurrentUser()
		{
			return GetCurrentUserAsync().GetAwaiter().GetResult();
		}

		/// <summary>
		/// Get User organization
		/// </summary>
		/// <returns></returns>
		protected async Task<Tenant> GetOrganizationOfUser()
		{
			var user = await GetCurrentUserAsync();
			if (user == null) return default;
			return await ApplicationDbContext.Tenants.FirstOrDefaultAsync(x => x.Id == user.TenantId);
		}
	}
}