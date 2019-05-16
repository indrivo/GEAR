using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using ST.Cache.Abstractions;
using ST.Cms.Abstractions;
using ST.Core;
using ST.Core.Helpers;
using ST.DynamicEntityStorage.Abstractions;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Controls.Querry;
using ST.Entities.Data;
using ST.Entities.Utils;
using ST.Identity.Abstractions;
using ST.Identity.CacheModels;
using ST.Identity.Data;
using ST.Identity.Data.MultiTenants;
using ST.Notifications.Abstractions;
using ST.Notifications.Abstractions.Models.Notifications;
using ST.Cms.ViewModels.InstallerModels;
using ST.Core.Abstractions;
using ST.Identity.Abstractions.Enums;
using ST.MultiTenant.Helpers;

namespace ST.Cms.Controllers
{
	public class InstallerController : Controller
	{
		/// <summary>
		/// Inject hosting env
		/// </summary>
		private readonly IHostingEnvironment _hostingEnvironment;

		/// <summary>
		/// Inject entity db context
		/// </summary>
		private readonly EntitiesDbContext _entitiesDbContext;

		/// <summary>
		/// Inject application context
		/// </summary>
		private readonly ApplicationDbContext _applicationDbContext;

		/// <summary>
		/// Inject SignIn Manager
		/// </summary>
		private readonly SignInManager<ApplicationUser> _signInManager;

		/// <summary>
		/// Inject permission dataService
		/// </summary>
		private readonly IPermissionService _permissionService;

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
		/// Is system configured
		/// </summary>
		private readonly bool _isConfigured;

		/// <summary>
		/// Queue for run background tasks
		/// </summary>
		private IBackgroundTaskQueue Queue { get; }

		private readonly IServiceScopeFactory _serviceScopeFactory;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="hostingEnvironment"></param>
		/// <param name="localService"></param>
		/// <param name="permissionService"></param>
		/// <param name="applicationDbContext"></param>
		/// <param name="signInManager"></param>
		/// <param name="notify"></param>
		/// <param name="cacheService"></param>
		/// <param name="entitiesDbContext"></param>
		/// <param name="dynamicService"></param>
		public InstallerController(IHostingEnvironment hostingEnvironment, IPermissionService permissionService, ApplicationDbContext applicationDbContext, SignInManager<ApplicationUser> signInManager, INotify<ApplicationRole> notify, ICacheService cacheService, EntitiesDbContext entitiesDbContext, IDynamicService dynamicService, IBackgroundTaskQueue queue, IServiceScopeFactory serviceScopeFactory)
		{
			_entitiesDbContext = entitiesDbContext;
			_dynamicService = dynamicService;
			Queue = queue;
			_serviceScopeFactory = serviceScopeFactory;
			_hostingEnvironment = hostingEnvironment;
			_applicationDbContext = applicationDbContext;
			_signInManager = signInManager;
			_permissionService = permissionService;
			_cacheService = cacheService;
			_notify = notify;
			_isConfigured = Installation.Application.IsConfigured(_hostingEnvironment);
		}

		/// <summary>
		/// Load setup page
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Setup()
		{
			if (_isConfigured) return RedirectToAction("Index", "Home");
			var model = new SetupModel();
			var settings = Installation.Application.Settings(_hostingEnvironment);
			if (settings != null)
			{
				model.DataBaseType = settings.ConnectionStrings.PostgreSQL.UsePostgreSQL
					? DbProviderType.PostgreSql
					: DbProviderType.MsSqlServer;

				model.DatabaseConnectionString = settings.ConnectionStrings.PostgreSQL.UsePostgreSQL
					? settings.ConnectionStrings.PostgreSQL.ConnectionString
					: settings.ConnectionStrings.MSSQLConnection;
			}

			model.SysAdminProfile = new SetupProfileModel
			{
				FirstName = "admin",
				Email = "admin@admin.com",
				LastName = "admin",
				Password = "admin",
				ConfirmPassword = "admin",
				UserName = "admin"
			};

			model.Organization = new SetupOrganizationViewModel
			{
				Name = "Indrivo"
			};

			return View(model);
		}

