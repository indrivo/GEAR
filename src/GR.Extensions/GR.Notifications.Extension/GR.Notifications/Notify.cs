using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Attributes.Documentation;
using GR.Core.Helpers;
using GR.Core.Helpers.Global;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using GR.Notifications.Abstractions.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
// ReSharper disable UnusedTypeParameter
#pragma warning disable 1998

namespace GR.Notifications
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class Notify<TContext, TRole, TUser> : INotify<TRole> where TContext : IdentityDbContext<TUser, TRole, Guid> where TRole : IdentityRole<Guid> where TUser : IdentityUser<Guid>
    {
        public virtual async Task SendNotificationAsync(IEnumerable<TRole> roles, Notification notification, Guid? tenantId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task SendNotificationAsync(IEnumerable<Guid> users, Notification notification)
        {
            throw new NotImplementedException();
        }

        public virtual async Task SendNotificationAsync(IEnumerable<Guid> users, Guid notificationType, string subject, string content)
        {
            throw new NotImplementedException();
        }

        public virtual async Task SendNotificationAsync(Notification notification)
        {
            throw new NotImplementedException();
        }

        public virtual async Task SendNotificationToSystemAdminsAsync(Notification notification)
        {
            throw new NotImplementedException();
        }

        public virtual async Task SendNotificationInBackgroundAsync(IEnumerable<Guid> userId, Notification notification)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<ResultModel<IEnumerable<SystemNotifications>>> GetNotificationsByUserIdAsync(Guid userId, bool onlyUnread = true)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<ResultModel<Guid>> MarkAsReadAsync(Guid notificationId)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsUserOnline(Guid userId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<ResultModel<Dictionary<string, object>>> GetNotificationByIdAsync(Guid? notificationId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<ResultModel> ClearAllUserNotificationsAsync(Guid userId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<ResultModel<PaginatedNotificationsViewModel>> GetUserNotificationsWithPaginationAsync(uint page = 1, uint perPage = 10, bool onlyUnread = true)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<ResultModel> PermanentlyDeleteNotificationAsync(Guid? notificationId)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<ResultModel> SendAsync(string subject, string message, string to)
        {
            throw new NotImplementedException();
        }

        public async Task<ResultModel> SendAsync<TUser1>(TUser1 user, string subject, string message) where TUser1 : IdentityUser<Guid>
        {
            throw new NotImplementedException();
        }

        public virtual object GetProvider()
        {
            throw new NotImplementedException();
        }
    }
}
