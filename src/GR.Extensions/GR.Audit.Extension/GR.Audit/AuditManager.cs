using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Abstractions;
using GR.Audit.Abstractions.Helpers;
using GR.Audit.Abstractions.Models;
using GR.Audit.Abstractions.ViewModels.AuditViewModels;
using GR.Core.Extensions;
using GR.Core.Helpers;

namespace GR.Audit
{
    public class AuditManager : IAuditManager
    {
        /// <summary>
        /// Get all for module
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public IEnumerable<TrackAuditsListViewModel> GetAllForModuleFiltered(string search, string sortOrder, int start,
            int length,
            out int totalCount, string moduleName)
        {
            if (moduleName.IsNullOrEmpty())
            {
                totalCount = 0;
                return new List<TrackAuditsListViewModel>();
            }

            var module = TrackerContextsInMemory.GetAll().FirstOrDefault(x => x.Key.Equals(moduleName));
            if (!module.IsNull())
                return GetAllFiltered(search, sortOrder, start, length, out totalCount,
                    new Dictionary<string, Type>
                    {
                        {
                            module.Key, module.Value
                        }
                    });
            totalCount = 0;
            return new List<TrackAuditsListViewModel>();
        }

        /// <summary>
        /// Filter list
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        public IEnumerable<TrackAuditsListViewModel> GetAllFiltered(string search, string sortOrder, int start, int length, out int totalCount, Dictionary<string, Type> targets = null)
        {
            var data = new List<TrackAuditsListViewModel>();
            var trackContexts = targets ?? TrackerContextsInMemory.GetAll();
            foreach (var item in trackContexts)
            {
                var context = (ITrackerDbContext)IoC.Resolve(item.Value);
                if (context == null) continue;
                var items = context.TrackAudits
                    .AsNoTracking()
                    .GroupBy(x => x.RecordId)
                    .Select(grp => grp.OrderByDescending(d => d.Version).First())
                    .ToList()
                    .Select(o => new TrackAuditsListViewModel
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
                        RecordId = o.RecordId,
                        ModuleName = item.Key
                    }).ToList();

                data.AddRange(items);
            }

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

            return result;
        }


        /// <summary>
        /// Gets TrackAudit with a context parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="id"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        protected virtual async Task<ResultModel<TrackAudit>> GetTrackDetailsAsync<T>(Guid? id, T context) where T : class, ITrackerDbContext
        {
            Arg.NotNull(context, nameof(GetTrackDetailsAsync));
            var result = new ResultModel<TrackAudit>();
            if (id == null) return result;
            var track = await context.TrackAudits
                .Include(x => x.AuditDetailses)
                .FirstOrDefaultAsync(x => x.Id.Equals(id));
            if (track == null) return result;
            result.IsSuccess = true;
            result.Result = track;
            return result;
        }

        /// <summary>
        /// Details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public async Task<ResultModel<TrackAudit>> GetDetailsAsync(Guid? id, string moduleName)
        {
            var result = new ResultModel<TrackAudit>();
            if (id == null || string.IsNullOrEmpty(moduleName))
            {
                return result;
            }

            var dbContext = TrackerContextsInMemory.GetContextModule(moduleName);
            if (dbContext == null) return result;
            var track = await GetTrackDetailsAsync(id, dbContext);

            return track;
        }

        /// <summary>
        /// Get entity versions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<TrackAudit>>> GetVersionsAsync(Guid? id, string moduleName)
        {
            var result = new ResultModel<IEnumerable<TrackAudit>>();
            if (id == null || string.IsNullOrEmpty(moduleName)) return result;

            var dbContext = TrackerContextsInMemory.GetContextModule(moduleName);
            if (dbContext == null) return result;
            var entry = await dbContext.TrackAudits.FirstOrDefaultAsync(x => x.Id == id);

            if (entry == null) return result;
            if (entry.RecordId.Equals(Guid.Empty)) return result;
            var data = dbContext.TrackAudits.Where(x => x.RecordId.Equals(entry.RecordId))
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Version)
                .Include(x => x.AuditDetailses).ToList();

            result.IsSuccess = true;
            result.Result = data;
            return result;
        }
    }
}
