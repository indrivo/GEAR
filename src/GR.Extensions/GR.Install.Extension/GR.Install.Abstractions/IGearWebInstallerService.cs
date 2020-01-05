using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Install.Abstractions.Models;

namespace GR.Install.Abstractions
{
    public interface IGearWebInstallerService
    {
        /// <summary>
        /// Install
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> InstallAsync(SetupModel model);
    }
}