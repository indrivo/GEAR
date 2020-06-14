using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.ApplePay.Abstractions.Helpers;
using GR.ApplePay.Abstractions.ViewModels;
using GR.Core.Attributes.Validation;
using Microsoft.AspNetCore.Http;

namespace GR.ApplePay.Razor.ViewModels
{
    public class ApplePaySettingsEditViewModel : ApplePaySettingsViewModel
    {
        [DataType(DataType.Upload)]
        [MaxFileSize(1024 * 10)]
        [AllowedExtensions(new[] { ".pfx", ".p12" })]
        public virtual IFormFile MerchantIdCertificate { get; set; }

        [DataType(DataType.Upload)]
        [MaxFileSize(1024 * 10)]
        [AllowedExtensions(new[] { ".txt" })]
        [FileNameEquals(ApplePayResources.DomainVerificationFile)]
        public virtual IFormFile DomainVerificationFile { get; set; }

        public virtual IEnumerable<string> SupportedMerchantCapabilities => new List<string>
        {
            "supports3DS",
            "supportsEMV",
            "supportsCredit",
            "supportsDebit"
        };

        public virtual IEnumerable<string> SupportedRequiredBillingContactFields => new List<string>
        {
            "postalAddress",
            "name",
            "phone",
            "email"
        };

        public virtual IEnumerable<string> SupportedRequiredShippingContactFields => new List<string>
        {
            "postalAddress",
            "name",
            "phone",
            "email"
        };

        public virtual IEnumerable<string> SupportedAcceptedCardSchemes => new List<string>
        {
            "amex", "masterCard", "visa"
        };
    }
}