using System;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Services;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.Subscriptions.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GR.Subscriptions.BackgroundServices
{
    public class SubscriptionValidationBackgroundService : BaseBackgroundService<SubscriptionValidationBackgroundService>
    {
        #region Injectable

        /// <summary>
        /// Inject service provider
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        #endregion
        public SubscriptionValidationBackgroundService(ILogger<SubscriptionValidationBackgroundService> logger, IServiceProvider serviceProvider)
            : base("Subscription Validation", logger)
        {
            _serviceProvider = serviceProvider;
            Interval = TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Execute
        /// </summary>
        /// <param name="state"></param>
        public override async Task Execute(object state)
        {
            if (!GearApplication.Configured) return;
            using (var scope = _serviceProvider.CreateScope())
            {
                var subscriptionService = scope.ServiceProvider.GetRequiredService<ISubscriptionExpirationService>();
                var productService = scope.ServiceProvider.GetRequiredService<IProductService<Product>>();
                var daysNotifySubscription = (await productService.GetSettingAsync<string>(CommerceResources.SettingsParameters.DAYS_NOTIFY_SUBSCRIPTION_EXPIRATION)).Result ?? "0";
                var expiredRequest = await subscriptionService.GetExpiredSubscriptionsAsync();
                if (expiredRequest.IsSuccess)
                    await subscriptionService.NotifyAndRemoveExpiredSubscriptionsAsync(expiredRequest.Result.ToList());

                var willExpireRequest = await subscriptionService.GetSubscriptionsThatExpireInAsync(TimeSpan.FromDays(Convert.ToInt32(daysNotifySubscription)));
                if (willExpireRequest.IsSuccess)
                    await subscriptionService.NotifySubscriptionsThatExpireAsync(willExpireRequest.Result.ToList());
            }
        }
    }
}