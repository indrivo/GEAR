using GR.Core.Attributes;

namespace GR.Paypal.Abstractions.Models.ViewModels
{
    public class PaypalExpressConfigFormViewModel
    {
        [RequiredTranslate(Key = "")] public bool IsSandbox { get; set; }


        [DisplayTranslate(Key = "eCommerce_Paypal_ClientId")]
        public string ClientId { get; set; }

        [DisplayTranslate(Key = "eCommerce_Paypal_ClientSecret")]
        public string ClientSecret { get; set; }

        public decimal PaymentFee { get; set; }

        public string Environment => IsSandbox ? "sandbox" : "production";

        public string EnvironmentUrlPart => IsSandbox ? ".sandbox" : "";
    }
}