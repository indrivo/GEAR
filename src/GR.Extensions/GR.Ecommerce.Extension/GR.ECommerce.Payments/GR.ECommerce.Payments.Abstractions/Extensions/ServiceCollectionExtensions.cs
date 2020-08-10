using System.Diagnostics;
using GR.Core;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Helpers;
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
            services.AddGearScoped<IPaymentService, TService>();
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
            services.AddScoped<IPaymentContext, TContext>();
            return services;
        }

        /// <summary>
        /// Register
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <typeparam name="TProviderAbstraction"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPaymentProvider<TProviderAbstraction, TProvider>(this IServiceCollection services, PaymentProvider<TProviderAbstraction, TProvider> config)
            where TProviderAbstraction : class, IPaymentMethodService
            where TProvider : class, TProviderAbstraction
        {
            Arg.NotNull(services, nameof(RegisterPaymentProvider));
            Arg.NotNull(config, nameof(RegisterPaymentProvider));

            IoC.RegisterService<IPaymentMethodService>($"payment_method_{config.Id}", typeof(TProvider));
            services.AddGearScoped<TProviderAbstraction, TProvider>();
            PaymentProviders.Register(config.Id, new PaymentProvider
            {
                Id = config.Id,
                ProviderName = config.ProviderName,
                Description = config.Description,
                DisplayName = config.DisplayName
            });

            if (GearApplication.IsDevelopment() && Debugger.IsAttached)
            {
                SystemEvents.Application.OnApplicationStarted += (sender, args) =>
                   {
                       PaymentProviders.SeedProviderAsync(config.Id).Wait();
                   };
            }

            SystemEvents.Database.OnSeed += (sender, args) =>
            {
                if (!(args.DbContext is IPaymentContext)) return;
                PaymentProviders.SeedProviderAsync(config.Id).Wait();
            };
            return services;
        }
    }
}
