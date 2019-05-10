using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ST.Cache.Abstractions;
using ST.Cms.Abstractions;
using ST.Cms.ViewModels.InstallerModels;
using ST.DynamicEntityStorage.Abstractions;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Data;
using ST.Identity.Abstractions;
using ST.Identity.CacheModels;
using ST.Identity.Data;
using ST.Identity.Data.MultiTenants;
using ST.Notifications.Abstractions;
using ST.Notifications.Abstractions.Models.Notifications;

namespace ST.Cms.Services
{
	public class SyncInstaller : ISyncInstaller
	{
		/// <summary>
		/// Inject entity db context
		/// </summary>
		private readonly EntitiesDbContext _entitiesDbContext;
		/// <summary>
		/// Inject cache dataService
		/// </summary>
		private readonly ICacheService _cacheService;
		/// <summary>
		/// Inject notifier
		/// </summary>
		private readonly INotify<ApplicationRole> _notify;

		/// <summary>
		/// Inject dynamic service
		/// </summary>
		private readonly IDynamicService _dynamicService;

		/// <summary>
		/// Inject application context
		/// </summary>
		private readonly ApplicationDbContext _applicationDbContext;

		/// <summary>
		/// Inject SignIn Manager
		/// </summary>
		private readonly UserManager<ApplicationUser> _userManager;

		public SyncInstaller(ICacheService cacheService, INotify<ApplicationRole> notify, IDynamicService dynamicService, UserManager<ApplicationUser> userManager, ApplicationDbContext applicationDbContext, EntitiesDbContext entitiesDbContext)
		{
			_cacheService = cacheService;
			_notify = notify;
			_dynamicService = dynamicService;
			_userManager = userManager;
			_applicationDbContext = applicationDbContext;
			_entitiesDbContext = entitiesDbContext;
		}

		public async Task RegisterCommerceUser(ApplicationUser user, Tenant tenant, SyncEcommerceAccountViewModel data)
		{
			var userCreate = await _userManager.CreateAsync(user, data.Password);
			if (!userCreate.Succeeded)
			{
				//response.Errors.Add(new ErrorModel("fail", "Fail to create user!"));
				return;
			}

			await _userManager.AddToRoleAsync(user, "Company Administrator");
			await _applicationDbContext.Tenants.AddAsync(tenant);

			//Update super user information
			await _applicationDbContext.SaveChangesAsync();

			//Create dynamic tables for configured tenant
			await _dynamicService.CreateDynamicTables(tenant.Id, tenant.MachineName);

			//Register new tenant to cache
			await _cacheService.Set($"_tenant_{tenant.MachineName}", new TenantSettings
			{
				AllowAccess = true,
				TenantId = tenant.Id,
				TenantName = tenant.MachineName
			});

			//Seed entity
			await _entitiesDbContext.EntityTypes.AddAsync(new EntityType
			{
				Changed = DateTime.Now,
				Created = DateTime.Now,
				IsSystem = true,
				Author = "System",
				MachineName = tenant.MachineName,
				Name = tenant.MachineName,
				TenantId = tenant.Id
			});

			await _entitiesDbContext.SaveChangesAsync();

			//Send welcome message to user
			await _notify.SendNotificationAsync(new List<Guid> { Guid.Parse(user.Id) }, new SystemNotifications
			{
				Content = $"Welcome to ISO DMS {data.User.FullName}",
				Subject = "Info",
				NotificationTypeId = NotificationType.Info
			});

			await _notify.SendNotificationToSystemAdminsAsync(new SystemNotifications
			{
				Content = $"{data.User.Email} was added into system as Company administrator",
				Subject = "Info",
				NotificationTypeId = NotificationType.Info
			});
		}
	}
}
