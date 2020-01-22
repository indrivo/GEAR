using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.ConnectionStrings;
using GR.DynamicEntityStorage.Abstractions;
using GR.Entities.Controls.QueryAbstractions;
using GR.Entities.EntityBuilder.MsSql.Controls.Query;
using GR.Entities.EntityBuilder.Postgres.Controls.Query;
using GR.Identity.Abstractions;
using GR.Identity.Permissions.Abstractions;
using GR.Install.Abstractions;
using GR.Install.Abstractions.Models;
using GR.MultiTenant.Abstractions.Helpers;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using GR.WebApplication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace GR.Install
{
    public class GearWebInstallerService : IGearWebInstallerService
    {
        #region Injectable

        /// <summary>
        /// Inject env
        /// </summary>
        private readonly IHostingEnvironment _hostingEnvironment;

        /// <summary>
        /// Inject application context
        /// </summary>
        private readonly IIdentityContext _applicationDbContext;

        /// <summary>
        /// Inject SignIn Manager
        /// </summary>
        private readonly SignInManager<GearUser> _signInManager;

        /// <summary>
        /// Inject permission dataService
        /// </summary>
        private readonly IPermissionService _permissionService;

        /// <summary>
        /// Inject notifier
        /// </summary>
        private readonly INotify<GearRole> _notify;

        #endregion

        public GearWebInstallerService(INotify<GearRole> notify, IPermissionService permissionService, SignInManager<GearUser> signInManager, IIdentityContext applicationDbContext, IHostingEnvironment hostingEnvironment)
        {
            _notify = notify;
            _permissionService = permissionService;
            _signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Install gear settings
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> InstallAsync(SetupModel model)
        {
            var response = new ResultModel();
            var settings = GearWebApplication.Settings(_hostingEnvironment);

            TableQueryBuilder instance = null;

            switch (model.DataBaseType)
            {
                case DbProviderType.MsSqlServer:
                    instance = new MsSqlTableQueryBuilder();
                    settings.ConnectionStrings.Provider = DbProvider.SqlServer;
                    break;

                case DbProviderType.PostgreSql:
                    instance = new NpgTableQueryBuilder();
                    settings.ConnectionStrings.Provider = DbProvider.PostgreSQL;
                    break;
            }

            if (instance == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "No provider registered"));
                return response;
            }

            var (isConnected, error) = instance.IsSqlServerConnected(model.DatabaseConnectionString);
            if (!isConnected)
            {
                response.Errors.Add(new ErrorModel(string.Empty, error));
                return response;
            }

            settings.ConnectionStrings.ConnectionString = model.DatabaseConnectionString;

            var tenantMachineName = TenantUtils.GetTenantMachineName(model.Organization.Name);
            if (string.IsNullOrEmpty(tenantMachineName))
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid name for organization"));
                return response;
            }
            settings.IsConfigured = true;
            settings.SystemConfig.MachineIdentifier = $"_{tenantMachineName}_";
            var result = JsonConvert.SerializeObject(settings, Formatting.Indented);
            GearWebApplication.InitModulesMigrations();
            await System.IO.File.WriteAllTextAsync(ResourceProvider.AppSettingsFilepath(_hostingEnvironment), result);
            await _permissionService.SetOrResetPermissionsOnCacheAsync();

            var tenant =
                await _applicationDbContext.Tenants.FirstOrDefaultAsync(x => x.MachineName == tenantMachineName || x.Id == GearSettings.TenantId);
            if (tenant == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Something went wrong"));
                return response;
            }

            tenant.Name = model.Organization.Name;
            tenant.SiteWeb = model.Organization.SiteWeb;
            _applicationDbContext.Tenants.Update(tenant);

            //Set user settings
            var superUser = await _signInManager.UserManager.Users.FirstOrDefaultAsync();

            if (superUser != null)
            {
                superUser.UserName = model.SysAdminProfile.UserName;
                superUser.Email = model.SysAdminProfile.Email;
                superUser.UserFirstName = model.SysAdminProfile.FirstName;
                superUser.UserLastName = model.SysAdminProfile.LastName;

                var hasher = new PasswordHasher<GearUser>();
                var hashedPassword = hasher.HashPassword(superUser, model.SysAdminProfile.Password);
                superUser.PasswordHash = hashedPassword;
                await _signInManager.UserManager.UpdateAsync(superUser);
            }

            var contextRequest = await _applicationDbContext.PushAsync();
            if (!contextRequest.IsSuccess) return contextRequest;

            GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async x =>
            {
                var service = x.InjectService<IDynamicService>();
                //Register in memory types
                await service.RegisterInMemoryDynamicTypesAsync();
            });

            //Send welcome message to user
            await _notify.SendNotificationAsync(new List<Guid>
                {
                    superUser?.Id.ToGuid() ?? Guid.Empty
                },
                new Notification
                {
                    Subject = $"Welcome to Gear App {model.SysAdminProfile.FirstName} {model.SysAdminProfile.LastName}",
                    Content = "The GEAR app is an integration system with your company's business, it has the skills to develop new applications, and allows you to create from the visual environment.",
                    NotificationTypeId = NotificationType.Info
                });

            //sign in user
            await _signInManager.PasswordSignInAsync(superUser, model.SysAdminProfile.Password, true, false);
            response.IsSuccess = true;
            GearApplication.AppState.InstallOnProgress = false;
            GearApplication.AppState.Installed = true;
            GearApplication.BackgroundTaskQueue.AddToExecutePendingBackgroundWorkItems();
            return response;
        }
    }
}
