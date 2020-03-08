using System.ComponentModel.DataAnnotations;
using GR.Core.Attributes;

namespace GR.MobilPay.Abstractions.Models
{
    public class MobilPayConfiguration
    {
        /// <summary>
        /// Is testing mode
        /// </summary>
        public virtual bool IsSandbox { get; set; }

        /// <summary>
        /// Service url
        /// </summary>
        public virtual string MobilPayUrl => IsSandbox ? "http://sandboxsecure.mobilpay.ro" : "https://secure.mobilpay.ro/";

        /// <summary>
        /// Signature
        /// </summary>
        [Required]
        [DisplayTranslate(Key = "system_signature")]
        public virtual string Signature { get; set; }

        /// <summary>
        /// path to private key
        /// </summary>
        public virtual string PathToPrivateKey { get; set; }

        /// <summary>
        /// Path to public key
        /// </summary>
        public virtual string PathToCertificate { get; set; }
    }
}
