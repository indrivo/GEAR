using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Helpers;
using GR.Core.Helpers.ConnectionStrings;
using GR.DynamicEntityStorage.Abstractions;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Controls.QueryAbstractions;
using GR.Entities.EntityBuilder.MsSql.Controls.Query;
using GR.Entities.EntityBuilder.Postgres.Controls.Query;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.Identity.Data;
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
        /// Inject entity db context
        /// </summary>
        private readonly IEntityContext _entitiesDbContext;

        /// <summary>
        /// Inject application context
        /// </summary>
        private readonly ApplicationDbContext _applicationDbContext;

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

        /// <summary>
        /// Inject dynamic service
        /// </summary>
        private readonly IDynamicService _dynamicService;

        /// <summary>
        /// Inject entity repository
        /// </summary>
        private readonly IEntityRepository _entityRepository;

        #endregion

        public GearWebInstallerService(IEntityContext entitiesDbContext, IDynamicService dynamicService, IEntityRepository entityRepository, INotify<GearRole> notify, IPermissionService permissionService, SignInManager<GearUser> signInManager, ApplicationDbContext applicationDbContext, IHostingEnvironment hostingEnvironment)
        {
            _entitiesDbContext = entitiesDbContext;
            _dynamicService = dynamicService;
            _entityRepository = entityRepository;
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
            GearWebApplication.InitMigrations();
            await System.IO.File.WriteAllTextAsync(ResourceProvider.AppSettingsFilepath(_hostingEnvironment), result);
            await _permissionService.RefreshCache();

            var tenantExist =
                await _applicationDbContext.Tenants.AnyAsync(x => x.MachineName == tenantMachineName || x.Id == GearSettings.TenantId);
            if (tenantExist)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid name for organization because is used for another organization or organization was configured"));
                return response;
            }

            var tenant = new Tenant
            {
                Id = GearSettings.TenantId,
                Name = model.Organization.Name,
                MachineName = GearSettings.DEFAULT_ENTITY_SCHEMA,
                Created = DateTime.Now,
                Changed = DateTime.Now,
                SiteWeb = model.Organization.SiteWeb,
                Author = GlobalResources.Roles.ANONIMOUS_USER
            };

            //Set user settings
            var superUser = await _applicationDbContext.Users.FirstOrDefaultAsync();
            if (superUser != null)
            {
                superUser.UserName = model.SysAdminProfile.UserName;
                superUser.Email = model.SysAdminProfile.Email;
                var hasher = new PasswordHasher<GearUser>();
                var hashedPassword = hasher.HashPassword(superUser, model.SysAdminProfile.Password);
                superUser.PasswordHash = hashedPassword;
                await _signInManager.UserManager.UpdateAsync(superUser);
            }
            await _applicationDbContext.Tenants.AddAsync(tenant);

            //Update super user information
            await _applicationDbContext.SaveChangesAsync();

            //Seed entity
            await _entitiesDbContext.EntityTypes.AddAsync(new EntityType
            {
                Changed = DateTime.Now,
                Created = DateTime.Now,
                IsSystem = true,
                Author = superUser?.Id,
                MachineName = tenant.MachineName,
                Name = tenant.MachineName,
                TenantId = tenant.Id
            });

            await _entitiesDbContext.SaveChangesAsync();

            //Create dynamic tables for configured tenant
            await _entityRepository.CreateDynamicTablesFromInitialConfigurationsFile(tenant.Id, GearSettings.DEFAULT_ENTITY_SCHEMA);

            //Register in memory types
            await _dynamicService.RegisterInMemoryDynamicTypesAsync();

            //Send welcome message to user
            await _notify.SendNotificationAsync(new List<Guid>
                {
                    Guid.Parse(superUser?.Id ?? string.Empty)
                },
                new Notification
                {
                    Content = $"Welcome to Gear {model.SysAdminProfile.FirstName} {model.SysAdminProfile.LastName}",
                    Subject = "Info",
                    NotificationTypeId = NotificationType.Info
                });

            //sign in user
            await _signInManager.SignInAsync(superUser, true);

            return response;
        }
    }
}
