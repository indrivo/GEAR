using System.Collections.Generic;
using System.Linq;
using GR.Core.Helpers;
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
            var logsActivated = _cacheService.Get<bool?>("TempLogsInMemoryActivated") ?? false;
            if (!logsActivated) return;
            var logs = _cacheService.Get<IEnumerable<string>>("TempLogs")?.ToList() ?? new List<string>();
            logs.Add(message);
            _cacheService.Set("TempLogs", logs);
            _cacheService.Set("LogsUpdated", true);
        }
    }
}
