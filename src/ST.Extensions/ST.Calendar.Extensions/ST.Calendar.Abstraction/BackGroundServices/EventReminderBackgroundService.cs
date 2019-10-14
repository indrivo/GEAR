using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ST.Calendar.Abstractions.Models;
using ST.Core.Helpers;
using ST.Identity.Abstractions;
using ST.Notifications.Abstractions;
using ST.Notifications.Abstractions.Models.Notifications;

namespace ST.Calendar.Abstractions.BackGroundServices
{
    public class EventReminderBackgroundService : IHostedService
    {
        /// <summary>
        /// Timer
        /// </summary>
        private Timer _timer;

        #region Injectable
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Inject calendar manager
        /// </summary>
        private readonly ICalendarDbContext _calendarDbContext;

        /// <summary>
        /// Notifier
        /// </summary>
        private readonly INotify<ApplicationRole> _notify;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="notify"></param>
        /// <param name="calendarDbContext"></param>
        public EventReminderBackgroundService(ILogger<EventReminderBackgroundService> logger, INotify<ApplicationRole> notify, ICalendarDbContext calendarDbContext)
        {
            _logger = logger;
            _notify = notify;
            _calendarDbContext = calendarDbContext;
        }


        /// <inheritdoc />
        /// <summary>
        /// Start async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calendar Event reminder Background Service is starting.");
            _timer = new Timer(Execute, null, TimeSpan.Zero,
                TimeSpan.FromMinutes(15));
            return Task.CompletedTask;
        }

        /// <summary>
        /// Send logs
        /// </summary>
        /// <param name="state"></param>
        private async void Execute(object state)
        {
            var events = await GetEventsForAllUsersWhatStartInAsync(59);
            if (!events.IsSuccess) return;
            var now = DateTime.Now;
            foreach (var evt in events.Result)
            {
                var minutes = (evt.StartDate - now).Minutes;
                if (evt.MinutesToRemind > minutes) continue;
                var users = new List<Guid> { evt.Organizer };
                users.AddRange(evt.EventMembers.Select(x => x.UserId));
                await _notify.SendNotificationAsync(users, NotificationType.Info, evt.Title, $"This event will take place over {minutes} minutes");
            }
        }

        /// <summary>
        /// Get today event for all users
        /// </summary>
        /// <returns></returns>
        private async Task<ResultModel<IEnumerable<CalendarEvent>>> GetEventsForAllUsersWhatStartInAsync(int minutes = 15)
        {
            var response = new ResultModel<IEnumerable<CalendarEvent>>();
            var now = DateTime.Now;
            var events = await _calendarDbContext.CalendarEvents
                .Include(x => x.EventMembers)
                .Where(x => (x.StartDate - now).TotalMinutes <= minutes && (x.StartDate - now).TotalMinutes > 0)
                .ToListAsync();

            response.IsSuccess = true;
            response.Result = events;
            return response;
        }

        /// <inheritdoc />
        /// <summary>
        /// Stop async
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Calendar Event reminder Background Service is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }
    }
}