using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using GR.Core.Helpers;
using GR.Core.Helpers.Filters;
using GR.Core.Helpers.Responses;
using GR.DynamicEntityStorage.Abstractions;
using GR.Email.Abstractions;
using GR.Entities.Abstractions.Enums;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Mappers;
using GR.Notifications.Abstractions.Models.Notifications;
using GR.Notifications.Abstractions.ViewModels;

namespace GR.Notifications.Services
{
    public class Notify<TContext, TRole, TUser> : INotify<TRole> where TContext : IdentityDbContext<TUser, TRole, string> where TRole : IdentityRole<string> where TUser : IdentityUser
    {
        /// <summary>
        /// Inject data service
        /// </summary>
        private readonly IDynamicService _dataService;
        /// <summary>
        /// Context
        /// </summary>
        private readonly TContext _context;
        /// <summary>
        /// Notification hub
        /// </summary>
        private readonly INotificationHub _hub;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<Notify<TContext, TRole, TUser>> _logger;

        /// <summary>
        /// Email sender
        /// </summary>
        private readonly IEmailSender _emailSender;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataService"></param>
        /// <param name="context"></param>
        /// <param name="hub"></param>
        /// <param name="logger"></param>
        /// <param name="emailSender"></param>
        /// <param name="userManager"></param>
        public Notify(IDynamicService dataService, TContext context, INotificationHub hub, ILogger<Notify<TContext, TRole, TUser>> logger, IEmailSender emailSender, IUserManager<GearUser> userManager)
        {
            _dataService = dataService;
            _context = context;
            _hub = hub;
            _logger = logger;
            _emailSender = emailSender;
            _userManager = userManager;
        }

