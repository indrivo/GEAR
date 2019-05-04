using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.Audit.Interfaces;
using ST.Audit.Models;
using ST.Cache.Abstractions;
using ST.Core;
using ST.Core.Attributes;
using ST.Entities.Data;
using ST.Identity.Abstractions;
using ST.Identity.Data;
using ST.MultiTenant.Helpers;
using ST.MultiTenant.Services.Abstractions;
using ST.Notifications.Abstractions;
using ST.Procesess.Data;
using ST.Cms.ViewModels.AuditViewModels;

namespace ST.Cms.Controllers.Audit
{
	public class AuditController : BaseController
	{
		/// <summary>
		/// Inject process db context
		/// </summary>
		private readonly ProcessesDbContext _processesDbContext;

		public AuditController(EntitiesDbContext context, ApplicationDbContext applicationDbContext, UserManager<ApplicationUser> userManager,
			RoleManager<ApplicationRole> roleManager, INotify<ApplicationRole> notify,
			IOrganizationService organizationService, ProcessesDbContext processesDbContext, ICacheService cacheService)
			: base(context, applicationDbContext, userManager, roleManager, notify, organizationService, cacheService)
		{
			_processesDbContext = processesDbContext;
		}

		/// <summary>
		/// Get list of audit Core
		/// </summary>
		/// <returns></returns>
		public IActionResult Index()
		{
			return View();
		}


		/// <summary>
		/// Filter list
		/// </summary>
		/// <param name="search"></param>
		/// <param name="sortOrder"></param>
		/// <param name="start"></param>
		/// <param name="length"></param>
		/// <param name="totalCount"></param>
		/// <returns></returns>
		private List<TrackAudit> GetTrackAuditFiltered(string search, string sortOrder, int start, int length,
			out int totalCount)
		{
			var data = new List<TrackAudit>();
			var coreData = ApplicationDbContext.TrackAudits.AsNoTracking().GroupBy(x => x.RecordId).Select(grp => grp.OrderByDescending(d => d.Version).First()).ToList();
			var processData = _processesDbContext.TrackAudits.AsNoTracking().GroupBy(x => x.RecordId).Select(grp => grp.OrderByDescending(d => d.Version).First()).ToList();
			var entityData = Context.TrackAudits.AsNoTracking().GroupBy(x => x.RecordId).Select(grp => grp.OrderByDescending(d => d.Version).First()).ToList();

			data.AddRange(coreData);
			data.AddRange(processData);
			data.AddRange(entityData);

			var result = data.Where(x =>
				search == null || x.Author != null && x.Author.ToLower().Contains(search.ToLower()) ||
				x.TypeFullName != null && x.TypeFullName.ToLower().Contains(search.ToLower()) ||
				x.UserName != null && x.UserName.ToLower().Contains(search.ToLower())).ToList();

			totalCount = result.Count;

			result = result.Skip(start).Take(length).ToList();

			switch (sortOrder)
			{
				case "changed":
					result = result.OrderBy(a => a.Changed).ToList();
					break;

				case "modifiedBy":
					result = result.OrderBy(a => a.ModifiedBy).ToList();
					break;

				case "trackEventType":
					result = result.OrderBy(a => a.TrackEventType).ToList();
					break;

				case "typeFullName":
					result = result.OrderBy(a => a.TypeFullName).ToList();
					break;

				case "version":
					result = result.OrderBy(a => a.Version).ToList();
					break;

				case "author DESC":
					result = result.OrderByDescending(a => a.Author).ToList();
					break;

				case "created DESC":
					result = result.OrderByDescending(a => a.Created).ToList();
					break;

				case "changed DESC":
					result = result.OrderByDescending(a => a.Changed).ToList();
					break;

				case "modifiedBy DESC":
					result = result.OrderByDescending(a => a.ModifiedBy).ToList();
					break;

				case "trackEventType DESC":
					result = result.OrderByDescending(a => a.TrackEventType).ToList();
					break;

				case "typeFullName DESC":
					result = result.OrderByDescending(a => a.TypeFullName).ToList();
					break;

				case "version DESC":
					result = result.OrderByDescending(a => a.Version).ToList();
					break;

				default:
					result = result.AsQueryable().ToList();
					break;
			}

			return result.ToList();
		}

