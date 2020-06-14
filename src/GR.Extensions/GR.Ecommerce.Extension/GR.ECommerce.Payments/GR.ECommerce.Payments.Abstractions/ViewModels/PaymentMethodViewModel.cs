using GR.ECommerce.Payments.Abstractions.Models;

namespace GR.ECommerce.Payments.Abstractions.ViewModels
{
    public class PaymentMethodViewModel: PaymentMethod
    {
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