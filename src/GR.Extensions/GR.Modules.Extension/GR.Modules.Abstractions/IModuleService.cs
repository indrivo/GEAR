using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Modules.Abstractions.Models;

namespace GR.Modules.Abstractions
{
    public interface IModuleService
    {
        /// <summary>
        /// Get all modules
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<IEnumerable<Module>>> GetAllModulesAsync();
    }
}