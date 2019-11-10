using System.ComponentModel.DataAnnotations;

namespace GR.ECommerce.Payments.Abstractions.Models
{
    public class PaymentMethod
    {
        /// <summary>
        /// Provider name
        /// </summary>
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public virtual string Name { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public virtual bool IsEnabled { get; set; }
    }
}
