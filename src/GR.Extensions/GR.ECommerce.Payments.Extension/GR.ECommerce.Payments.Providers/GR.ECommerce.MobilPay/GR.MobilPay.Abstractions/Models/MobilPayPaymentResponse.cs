namespace GR.MobilPay.Abstractions.Models
{
    public class MobilPayPaymentResponse
    {
        public string ErrorType { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}
