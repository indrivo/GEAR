using System.Linq;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
using GR.ECommerce.Payments.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Braintree.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register Braintree
        /// </summary>
        /// <typeparam name="TBraintreeProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterBraintreeProvider<TBraintreeProvider>(this IServiceCollection services)
            where TBraintreeProvider : class, IBraintreePaymentMethod
        {
            services.RegisterPaymentProvider(new PaymentProvider<TBraintreeProvider>
            {
                DisplayName = "Braintree",
                Description = "Integrating with Braintree offers your customers many different ways to pay. These payment methods can be added to your checkout. Each payment method differs in availability, configuration, and implementation."
            });
            services.AddTransient<IBraintreePaymentMethod, TBraintreeProvider>();
            SystemEvents.Database.OnSeed += async (sender, args) =>
            {
                if (!(args.DbContext is IPaymentContext)) return;
                var context = IoC.Resolve<IPaymentContext>();

                if (context.PaymentMethods.Any(x => x.Name == "Braintree")) return;
                var paymentMethod = new PaymentMethod
                {
                    Name = "Braintree",
                    IsEnabled = false
                };
                await context.PaymentMethods.AddAsync(paymentMethod);
                context.PushAsync().Wait();
            };

            return services;
        }
    }
}
