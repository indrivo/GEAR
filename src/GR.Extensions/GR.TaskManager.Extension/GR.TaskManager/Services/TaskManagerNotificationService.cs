using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using GR.TaskManager.Abstractions;
using TaskStatus = GR.TaskManager.Abstractions.Enums.TaskStatus;

namespace GR.TaskManager.Services
{
    public sealed class TaskManagerNotificationService : ITaskManagerNotificationService
    {
        private readonly INotify<ApplicationRole> _notify;
        private readonly IUserManager<ApplicationUser> _identity;
        private readonly ITaskManagerContext _context;

        private const string TaskCreated = "Task #{0} has been assigned to you.";
        private const string TaskUpdated = "Task #{0} has been updated by {1}.";
        private const string TaskCompleted = "Task #{0} has been completed by {1}.";
        private const string TaskRemoved = "Task #{0} has been removed.";
        private const string TaskTitle = "Task Notification";
        private const string TaskExpires = "Task #{0} expires tomorrow.";

        public TaskManagerNotificationService(IUserManager<ApplicationUser> identity)
        {
            _notify = IoC.Resolve<INotify<ApplicationRole>>();
            _identity = identity;
            _context = IoC.Resolve<ITaskManagerContext>();
        }

        internal async Task AddTaskNotificationAsync(Abstractions.Models.Task task)
        {
            await _notify.SendNotificationAsync(new List<Guid>
            {
                task.UserId
            }, new Notification
            {
                Content = string.Format(TaskCreated, task.TaskNumber),
                Subject = TaskTitle,
                NotificationTypeId = NotificationType.Info
            });
        }

        internal async Task UpdateTaskNotificationAsync(Abstractions.Models.Task task)
        {
            string content;
            var recipients = new List<Guid>();


            var currentUser = await _identity.GetCurrentUserAsync();
            if (!currentUser.IsSuccess) return;

            if (currentUser.Result.Id.ToGuid() == task.UserId)
            {
                var user = await _identity.UserManager.FindByNameAsync(task.Author);
                recipients.Add(user.Id.ToGuid());
                content = task.Status != TaskStatus.Completed ? string.Format(TaskUpdated, task.TaskNumber, currentUser.Result.UserName) : string.Format(TaskCompleted, task.TaskNumber, currentUser.Result.UserName);
            }
            else
            {
                recipients.Add(currentUser.Result.Id.ToGuid());
                content = string.Format(TaskUpdated, task.TaskNumber, task.Author);
            }

            await _notify.SendNotificationAsync(recipients,
                new Notification
                {
                    Content = content,
                    Subject = TaskTitle,
                    NotificationTypeId = NotificationType.Info
                });
        }

        internal async Task DeleteTaskNotificationAsync(Abstractions.Models.Task task)
        {
            await _notify.SendNotificationAsync(new List<Guid>
            {
                task.UserId
            }, new Notification
            {
                Content = string.Format(TaskRemoved, task.TaskNumber),
                Subject = TaskTitle,
                NotificationTypeId = NotificationType.Info
            });
        }

        public async Task TaskExpirationNotificationAsync()
        {
            var notificationItems = await _context.Tasks.Where(x => x.EndDate.Date == DateTime.Now.Date.AddDays(0)).ToListAsync();
            foreach (var item in notificationItems)
                await _notify.SendNotificationAsync(new List<Guid>
                {
                    item.UserId
                    }, new Notification
                    {
                        Content = string.Format(TaskExpires, item.TaskNumber),
                        Subject = TaskTitle,
                        NotificationTypeId = NotificationType.Info
                    });
        }
    }
}
