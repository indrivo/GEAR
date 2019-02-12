using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ST.CORE.Extensions.Installer;
using ST.CORE.Models.InstallerModels;

namespace ST.CORE.Controllers
{
	public class InstallerController : Controller
	{
		private const string Filepath = "appsettings.json";
		private const string FilepathInitial = "InitialSetting.json";

		public async Task<IActionResult> Setup()
		{
			var model = new DbSetupModel();
			using (var r = new StreamReader(FilepathInitial))
			{
				var json = await r.ReadToEndAsync();
				var jobject = JsonConvert.DeserializeObject<AppSettingsModel.RootObject>(json);
				model.Languages = jobject.LocalizationConfig.Languages.Adapt<List<LanguageSetup>>();
				model.Languages.Single(x => x.Identifier == "en").Selected = true;
				// Temporary default configuration
				model.DbAdrress = "192.168.1.209";
				model.DbName = "BTMS";
				model.UserName = "sa";
				model.UserPassword = "Soft-Tehnica2017";
			}

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Setup(DbSetupModel model)
		{
			var settings = new AppSettingsModel
			{
				RootObjects = new AppSettingsModel.RootObject
				{
					ConnectionStrings = new AppSettingsModel.ConnectionStrings(),
					Logging = new AppSettingsModel.Logging
					{
						LogLevel = new AppSettingsModel.LogLevel()
					},
					HealthCheck = new AppSettingsModel.HealthCheck(),
					LocalizationConfig = new AppSettingsModel.LocalizationConfig(),
					IsConfigurated = true
				}
			};
			var connectionString = new StringBuilder();
			connectionString.AppendFormat(
				"Server={0};Database={1}.db;Trusted_Connection=False;User Id={2};Password={3};MultipleActiveResultSets=true",
				model.DbAdrress, model.DbName, model.UserName, model.UserPassword);
			settings.RootObjects.ConnectionStrings.DefaultConnection = connectionString.ToString();
			settings.RootObjects.Logging.IncludeScopes = false;
			settings.RootObjects.Logging.LogLevel.Default = "Warning";
			settings.RootObjects.HealthCheck.Path = "/health";
			settings.RootObjects.HealthCheck.Timeout = 3;
			settings.RootObjects.LocalizationConfig.DefaultLanguage = "en";
			settings.RootObjects.LocalizationConfig.Path = "Localization";
			settings.RootObjects.LocalizationConfig.SessionStoreKeyName = "lang";
			settings.RootObjects.LocalizationConfig.Languages = new List<AppSettingsModel.Language>();

			foreach (var item in model.Languages)
			{
				if (item.Selected)
				{
					settings.RootObjects.LocalizationConfig.Languages.Add(new AppSettingsModel.Language
					{
						Name = item.Name,
						Identifier = item.Identifier
					});
				}
			}

			string result;
			using (new StreamReader(Filepath))
			{
				result = JsonConvert.SerializeObject(settings.RootObjects);
			}

			await System.IO.File.WriteAllTextAsync(Filepath, result);
			ProgramExtension.AddMigrations();
			ProgramExtension.CreateDynamicTables();
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Index()
		{
			return View();
		}
	}
}