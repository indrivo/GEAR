namespace ST.ECommerce.Payments.Abstractions.Configurator
{
    public class PaymentProvider<TProvider> where TProvider : class, IPaymentManager
    {
        /// <summary>
        /// Provider name
        /// </summary>
        public virtual string ProviderName => typeof(TProvider).Name;
    }
}
