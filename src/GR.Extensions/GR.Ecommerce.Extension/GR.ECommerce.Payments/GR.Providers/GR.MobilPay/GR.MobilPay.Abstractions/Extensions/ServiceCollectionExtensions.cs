using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
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
            services.RegisterPaymentProvider(new PaymentProvider<IMobilPayPaymentMethod, TMobilPayProvider>
            {
                Id = "MobilPay",
                DisplayName = "MobilPay",
                Description = "mobilPay was developed for companies to facilitate payments between merchants and their customers. I understand that these transactions processed by a third party have their own needs and we have developed the mobilePay platform keeping in mind your needs."
            });
            services.AddTransient<IMobilPayPaymentMethod, TMobilPayProvider>();

            return services;
        }
    }
}
