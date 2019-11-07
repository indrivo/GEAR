using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using GR.Subscriptions.Abstractions.Models;
using GR.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GR.Subscriptions.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {

        /// <summary>
        /// Register subscription services
        /// </summary>
        /// <typeparam name="TSubsctiption"></typeparam>
        /// <typeparam name="TSubscriptionService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterSubscriptionServices<TSubsctiption, TSubscriptionService>(this IServiceCollection services)
            where TSubscriptionService : class, ISubscriptionService<TSubsctiption>
            where TSubsctiption : Subscription
        {
            IoC.RegisterTransientService<ISubscriptionService<TSubsctiption>, TSubscriptionService>();

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
    }
}
