using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ST.Core;
using ST.Core.Helpers;
using ST.Dashboard.Abstractions.Models;
using ST.Dashboard.Abstractions.Models.ViewModels;

namespace ST.Dashboard.Abstractions
{
    public interface IDashboardService
    {
        /// <summary>
        /// Dashboards
        /// </summary>
        IQueryable<DashBoard> DashBoards { get; }

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

        /// <summary>
        /// Add new dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> CreateDashBoardAsync(DashBoard model);

        /// <summary>
        /// Update dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateDashBoardAsync(DashBoard model);

        /// <summary>
        /// Add or update dashboard settings
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<DashboardRowViewModel>>> AddOrUpdateDashboardConfigurationAsync(DashBoardConfigurationViewModel configuration);

        /// <summary>
        /// Get dashboard for render in view
        /// </summary>
        /// <returns></returns>

        Task<ResultModel<IEnumerable<Row>>> GetDashboardConfigurationForRenderAsync();
    }
}