		/// <summary>
		/// For ajax request
		/// </summary>
		/// <param name="param"></param>
		/// <returns></returns>
		[AjaxOnly]
		[HttpPost]
		public JsonResult TrackAuditList(DTParameters param)
		{
			var filtered = GetTrackAuditFiltered(param.Search.Value, param.SortOrder, param.Start, param.Length,
				out var totalCount);
			var trackAuditsList = filtered.Select(o => new TrackAuditsListViewModel
			{
				Id = o.Id,
				Author = o.Author,
				ChangedString = o.Changed.ToShortDateString(),
				CreatedString = o.Created.ToShortDateString(),
				ModifiedBy = o.ModifiedBy,
				TypeFullName = o.TypeFullName,
				EntityName = o.TypeFullName.Split(".").LastOrDefault(),
				IsDeleted = o.IsDeleted,
				TrackEventType = o.TrackEventType,
				Version = o.Version,
				EventType = o.TrackEventType.ToString(),
				DatabaseContextName = o.DatabaseContextName,
				RecordId = o.RecordId
			});

			var finalResult = new DTResult<TrackAuditsListViewModel>
			{
				Draw = param.Draw,
				Data = trackAuditsList.ToList(),
				RecordsFiltered = totalCount,
				RecordsTotal = filtered.Count
			};
			return Json(finalResult);
		}

		/// <summary>
		/// Get details audit
		/// </summary>
		/// <param name="id"></param>
		/// <param name="contextName"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> Details(Guid? id, string contextName)
		{
			if (id == null && string.IsNullOrEmpty(contextName))
			{
				return NotFound();
			}

			dynamic dbContext = null;
			if (typeof(ApplicationDbContext).FullName == contextName)
			{
				dbContext = ApplicationDbContext;
			}
			else if (typeof(EntitiesDbContext).FullName == contextName)
			{
				dbContext = Context;
			}
			else if (typeof(ProcessesDbContext).FullName == contextName)
			{
				dbContext = _processesDbContext;
			}

			TrackAudit track = await GetTrackDetails(id, dbContext);

			if (track == null) return NotFound();

			return View(track);
		}

		/// <summary>
		/// Get versions audit
		/// </summary>
		/// <param name="id"></param>
		/// <param name="contextName"></param>
		/// <returns></returns>
		[HttpGet]
		public IActionResult Versions(Guid? id, string contextName)
		{
			if (id == null && string.IsNullOrEmpty(contextName))
			{
				return NotFound();
			}
			dynamic dbContext = null;
			if (typeof(ApplicationDbContext).FullName == contextName)
			{
				dbContext = ApplicationDbContext;
			}
			else if (typeof(EntitiesDbContext).FullName == contextName)
			{
				dbContext = Context;
			}
			else if (typeof(ProcessesDbContext).FullName == contextName)
			{
				dbContext = _processesDbContext;
			}

			List<TrackAudit> listTrack = GetTrackVersions(id, dbContext);

			return View(listTrack);
		}

		/// <summary>
		/// Gets TrackAudit with a context parameter
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public async Task<TrackAudit> GetTrackDetails<T>(Guid? id, T context) where T : ITrackerDbContext
		{
			var track = await context.TrackAudits.FirstOrDefaultAsync(x => x.Id.Equals(id));
			if (track == null) return null;

			var details = await context.TrackAuditDetails.Where(x => x.TrackAuditId.Equals(id)).ToListAsync();
			track.AuditDetailses = details;

			return track;
		}

		/// <summary>
		/// Gets TrackAudit with a context parameter
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="id"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public List<TrackAudit> GetTrackVersions<T>(Guid? id, T context) where T : ITrackerDbContext
		{
			var recordId = context.TrackAudits.FirstOrDefault(x => x.Id == id)?.RecordId;

			if (recordId == Guid.Empty) return null;

			var result = context.TrackAudits.Where(x => x.RecordId.Equals(recordId)).Where(x => !x.IsDeleted).OrderByDescending(x => x.Version)
				.Include(x => x.AuditDetailses).ToList();

			return result;
		}
	}
}