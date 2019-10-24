using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GR.Core;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions.Models;
using GR.Dashboard.Abstractions.Models.Permissions;
using GR.Dashboard.Abstractions.Models.ViewModels;

namespace GR.Dashboard.Abstractions
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
        /// Get active dashboard
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<DashBoard>> GetActiveDashboardAsync();

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

        /// <summary>
        /// Get dashboard configuration
        /// </summary>
        /// <param name="dashboardId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<DashboardRowViewModel>>> GetDashBoardConfigurationAsync(Guid? dashboardId);

        /// <summary>
        /// Delete row 
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteRowAsync(Guid? rowId);

        /// <summary>
        /// Seed widgets async
        /// </summary>
        /// <returns></returns>
        Task SeedWidgetsAsync();

        /// <summary>
        /// Get widgets by row group id
        /// </summary>
        /// <param name="widgetGroupId"></param>
        /// <returns></returns>
        Task<IEnumerable<Widget>> GetWidgetGroupRowsAsync(Guid? widgetGroupId);

        /// <summary>
        /// Delete ma
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        Task<ResultModel> DeleteMappedWidgetToRowAsync(Guid? rowId, Guid? widgetId);

        /// <summary>
        /// Get configuration for widget
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<RowWidgetAcl>>> GetRowWidgetAclInfoAsync(Guid? widgetId, Guid? rowId);

        /// <summary>
        /// Update acl for widget
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <param name="configuration"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateAclAsync(Guid? widgetId, Guid? rowId, IEnumerable<RowWidgetAclBase> configuration);

        /// <summary>
        /// Check access
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        Task<bool> HasAccess(Guid? rowId, Guid? widgetId);

        /// <summary>
        /// Get ui settings
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        Task<ResultModel<WidgetUISettings>> GetUISettingsForWidgetAsync(Guid? widgetId, Guid? rowId);

        /// <summary>
        /// Change ui settings for mapped widget
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <param name="uiSettings"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateUISettingsAsync(Guid? widgetId, Guid? rowId, WidgetUISettings uiSettings);
    }
}