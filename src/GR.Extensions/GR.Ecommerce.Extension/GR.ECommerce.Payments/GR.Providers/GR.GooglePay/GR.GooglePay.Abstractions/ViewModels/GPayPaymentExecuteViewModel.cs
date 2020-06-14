using System;
using System.ComponentModel.DataAnnotations;

namespace GR.GooglePay.Abstractions.ViewModels
{
    public class GPayPaymentExecuteViewModel
    {
        /// <summary>
        /// Order id
        /// </summary>
        [Required]
        public virtual Guid OrderId { get; set; }

        /// <summary>
        /// Api version
        /// </summary>
        public int ApiVersion { get; set; }
        /// <summary>
        /// Api version minor
        /// </summary>
        public int ApiVersionMinor { get; set; }

        /// <summary>
        /// Payment method data
        /// </summary>
        [Required]
        public PaymentMethodData PaymentMethodData { get; set; }
    }

    public class PaymentMethodData
    {
        [Required]
        public string Type { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public Info Info { get; set; }
        [Required]
        public TokenizationData TokenizationData { get; set; }
    }

    public class Info
    {
        public string CardNetwork { get; set; }
        public string CardDetails { get; set; }
    }

    public class TokenizationData
    {
        [Required]
        public string Type { get; set; }
        [Required]
        public string Token { get; set; }
    }
}