using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ST.CORE.Models.ApiClientViewModels;

namespace ST.CORE.Controllers.Identity
{
	[Authorize]
	public class ApiClientsController : Controller
	{
		private ConfigurationDbContext ConfigurationDbContext { get; }

		private ILogger<ApiClientsController> Logger { get; }

		public ApiClientsController(ConfigurationDbContext configurationDbContext, ILoggerFactory loggerFactory)
		{
			Logger = loggerFactory.CreateLogger<ApiClientsController>();
			ConfigurationDbContext = configurationDbContext;
		}

		public IActionResult Create()
		{
			var ls = new List<SelectListItem>();

			ls.AddRange(ConfigurationDbContext.ApiResources.Select(f => new SelectListItem
			{
				Text = f.Name,
				Value = f.Name
			}));

			ls.AddRange(ConfigurationDbContext.IdentityResources.Select(f => new SelectListItem
			{
				Text = f.Name,
				Value = f.Name
			}));
			return View("_Layout");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Create(ApiClientCreateViewModel client)
		{
			if (!ModelState.IsValid) return View("_Layout");

			try
			{
				var cl = new Client
				{
					ClientName = client.ClientName,
					ClientId = client.ClientId,
					AllowedGrantTypes = GetGrantTypesFromViewModel(client)
				};
				ConfigurationDbContext.Clients.Add(cl.ToEntity());
				await ConfigurationDbContext.SaveChangesAsync();
				return RedirectToAction(nameof(Index));
			}
			catch (DbUpdateException ex)
			{
				if (ex.InnerException is SqlException exception)
				{
					switch (exception.Number)
					{
						case 2601:
							ModelState.AddModelError(string.Empty, "The API Client Already Exists");
							Logger.LogError(exception, "The API Client already exists");
							break;

						default:
							ModelState.AddModelError(string.Empty, "An unknown error occured");
							Logger.LogError(exception, "Unknown sql error");
							break;
					}
				}
				else
				{
					Logger.LogError(ex, "A db error occured");
					ModelState.AddModelError(string.Empty, ex.Message);
				}
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
			}

			return View(client);
		}

		// GET: ApiClients
		public ActionResult Index()
		{
			var apiClients = ConfigurationDbContext.Clients
				.Include(c => c.AllowedScopes)
				.Include(c => c.AllowedGrantTypes)
				.Select(c => c);
			return View(apiClients);
		}

		/// <summary>
		/// Get grand types from view model
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		private static ICollection<string> GetGrantTypesFromViewModel(ApiClientCreateViewModel model)
		{
			var result = new Collection<string>();
			if (model.AuthorizationCodeGrantType)
				result.Add(GrantType.AuthorizationCode);
			if (model.ImplicitGrantType)
				result.Add(GrantType.Implicit);
			if (model.ClientCredentialsGrantType)
				result.Add(GrantType.ClientCredentials);
			if (model.HybridGrantType)
				result.Add(GrantType.Hybrid);
			if (model.ResourceOwnerPasswordGrantType)
				result.Add(GrantType.ResourceOwnerPassword);
			return result;
		}
	}
}