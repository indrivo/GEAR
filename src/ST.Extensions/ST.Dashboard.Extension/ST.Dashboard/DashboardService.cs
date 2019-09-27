using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Dashboard.Abstractions;
using ST.Dashboard.Abstractions.Models;
using ST.Dashboard.Abstractions.Models.ViewModels;
using ST.DynamicEntityStorage.Abstractions.Extensions;

namespace ST.Dashboard
{
    public class DashboardService : IDashboardService
    {
        #region Injectable
        private readonly IDashboardDbContext _context;
        #endregion

        public DashboardService(IDashboardDbContext context)
        {
            _context = context;
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

        /// <summary>
        /// Get widget groups
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel> GetWidgetGroupsAsync()
        {
            var result = new ResultModel();

            return result;
        }

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
                .FirstOrDefaultAsync(x => x.Id.Equals(dashboardId));

            if (dashboard == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(NotFoundObjectResult)));
                return result;
            }

            result.Result = dashboard.Rows.Select(x => new DashboardRowViewModel
            {
                Order = x.Order,
                RowId = x.Id
            });

            result.IsSuccess = true;

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
    }
}
