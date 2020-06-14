using System.ComponentModel.DataAnnotations;

namespace GR.Braintree.Abstractions.Models
{
    public class BraintreeConfiguration
    {
        [Required]
        public virtual bool IsProduction { get; set; } = false;
        [Required]
        public virtual string MerchantId { get; set; }
        [Required]
        public virtual string PublicKey { get; set; }
        [Required]
        public virtual string PrivateKey { get; set; }
    }
}
