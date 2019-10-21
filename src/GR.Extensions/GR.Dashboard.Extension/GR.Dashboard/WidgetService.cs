using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Constants;
using GR.Dashboard.Abstractions.Helpers.Enums;
using GR.Dashboard.Abstractions.Models;
using GR.Dashboard.Abstractions.Models.ViewModels;
using GR.Dashboard.Abstractions.Models.WidgetTypes;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Report.Abstractions;

namespace GR.Dashboard
{
    public class WidgetService : IWidgetService
    {
        #region Injectable

        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly IDashboardDbContext _context;

        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly IReportContext _reportContext;

        #endregion

        /// <summary>
        /// Context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="reportContext"></param>
        public WidgetService(IDashboardDbContext context, IReportContext reportContext)
        {
            _context = context;
            _reportContext = reportContext;
        }

        /// <inheritdoc />
        /// <summary>
        /// Custom widgets
        /// </summary>
        public IQueryable<CustomWidget> Widgets => _context.CustomWidgets;

        /// <inheritdoc />
        /// <summary>
        /// Get widget groups
        /// </summary>
        public IQueryable<WidgetGroup> Groups => _context.WidgetGroups.Where(x => (!x.IsSystem || x.Id.Equals(WidgetType.CUSTOM)) && !x.IsDeleted);

        /// <inheritdoc />
        /// <summary>
        /// Create widget
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> CreateWidgetAsync(WidgetViewModel model)
        {
            var result = new ResultModel();
            if (model == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(ArgumentNullException)));
                return result;
            }

            await _context.CustomWidgets.AddAsync(model);
            var dbResult = await _context.PushAsync();
            if (!dbResult.IsSuccess) return dbResult;
            result.IsSuccess = true;
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Update widget
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateWidgetAsync(WidgetViewModel model)
        {
            var result = new ResultModel();
            if (model == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(ArgumentNullException)));
                return result;
            }
            _context.CustomWidgets.Update(model);
            return await _context.PushAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Get custom widgets in jquery table format
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual JsonResult GetWidgetsInJqueryTableFormat(DTParameters param)
        {
            var filtered = _context.FilterAbstractContext<CustomWidget>(param.Search.Value,
                param.SortOrder, param.Start,
                param.Length,
                out var totalCount).ToList();

            var result = new DTResult<CustomWidget>
            {
                Draw = param.Draw,
                Data = filtered,
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return new JsonResult(result);
        }

        /// <inheritdoc />
        /// <summary>
        /// Create new report widget
        /// </summary>
        /// <param name="reportId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> CreateNewReportWidgetAsync(Guid? reportId)
        {
            var result = new ResultModel();
            var report = await _reportContext.DynamicReports.FirstOrDefaultAsync(x => x.Id.Equals(reportId));
            if (report == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(NullReferenceException)));
                return result;
            }

            var widget = new ReportWidget
            {
                WidgetGroupId = WidgetType.REPORT,
                ReportId = report.Id,
                Name = report.Name,
                WidgetTemplateType = WidgetTemplateType.Razor,
                Template = ""
            };
            await _context.ReportWidgets.AddAsync(widget);
            return await _context.PushAsync();
        }
    }
}
