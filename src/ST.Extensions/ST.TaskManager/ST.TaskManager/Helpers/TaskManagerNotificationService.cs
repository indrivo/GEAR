using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Identity.Abstractions;
using ST.Notifications.Abstractions;
using ST.Notifications.Abstractions.Models.Notifications;
using ST.TaskManager.Abstractions;
using TaskStatus = ST.TaskManager.Abstractions.Enums.TaskStatus;

namespace ST.TaskManager.Helpers
{
    public sealed class TaskManagerNotificationService : ITaskManagerNotificationService
    {
        private readonly INotify<ApplicationRole> _notify;
        private readonly IUserManager<ApplicationUser> _identity;
        private readonly ITaskManagerContext _context;

        private const string TaskCreated = "Task {0} has been assigned to you.";
        private const string TaskUpdated = "Task {0} has been updated by {1}.";
        private const string TaskCompleted = "Task {0} has been completed by {1}.";
        private const string TaskRemoved = "Task {0} has been removed.";
        private const string TaskTitle = "Task Notification";
        private const string TaskExpires = "Task {0} expires tomorrow.";

        public TaskManagerNotificationService(IUserManager<ApplicationUser> identity)
        {
            _notify = IoC.Resolve<INotify<ApplicationRole>>();
            _identity = identity;
            _context = IoC.Resolve<ITaskManagerContext>();
        }

        public async Task AddTaskNotificationAsync(Abstractions.Models.Task task)
        {
            await _notify.SendNotificationAsync(new List<Guid>
            {
                task.UserId
            }, new SystemNotifications
            {
                Content = string.Format(TaskCreated, task.Name),
                Subject = TaskTitle,
                NotificationTypeId = NotificationType.Info
            });
        }

        public async Task UpdateTaskNotificationAsync(Abstractions.Models.Task task)
        {
            string content;
            var recipients = new List<Guid>();


            var currentUser = await _identity.GetCurrentUserAsync();
            if (!currentUser.IsSuccess) return;

            if (currentUser.Result.Id.ToGuid() == task.UserId)
            {
                var user = await _identity.UserManager.FindByNameAsync(task.Author);
                recipients.Add(user.Id.ToGuid());
                content = task.Status != TaskStatus.Completed ? string.Format(TaskUpdated, task.Name, currentUser.Result.UserName) : string.Format(TaskCompleted, task.Name, currentUser.Result.UserName);
            }
            else
            {
                recipients.Add(currentUser.Result.Id.ToGuid());
                content = string.Format(TaskUpdated, task.Name, task.Author);
            }

            await _notify.SendNotificationAsync(recipients,
                new SystemNotifications
                {
                    Content = content,
                    Subject = TaskTitle,
                    NotificationTypeId = NotificationType.Info
                });
        }

        public async Task DeleteTaskNotificationAsync(Abstractions.Models.Task task)
        {
            await _notify.SendNotificationAsync(new List<Guid>
            {
                task.UserId
            }, new SystemNotifications
            {
                Content = string.Format(TaskRemoved, task.Name),
                Subject = TaskTitle,
                NotificationTypeId = NotificationType.Info
            });
        }

        public async Task TaskExpirationNotificationAsync()
        {
            var notificationItems = _context.Tasks.Where(x => x.EndDate.Date == DateTime.Now.Date.AddDays(0)).ToList();


            foreach (var item in notificationItems)
            {
                await _notify.SendNotificationAsync(new List<Guid>
                {
                    item.Id
                }, new SystemNotifications
                {
                    Content = string.Format(TaskExpires, item.Name),
                    Subject = TaskTitle,
                    NotificationTypeId = NotificationType.Info
                });
            }
        }
    }
}
