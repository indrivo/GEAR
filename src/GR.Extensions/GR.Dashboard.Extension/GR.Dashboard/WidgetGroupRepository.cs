using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions;
using GR.Dashboard.Abstractions.Models;
using GR.DynamicEntityStorage.Abstractions.Extensions;

namespace GR.Dashboard
{
    [Author("Lupei Nicolae", 1.1)]
    [Documentation("This repo is for manage widget groups")]
    public sealed class WidgetGroupRepository : IWidgetGroupRepository
    {
        #region Injectable

        /// <summary>
        /// Inject db context
        /// </summary>
        private readonly IDashboardDbContext _context;

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public WidgetGroupRepository(IDashboardDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        /// <summary>
        /// Widget groups
        /// </summary>
        public IQueryable<WidgetGroup> WidgetGroups => _context.WidgetGroups;

        /// <inheritdoc />
        /// <summary>
        /// Add new dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel> CreateWidgetGroupAsync(WidgetGroup model)
        {
            var result = new ResultModel();
            if (model == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(ArgumentNullException)));
                return result;
            }

            await _context.WidgetGroups.AddAsync(model);
            var dbResult = await _context.PushAsync();
            if (!dbResult.IsSuccess) return dbResult;
            result.IsSuccess = true;
            return result;
        }


        /// <inheritdoc />
        /// <summary>
        /// Update dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<ResultModel> UpdateWidgetGroupAsync(WidgetGroup model)
        {
            var result = new ResultModel();
            if (model == null)
            {
                result.Errors.Add(new ErrorModel(string.Empty, nameof(ArgumentNullException)));
                return result;
            }
            _context.WidgetGroups.Update(model);
            return await _context.PushAsync();
        }

        /// <inheritdoc />
        /// <summary>
        /// Get dashboards
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public JsonResult GetWidgetGroupsInJqueryTableFormat(DTParameters param)
        {
            var filtered = _context.FilterAbstractContext<WidgetGroup>(param.Search.Value,
                param.SortOrder, param.Start,
                param.Length,
                out var totalCount).ToList();

            var result = new DTResult<WidgetGroup>
            {
                Draw = param.Draw,
                Data = filtered,
                RecordsFiltered = totalCount,
                RecordsTotal = filtered.Count
            };
            return new JsonResult(result);
        }
    }
}
