namespace GR.ECommerce.Payments.Abstractions.Configurator
{
    public class PaymentProvider<TProvider> where TProvider : class, IPaymentService
    {
        /// <summary>
        /// Provider name
        /// </summary>
        public virtual string ProviderName => typeof(TProvider).Name;
    }
}
