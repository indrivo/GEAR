using System;
using System.Linq;
using GR.Core.Services;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
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

        /// <summary>
        /// Inject product service
        /// </summary>
        private readonly IProductService<Product> _productService;

        #endregion
        public SubscriptionValidationBackgroundService(ILogger<SubscriptionValidationBackgroundService> logger, ISubscriptionService<Subscription> subscriptionService, IProductService<Product> productService)
            : base("Subscription Validation", logger)
        {
            _subscriptionService = subscriptionService;
            _productService = productService;
            Interval = TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="state"></param>
        public override async void Execute(object state)
        {
            var daysNotifySubscription = (await _productService.GetSettingAsync<string>(CommerceResources.SettingsParameters.DAYS_NOTIFY_SUBSCRIPTION_EXPIRATION)).Result ?? "0";
            var expiredRequest = await _subscriptionService.GetExpiredSubscriptionsAsync();
            if (expiredRequest.IsSuccess)
                await _subscriptionService.NotifyAndRemoveExpiredSubscriptionsAsync(expiredRequest.Result.ToList());

            var willExpireRequest = await _subscriptionService.GetSubscriptionsThatExpireInAsync(TimeSpan.FromDays(Convert.ToInt32(daysNotifySubscription)));
            if (willExpireRequest.IsSuccess)
                await _subscriptionService.NotifySubscriptionsThatExpireAsync(willExpireRequest.Result.ToList());
        }
    }
}
