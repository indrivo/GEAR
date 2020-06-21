using System;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.ECommerce.Abstractions;
using GR.ECommerce.Abstractions.Helpers;
using GR.ECommerce.Abstractions.Models;
using GR.Identity.Abstractions.Events;
using GR.MultiTenant.Abstractions.Events;
using GR.Orders.Abstractions;
using GR.Orders.Abstractions.Events;
using GR.Orders.Abstractions.Models;
using GR.Subscriptions.Abstractions.Events.EventArgs;
using GR.Subscriptions.Abstractions.Helpers;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;

namespace GR.Subscriptions.Abstractions.Events
{
    public static class SubscriptionEvents
    {
        public struct Subscriptions
        {
            #region Events

            /// <summary>
            /// On subscription upgrade
            /// </summary>
            public static event EventHandler<UpgradeSubscriptionEventArgs> OnSubscriptionUpgrade;

            /// <summary>
            /// On subscription seed
            /// </summary>
            public static event EventHandler<SeedSubscriptionEventArgs> OnSubscriptionSeed;

            #endregion

            #region Triggers

            /// <summary>
            /// Trigger update subscription
            /// </summary>
            /// <param name="e"></param>
            public static void TriggerSubscriptionUpgrade(UpgradeSubscriptionEventArgs e)
                => SystemEvents.InvokeEvent(null, OnSubscriptionUpgrade, e, nameof(OnSubscriptionUpgrade));

            /// <summary>
            /// Trigger seed
            /// </summary>
            /// <param name="e"></param>
            public static void TriggerSubscriptionSeed(SeedSubscriptionEventArgs e)
                => SystemEvents.InvokeEvent(null, OnSubscriptionSeed, e, nameof(OnSubscriptionSeed));

            #endregion
        }

        /// <summary>
        /// Register events
        /// </summary>
        public static void RegisterEvents()
        {
            SystemEvents.Common.RegisterEventGroup(nameof(Subscriptions), SystemEvents.GetEvents(typeof(Subscriptions)));

            //Seed product type
            Subscriptions.OnSubscriptionSeed += async (sender, args) =>
            {
                var productService = IoC.Resolve<IProductService<Product>>();

                //add subscription product type
                await productService.AddProductTypeAsync(new ProductType
                {
                    Id = SubscriptionResources.SubscriptionPlanProductType,
                    DisplayName = nameof(Subscription),
                    Name = nameof(Subscription)
                });

                //add brand subscription
                await productService.AddBrandAsync(new Brand
                {
                    Id = SubscriptionResources.SubscriptionBrand,
                    Name = nameof(Subscription),
                    DisplayName = nameof(Subscription)
                });

                //add subscription as product
                await productService.AddProductAsync(new Product
                {
                    Id = SubscriptionResources.DefaultSubscriptionPlan,
                    Name = "Free",
                    DisplayName = "Free",
                    IsPublished = false,
                    ProductTypeId = SubscriptionResources.SubscriptionPlanProductType,
                    BrandId = SubscriptionResources.SubscriptionBrand
                });
            };

            //Receive payment and change subscription
            OrderEvents.Orders.OnPaymentReceived += (sender, args) =>
            {
                GearApplication.BackgroundTaskQueue.PushBackgroundWorkItemInQueue(async cancelToken =>
                    {
                        await GearPolicy.ExecuteAndRetry(async () => await UpdateUserSubscriptionAsync(args.OrderId, args.UserId));
                    });
            };
        }

        /// <summary>
        /// Update user subscription
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private static async Task<ResultModel> UpdateUserSubscriptionAsync(Guid orderId, Guid userId)
        {
            var orderService = IoC.Resolve<IOrderProductService<Order>>();
            var subscriptionService = IoC.Resolve<ISubscriptionService<Subscription>>();
            var productService = IoC.Resolve<IProductService<Product>>();

            var orderRequest = await orderService.GetOrderByIdAsync(orderId);
            if (orderRequest.IsSuccess.Negate()) return orderRequest.ToBase();
            var order = orderRequest.Result;
            var checkIfProductIsSubscription = order.ProductOrders
                .FirstOrDefault(x => x.Product?.ProductTypeId == SubscriptionResources.SubscriptionPlanProductType)?.ProductId;
            if (checkIfProductIsSubscription == null) return new NotFoundResultModel();
            var planRequest = await productService.GetProductByIdAsync(checkIfProductIsSubscription);
            if (planRequest.IsSuccess.Negate()) return planRequest.ToBase();
            var plan = planRequest.Result;
            var permissions = SubscriptionMapper.Map(plan.ProductAttributes).ToList();
            var variationId = order.ProductOrders.FirstOrDefault(x => x.ProductId == checkIfProductIsSubscription)?.ProductVariationId;
            var variation = plan.ProductVariations.FirstOrDefault(x => x.Id.Equals(variationId));

            var userSubscriptionRequest = await subscriptionService.GetLastSubscriptionForUserAsync(userId);

            var newSubscription = new SubscriptionAddViewModel
            {
                Name = plan.Name,
                OrderId = orderId,
                StartDate = DateTime.Now,
                Availability = subscriptionService.GetSubscriptionDuration(variation),
                UserId = order.UserId,
                SubscriptionPermissions = permissions,
            };

            if (userSubscriptionRequest.IsSuccess)
            {
                var userSubscription = userSubscriptionRequest.Result;
                newSubscription.Id = userSubscription.Id;
                newSubscription.Availability += userSubscription.Availability;
                newSubscription.IsFree = false;
            }

            return (await subscriptionService.AddOrUpdateSubscriptionAsync(newSubscription)).ToBase();
        }

        /// <summary>
        /// On add new user, create subscription
        /// </summary>
        public static void RegisterOnUserCreateEvents()
        {
            IdentityEvents.Users.OnUserCreated += async (sender, args) =>
            {
                var productService = IoC.Resolve<IProductService<Product>>();
                var subscriptionService = IoC.Resolve<ISubscriptionService<Subscription>>();

                var planRequest = await productService.GetProductByIdAsync(SubscriptionResources.DefaultSubscriptionPlan);

                if (!planRequest.IsSuccess) return;
                var plan = planRequest.Result;
                var permissions = SubscriptionMapper.Map(plan.ProductAttributes).ToList();
                await subscriptionService.AddOrUpdateSubscriptionAsync(new SubscriptionAddViewModel
                {
                    Name = plan.DisplayName,
                    StartDate = DateTime.Now,
                    Availability = 0,
                    UserId = args.UserId,
                    IsFree = true,
                    SubscriptionPermissions = permissions
                });
            };
        }

        /// <summary>
        /// On add new tenant, add default free subscription 
        /// </summary>
        public static void RegisterOnTenantCreateEvents()
        {
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
                    await subscriptionService.AddOrUpdateSubscriptionAsync(new SubscriptionAddViewModel
                    {
                        Name = "Free trial",
                        StartDate = DateTime.Now,
                        Availability = int.Parse(freeTrialPeriodStr),
                        UserId = args.UserId,
                        IsFree = true,
                        SubscriptionPermissions = permissions
                    });
                }
            };
        }
    }
}
