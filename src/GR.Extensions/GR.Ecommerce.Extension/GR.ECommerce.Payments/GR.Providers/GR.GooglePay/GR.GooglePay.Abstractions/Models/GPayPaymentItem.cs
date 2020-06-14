namespace GR.GooglePay.Abstractions.Models
{
    public class GPayPaymentItem
    {
        public virtual string Label { get; set; }
        public virtual string Type { get; set; }
        public virtual string Price { get; set; }
    }
}