using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Constants;
using GR.Dashboard.Abstractions.Models;
using GR.Dashboard.Abstractions.Models.Permissions;
using GR.Dashboard.Abstractions.Models.RowWidgets;
using GR.Dashboard.Abstractions.Models.ViewModels;
using GR.DynamicEntityStorage.Abstractions.Extensions;
using GR.Identity.Abstractions;
using Mapster;
using Microsoft.AspNetCore.Identity;

namespace GR.Dashboard
{
    [Author("Lupei Nicolae", 1.1)]
    [Documentation("This class provide services for manage dashboards and it's content")]
    public class DashboardService : IDashboardService
    {
        #region Injectable

        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly IDashboardDbContext _context;

        /// <summary>
        /// Inject role manager
        /// </summary>
        private readonly RoleManager<GearRole> _roleManager;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;
        #endregion

        public DashboardService(IDashboardDbContext context, RoleManager<GearRole> roleManager, IUserManager<GearUser> userManager)
        {
            _context = context;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        /// <inheritdoc />
        /// <summary>
        /// Dashboards
        /// </summary>
        public virtual IQueryable<DashBoard> DashBoards => _context.Dashboards;


        /// <inheritdoc />
        /// <summary>
        /// Get dashboard for render in view
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Row>>> GetDashboardConfigurationForRenderAsync()
        {
            var response = new ResultModel<IEnumerable<Row>>();
            var dashboard = await _context.Dashboards
                .Include(x => x.Rows)
                    .ThenInclude(x => x.CustomWidgets)
                        .ThenInclude(x => x.CustomWidget)

                .Include(x => x.Rows)
                    .ThenInclude(x => x.ReportWidgets)
                        .ThenInclude(x => x.ReportWidget)

                .Include(x => x.Rows)
                    .ThenInclude(x => x.ChartWidgets)
                        .ThenInclude(x => x.ChartWidget)
                .FirstOrDefaultAsync(x => x.IsActive);

            if (dashboard.IsNull())
            {
                response.Errors.Add(new ErrorModel(string.Empty, "No active dashboard present!"));
                return response;
            }

            response.IsSuccess = true;
            response.Result = dashboard.Rows.OrderBy(x => x.Order);
            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get dashboard configuration
        /// </summary>
        /// <param name="dashboardId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DashboardRowViewModel>>> GetDashBoardConfigurationAsync(Guid? dashboardId)
        {
            var result = new ResultModel<IEnumerable<DashboardRowViewModel>>();
            if (dashboardId.HasValue.Negate())
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(ArgumentNullException)));
                return result;
            }

            var dashboard = await _context.Dashboards
                .Include(x => x.Rows)
                .ThenInclude(x => x.CustomWidgets)
                .ThenInclude(x => x.CustomWidget)

                .Include(x => x.Rows)
                .ThenInclude(x => x.ChartWidgets)
                .ThenInclude(x => x.ChartWidget)

                .Include(x => x.Rows)
                .ThenInclude(x => x.ReportWidgets)
                .ThenInclude(x => x.ReportWidget)

                .FirstOrDefaultAsync(x => x.Id.Equals(dashboardId));

