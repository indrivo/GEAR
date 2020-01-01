using System.Collections.Generic;
using GR.Core;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions.Helpers.Attributes;
using GR.Logger.Abstractions.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace GR.Logger.Razor.Controllers
{
    [Authorize]
    [Roles(GlobalResources.Roles.ADMINISTRATOR)]
    public class LogsController : Controller
    {
        #region Injectable

        /// <summary>
        /// Inject memory cache
        /// </summary>
        private readonly IMemoryCache _memoryCache;
        #endregion


        public LogsController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }


        /// <summary>
        /// Logs
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            _memoryCache.Set(LoggerResources.TEMP_LOGS_IN_MEMORY_ACTIVATED, true);
            return View();
        }

        /// <summary>
        /// Get logs
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/[controller]/[action]")]
        [Produces("application/json", Type = typeof(List<string>))]
        public JsonResult GetLogs()
        {
            var updated = _memoryCache.Get<bool>(LoggerResources.LOGS_UPDATED);
            var logs = updated ? _memoryCache.Get<IEnumerable<object>>(LoggerResources.TEMP_LOGS)
                : new List<string>();
            _memoryCache.Set(LoggerResources.TEMP_LOGS, new List<object>());
            _memoryCache.Set(LoggerResources.LOGS_UPDATED, false);

            return Json(logs);
        }

        /// <summary>
        /// Stop store logs
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[controller]/[action]")]
        public JsonResult StopStoreLogs()
        {
            _memoryCache.Set(LoggerResources.TEMP_LOGS_IN_MEMORY_ACTIVATED, false);
            _memoryCache.Set(LoggerResources.TEMP_LOGS, new List<object>());
            _memoryCache.Set(LoggerResources.LOGS_UPDATED, false);
            return Json(new SuccessResultModel<object>());
        }
    }
}