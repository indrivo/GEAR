using System;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Services;
using Microsoft.Extensions.Logging;

namespace GR.TaskManager.Abstractions.BackgroundServices
{
    public class TaskManagerBackgroundService : BaseBackgroundService<TaskManagerBackgroundService>
    {
        #region Injectable

        /// <summary>
        /// Inject service
        /// </summary>
        private readonly ITaskManagerNotificationService _managerNotificationService;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="managerNotificationService"></param>
        public TaskManagerBackgroundService(ILogger<TaskManagerBackgroundService> logger, ITaskManagerNotificationService managerNotificationService)
            : base("Task manager", logger)
        {
            _managerNotificationService = managerNotificationService;
            Interval = TimeSpan.FromHours(24);
        }

        /// <summary>
        /// Send logs
        /// </summary>
        /// <param name="state"></param>
        public override async Task Execute(object state)
        {
            if (!GearApplication.Configured) return;
            await _managerNotificationService.TaskExpirationNotificationAsync();
        }
    }
}
