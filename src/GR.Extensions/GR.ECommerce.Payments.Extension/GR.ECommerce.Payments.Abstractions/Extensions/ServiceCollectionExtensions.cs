using GR.Core.Helpers;
using GR.ECommerce.Payments.Abstractions.Configurator;
using Microsoft.Extensions.DependencyInjection;

namespace GR.ECommerce.Payments.Abstractions.Extensions
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
            where TProvider : class, IPaymentService
        {
            Arg.NotNull(services, nameof(RegisterPaymentProvider));
            Arg.NotNull(config, nameof(RegisterPaymentProvider));

            IoC.RegisterService<IPaymentService>(config.ProviderName, typeof(TProvider));

            return services;
        }
    }
}
