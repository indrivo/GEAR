using System.Threading.Tasks;
using GR.Core;
using GR.Logger.Abstractions.ViewModels;

namespace GR.Logger.Abstractions
{
    public interface ILoggerService
    {
        /// <summary>
        /// Get logs with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        Task<DTResult<LogEventViewModel>> GetLogsWithPaginationAsync(DTParameters parameters);
    }
}