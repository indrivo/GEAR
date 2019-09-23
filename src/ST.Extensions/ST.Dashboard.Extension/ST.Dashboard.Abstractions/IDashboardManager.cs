using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ST.Core;
using ST.Core.Helpers;

namespace ST.Dashboard.Abstractions
{
    public interface IDashboardManager
    {
        /// <summary>
        /// Get dashboards
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        JsonResult GetDashboards(DTParameters param);

        /// <summary>
        /// Set active dashboard
        /// </summary>
        /// <param name="dashboardId"></param>
        /// <returns></returns>
        Task<ResultModel> SetActiveDashBoardAsync(Guid? dashboardId);
    }
}