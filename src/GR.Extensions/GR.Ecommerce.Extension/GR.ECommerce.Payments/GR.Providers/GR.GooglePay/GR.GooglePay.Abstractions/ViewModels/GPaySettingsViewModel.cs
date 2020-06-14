using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.GooglePay.Abstractions.ViewModels
{
    public class GPaySettingsViewModel
    {
        [Required]
        public virtual string MerchantId { get; set; }

        [Required]
        public virtual string MerchantName { get; set; }
        [Required]
        public virtual int ApiVersion { get; set; }
        public virtual bool IsSandbox { get; set; } = true;
        public virtual IEnumerable<string> AllowedCardNetworks { get; set; }
        public virtual IEnumerable<string> AllowedCardAuthMethods { get; set; }
        public virtual string Environment => IsSandbox ? "TEST" : "PRODUCTION";
    }
}
