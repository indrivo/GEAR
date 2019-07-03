using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ST.Notifications.Abstractions.Models.Config;
using ST.Notifications.Abstractions.Models.Notifications;

namespace ST.Notifications.Abstractions
{
	public interface INotificationHub
	{
		/// <summary>
		/// Send email notification to users
		/// </summary>
		/// <param name="userEmailNotification"></param>
		void SendEmailNotification(SignalrEmail userEmailNotification);
        /// <summary>
        /// Send notification to client
        /// </summary>
        /// <param name="users"></param>
        /// <param name="notification"></param>
	    void SendNotification(IEnumerable<Guid> users, SystemNotifications notification);
        /// <summary>
        /// Check if user is online 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
	    bool IsUserOnline(Guid userId);

        /// <summary>
        /// Check if user is online
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        bool IsUserOnline<TUser>(TUser user) where TUser : IdentityUser;
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
