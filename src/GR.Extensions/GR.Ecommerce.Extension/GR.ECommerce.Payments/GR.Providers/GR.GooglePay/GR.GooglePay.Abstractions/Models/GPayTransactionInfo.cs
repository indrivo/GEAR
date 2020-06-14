using System.Collections.Generic;

namespace GR.GooglePay.Abstractions.Models
{
    public class GPayTransactionInfo
    {
        /// <summary>
        /// Display items
        /// </summary>
        public virtual IList<GPayPaymentItem> DisplayItems { get; set; } = new List<GPayPaymentItem>();

        public virtual string CountryCode { get; set; }
        public virtual string CurrencyCode { get; set; }
        public virtual string TotalPriceStatus { get; set; }
        public virtual string TotalPrice { get; set; }
        public virtual string TotalPriceLabel { get; set; }
    }
}
