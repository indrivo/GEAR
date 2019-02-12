using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ST.CORE.Models.ApiClientViewModels;

namespace ST.CORE.Controllers
{
	public class ApiClientsController : Controller
	{
		private readonly ConfigurationDbContext _configurationDbContext;
		private readonly ILogger<ApiClientsController> _logger;

		public ApiClientsController(ConfigurationDbContext configurationDbContext, ILoggerFactory loggerFactory)
		{
			_logger = loggerFactory.CreateLogger<ApiClientsController>();
			_configurationDbContext = configurationDbContext;
		}

		/// <summary>
		/// Get clients
		/// </summary>
		/// <returns></returns>
		public ActionResult Index()
		{
			var apiClients = _configurationDbContext.Clients
				.Include(c => c.AllowedScopes)
				.Include(c => c.AllowedGrantTypes)
				.Select(c => c);
			return View(apiClients);
		}
		/// <summary>
		/// Create client
		/// </summary>
		/// <returns></returns>
		public IActionResult Create()
		{
			var ls = new List<SelectListItem>();

			ls.AddRange(_configurationDbContext.ApiResources.Select(f => new SelectListItem
			{
				Text = f.Name,
				Value = f.Name
			}));

			ls.AddRange(_configurationDbContext.IdentityResources.Select(f => new SelectListItem
			{
				Text = f.Name,
				Value = f.Name
			}));
			ViewData["ApiScopes"] = ls;
			return View();
		}
		/// <summary>
		/// Create client
		/// </summary>
		/// <param name="client"></param>
		/// <returns></returns>
		[HttpPost, ValidateAntiForgeryToken]
		public IActionResult Create(ApiClientCreateViewModel client)
		{
			if (!ModelState.IsValid) return View();

			try
			{
				var cl = new Client
				{
					ClientName = client.ClientName,
					ClientId = client.ClientId,
					AllowedGrantTypes = GetGrantTypesFromViewModel(client)
				};
				_configurationDbContext.Clients.Add(cl.ToEntity());
				_configurationDbContext.SaveChanges();
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
							_logger.LogError(exception, "The API Client already exists");
							break;
						default:
							ModelState.AddModelError(string.Empty, "An unknown error occured");
							_logger.LogError(exception, "Unknown sql error");
							break;
					}
				}
				else
				{
					_logger.LogError(ex, "A db error occured");
					ModelState.AddModelError(string.Empty, ex.Message);
				}
			}
			catch (Exception e)
			{
				ModelState.AddModelError(string.Empty, e.Message);
			}
			return View(client);
		}
		/// <summary>
		/// GetGrantTypesFromViewModel
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
