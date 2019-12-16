using System;
using System.Linq;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.Identity.Abstractions;
using GR.MultiTenant.Abstractions.Events;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Events;
using GR.Orders.Abstractions.Models;
using GR.Subscriptions.Abstractions.Helpers;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;

namespace GR.Subscriptions.Abstractions.Events
{
    public static class SubscriptionEvents
    {
        public struct Subscriptions
        {

        }

        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            SystemEvents.Common.RegisterEventGroup(nameof(Subscriptions), SystemEvents.GetEvents(typeof(Subscriptions)));

            TenantEvents.Company.OnCompanyRegistered += async (sender, args) =>
            {
                var productService = IoC.Resolve<IProductService<Product>>();
                var subscriptionService = IoC.Resolve<ISubscriptionService<Subscription>>();
                var freeTrialPeriodStr = (await productService.GetSettingAsync<string>(CommerceResources.SettingsParameters.FREE_TRIAL_PERIOD_DAYS)).Result ?? "15";

                var planRequest = await productService.GetProductByAttributeMinNumberValueAsync("Number of users");

                if (planRequest.IsSuccess)
                {
                    var plan = planRequest.Result;

                    var permissions = SubscriptionMapper.Map(plan.ProductAttributes).ToList();
                    var result =  await subscriptionService.CreateUpdateSubscriptionAsync(new SubscriptionViewModel
                    {
                        Name = "Free trial",
                        StartDate = DateTime.Now,
                        Availability = int.Parse(freeTrialPeriodStr), 
                        UserId = args.UserId.ToGuid(),
                        IsFree = true,
                        SubscriptionPermissions = permissions
                    });
                }
            };

            OrderEvents.Orders.OnPaymentReceived += async (sender, args) =>
            {
                var orderService = IoC.Resolve<IOrderProductService<Order>>();
                var subscriptionService = IoC.Resolve<ISubscriptionService<Subscription>>();
                var productService = IoC.Resolve<IProductService<Product>>();

                var orderRequest = await orderService.GetOrderByIdAsync(args.OrderId);
                if (orderRequest.IsSuccess.Negate()) return;
                var order = orderRequest.Result;
                var checkIfProductIsSubscription = order.ProductOrders
                    .FirstOrDefault(x => x.Product?.ProductTypeId == ProductTypes.SubscriptionPlan)?.ProductId;
                if (checkIfProductIsSubscription == null) return;
                var planRequest = await productService.GetProductByIdAsync(checkIfProductIsSubscription);
                if (planRequest.IsSuccess.Negate()) return;
                var plan = planRequest.Result;
                var permissions = SubscriptionMapper.Map(plan.ProductAttributes).ToList();
                var variationId = order.ProductOrders.FirstOrDefault(x => x.ProductId == checkIfProductIsSubscription)?.ProductVariationId;
                var variation = plan.ProductVariations.FirstOrDefault(x => x.Id.Equals(variationId));


                var userSubscription = await subscriptionService.GetLastSubscriptionForUserAsync();

                var newSubscription = new SubscriptionViewModel
                {
                    Name = plan.Name,
                    OrderId = args.OrderId,
                    StartDate = DateTime.Now,
                    Availability = subscriptionService.GetSubscriptionDuration(variation),
                    UserId = order.UserId,
                    SubscriptionPermissions = permissions,
                };

                if (userSubscription.IsSuccess)
                {
                    newSubscription.Id = userSubscription.Result.Id;
                    newSubscription.Availability += userSubscription.Result.Availability;
                    newSubscription.IsFree = false;
                }

                await subscriptionService.CreateUpdateSubscriptionAsync(newSubscription);
            };
        }
    }
}
