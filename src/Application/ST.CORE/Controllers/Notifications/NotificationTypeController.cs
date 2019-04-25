using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ST.DynamicEntityStorage.Abstractions;
using ST.Notifications.Abstractions.Models.Notifications;
using ST.Shared;

namespace ST.CORE.Controllers.Notifications
{
	[Authorize]
	public class NotificationTypeController : Controller
	{
		#region Inject
		/// <summary>
		/// Inject data dataService
		/// </summary>
		private readonly IDynamicService _service;

		public NotificationTypeController(IDynamicService service)
		{
			_service = service;
		}

		#endregion
		/// <summary>
		/// List of notification types
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
		{
			return View();
		}

		/// <summary>
		/// Get application roles list
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<JsonResult> NotificationTypesList(DTParameters param)
		{
			var filtered = await GetNotificationTypesFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length);

			var finalResult = new DTResult<NotificationTypes>
			{
				Draw = param.Draw,
				Data = filtered.Item1,
				RecordsFiltered = filtered.Item2,
				RecordsTotal = filtered.Item1.Count()
			};

			return Json(finalResult);
		}
		/// <summary>
		/// Get application role filtered
		/// </summary>
		/// <param name="search"></param>
		/// <param name="sortOrder"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <returns></returns>
		private async Task<(List<NotificationTypes>, int)> GetNotificationTypesFiltered(string search, string sortOrder, int start, int length)
		{
			var result = (await _service.GetAllWithInclude<NotificationTypes, NotificationTypes>()).Result.Where(p =>
				search == null || p.Name != null &&
				p.Name.ToLower().Contains(search.ToLower()) || p.Author != null &&
				p.Author.ToLower().Contains(search.ToLower()) || p.ModifiedBy != null &&
				p.ModifiedBy.ToString().ToLower().Contains(search.ToLower()) || p.Created.ToString(CultureInfo.InvariantCulture).ToLower().Contains(search.ToLower())).ToList();
			var totalCount = result.Count();

			result = result.Skip(start).Take(length).ToList();
			switch (sortOrder)
			{
				case "name":
					result = result.OrderBy(a => a.Name).ToList();
					break;
				case "created":
					result = result.OrderBy(a => a.Created).ToList();
					break;
				case "author":
					result = result.OrderBy(a => a.Author).ToList();
					break;
				case "modifiedBy":
					result = result.OrderBy(a => a.ModifiedBy).ToList();
					break;
				case "changed":
					result = result.OrderBy(a => a.Changed).ToList();
					break;
				case "isDeleted":
					result = result.OrderBy(a => a.IsDeleted).ToList();
					break;
				case "name DESC":
					result = result.OrderByDescending(a => a.Name).ToList();
					break;
				case "created DESC":
					result = result.OrderByDescending(a => a.Created).ToList();
					break;
				case "author DESC":
					result = result.OrderByDescending(a => a.Author).ToList();
					break;
				case "modifiedBy DESC":
					result = result.OrderByDescending(a => a.ModifiedBy).ToList();
					break;
				case "changed DESC":
					result = result.OrderByDescending(a => a.Changed).ToList();
					break;
				case "isDeleted DESC":
					result = result.OrderByDescending(a => a.IsDeleted).ToList();
					break;
				default:
					result = result.AsQueryable().ToList();
					break;
			}

			return (result.ToList(), totalCount);
		}
	}
}