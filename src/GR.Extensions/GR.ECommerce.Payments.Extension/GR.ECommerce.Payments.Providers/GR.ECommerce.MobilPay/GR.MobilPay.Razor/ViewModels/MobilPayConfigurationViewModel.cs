using System.ComponentModel.DataAnnotations;
using GR.Core.Attributes;
using GR.MobilPay.Abstractions.Models;
using Microsoft.AspNetCore.Http;

namespace GR.MobilPay.Razor.ViewModels
{
    public sealed class MobilPayConfigurationViewModel : MobilPayConfiguration
    {
        /// <summary>
        /// Private certificate
        /// </summary>
        [DisplayTranslate(Key = "system_private_key"), Required]
        public IFormFile PrivateCertificate { get; set; }

        /// <summary>
        /// Public certificate
        /// </summary>
        [DisplayTranslate(Key = "system_public_key"), Required]
        public IFormFile PublicCertificate { get; set; }
    }
}
