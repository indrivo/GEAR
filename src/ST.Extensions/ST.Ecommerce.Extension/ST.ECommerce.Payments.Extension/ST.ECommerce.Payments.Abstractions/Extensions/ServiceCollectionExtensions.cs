using Microsoft.Extensions.DependencyInjection;
using ST.Core.Helpers;
using ST.ECommerce.Payments.Abstractions.Configurator;

namespace ST.ECommerce.Payments.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register
        /// </summary>
        /// <typeparam name="TProvider"></typeparam>
        /// <param name="services"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterPaymentProvider<TProvider>(this IServiceCollection services, PaymentProvider<TProvider> config)
            where TProvider : class, IPaymentManager
        {
            Arg.NotNull(services, nameof(RegisterPaymentProvider));
            Arg.NotNull(config, nameof(RegisterPaymentProvider));

            IoC.RegisterService<IPaymentManager>(config.ProviderName, typeof(TProvider));
            return services;
        }
    }
}
