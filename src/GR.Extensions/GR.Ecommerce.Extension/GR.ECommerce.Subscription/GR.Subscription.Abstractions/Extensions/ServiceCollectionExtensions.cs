using System;
using Microsoft.Extensions.DependencyInjection;
using GR.Subscriptions.Abstractions.Models;
using GR.Core.Helpers;
using GR.Subscriptions.Abstractions.Events;
using GR.Subscriptions.Abstractions.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GR.Subscriptions.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register subscription services
        /// </summary>
        /// <typeparam name="TSubscription"></typeparam>
        /// <typeparam name="TSubscriptionService"></typeparam>
        /// <param name="services"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSubscriptionServices<TSubscription, TSubscriptionService>(this IServiceCollection services, Action<SubscriptionConfiguration> options = null)
            where TSubscriptionService : class, ISubscriptionService<TSubscription>
            where TSubscription : Subscription
        {
            IoC.RegisterTransientService<ISubscriptionService<TSubscription>, TSubscriptionService>();

            options?.Invoke(SubscriptionResources.Configuration);
            return services;
        }

        /// <summary>
        /// Register subscription storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSubscriptionStorage<TContext>(this IServiceCollection services)
            where TContext : DbContext, ISubscriptionDbContext
        {
            services.AddTransient<ISubscriptionDbContext, TContext>();
            IoC.RegisterService<ISubscriptionDbContext>(nameof(ISubscriptionDbContext), typeof(TContext));
            return services;
        }

        /// <summary>
        /// Register subscription events
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSubscriptionEvents(this IServiceCollection services)
        {
            SubscriptionEvents.RegisterEvents();
            return services;
        }

        /// <summary>
        /// On tenant create events
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSubscriptionOnTenantCreateEvents(this IServiceCollection services)
        {
            SubscriptionEvents.RegisterOnTenantCreateEvents();
            return services;
        }

        /// <summary>
        /// On tenant create events
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSubscriptionOnUserCreateEvents(this IServiceCollection services)
        {
            SubscriptionEvents.RegisterOnUserCreateEvents();
            return services;
        }

        /// <summary>
        /// Register rules for company subscriptions
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSubscriptionRules(this IServiceCollection services)
        {
            //Register limit users per company by subscription
            SubscriptionRules.RegisterLimitNumberOfUsersRule();
            return services;
        }
    }
}
