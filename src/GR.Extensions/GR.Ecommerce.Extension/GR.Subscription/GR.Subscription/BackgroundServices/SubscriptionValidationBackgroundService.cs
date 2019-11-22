using System;
using System.Linq;
using GR.Core.Services;
using GR.Subscriptions.Abstractions;
using GR.Subscriptions.Abstractions.Models;
using Microsoft.Extensions.Logging;

namespace GR.Subscriptions.BackgroundServices
{
    public class SubscriptionValidationBackgroundService : BaseBackgroundService<SubscriptionValidationBackgroundService>
    {
        #region Injectable

        /// <summary>
        /// Inject subscription service
        /// </summary>
        private readonly ISubscriptionService<Subscription> _subscriptionService;

        #endregion
        public SubscriptionValidationBackgroundService(ILogger<SubscriptionValidationBackgroundService> logger, ISubscriptionService<Subscription> subscriptionService)
            : base("Subscription Validation", logger)
        {
            _subscriptionService = subscriptionService;
            Interval = TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="state"></param>
        public override async void Execute(object state)
        {
            var expiredRequest = await _subscriptionService.GetExpiredSubscriptionsAsync();
            if (expiredRequest.IsSuccess)
                await _subscriptionService.NotifyAndRemoveExpiredSubscriptionsAsync(expiredRequest.Result.ToList());

            var willExpireRequest = await _subscriptionService.GetSubscriptionsThatExpireInAsync(TimeSpan.FromDays(4));
            if (willExpireRequest.IsSuccess)
                await _subscriptionService.NotifySubscriptionsThatExpireAsync(willExpireRequest.Result.ToList());
        }
    }
}
