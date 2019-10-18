using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GR.Core;
using GR.Core.Helpers;
using GR.Dashboard.Abstractions.Models;

namespace GR.Dashboard.Abstractions
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