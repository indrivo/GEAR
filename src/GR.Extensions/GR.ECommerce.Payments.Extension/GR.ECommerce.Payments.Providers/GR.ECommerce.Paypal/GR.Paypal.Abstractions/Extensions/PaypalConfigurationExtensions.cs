using GR.Core.Events;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
using GR.ECommerce.Payments.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using GR.Core.Extensions;

namespace GR.Paypal.Abstractions.Extensions
{
    public static class PaypalConfigurationExtensions
    {
        /// <summary>
        /// Register paypal provider
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPaypalProvider<TPayPalProvider>(this IServiceCollection services)
            where TPayPalProvider : class, IPaypalPaymentMethodService
        {
            services.RegisterPaymentProvider(new PaymentProvider<TPayPalProvider>
            {
                DisplayName = "Paypal",
                Description = "Paypal is an American company operating a worldwide online payments"
            });

            SystemEvents.Database.OnSeed += async (sender, args) =>
            {
                if (!(args.DbContext is IPaymentContext)) return;
                var context = IoC.Resolve<IPaymentContext>();

                if (context.PaymentMethods.Any(x => x.Name == "Paypal")) return;
                var paymentMethod = new PaymentMethod
                {
                    Name = "Paypal",
                    IsEnabled = false
                };
                await context.PaymentMethods.AddAsync(paymentMethod);
                await context.PushAsync();
            };

            return services;
        }
    }
}