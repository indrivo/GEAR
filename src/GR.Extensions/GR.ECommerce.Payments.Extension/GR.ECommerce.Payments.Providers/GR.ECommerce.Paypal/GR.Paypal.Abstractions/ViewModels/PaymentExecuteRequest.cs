using Newtonsoft.Json;

namespace GR.Paypal.Razor.ViewModels
{
    public class PaymentExecuteRequest
    {
        [JsonProperty("payer_id")] public string PayerId { get; set; }
    }
}