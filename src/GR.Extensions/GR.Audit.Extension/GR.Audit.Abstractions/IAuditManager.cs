using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Audit.Abstractions.Models;
using GR.Audit.Abstractions.ViewModels.AuditViewModels;
using GR.Core.Helpers;

namespace GR.Audit.Abstractions
{
    public interface IAuditManager
    {
        /// <summary>
        /// Get filtered
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <param name="targets"></param>
        /// <returns></returns>
        IEnumerable<TrackAuditsListViewModel> GetAllFiltered(string search, string sortOrder, int start, int length,
            out int totalCount, Dictionary<string, Type> targets = null);

        /// <summary>
        /// Get filtered
        /// </summary>
        /// <param name="search"></param>
        /// <param name="sortOrder"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <param name="totalCount"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        IEnumerable<TrackAuditsListViewModel> GetAllForModuleFiltered(string search, string sortOrder, int start,
            int length,
            out int totalCount, string moduleName);

        /// <summary>
        /// Get details
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        Task<ResultModel<TrackAudit>> GetDetailsAsync(Guid? id, string moduleName);

        /// <summary>
        /// Get entity versions
        /// </summary>
        /// <param name="id"></param>
        /// <param name="moduleName"></param>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<TrackAudit>>> GetVersionsAsync(Guid? id, string moduleName);
    }
}
