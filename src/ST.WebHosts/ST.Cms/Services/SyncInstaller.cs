using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ST.Cache.Abstractions;
using ST.Cms.Abstractions;
using ST.Cms.ViewModels.InstallerModels;
using ST.Core.Helpers;
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
		public async Task RegisterCommerceUser(ApplicationUser user, Tenant tenant, SyncEcommerceAccountViewModel data)
		{
			var dynamicService = IoC.Resolve<IDynamicService>();
			var userManager = IoC.Resolve<UserManager<ApplicationUser>>();
			var entitiesDbContext = IoC.Resolve<EntitiesDbContext>();
			var cacheService = IoC.Resolve<ICacheService>();
			var applicationDbContext = IoC.Resolve<ApplicationDbContext>();
			var userCreate = await userManager.CreateAsync(user, data.Password);
			var notify = IoC.Resolve<INotify<ApplicationRole>>();
			if (!userCreate.Succeeded)
			{
				//response.Errors.Add(new ErrorModel("fail", "Fail to create user!"));
				return;
			}

			await userManager.AddToRoleAsync(user, "Company Administrator");
			await applicationDbContext.Tenants.AddAsync(tenant);

			//Update super user information
			await applicationDbContext.SaveChangesAsync();

			//Create dynamic tables for configured tenant
			await dynamicService.CreateDynamicTables(tenant.Id, tenant.MachineName);

			//Register new tenant to cache
			await cacheService.Set($"_tenant_{tenant.MachineName}", new TenantSettings
			{
				AllowAccess = true,
				TenantId = tenant.Id,
				TenantName = tenant.MachineName
			});

			//Seed entity
			await entitiesDbContext.EntityTypes.AddAsync(new EntityType
			{
				Changed = DateTime.Now,
				Created = DateTime.Now,
				IsSystem = true,
				Author = "System",
				MachineName = tenant.MachineName,
				Name = tenant.MachineName,
				TenantId = tenant.Id
			});

			await entitiesDbContext.SaveChangesAsync();

			//Send welcome message to user
			await notify.SendNotificationAsync(new List<Guid> { Guid.Parse(user.Id) }, new SystemNotifications
			{
				Content = $"Welcome to ISO DMS {data.User.FullName}",
				Subject = "Info",
				NotificationTypeId = NotificationType.Info
			});

			await notify.SendNotificationToSystemAdminsAsync(new SystemNotifications
			{
				Content = $"{data.User.Email} was added into system as Company administrator",
				Subject = "Info",
				NotificationTypeId = NotificationType.Info
			});
		}
	}
}
