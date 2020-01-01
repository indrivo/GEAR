using System.Collections.Generic;
using System.Linq;
using GR.Core.Helpers;
using GR.Logger.Abstractions.Helpers;
using Microsoft.Extensions.Caching.Memory;
using NLog;
using NLog.Targets;

namespace GR.Logger
{
    [Target("GearTarget")]
    public class GearLoggerTarget : TargetWithLayout
    {
        #region Injectable

        private static IMemoryCache CacheService { get; set; }

        /// <summary>
        /// Inject cache service
        /// </summary>
        private IMemoryCache _cacheService => CacheService ?? (CacheService = IoC.ResolveNonRequired<IMemoryCache>());

        #endregion

        /// <summary>
        /// Write logs
        /// </summary>
        /// <param name="logEvent"></param>
        protected override void Write(LogEventInfo logEvent)
        {
            var message = Layout.Render(logEvent);
            if (_cacheService == null) return;
            var logsActivated = _cacheService.Get<bool?>(LoggerResources.TEMP_LOGS_IN_MEMORY_ACTIVATED) ?? false;
            if (!logsActivated) return;
            var logs = _cacheService.Get<IEnumerable<object>>(LoggerResources.TEMP_LOGS)?.ToList() ?? new List<object>();
            logs.Add(new
            {
                logEvent.Level,
                Message = message
            });
            _cacheService.Set(LoggerResources.TEMP_LOGS, logs);
            _cacheService.Set(LoggerResources.LOGS_UPDATED, true);
        }
    }
}