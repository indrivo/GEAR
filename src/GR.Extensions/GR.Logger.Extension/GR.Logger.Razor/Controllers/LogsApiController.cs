using System.Threading.Tasks;
using GR.Core;
using GR.Core.Razor.Attributes;
using GR.Core.Razor.BaseControllers;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Logger.Abstractions;
using GR.Logger.Abstractions.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace GR.Logger.Razor.Controllers
{
    [Admin]
    [Route(DefaultApiRouteTemplate)]
    public class LogsApiController : BaseGearController
    {
        #region Injectable

        /// <summary>
        /// Inject logger service
        /// </summary>
        private readonly ILoggerService _loggerService;

        #endregion

        public LogsApiController(ILoggerService loggerService)
        {
            _loggerService = loggerService;
        }

        /// <summary>
        /// Get log with pagination
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        [HttpGet]
        [JsonProduces(typeof(DTResult<LogEventViewModel>))]
        public async Task<JsonResult> GetLogsWithPagination(DTParameters parameters)
            => await JsonAsync(_loggerService.GetLogsWithPaginationAsync(parameters));
    }
}