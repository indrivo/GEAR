using System;
using System.Collections.Generic;
using System.Linq;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Config;
using GR.Notifications.Abstractions.Models.Notifications;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace GR.Notifications.Hub.Hubs
{
    public class CommunicationHub : ICommunicationHub
    {
        #region Injectable

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly UserManager<GearUser> _userManager;

        /// <summary>
        /// Inject hub
        /// </summary>
        private readonly IHubContext<GearNotificationHub> _hub;

        #endregion

        public CommunicationHub(UserManager<GearUser> userManager, IHubContext<GearNotificationHub> hub)
        {
            _userManager = userManager;
            _hub = hub;
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
                var userConnections = GearNotificationHub.UserConnections.Connections.GetConnectionsOfUserById(Guid.Parse(user.Id));
                userConnections.ToList().ForEach(c =>
                {
                    _hub.Clients.Client(c).SendAsync(SignalrSendMethods.SendClientEmail,
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
                var userConnections = GearNotificationHub.UserConnections.Connections.GetConnectionsOfUserById(user);
                userConnections.ToList().ForEach(c =>
                {
                    notification.UserId = user;
                    _hub.Clients.Client(c).SendAsync(SignalrSendMethods.SendClientNotification, notification);
                });
            }
        }

        /// <summary>
        /// Send data
        /// </summary>
        /// <param name="users"></param>
        /// <param name="data"></param>
        public void SendData(IEnumerable<Guid> users, Dictionary<string, object> data)
        {
            if (data == null) return;
            foreach (var user in users)
            {
                var userConnections = GearNotificationHub.UserConnections.Connections.GetConnectionsOfUserById(user);
                userConnections.ToList().ForEach(c =>
                {
                    _hub.Clients.Client(c).SendAsync(SignalrSendMethods.SendData, data);
                });
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Check if user is online
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool GetUserOnlineStatus(Guid userId)
        {
            var userConnections = GearNotificationHub.UserConnections.Connections.GetConnectionsOfUserById(userId);
            return userConnections.Any();
        }

        /// <inheritdoc />
        /// <summary>
        /// Check if user is online
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public bool IsUserOnline(GearUser user)
        {
            return user == null ? default : GetUserOnlineStatus(Guid.Parse(user.Id));
        }
        /// <inheritdoc />
        /// <summary>
        /// Get sessions count
        /// </summary>
        /// <returns></returns>
        public int GetSessionsCount()
        {
            return GearNotificationHub.UserConnections.Connections.GetSessionCount();
        }
        /// <inheritdoc />
        /// <summary>
        /// Get sessions by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public int GetSessionsCountByUserId(Guid userId)
        {
            return GearNotificationHub.UserConnections.Connections.GetSessionsByUserId(userId);
        }
        /// <inheritdoc />
        /// <summary>
        /// Get online users
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Guid> GetOnlineUsers()
        {
            return GearNotificationHub.UserConnections.Connections.GetUsersOnline();
        }
    }
}