		/// <summary>
		/// Complete installation of system
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Setup(SetupModel model)
		{
			var settings = Installation.Application.Settings(_hostingEnvironment);

			if (model.DataBaseType == DbProviderType.MsSqlServer)
			{
				var (isConnected, error) = TableQuerryBuilder.IsSqlServerConnected(model.DatabaseConnectionString);
				if (!isConnected)
				{
					ModelState.AddModelError(string.Empty, error);
					return View(model);
				}

				settings.ConnectionStrings.PostgreSQL.UsePostgreSQL = false;
				settings.ConnectionStrings.MSSQLConnection = model.DatabaseConnectionString;
			}
			else
			{
				var (isConnected, error) = NpgTableQuerryBuilder.IsNpgServerConnected(model.DatabaseConnectionString);
				if (!isConnected)
				{
					ModelState.AddModelError(string.Empty, error);
					return View(model);
				}
				settings.ConnectionStrings.PostgreSQL.UsePostgreSQL = true;
				settings.ConnectionStrings.PostgreSQL.ConnectionString = model.DatabaseConnectionString;
			}

			var tenantMachineName = TenantUtils.GetTenantMachineName(model.Organization.Name);
			if (string.IsNullOrEmpty(tenantMachineName))
			{
				ModelState.AddModelError(string.Empty, "Invalid name for organization");
				return View(model);
			}
			settings.IsConfigured = true;
			var result = JsonConvert.SerializeObject(settings, Formatting.Indented);
			await System.IO.File.WriteAllTextAsync(ResourceProvider.AppSettingsFilepath(_hostingEnvironment), result);
			Installation.Application.InitMigrations(new string[] { });

			await _permissionService.RefreshCache();

			var tenantExist =
				await _applicationDbContext.Tenants.AnyAsync(x => x.MachineName == tenantMachineName || x.Id == Settings.TenantId);
			if (tenantExist)
			{
				ModelState.AddModelError(string.Empty, "Invalid name for organization because is used for another organization or organization was configured");
				return View(model);
			}

			var tenant = new Tenant
			{
				Id = Settings.TenantId,
				Name = model.Organization.Name,
				MachineName = tenantMachineName,
				Created = DateTime.Now,
				Changed = DateTime.Now,
				SiteWeb = model.Organization.SiteWeb,
				Author = "System"
			};

			//Register new tenant to cache
			await _cacheService.Set($"_tenant_{tenant.MachineName}", new TenantSettings
			{
				AllowAccess = true,
				TenantId = tenant.Id,
				TenantName = tenant.MachineName
			});

			//Set user settings
			var superUser = await _applicationDbContext.Users.FirstOrDefaultAsync();
			if (superUser != null)
			{
				superUser.UserName = model.SysAdminProfile.UserName;
				superUser.Email = model.SysAdminProfile.Email;
				var hasher = new PasswordHasher<ApplicationUser>();
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

			//Create system tables
			await Installation.Application.SyncDefaultEntityFrameWorkEntities(tenant.Id);

			//Create dynamic tables for configured tenant
			await _dynamicService.CreateDynamicTables(tenant.Id, tenantMachineName);

			await Installation.Application.SeedDynamicDataAsync();

			//Register in memory types
			await _dynamicService.RegisterInMemoryDynamicTypes();

			//Send welcome message to user
			await _notify.SendNotificationAsync(new List<Guid> { Guid.Parse(superUser?.Id) }, new SystemNotifications
			{
				Content = $"Welcome to Gear Bpm {model.SysAdminProfile.FirstName} {model.SysAdminProfile.LastName}",
				Subject = "Info",
				NotificationTypeId = NotificationType.Info
			});

			//sign in user
			await _signInManager.SignInAsync(superUser, true);
			return RedirectToAction("Index", "Home");
		}


		/// <summary>
		/// Sync commerce data 
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<JsonResult> SyncCommerceAccountAsync([FromBody]SyncEcommerceAccountViewModel model)
		{
			var response = new ResultModel<CommerceSyncResultViewModel>
			{
				Errors = new List<IErrorModel>()
			};

			if (!ModelState.IsValid)
			{
				response.Errors.Add(new ErrorModel("validate", "Model is not valid!"));
				return Json(response);
			}

			var tenantMachineName = TenantUtils.GetTenantMachineName(model.CompanyName);
			if (string.IsNullOrEmpty(tenantMachineName))
			{
				response.Errors.Add(new ErrorModel(string.Empty, "Invalid name for organization"));
				return Json(response);
			}

			var tenantExist =
				await _applicationDbContext.Tenants.AnyAsync(x => x.MachineName == tenantMachineName);
			if (tenantExist)
			{
				response.Errors.Add(new ErrorModel(string.Empty,
					"Invalid name for organization because is used for another organization or organization was configured"));
				return Json(response);
			}

			var tenant = new Tenant
			{
				Name = model.CompanyName,
				MachineName = tenantMachineName,
				Created = DateTime.Now,
				Changed = DateTime.Now,
				Author = "System"
			};

			var userExist = await _applicationDbContext.Users.FirstOrDefaultAsync(x => string.Equals(x.Email, model.User.Email, StringComparison.CurrentCultureIgnoreCase));
			if (userExist != null)
			{
				response.Errors.Add(new ErrorModel(string.Empty,
					"Invalid user!"));
				return Json(response);
			}

			var user = new ApplicationUser
			{
				UserName = model.User.UserName,
				Email = model.User.Email,
				AuthenticationType = AuthenticationType.Local,
				TenantId = tenant.Id,
				Created = DateTime.Now,
				Changed = DateTime.Now,
				Author = "System",
				ModifiedBy = "System"
			};

			Queue.PushQueueBackgroundWorkItem(async token =>
			{
				using (var scope = _serviceScopeFactory.CreateScope())
				{
					var scopedServices = scope.ServiceProvider;
					var service = scopedServices.GetRequiredService<ISyncInstaller>();
					var localUser = user;
					var localTenant = tenant;
					var localData = model;
					await service.RegisterCommerceUser(localUser, localTenant, localData);
				}
			});

			response.IsSuccess = true;
			response.Result = new CommerceSyncResultViewModel
			{
				TenantId = tenant.Id,
				TenantMachineName = tenantMachineName
			};
			return Json(response);
		}

		/// <summary>
		/// Load welcome page
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
		{
			if (_isConfigured) return RedirectToAction("Index", "Home");
			return View();
		}
	}
}