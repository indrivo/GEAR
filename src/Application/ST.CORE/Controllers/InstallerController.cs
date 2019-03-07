using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ST.CORE.Installation;
using ST.CORE.Models.InstallerModels;
using ST.Entities.Controls.Querry;
using ST.Entities.Utils;
using ST.Identity.Abstractions;
using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;
using ST.Identity.LDAP.Services;
using ST.Organization;
using ST.Organization.Models;
using ST.Organization.Utils;

namespace ST.CORE.Controllers
{
	public class InstallerController : Controller
	{
		/// <summary>
		/// Inject hosting env
		/// </summary>
		private readonly IHostingEnvironment _hostingEnvironment;

		/// <summary>
		/// Inject application context
		/// </summary>
		private readonly ApplicationDbContext _applicationDbContext;
		/// <summary>
		/// Inject Ldap User Manager
		/// </summary>
		private readonly LdapUserManager _ldapUserManager;

		private readonly ILocalService _localService;

		/// <summary>
		/// Inject SignIn Manager
		/// </summary>
		private readonly SignInManager<ApplicationUser> _signInManager;

		/// <summary>
		/// Inject permmision service
		/// </summary>
		private readonly IPermissionService _permissionService;

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="hostingEnvironment"></param>
		/// <param name="applicationDbContext"></param>
		/// <param name="ldapUserManager"></param>
		/// <param name="signInManager"></param>
		public InstallerController(IHostingEnvironment hostingEnvironment, ILocalService localService, IPermissionService permissionService, ApplicationDbContext applicationDbContext, LdapUserManager ldapUserManager, SignInManager<ApplicationUser> signInManager)
		{
			_hostingEnvironment = hostingEnvironment;
			_applicationDbContext = applicationDbContext;
			_ldapUserManager = ldapUserManager;
			_signInManager = signInManager;
			_localService = localService;
			_permissionService = permissionService;
		}

		/// <summary>
		/// Load setup page
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Setup()
		{
			var model = new SetupModel();
			var settings = Application.Settings(_hostingEnvironment);
			if (settings != null)
			{
				model.DataBaseType = settings.ConnectionStrings.PostgreSQL.UsePostgreSQL
					? DbProviderType.PostgreSql
					: DbProviderType.MsSqlServer;

				model.DatabaseConnectionString = settings.ConnectionStrings.PostgreSQL.UsePostgreSQL
					? settings.ConnectionStrings.PostgreSQL.ConnectionString
					: settings.ConnectionStrings.MSSQLConnection;
			}

			model.SysAdminProfile = new SetupProfileModel {
				FirstName = "admin",
				Email = "admin@admin.com",
				LastName = "admin",
				Password = "admin",
				ConfirmPassword = "admin",
				UserName = "admin"
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
			var settings = Application.Settings(_hostingEnvironment);

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
			settings.IsConfigurated = true;
			var result = JsonConvert.SerializeObject(settings);
			await System.IO.File.WriteAllTextAsync(Application.AppSettingsFilepath(_hostingEnvironment), result);
			Application.InitMigrations(new string[] { });

			await _permissionService.RefreshCache();

			var tenantExist =
				await _applicationDbContext.Tenants.AnyAsync(x => x.MachineName == tenantMachineName || x.Id == DefaultTenantSettings.TenantId);
			if (tenantExist)
			{
				ModelState.AddModelError(string.Empty, "Invalid name for organization because is used for another organization or organization was configured");
				return View(model);
			}

			var tenant = new Tenant
			{
				Id = DefaultTenantSettings.TenantId,
				Name = model.Organization.Name,
				MachineName = tenantMachineName,
				Created = DateTime.Now,
				Changed = DateTime.Now,
				SiteWeb = model.Organization.SiteWeb,
				Author = "System"
			};

			//Set user settings
			var superUser = await _applicationDbContext.Users.FirstOrDefaultAsync();
			superUser.UserName = model.SysAdminProfile.UserName;
			superUser.Email = model.SysAdminProfile.Email;
			var hasher = new PasswordHasher<ApplicationUser>();
			var hashedPassword = hasher.HashPassword(superUser, model.SysAdminProfile.Password);
			superUser.PasswordHash = hashedPassword;
			_applicationDbContext.Update(superUser);
			await _applicationDbContext.Tenants.AddAsync(tenant);
			await _applicationDbContext.SaveChangesAsync();

			_localService.SetAppName("core", model.SiteName);

			tenant.CreateDynamicTables();

			await Application.SeedDynamicDataAsync();
			//sign in user
			await _signInManager.SignInAsync(superUser, true);
			return RedirectToAction("Index", "Home");
		}

		/// <summary>
		/// Load welcome page
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
		{
			return View();
		}
	}
}