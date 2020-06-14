using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.ApplePay.Abstractions.ViewModels
{
    public class ApplePaySettingsViewModel
    {
        [Required]
        public virtual string StoreName { get; set; }
        public virtual IEnumerable<string> AcceptedCardSchemes { get; set; }
        public virtual IEnumerable<string> MerchantCapabilities { get; set; }
        public virtual IEnumerable<string> RequiredBillingContactFields { get; set; }
        public virtual IEnumerable<string> RequiredShippingContactFields { get; set; }
        [Range(1, 6)]
        [Required]
        public virtual int ApplePayVersion { get; set; }
        public virtual bool UseCertificateStore { get; set; }
        public virtual string MerchantCertificateFileName { get; set; }
        public virtual string MerchantCertificatePassword { get; set; }
        public virtual string MerchantCertificateThumbprint { get; set; }
        public virtual bool UsePolyfill { get; set; }
    }
}