            if (dashboard == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(NotFoundObjectResult)));
                return result;
            }

            var data = new List<DashboardRowViewModel>();

            foreach (var row in dashboard.Rows.OrderBy(x => x.Order))
            {
                var rWidgets = new List<RowWidgetViewModel>();
                if (row.ReportWidgets.Any())
                {
                    rWidgets.AddRange(row.ReportWidgets.Select(x => new RowWidgetViewModel
                    {
                        Id = x.ReportWidgetId,
                        GroupId = x.ReportWidget?.WidgetGroupId,
                        Order = x.Order,
                        Name = x.ReportWidget?.Name
                    }));
                }

                if (row.CustomWidgets.Any())
                {
                    rWidgets.AddRange(row.CustomWidgets.Select(x => new RowWidgetViewModel
                    {
                        Id = x.CustomWidgetId,
                        GroupId = x.CustomWidget?.WidgetGroupId,
                        Order = x.Order,
                        Name = x.CustomWidget?.Name
                    }));
                }

                data.Add(new DashboardRowViewModel
                {
                    RowId = row.Id,
                    Order = row.Order,
                    Widgets = rWidgets.OrderBy(x => x.Order).ToList()
                });
            }

            result.IsSuccess = true;
            result.Result = data;

            return result;
        }


        /// <inheritdoc />
        /// <summary>
        /// Add new dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> CreateDashBoardAsync(DashBoard model)
        {
            var result = new ResultModel();
            if (model == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(ArgumentNullException)));
                return result;
            }

            await _context.Dashboards.AddAsync(model);
            var dbResult = await _context.PushAsync();
            if (!dbResult.IsSuccess) return dbResult;
            if (model.IsActive)
            {
                return await SetActiveDashBoardAsync(model.Id);
            }

            result.IsSuccess = true;
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Update dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateDashBoardAsync(DashBoard model)
        {
            var result = new ResultModel();
            if (model == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(ArgumentNullException)));
                return result;
            }
            _context.Dashboards.Update(model);
            var dbResult = await _context.PushAsync();
            if (!dbResult.IsSuccess) return dbResult;
            if (model.IsActive)
            {
                var req = await SetActiveDashBoardAsync(model.Id);
                if (!req.IsSuccess) return req;
            }

            result.IsSuccess = true;
            return result;
        }


        /// <inheritdoc />
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

        /// <inheritdoc />
        /// <summary>
        /// Get active dashboard
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<DashBoard>> GetActiveDashboardAsync()
        {
            var response = new ResultModel<DashBoard>();
            var dashboard = await _context.Dashboards.FirstOrDefaultAsync(x => x.IsActive);
            if (dashboard == null)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "No active dashboard present"));
                return response;
            }

            response.IsSuccess = true;
            response.Result = dashboard;
            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Add or update dashboard configuration
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<DashboardRowViewModel>>> AddOrUpdateDashboardConfigurationAsync(DashBoardConfigurationViewModel configuration)
        {
            var result = new ResultModel<IEnumerable<DashboardRowViewModel>>();
            if (configuration == null || configuration.DashboardId.HasValue.Negate())
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(ArgumentNullException)));
                return result;
            }

            var dashboard = await _context.Dashboards
                .Include(x => x.Rows)
                .FirstOrDefaultAsync(x => x.Id.Equals(configuration.DashboardId));
            if (dashboard == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(NotFoundObjectResult)));
                return result;
            }

            if (configuration.Rows.Any().Negate())
            {
                result.IsSuccess = true;
                return result;
            }

            var rowsConf = new List<DashboardRowViewModel>();

            foreach (var row in configuration.Rows)
            {
                var existentRow = dashboard.Rows.FirstOrDefault(x => x.Id.Equals(row.RowId));
                if (row.RowId.HasValue && existentRow != null)
                {
                    existentRow.Order = row.Order;
                    _context.Update(existentRow);
                    await AddOrUpdateWidgetsToRowAsync(row);
                }
                else
                {
                    var newRow = new Row
                    {
                        Order = row.Order,
                        DashboardId = dashboard.Id
                    };

                    rowsConf.Add(new DashboardRowViewModel
                    {
                        RowId = newRow.Id,
                        Order = newRow.Order
                    });
                    await _context.Rows.AddAsync(newRow);
                    await _context.PushAsync();
                    row.RowId = newRow.Id;
                    await AddOrUpdateWidgetsToRowAsync(row, true);
                }
            }

            var dbResult = await _context.PushAsync();
            if (dbResult.IsSuccess.Negate())
            {
                result.Errors = dbResult.Errors;
                return result;
            }

            result.Result = rowsConf;
            result.IsSuccess = true;
            return result;
        }


        /// <summary>
        /// Add or update widgets to row
        /// </summary>
        /// <param name="row"></param>
        /// <param name="isNew"></param>
        /// <returns></returns>
        protected virtual async Task<ResultModel> AddOrUpdateWidgetsToRowAsync(DashboardRowViewModel row, bool? isNew = false)
        {
            var mappedReportWidgets = isNew.GetValueOrDefault()
                ? new List<RowReportWidget>()
                : await _context.RowReportWidgets
                    .Include(x => x.ReportWidget)
                    .Where(x => x.RowId.Equals(row.RowId)).ToListAsync();

            var mappedCustomWidgets = isNew.GetValueOrDefault()
                ? new List<RowCustomWidget>()
                : await _context.RowCustomWidgets
                    .Include(x => x.CustomWidget)
                    .Where(x => x.RowId.Equals(row.RowId)).ToListAsync();

            if (row.Widgets.Any())
            {
                foreach (var widget in row.Widgets)
                {
                    if (!widget.Id.HasValue || !widget.GroupId.HasValue) continue;
                    if (widget.GroupId == WidgetType.REPORT)
                    {
                        var dbWidget = await _context.ReportWidgets
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id.Equals(widget.Id));
                        if (dbWidget == null) continue;
                        var check = mappedReportWidgets.FirstOrDefault(x => x.ReportWidgetId.Equals(widget.Id));
                        if (check == null)
                        {
                            await _context.RowReportWidgets.AddAsync(new RowReportWidget
                            {
                                RowId = row.RowId.GetValueOrDefault(),
                                ReportWidgetId = widget.Id.Value,
                                Order = widget.Order
                            });
                        }
                        else if (check.Order != widget.Order)
                        {
                            check.Order = widget.Order;
                            _context.RowReportWidgets.Update(check);
                        }
                    }
                    else if (widget.GroupId == WidgetType.CHARTS)
                    {
                        var dbWidget = await _context.ChartWidgets
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id.Equals(widget.Id));
                        if (dbWidget == null) continue;
                        await _context.RowChartWidgets.AddAsync(new RowChartWidget
                        {
                            RowId = row.RowId.GetValueOrDefault(),
                            ChartWidgetId = widget.Id.Value,
                            Order = widget.Order
                        });
                    }
                    else
                    {
                        var dbWidget = await _context.CustomWidgets
                            .AsNoTracking()
                            .FirstOrDefaultAsync(x => x.Id.Equals(widget.Id));
                        if (dbWidget == null) continue;

                        var check = mappedCustomWidgets.FirstOrDefault(x => x.CustomWidgetId.Equals(widget.Id));

                        if (check == null)
                        {
                            await _context.RowCustomWidgets.AddAsync(new RowCustomWidget
                            {
                                RowId = row.RowId.GetValueOrDefault(),
                                CustomWidgetId = widget.Id.Value,
                                Order = widget.Order
                            });
                        }
                        else if (check.Order != widget.Order)
                        {
                            check.Order = widget.Order;
                            _context.RowCustomWidgets.Update(check);
                        }
                    }
                }
            }

            return await _context.PushAsync();
        }


        /// <inheritdoc />
        /// <summary>
        /// Set active dashboard
        /// </summary>
        /// <param name="dashboardId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SetActiveDashBoardAsync(Guid? dashboardId)
        {
            var result = new ResultModel();
            if (dashboardId.HasValue.Negate())
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

            if (dashboard.IsActive.Negate())
            {
                dashboard.IsActive = true;
                _context.Update(dashboard);
            }

            var others = await _context.Dashboards.Where(x => x.Id != dashboardId).ToListAsync();

            foreach (var o in others)
            {
                o.IsActive = false;
                _context.Update(o);
            }

            return await _context.PushAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete row 
        /// </summary>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteRowAsync(Guid? rowId)
        {
            var response = new ResultModel();
            if (rowId.HasValue.Negate())
            {
                response.Errors.Add(new ErrorModel(string.Empty, nameof(NullReferenceException)));
                return response;
            }

            var row = await _context.Rows.FirstOrDefaultAsync(x => x.Id.Equals(rowId));
            if (row.IsNull())
            {
                response.Errors.Add(new ErrorModel(string.Empty, nameof(NotFoundObjectResult)));
                return response;
            }

            _context.Rows.Remove(row);
            return await _context.PushAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Get widgets
        /// </summary>
        /// <param name="widgetGroupId"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<Widget>> GetWidgetGroupRowsAsync(Guid? widgetGroupId)
        {
            var data = new List<Widget>();
            if (widgetGroupId.HasValue.Negate()) return data;
            var rWidgets = await _context.ReportWidgets.Where(x => x.WidgetGroupId.Equals(widgetGroupId)).ToListAsync();
            if (rWidgets.Any()) data.AddRange(rWidgets);

            var customWidgets = await _context.CustomWidgets.Where(x => x.WidgetGroupId.Equals(widgetGroupId)).ToListAsync();
            if (customWidgets.Any()) data.AddRange(customWidgets);

            var chartWidgets = await _context.ChartWidgets.Where(x => x.WidgetGroupId.Equals(widgetGroupId)).ToListAsync();
            if (chartWidgets.Any()) data.AddRange(chartWidgets);
            return data;
        }

        /// <inheritdoc />
        /// <summary>
        /// Delete map row
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> DeleteMappedWidgetToRowAsync(Guid? rowId, Guid? widgetId)
        {
            var result = new ResultModel();
            if (!rowId.HasValue || !widgetId.HasValue)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(NullReferenceException)));
                return result;
            }

            var exist = false;
            var customCheck =
                await _context.RowCustomWidgets.FirstOrDefaultAsync(x =>
                    x.CustomWidgetId.Equals(widgetId) && x.RowId.Equals(rowId));
            if (customCheck != null)
            {
                _context.RowCustomWidgets.Remove(customCheck);
                exist = true;
            }
            else
            {
                var reportCheck =
                    await _context.RowReportWidgets.FirstOrDefaultAsync(x =>
                        x.ReportWidgetId.Equals(widgetId) && x.RowId.Equals(rowId));

                if (reportCheck != null)
                {
                    _context.RowReportWidgets.Remove(reportCheck);
                    exist = true;
                }
                else
                {
                    var chartCheck =
                        await _context.RowChartWidgets.FirstOrDefaultAsync(x =>
                            x.ChartWidgetId.Equals(widgetId) && x.RowId.Equals(rowId));

                    if (chartCheck != null)
                    {
                        _context.RowChartWidgets.Remove(chartCheck);
                        exist = true;
                    }
                }
            }

            if (exist) return await _context.PushAsync();
            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Seed async
        /// </summary>
        /// <returns></returns>
        public virtual Task SeedWidgetsAsync()
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get ui settings for mapped
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public async Task<ResultModel<WidgetUISettings>> GetUISettingsForWidgetAsync(Guid? widgetId, Guid? rowId)
        {
            var response = new ResultModel<WidgetUISettings>();
            if (!widgetId.HasValue || !rowId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                return response;
            }

            var customWidgets =
                await _context.RowCustomWidgets.FirstOrDefaultAsync(
                    x => x.RowId.Equals(rowId) && x.CustomWidgetId.Equals(widgetId));
            if (customWidgets != null)
            {
                response.IsSuccess = true;
                response.Result = customWidgets;
                return response;
            }

            response.Errors.Add(new ErrorModel(string.Empty, "UI settings are not currently available for this widget group"));

            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Update
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <param name="uiSettings"></param>
        /// <returns></returns>
        public async Task<ResultModel> UpdateUISettingsAsync(Guid? widgetId, Guid? rowId, WidgetUISettings uiSettings)
        {
            var response = new ResultModel();
            if (!widgetId.HasValue || !rowId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                return response;
            }

            var customWidget =
                await _context.RowCustomWidgets.FirstOrDefaultAsync(
                    x => x.RowId.Equals(rowId) && x.CustomWidgetId.Equals(widgetId));
            if (customWidget != null)
            {
                customWidget.BackGroundColor = uiSettings.BackGroundColor;
                customWidget.BorderRadius = uiSettings.BorderRadius;
                customWidget.BorderStyle = uiSettings.BorderStyle;
                customWidget.ClassAttribute = uiSettings.ClassAttribute;
                customWidget.Height = uiSettings.Height;
                customWidget.Width = uiSettings.Width;
                _context.Update(customWidget);
                return await _context.PushAsync();
            }

            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get acl for widget mapped on row
        /// </summary>
        /// <param name="widgetId"></param>
        /// <param name="rowId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<RowWidgetAcl>>> GetRowWidgetAclInfoAsync(Guid? widgetId, Guid? rowId)
        {
            var response = new ResultModel<IEnumerable<RowWidgetAcl>>();
            if (!widgetId.HasValue || !rowId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                return response;
            }

            var conf = await _context.WidgetAcls.Where(x => x.WidgetId.Equals(widgetId) && x.RowId.Equals(rowId)).ToListAsync();
            response.IsSuccess = true;
            response.Result = conf;
            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Update conf
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="configuration"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> UpdateAclAsync(Guid? widgetId, Guid? rowId, IEnumerable<RowWidgetAclBase> configuration)
        {
            var response = new ResultModel();
            if (!widgetId.HasValue || !rowId.HasValue)
            {
                response.Errors.Add(new ErrorModel(string.Empty, "Invalid parameters"));
                return response;
            }
            var roles = await _roleManager.Roles.NonDeleted().ToListAsync();

            var dbConfRequest = await GetRowWidgetAclInfoAsync(widgetId, rowId);
            if (!dbConfRequest.IsSuccess) return dbConfRequest.ToBase();

            var oldConf = dbConfRequest.Result.ToList();
            _context.WidgetAcls.RemoveRange(oldConf);

            if (configuration != null)
            {
                foreach (var item in configuration)
                {
                    if (roles.FirstOrDefault(x => x.Id.ToGuid().Equals(item.RoleId)) == null) continue;

                    var o = item.Adapt<RowWidgetAcl>();
                    o.RowId = rowId.GetValueOrDefault();
                    o.WidgetId = widgetId.GetValueOrDefault();
                    await _context.WidgetAcls.AddAsync(o);
                }
            }

            return await _context.PushAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Check permissions for view widget
        /// </summary>
        /// <param name="rowId"></param>
        /// <param name="widgetId"></param>
        /// <returns></returns>
        public async Task<bool> HasAccess(Guid? rowId, Guid? widgetId)
        {
            if (rowId == null || widgetId == null) return false;
            var confRequest = await GetRowWidgetAclInfoAsync(widgetId, rowId);
            if (!confRequest.IsSuccess) return false;
            if (!confRequest.Result.Any()) return true;
            var currentUserRequest = await _userManager.GetCurrentUserAsync();
            if (!currentUserRequest.IsSuccess) return false;
            var roles = (await _userManager.GetUserRolesAsync(currentUserRequest.Result))
                .Select(x => x.Id.ToGuid()).ToList();

            var selected = confRequest.Result.Where(x => x.Allow).Select(x => x.RoleId).ToList();
            return roles.Intersect(selected).Any();
        }
    }
}
