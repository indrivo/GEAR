namespace GR.ECommerce.Payments.Abstractions.Configurator
{
    // ReSharper disable once UnusedTypeParameter
    public class PaymentProvider<TProviderAbstraction, TProvider> : PaymentProvider
        where TProvider : class, IPaymentMethodService
        where TProviderAbstraction : IPaymentMethodService
    {
        /// <summary>
        /// Provider name
        /// </summary>
        public override string ProviderName => typeof(TProvider).Name;
    }

    public class PaymentProvider
    {
        /// <summary>
        /// Provider name
        /// </summary>
        public virtual string ProviderName { get; set; }

        /// <summary>
        /// Payment method id
        /// </summary>
        public virtual string Id { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public virtual string DisplayName { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }
    }
}