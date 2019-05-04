using System;
using System.Collections.Generic;
using System.Linq;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ST.Core;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Data;
using ST.Entities.ViewModels.TableTypes;
using ST.Identity.Attributes;
using ST.Identity.Data.Permissions;

namespace ST.Cms.Controllers.Entity
{
	/// <inheritdoc />
	/// <summary>
	/// Schema manipulation
	/// </summary>
	[Authorize]
	public class EntityTypeController : Controller
	{
		private EntitiesDbContext Context { get; }

		/// <summary>
		/// Inject logger
		/// </summary>
		private readonly ILogger<EntityTypeController> _logger;

		public EntityTypeController(EntitiesDbContext context, ILogger<EntityTypeController> logger)
		{
			Context = context;
			_logger = logger;
		}

		/// <summary>
		/// List with schemas
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityRead)]
		public IActionResult Index()
		{
			return View();
		}

		private List<EntityType> GetOrderFiltered(string search, string sortOrder, int start, int length,
			out int totalCount)
		{
			var result = Context.EntityTypes.Where(p =>
				search == null || p.Name != null &&
				p.Name.ToLower().Contains(search.ToLower()) || p.Description != null &&
				p.Description.ToLower().Contains(search.ToLower()) || p.Author != null &&
				p.Author.ToString().ToLower().Contains(search.ToLower()) || p.ModifiedBy != null &&
				p.ModifiedBy.ToString().ToLower().Contains(search.ToLower())).ToList();
			totalCount = result.Count;

			result = result.Skip(start).Take(length).ToList();
			switch (sortOrder)
			{
				case "id":
					result = result.OrderBy(a => a.Id).ToList();
					break;
				case "name":
					result = result.OrderBy(a => a.Name).ToList();
					break;
				case "description":
					result = result.OrderBy(a => a.Description).ToList();
					break;
				case "created":
					result = result.OrderBy(a => a.Created).ToList();
					break;
				case "modifiedBy":
					result = result.OrderBy(a => a.ModifiedBy).ToList();
					break;
				case "isDeleted":
					result = result.OrderBy(a => a.IsDeleted).ToList();
					break;
				case "id DESC":
					result = result.OrderByDescending(a => a.Id).ToList();
					break;
				case "name DESC":
					result = result.OrderByDescending(a => a.Name).ToList();
					break;
				case "description DESC":
					result = result.OrderByDescending(a => a.Description).ToList();
					break;
				case "created DESC":
					result = result.OrderByDescending(a => a.Created).ToList();
					break;
				case "modifiedBy DESC":
					result = result.OrderByDescending(a => a.ModifiedBy).ToList();
					break;
				case "isDeleted DESC":
					result = result.OrderByDescending(a => a.IsDeleted).ToList();
					break;
				default:
					result = result.AsQueryable().ToList();
					break;
			}

			return result.ToList();
		}

		[HttpPost]
		public JsonResult OrderList(DTParameters param)
		{
			var filtered = GetOrderFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
				out var totalCount);

			//var OrderList = filtered.Select(o => new Profile()
			//{
			//	Id = o.Customer.CompanyName,
			//	ContactName = o.Customer.ContactName,
			//	OrderDate = o.OrderDate.HasValue ? o.OrderDate.Value.ToString() : "",
			//	RequiredDate = o.RequiredDate.HasValue ? o.RequiredDate.Value.ToString() : "",
			//	ShippedDate = o.ShippedDate.HasValue ? o.ShippedDate.Value.ToString() : "",
			//	Freight = o.Freight,
			//	ShipCity = o.ShipCity,
			//	ShipAddress = o.ShipAddress,
			//	ShipName = o.ShipName

			//});


			var finalresult = new DTResult<EntityType>
			{
				Draw = param.Draw,
				Data = filtered.ToList(),
				RecordsFiltered = totalCount,
				RecordsTotal = filtered.Count
			};

			return Json(finalresult);
		}


		/// <summary>
		/// Check if schema exist or not
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		[HttpGet]
		public JsonResult CheckEnitityType(string type)
		{
			if (type == null) return Json(null);
			var result = Context.EntityTypes.FirstOrDefault(x => x.Name == type);
			return Json(result != null);
		}

		/// <summary>
		/// View for create a schema
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityCreate)]
		public IActionResult Create() => View();

		/// <summary>
		/// Add new schema
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityCreate)]
		public IActionResult Create(EntityTypeCreateViewModel model)
		{
			if (!ModelState.IsValid) return View(model);
			try
			{
				Context.EntityTypes.Add(model.Adapt<EntityType>());
				Context.SaveChanges();
				return RedirectToAction(nameof(Index), "EntityType");
			}
			catch (Exception e)
			{
				ModelState.AddModelError("fail", e.Message);
			}

			return View(model);
		}

		/// <summary>
		/// Get schema by id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityUpdate)]
		public IActionResult Edit(Guid id)
		{
			var response = Context.EntityTypes.FirstOrDefault(x => x.Id == id);
			if (response != null)
			{
				return View(response.Adapt<EntityTypeUpdateViewModel>());
			}

			return RedirectToAction(nameof(Index), "EntityType");
		}

		/// <summary>
		/// Update schema model
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		[HttpPost]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityUpdate)]
		public IActionResult Edit(EntityTypeUpdateViewModel model)
		{
			if (!ModelState.IsValid) return View(model);

			try
			{
				Context.EntityTypes.Update(model.Adapt<EntityType>());
				Context.SaveChanges();
				return RedirectToAction(nameof(Index), "EntityType");
			}
			catch (Exception e)
			{
				ModelState.AddModelError("fail", e.Message);
			}

			return View(model);
		}


		/// <summary>
		/// Delete role
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
		[AuthorizePermission(PermissionsConstants.CorePermissions.BpmEntityDelete)]
		public JsonResult Delete(Guid? id)
		{
			if (!id.HasValue)
			{
				return Json(new { success = false, message = "Id not found" });
			}

			var typeName = Context.EntityTypes.AsNoTracking().SingleOrDefault(x => x.Id == id);
			if (typeName == null)
			{
				return Json(new { success = false, message = "EntityTypes not found" });
			}

			var isUsed = Context.Table.Any(x => x.EntityType == typeName.Name);
			if (isUsed)
			{
				return Json(new { success = false, message = "EntityTypes is used" });
			}

			try
			{
				Context.EntityTypes.Remove(typeName);
				Context.SaveChanges();
				return Json(new { success = true, message = "EntityType deleted !" });
			}
			catch (Exception e)
			{
				_logger.LogError(e.Message);
				return Json(new { success = false, message = "Error on save in DB" });
			}
		}
	}
}