using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Dashboard.Abstractions;
using ST.Dashboard.Abstractions.Models;
using ST.DynamicEntityStorage.Abstractions.Extensions;

namespace ST.Dashboard
{
    public class DashboardManager : IDashboardManager
    {
        #region Injectable
        private readonly IDashboardDbContext _context;
        #endregion

        public DashboardManager(IDashboardDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get dashboards
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual JsonResult GetDashboards(DTParameters param)
        {
            var filtered = _context.FilterAbstractContext<DashBoard>(param.Search.Value,
                param.SortOrder, param.Start,
                param.Length,
                out var totalCount).ToList();

            var result = new DTResult<DashBoard>
            {
                Draw = param.Draw,
                Data = filtered,
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return new JsonResult(result);
        }

        /// <summary>
        /// Set active dashboard
        /// </summary>
        /// <param name="dashboardId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SetActiveDashBoardAsync(Guid? dashboardId)
        {
            var result = new ResultModel();
            if (!dashboardId.HasValue)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                return result;
            }
            var dashboard = await _context.Dashboards.FirstOrDefaultAsync(x => x.Id.Equals(dashboardId));
            if (dashboard == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, "Dashboard not found"));
                return result;
            }

            if (dashboard.IsActive)
            {
                result.IsSuccess = true;
                return result;
            }

            dashboard.IsActive = true;

            var others = await _context.Dashboards.Where(x => x.Id != dashboardId).ToListAsync();
            _context.Update(dashboard);
            foreach (var o in others)
            {
                o.IsActive = false;
                _context.Update(o);
            }

            return await _context.PushAsync();
        }
    }
}
