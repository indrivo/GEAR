using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using GR.Core;
using GR.Core.Attributes.Documentation;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Filters;
using GR.Core.Helpers.Global;
using GR.Core.Helpers.Responses;
using GR.DynamicEntityStorage.Abstractions;
using GR.Entities.Abstractions.Enums;
using GR.Identity.Abstractions;
using GR.Notifications.Abstractions;
using GR.Notifications.Abstractions.Models.Notifications;
using GR.Notifications.Abstractions.ViewModels;
using GR.Notifications.Dynamic.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GR.Notifications.Dynamic
{
    [Author(Authors.LUPEI_NICOLAE)]
    public class NotifyWithDynamicEntities<TContext, TRole, TUser> : INotify<TRole> where TContext : IdentityDbContext<TUser, TRole, Guid> where TRole : IdentityRole<Guid> where TUser : IdentityUser<Guid>
    {
        #region Injectable

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
        private readonly ICommunicationHub _hub;
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<NotifyWithDynamicEntities<TContext, TRole, TUser>> _logger;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<GearUser> _userManager;

        /// <summary>
        /// Inject mapper
        /// </summary>
        private readonly IMapper _mapper;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataService"></param>
        /// <param name="context"></param>
        /// <param name="hub"></param>
        /// <param name="logger"></param>
        /// <param name="userManager"></param>
        /// <param name="mapper"></param>
        public NotifyWithDynamicEntities(IDynamicService dataService, TContext context, ICommunicationHub hub, ILogger<NotifyWithDynamicEntities<TContext, TRole, TUser>> logger, IUserManager<GearUser> userManager, IMapper mapper)
        {
            _dataService = dataService;
            _context = context;
            _hub = hub;
            _logger = logger;
            _userManager = userManager;
            _mapper = mapper;
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
            var usersUniques = usersRequest.Result.Select(x => x.Id).ToList();
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
            var users = _context.Users.Select(x => x.Id).ToList();
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
                foreach (var userId in users)
                {
                    var user = await _userManager.UserManager.FindByIdAsync(userId.ToString());
                    if (user == null) continue;
                    notification.Id = Guid.NewGuid();
                    notification.UserId = userId;
                    var tenant = await _userManager.IdentityContext.Tenants.FirstOrDefaultAsync(x => x.Id.Equals(user.TenantId));
                    var response = await _dataService.Add<SystemNotifications>(_dataService.GetDictionary(notification), tenant.MachineName);
                    if (!response.IsSuccess) _logger.LogError("Fail to add new notification in database");
                }

                var mapped = _mapper.Map<SystemNotifications>(notification);
                _hub.SendNotification(users, mapped);
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
            if (!(await _context.Roles.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Name.Equals(GlobalResources.Roles.ADMINISTRATOR)) is GearRole role)) return;
            var usersRequest = await _userManager.GetUsersInRolesAsync(new List<GearRole> { role });
            if (!usersRequest.IsSuccess) return;
            var users = usersRequest.Result.Select(x => x.Id).ToList();
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
                new Filter(nameof(SystemNotifications.UserId), userId)
            };

            if (onlyUnread) filters.Add(new Filter(nameof(BaseModel.IsDeleted), false));

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
        /// <param name="onlyUnread"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<PaginatedNotificationsViewModel>> GetUserNotificationsWithPaginationAsync(uint page = 1, uint perPage = 10, bool onlyUnread = true)
        {
            var userRequest = _userManager.FindUserIdInClaims();
            if (!userRequest.IsSuccess) return userRequest.Map<PaginatedNotificationsViewModel>();
            var userId = userRequest.Result;
            var filters = new List<Filter>
            {
                new Filter(nameof(SystemNotifications.UserId),userId)
            };

            if (onlyUnread) filters.Add(new Filter(nameof(BaseModel.IsDeleted), false));

            var sortableDirection = new Dictionary<string, EntityOrderDirection>
            {
                { nameof(SystemNotifications.Created), EntityOrderDirection.Desc }
            };

            var paginatedResult = await _dataService.GetPaginatedResultAsync<SystemNotifications>(page, perPage, null, filters, sortableDirection, false);
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
                Notifications = NotificationsDictionaryConvertor.Convert(paginatedResult.Result.ViewModel.Values)
            };

            return new SuccessResultModel<PaginatedNotificationsViewModel>(result);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get notification by id
        /// </summary>
        /// <param name="notificationId"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<Notification>> GetNotificationByIdAsync(Guid? notificationId)
        {
            if (notificationId == null) return new NotFoundResultModel<Notification>();
            return await _dataService.GetByIdWithReflection<SystemNotifications, Notification>(notificationId.Value);
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

        /// <summary>
        /// Send notification in background
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="notification"></param>
        /// <returns></returns>
        public virtual Task SendNotificationInBackgroundAsync(IEnumerable<Guid> userId, Notification notification)
        {
            GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async token =>
            {
                var notifier = IoC.Resolve<INotify<GearRole>>();
                await notifier.SendNotificationAsync(userId, notification);
            });
            return Task.CompletedTask;
        }
        
        /// <summary>
        /// Send notification
        /// </summary>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SendAsync(string subject, string message, string to)
        {
            await SendNotificationAsync(new List<Guid> { to.ToGuid() }, new Notification
            {
                Subject = subject,
                Content = message
            });
            return new ResultModel
            {
                IsSuccess = true
            };
        }

        /// <summary>
        /// Send user real time notification
        /// </summary>
        /// <typeparam name="TUser1"></typeparam>
        /// <param name="user"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task<ResultModel> SendAsync<TUser1>(TUser1 user, string subject, string message) where TUser1 : IdentityUser<Guid>
        {
            return await SendAsync(subject, message, user.Id.ToString());
        }

        /// <summary>
        /// Get provider
        /// </summary>
        /// <returns></returns>
        public virtual object GetProvider() => this;
    }
}
