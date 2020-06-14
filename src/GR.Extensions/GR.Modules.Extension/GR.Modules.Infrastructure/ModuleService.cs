using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Modules.Abstractions;
using GR.Modules.Abstractions.Models;

namespace GR.Modules.Infrastructure
{
    public class ModuleService : IModuleService
    {
        public async Task<ResultModel<IEnumerable<Module>>> GetAllModulesAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}