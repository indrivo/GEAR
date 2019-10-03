using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ST.Core;
using ST.Core.Helpers;
using ST.Dashboard.Abstractions.Models;

namespace ST.Dashboard.Abstractions
{
    public interface IWidgetGroupRepository
    {
        /// <summary>
        /// Widget groups
        /// </summary>
        IQueryable<WidgetGroup> WidgetGroups { get; }

        /// <summary>
        /// Create widget group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> CreateWidgetGroupAsync(WidgetGroup model);

        /// <summary>
        /// Update widget groups
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> UpdateWidgetGroupAsync(WidgetGroup model);

        /// <summary>
        /// Get list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        JsonResult GetWidgetGroupsInJqueryTableFormat(DTParameters param);
    }
}