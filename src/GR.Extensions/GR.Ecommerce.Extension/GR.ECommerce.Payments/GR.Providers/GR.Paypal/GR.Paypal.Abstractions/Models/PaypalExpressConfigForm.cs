using System.ComponentModel.DataAnnotations;

namespace GR.Paypal.Abstractions.Models
{
    public class PaypalExpressConfigForm
    {
        public bool IsSandbox { get; set; }

        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        [Range(0.0, 100)]
        public decimal PaymentFee { get; set; } = 0;

        public string Environment => IsSandbox ? "sandbox" : "production";

        public string EnvironmentUrlPart => IsSandbox ? ".sandbox" : "";
    }
}