using System;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Services;
using Microsoft.Extensions.DependencyInjection;
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

        /// <summary>
        /// Scope
        /// </summary>
        private readonly IServiceScope _scope;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="serviceProvider"></param>
        public TaskManagerBackgroundService(ILogger<TaskManagerBackgroundService> logger, IServiceProvider serviceProvider)
            : base("Task manager", logger)
        {
            _scope = serviceProvider.CreateScope();
            _managerNotificationService = _scope.ServiceProvider.GetService<ITaskManagerNotificationService>();
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

        public override void Dispose()
        {
            base.Dispose();
            _scope.Dispose();
        }
    }
}
