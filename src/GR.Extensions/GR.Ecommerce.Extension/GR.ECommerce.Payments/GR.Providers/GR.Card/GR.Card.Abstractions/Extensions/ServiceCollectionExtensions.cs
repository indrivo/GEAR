using GR.Card.Abstractions.Helpers;
using GR.ECommerce.Payments.Abstractions.Configurator;
using GR.ECommerce.Payments.Abstractions.Extensions;
using Microsoft.Extensions.DependencyInjection;

namespace GR.Card.Abstractions.Extensions
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Register card provider
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection RegisterCreditCardPayProvider<TProvider>(this IServiceCollection services)
            where TProvider : class, ICardPayPaymentMethodService
        {
            services.RegisterPaymentProvider(new PaymentProvider<ICardPayPaymentMethodService, TProvider>
            {
                Id = CreditCardResources.CreditCardProvider,
                DisplayName = CreditCardResources.CreditCardProvider,
                Description = "A credit card is a payment card issued to users (cardholders) to enable the cardholder " +
                              "to pay a merchant for goods and services based on the cardholder's promise to the card issuer to pay them for the amounts plus the other agreed charges."
            });

            return services;
        }
    }
}