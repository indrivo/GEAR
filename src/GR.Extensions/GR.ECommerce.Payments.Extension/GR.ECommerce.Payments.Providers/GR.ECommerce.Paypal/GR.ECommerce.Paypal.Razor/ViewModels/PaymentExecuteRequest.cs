using Newtonsoft.Json;

namespace ST.ECommerce.Paypal.Razor.ViewModels
{
    public class PaymentExecuteRequest
    {
        [JsonProperty("payer_id")] public string PayerId { get; set; }
    }
}