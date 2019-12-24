using System.Linq;
using GR.Core.Events;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions;
using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
using GR.ECommerce.Payments.Abstractions.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GR.MobilPay.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register MobilPay
        /// </summary>
        /// <typeparam name="TMobilPayProvider"></typeparam>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterMobilPayProvider<TMobilPayProvider>(this IServiceCollection services)
            where TMobilPayProvider : class, IMobilPayPaymentMethod
        {
            services.RegisterPaymentProvider(new PaymentProvider<TMobilPayProvider>
            {
                DisplayName = "MobilPay",
                Description = "mobilPay was developed for companies to facilitate payments between merchants and their customers. I understand that these transactions processed by a third party have their own needs and we have developed the mobilePay platform keeping in mind your needs."
            });
            services.AddTransient<IMobilPayPaymentMethod, TMobilPayProvider>();
            SystemEvents.Database.OnSeed += async (sender, args) =>
             {
                 if (!(args.DbContext is IPaymentContext)) return;
                 var context = IoC.Resolve<IPaymentContext>();

                 if (context.PaymentMethods.Any(x => x.Name == "MobilPay")) return;
                 var paymentMethod = new PaymentMethod
                 {
                     Name = "MobilPay",
                     IsEnabled = false
                 };
                 await context.PaymentMethods.AddAsync(paymentMethod);
                 context.PushAsync().Wait();
             };

            return services;
        }
    }
}
