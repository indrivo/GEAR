using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using ST.Notifications.Abstractions;
using ST.Notifications.Abstractions.Models.Config;
using ST.Notifications.Abstractions.Models.Notifications;

namespace ST.Notifications.Hubs
{
    /// <inheritdoc />
    // ReSharper disable once ClassNeverInstantiated.Global
    public class NotificationProvider<TUserEntity> : INotificationHub where TUserEntity : IdentityUser
    {
        private readonly UserManager<TUserEntity> _userManager;
        private readonly IHubContext<NotificationsHub> _hubContext;

        public NotificationProvider(UserManager<TUserEntity> userManager, IHubContext<NotificationsHub> hubContext)
        {
            _userManager = userManager;
            _hubContext = hubContext;
        }
        /// <inheritdoc />
        /// <summary>
        /// Sent email notification
        /// </summary>
        /// <param name="userEmailNotification"></param>
        public void SendEmailNotification(SignalrEmail userEmailNotification)
        {
            var fromUser = _userManager.Users.FirstOrDefault(x => x.Id == userEmailNotification.UserId.ToString());
            if (userEmailNotification?.EmailRecipients == null) return;
            foreach (var x in userEmailNotification.EmailRecipients)
            {
                var user = _userManager.Users.FirstOrDefault(y => y.Id.Equals(x));
                if (user == null) return;
                var userConnections = NotificationsHub.Connections.GetConnectionsOfUserById(Guid.Parse(user.Id));
                userConnections.ToList().ForEach(c =>
                {
                    _hubContext.Clients.Client(c).SendAsync(SignalrSendMethods.SendClientEmail,
                        userEmailNotification.Subject, userEmailNotification.Message, fromUser?.Email, fromUser?.UserName, fromUser?.Id);
                });
            }
        }
        /// <inheritdoc />
        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="users"></param>
        /// <param name="notification"></param>
        public void SendNotification(IEnumerable<Guid> users, SystemNotifications notification)
        {
            if (notification == null) return;
            foreach (var user in users)
            {
                var userConnections = NotificationsHub.Connections.GetConnectionsOfUserById(user);
                userConnections.ToList().ForEach(c =>
                {
                    notification.UserId = user;
                    _hubContext.Clients.Client(c).SendAsync(SignalrSendMethods.SendClientNotification, notification);
                });
            }
        }
        /// <inheritdoc />
        /// <summary>
        /// Check if user is online
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsUserOnline(Guid userId)
        {
            var userConnections = NotificationsHub.Connections.GetConnectionsOfUserById(userId);
            return userConnections.Any();
        }

        /// <inheritdoc />
        /// <summary>
        /// Check if user is online
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsUserOnline<TUser>(TUser user) where TUser : IdentityUser
        {
            return user == null ? default : IsUserOnline(Guid.Parse(user.Id));
        }
        /// <inheritdoc />
        /// <summary>
        /// Get sessions count
        /// </summary>
        /// <returns></returns>
        public int GetSessionsCount()
        {
            return NotificationsHub.Connections.GetSessionCount();
        }
        /// <inheritdoc />
        /// <summary>
        /// Get sessions by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetSessionsCountByUserId(Guid userId)
        {
            return NotificationsHub.Connections.GetSesionsByUserId(userId);
        }
        /// <inheritdoc />
        /// <summary>
        /// Get online users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Guid> GetOnlineUsers()
        {
            return NotificationsHub.Connections.GetUsersOnline();
        }
    }
}
