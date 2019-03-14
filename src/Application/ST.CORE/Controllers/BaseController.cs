using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.Audit.Extensions;
using ST.Entities.Data;
using ST.Identity.Data;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Services.Abstractions;
using ST.Notifications.Abstraction;
using ST.Organization.Models;
using ST.Procesess.Data;

namespace ST.CORE.Controllers
{
	[Authorize]
	public class BaseController : Controller
	{
		/// <summary>
		/// Inject organization service
		/// </summary>
		protected readonly IOrganizationService OrganizationService;
		/// <summary>
		/// Inject notifier
		/// </summary>
		protected readonly INotify Notify;
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
		/// Inject processes db context
		/// </summary>
		protected readonly ProcessesDbContext ProcessesDbContext;

		/// <summary>
		/// Inject RoleManager
		/// </summary>
		protected readonly RoleManager<ApplicationRole> RoleManager;


		/// <summary>
		/// Tenant id
		/// </summary>
		protected Guid? CurrentUserTenantId
		{
			get => User?.Claims?.FirstOrDefault(x => x.Type == "tenant")?.Value?.ToGuid();
		}

		public BaseController(EntitiesDbContext context, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, INotify notify, IOrganizationService organizationService, ProcessesDbContext processesDbContext)
		{
			Context = context;
			ApplicationDbContext = applicationDbContext;
			UserManager = userManager;
			RoleManager = roleManager;
			Notify = notify;
			OrganizationService = organizationService;
			ProcessesDbContext = processesDbContext;
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