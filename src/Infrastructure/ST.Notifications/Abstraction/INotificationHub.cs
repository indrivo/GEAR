using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ST.Entities.Models.Notifications;
using ST.Notifications.Hubs;

namespace ST.Notifications.Abstraction
{
	public interface INotificationHub
	{
		/// <summary>
		/// Send email notification to users
		/// </summary>
		/// <param name="userEmailNotification"></param>
		void SentEmailNotification(SignalrEmail userEmailNotification);
        /// <summary>
        /// Send notification to client
        /// </summary>
        /// <param name="users"></param>
        /// <param name="notification"></param>
	    void SentNotification(IEnumerable<Guid> users, SystemNotifications notification);
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