        /// <inheritdoc />
        /// <summary>
        /// Send Notification
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="notification"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public virtual async Task SendNotificationAsync(IEnumerable<TRole> roles, Notification notification, Guid? tenantId = null)
        {
            var usersRequest = await _userManager.GetUsersInRolesAsync((IEnumerable<GearRole>)roles, tenantId);
            if (!usersRequest.IsSuccess) return;
            var usersUniques = usersRequest.Result.Select(x => Guid.Parse(x.Id)).ToList();
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
        public virtual async Task SendNotificationAsync(IEnumerable<Guid> users, Guid notificationType, string subject,
            string content)
        {
            await SendNotificationAsync(users, new Notification
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
        public virtual async Task SendNotificationAsync(Notification notification)
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
        public virtual async Task SendNotificationAsync(IEnumerable<Guid> usersIds, Notification notification)
        {
            if (notification == null) throw new NullReferenceException();
            try
            {
                var users = usersIds.ToList();
                if (!users.Any()) return;
                if (!notification.SendLocal && !notification.SendEmail) return;
                var emails = new HashSet<string>();
                foreach (var userId in users)
                {
                    var user = await _userManager.UserManager.FindByIdAsync(userId.ToString());
                    if (user == null) continue;
                    //send email only if email was confirmed
                    if (notification.SendEmail && user.EmailConfirmed) emails.Add(user.Email);

                    if (!notification.SendLocal) continue;
                    notification.Id = Guid.NewGuid();
                    notification.UserId = userId;
                    var tenant = await _userManager.IdentityContext.Tenants.FirstOrDefaultAsync(x => x.Id.Equals(user.TenantId));
                    var response = await _dataService.Add<SystemNotifications>(_dataService.GetDictionary(notification), tenant.MachineName);
                    if (!response.IsSuccess) _logger.LogError("Fail to add new notification in database");
                }
                if (notification.SendLocal) _hub.SendNotification(users, NotificationMapper.Map(notification));
                if (notification.SendEmail) await _emailSender.SendEmailAsync(emails, notification.Subject, notification.Content);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e.Message);
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Send notification to system admins
        /// </summary>
        /// <param name="notification"></param>
        /// <returns></returns>
        public virtual async Task SendNotificationToSystemAdminsAsync(Notification notification)
        {
            var users = new List<Guid>();
            var roles = await _context.Roles.AsNoTracking().ToListAsync();
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
        /// <param name="onlyUnread"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<SystemNotifications>>> GetNotificationsByUserIdAsync(Guid userId, bool onlyUnread = true)
        {
            var filters = new List<Filter>
            {
                new Filter(nameof(SystemNotifications.UserId), userId),
                new Filter(nameof(BaseModel.IsDeleted), !onlyUnread)
            };

            var notifications = await _dataService.GetAllWithInclude<SystemNotifications, SystemNotifications>(null, filters);
            if (notifications.IsSuccess)
            {
                notifications.Result = notifications.Result.OrderBy(x => x.Created);
            }

            return notifications;
        }

        /// <summary>
        /// Get notifications with pagination
        /// </summary>
        /// <param name="page"></param>
        /// <param name="perPage"></param>
        /// <param name="isDeleted"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<PaginatedNotificationsViewModel>> GetUserNotificationsWithPaginationAsync(uint page = 1, uint perPage = 10, bool isDeleted = false)
        {
            var userRequest = await _userManager.GetCurrentUserAsync();
            if (!userRequest.IsSuccess) return userRequest.Map<PaginatedNotificationsViewModel>();
            var user = userRequest.Result;
            var filters = new List<Filter>
            {
                new Filter(nameof(SystemNotifications.UserId),user.Id.ToGuid()),
                new Filter(nameof(BaseModel.IsDeleted), isDeleted)
            };

            var sortableDirection = new Dictionary<string, EntityOrderDirection>
            {
                { nameof(SystemNotifications.Created), EntityOrderDirection.Desc }
            };

            var paginatedResult = await _dataService.GetPaginatedResultAsync<SystemNotifications>(page, perPage, null, filters, sortableDirection);
            if (!paginatedResult.IsSuccess) return paginatedResult.Map(new PaginatedNotificationsViewModel
            {
                Page = page,
                PerPage = perPage,
                Total = 0
            });
            var result = new PaginatedNotificationsViewModel
            {
                PerPage = paginatedResult.Result.PerPage,
                Page = paginatedResult.Result.Page,
                Total = paginatedResult.Result.Total,
                Notifications = paginatedResult.Result.ViewModel.Values
            };

            return new SuccessResultModel<PaginatedNotificationsViewModel>(result);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get notification by id
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Dictionary<string, object>>> GetNotificationByIdAsync(Guid? notificationId)
        {
            if (notificationId == null) return new NotFoundResultModel<Dictionary<string, object>>();
            return await _dataService.GetById<SystemNotifications>(notificationId.Value);
        }

        /// <inheritdoc />
        /// <summary>
        /// Mark notification as read 
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Guid>> MarkAsReadAsync(Guid notificationId)
        {
            if (notificationId == Guid.Empty) return new NotFoundResultModel<Guid>();
            var exists = await _dataService.Exists<SystemNotifications>(notificationId);
            if (!exists.IsSuccess) return default;
            var response = await _dataService.Delete<SystemNotifications>(notificationId);
            return response;
        }


        /// <inheritdoc />
        /// <summary>
        /// Delete notification
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> PermanentlyDeleteNotificationAsync(Guid? notificationId)
        {
            if (notificationId == null) return new NotFoundResultModel();
            var exists = await _dataService.Exists<SystemNotifications>(notificationId.Value);
            if (!exists.IsSuccess) return default;
            var response = await _dataService.DeletePermanent<SystemNotifications>(notificationId.Value);
            return response.ToBase();
        }

        /// <summary>
        /// Clear all notifications
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> ClearAllUserNotificationsAsync(Guid userId)
        {
            if (userId == Guid.Empty) return new InvalidParametersResultModel();
            var notificationsRequest = await GetNotificationsByUserIdAsync(userId);
            if (!notificationsRequest.IsSuccess) return notificationsRequest.ToBase();
            var notifications = notificationsRequest.Result.ToList();
            var fails = 0;
            foreach (var notification in notifications)
            {
                var response = await _dataService.Delete<SystemNotifications>(notification.Id);
                if (!response.IsSuccess) fails++;
            }
            if (fails == 0) return new SuccessResultModel<object>().ToBase();
            return new ResultModel
            {
                Errors = new List<IErrorModel>
                {
                    new ErrorModel(string.Empty, "Some notifications could not be deleted")
                }
            };
        }

        /// <inheritdoc />
        /// <summary>
        /// Check if user is online
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual bool IsUserOnline(Guid userId) => userId != Guid.Empty && _hub.GetUserOnlineStatus(userId);
    }
}
