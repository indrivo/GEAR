using System;
using System.Collections.Generic;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;

namespace GR.Notifications.Abstractions
{
	public interface ICommunicationHub
	{
        /// <summary>
        /// Send notification to client
        /// </summary>
        /// <param name="users"></param>
        /// <param name="notification"></param>
	    void SendNotification(IEnumerable<Guid> users, SystemNotifications notification);

        /// <summary>
        /// Send any data to clients data
        /// </summary>
        /// <param name="users"></param>
        /// <param name="data"></param>
        void SendData(IEnumerable<Guid> users, Dictionary<string, object> data);

        /// <summary>
        /// Send data
        /// </summary>
        /// <param name="users"></param>
        /// <param name="data"></param>
        void SendData(IEnumerable<Guid> users, object data);

        /// <summary>
        /// Check if user is online 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool GetUserOnlineStatus(Guid userId);

        /// <summary>
        /// Check if user is online
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool IsUserOnline(GearUser user);

        /// <summary>
        /// Get sessions count
        /// </summary>
        /// <returns></returns>
        int GetSessionsCount();

        /// <summary>
        /// Get online users
        /// </summary>
        /// <returns></returns>
        IEnumerable<Guid> GetOnlineUsers();

        /// <summary>
        /// Get sessions by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
	    int GetSessionsCountByUserId(Guid userId);
	}
}
