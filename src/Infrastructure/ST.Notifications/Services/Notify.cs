using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ST.BaseBusinessRepository;
using ST.Entities.Models.Notifications;
using ST.Entities.Services.Abstraction;
using ST.Identity.Data;
using ST.Identity.Data.Permissions;
using ST.Notifications.Abstraction;

namespace ST.Notifications.Services
{
    public class Notify : INotify
    {
        /// <summary>
        /// Inject data service
        /// </summary>
        private readonly IDynamicEntityDataService _dataService;
        /// <summary>
        /// Context
        /// </summary>
        private readonly ApplicationDbContext _context;
        /// <summary>
        /// Notification hub
        /// </summary>
        private readonly INotificationHub _hub;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<Notify> _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataService"></param>
        /// <param name="context"></param>
        /// <param name="hub"></param>
        /// <param name="logger"></param>
        public Notify(IDynamicEntityDataService dataService, ApplicationDbContext context, INotificationHub hub, ILogger<Notify> logger)
        {
            _dataService = dataService;
            _context = context;
            _hub = hub;
            _logger = logger;
        }
        /// <inheritdoc />
        /// <summary>
        /// Send Notification
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task SendNotificationAsync(IEnumerable<ApplicationRole> roles, SystemNotifications notification)
        {
            var users = new List<string>();
            foreach (var role in roles)
            {
                var list = _context.UserRoles.Where(x => x.RoleId.Equals(role.Id)).Select(x => x.UserId).ToHashSet();
                users.AddRange(list);
            }
            var usersUniques = users.ToHashSet().Select(Guid.Parse).ToList();
            await SendNotificationAsync(usersUniques, notification);
        }
        /// <inheritdoc />
        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="users"></param>
        /// <param name="notificationType"></param>
        /// <param name="subject"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public async Task SendNotificationAsync(IEnumerable<Guid> users, Guid notificationType, string subject,
            string content)
        {
            await SendNotificationAsync(users, new SystemNotifications
            {
                Content = content,
                Subject = subject,
                NotificationTypeId = notificationType
            });
        }
        /// <inheritdoc />
        /// <summary>
        /// Send notification to all users
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task SendNotificationAsync(SystemNotifications notification)
        {
            var users = _context.Users.Select(x => Guid.Parse(x.Id)).ToList();
            await SendNotificationAsync(users, notification);
        }

        /// <inheritdoc />
        /// <summary>
        /// Send Notification
        /// </summary>
        /// <param name="usersIds"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task SendNotificationAsync(IEnumerable<Guid> usersIds, SystemNotifications notification)
        {
            var users = usersIds.ToList();
            _hub.SentNotification(users, notification);
            foreach (var user in users)
            {
                notification.Id = Guid.NewGuid();
                notification.UserId = user;
                var response = await _dataService.AddSystem(notification);
                if (!response.IsSuccess)
                {
                    _logger.LogError("Fail to add new notification in database");
                }
            }
        }
        /// <inheritdoc />
        /// <summary>
        /// Send notification to system admins
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public async Task SendNotificationToSystemAdminsAsync(SystemNotifications notification)
        {
            var users = new List<Guid>();
            var roles = await _context.Roles.AsNoTracking().Where(x => x.IsNoEditable).ToListAsync();
            foreach (var role in roles)
            {
                var userRoles = await _context.UserRoles.AsNoTracking().Where(x => x.RoleId.Equals(role.Id)).ToListAsync();
                foreach (var userRole in userRoles)
                {
                    var receivers = await _context.Users.Where(x => x.Id.Equals(userRole.UserId))
                         .Select(x => Guid.Parse(x.Id)).ToListAsync();
                    receivers.ForEach(x =>
                    {
                        if (!users.Contains(x)) users.Add(x);
                    });
                }
            }

            await SendNotificationAsync(users, notification);
        }
        /// <inheritdoc />
        /// <summary>
        /// Get notifications by user id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<ResultModel<IEnumerable<SystemNotifications>>> GetNotificationsByUserIdAsync(Guid userId)
        {
            var notifications = await _dataService.GetAllSystem<SystemNotifications, SystemNotifications>();
            if (notifications.IsSuccess)
            {
                notifications.Result = notifications.Result.Where(x => x.UserId.Equals(userId)).OrderBy(x => x.Created);
            }
            return notifications;
        }
        /// <inheritdoc />
        /// <summary>
        /// Mark notification as read 
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public async Task<ResultModel<Guid>> MarkAsReadAsync(Guid notificationId)
        {
            if (notificationId == Guid.Empty)
            {
                return default;
            }
            var exists = await _dataService.Exists<SystemNotifications>(notificationId);
            if (!exists.IsSuccess) return default;
            var response = await _dataService.DeletePermanent<SystemNotifications>(notificationId);
            return response;
        }
        /// <inheritdoc />
        /// <summary>
        /// Check if user is online
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public bool IsUserOnline(Guid userId)
        {
            return _hub.IsUserOnline(userId);
        }
    }
}
