using System;

namespace GR.Braintree.Abstractions.Models
{
    public class BraintreeSuccessPaymentResult
    {
        public virtual Guid OrderId { get; set; }
        public virtual string TransactionId { get; set; }
    }
}
