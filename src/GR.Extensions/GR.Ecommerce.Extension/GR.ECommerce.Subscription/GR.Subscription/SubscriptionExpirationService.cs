using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Abstractions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.Identity.Abstractions;
using GR.Subscriptions.Abstractions;
using GR.Subscriptions.Abstractions.Events;
using GR.Subscriptions.Abstractions.Events.EventArgs;
using GR.Subscriptions.Abstractions.Helpers;
using GR.Subscriptions.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.Subscriptions
{
    public class SubscriptionExpirationService : ISubscriptionExpirationService
    {
        #region Injectable

        /// <summary>
        /// Inject subscription db context
        /// </summary>
        protected readonly ISubscriptionDbContext SubscriptionDbContext;

        /// <summary>
        /// Inject subscription service
        /// </summary>
        protected readonly ISubscriptionService<Subscription> SubscriptionService;

        /// <summary>
        /// Inject notifier
        /// </summary>
        protected readonly IAppSender Sender;

        /// <summary>
        /// Inject user manager
        /// </summary>
        protected readonly IUserManager<GearUser> UserManager;

        /// <summary>
        /// Config
        /// </summary>
        protected readonly SubscriptionConfiguration SubscriptionConfiguration;

        #endregion

        public SubscriptionExpirationService(ISubscriptionDbContext subscriptionDbContext, ISubscriptionService<Subscription> subscriptionService, IAppSender sender, IUserManager<GearUser> userManager, SubscriptionConfiguration subscriptionConfiguration)
        {
            SubscriptionDbContext = subscriptionDbContext;
            SubscriptionService = subscriptionService;
            Sender = sender;
            UserManager = userManager;
            SubscriptionConfiguration = subscriptionConfiguration;
        }

        /// <summary>
        /// Get subscriptions that will expire after some period
        /// </summary>
        /// <param name="timeSpan"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Subscription>>> GetSubscriptionsThatExpireInAsync(TimeSpan timeSpan)
        {
            var days = timeSpan.TotalDays;
            var data = await SubscriptionDbContext.Subscription
                .AsNoTracking()
                .Where(x => !x.IsFree && (x.StartDate.AddDays(x.Availability) > DateTime.Now || x.IsFree))
                .ToListAsync();

            return new SuccessResultModel<IEnumerable<Subscription>>(data.Where(x => (x.StartDate.AddDays(x.Availability) - DateTime.Now).Days < days).ToList());
        }

        /// <summary>
        /// Get expired subscriptions
        /// </summary>
        /// <returns></returns>
        public virtual async Task<ResultModel<IEnumerable<Subscription>>> GetExpiredSubscriptionsAsync()
        {
            var data = await SubscriptionDbContext.Subscription
                .AsNoTracking()
                .Where(x => !(x.StartDate.AddDays(x.Availability) > DateTime.Now || x.IsFree)).ToListAsync();
            return new SuccessResultModel<IEnumerable<Subscription>>(data);
        }

        /// <summary>
        /// Notify expired subscriptions
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> NotifyAndRemoveExpiredSubscriptionsAsync([Required] IList<Subscription> subscriptions)
        {
            if (subscriptions == null) return new NullReferenceResultModel<object>().ToBase();
            var removeRequest = await SubscriptionService.RemoveRangeAsync(subscriptions);
            if (!removeRequest.IsSuccess) return removeRequest;
            foreach (var subscription in subscriptions)
            {
                var userReq = await UserManager.FindUserByIdAsync(subscription.UserId);
                if (!userReq.IsSuccess) continue;
                var message = $"Subscription {subscription.Name}, valid from {subscription.StartDate} " +
                              $"to {subscription.ExpirationDate}, has expired";
                await Sender.SendAsync(userReq.Result, $"Subscription {subscription.Name} expired", message, SubscriptionConfiguration.NotificationProviders.ToArray());
                SubscriptionEvents.Subscriptions.TriggerSubscriptionChange(new ChangeSubscriptionEventArgs
                {
                    SubscriptionId = subscription.Id,
                    UserId = subscription.UserId
                });
            }

            return removeRequest;
        }

        /// <summary>
        /// Notify subscriptions that will expire
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <returns></returns>
        public virtual async Task NotifySubscriptionsThatExpireAsync([Required] IList<Subscription> subscriptions)
        {
            if (subscriptions == null) throw new NullReferenceException();
            foreach (var subscription in subscriptions)
            {
                var userReq = await UserManager.FindUserByIdAsync(subscription.UserId);
                if (!userReq.IsSuccess) continue;
                var message = $"Subscription {subscription.Name}, valid from {subscription.StartDate} " +
                              $"to {subscription.ExpirationDate}, expires in {subscription.RemainingDays} days";
                await Sender.SendAsync(userReq.Result, "The subscription expires soon", message, SubscriptionConfiguration.NotificationProviders.ToArray());
            }
        }
    }
}
