using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GR.Core;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions.Models;
using GR.Dashboard.Abstractions.Models.ViewModels;
using GR.Dashboard.Abstractions.Models.WidgetTypes;

namespace GR.Dashboard.Abstractions
{
    public interface IWidgetService
    {
        /// <summary>
        /// Widget groups
        /// </summary>
        IQueryable<WidgetGroup> Groups { get; }

        /// <summary>
        /// Widgets
        /// </summary>
        IQueryable<CustomWidget> Widgets { get; }

        /// <summary>
        /// Create new widget
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> CreateWidgetAsync(WidgetViewModel model);

        /// <summary>
        /// Update widget
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateWidgetAsync(WidgetViewModel model);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        JsonResult GetWidgetsInJqueryTableFormat(DTParameters param);

        /// <summary>
        /// Create new report widget
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        Task<ResultModel> CreateNewReportWidgetAsync(Guid? reportId);
    }
}