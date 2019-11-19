using System;
using System.Collections.Generic;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions.Configurator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GR.ECommerce.Payments.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register payments
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPayments<TService>(this IServiceCollection services)
            where TService : class, IPaymentService
        {
            IoC.RegisterServiceCollection(new Dictionary<Type, Type>
            {
                { typeof(IPaymentService), typeof(TService) }
            });
            return services;
        }

        /// <summary>
        /// Register payment storage
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPaymentStorage<TContext>(this IServiceCollection services)
            where TContext : DbContext, IPaymentContext
        {
            IoC.RegisterService<IPaymentContext>(nameof(IPaymentContext), typeof(TContext));
            return services;
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPaymentProvider<TProvider>(this IServiceCollection services, PaymentProvider<TProvider> config)
            where TProvider : class, IPaymentMethodService
        {
            Arg.NotNull(services, nameof(RegisterPaymentProvider));
            Arg.NotNull(config, nameof(RegisterPaymentProvider));

            IoC.RegisterService<IPaymentMethodService>(config.ProviderName, typeof(TProvider));
            return services;
        }
    }
}
