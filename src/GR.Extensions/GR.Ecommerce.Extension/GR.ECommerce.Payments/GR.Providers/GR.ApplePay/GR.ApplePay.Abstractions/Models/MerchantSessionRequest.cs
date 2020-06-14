using Newtonsoft.Json;

namespace GR.ApplePay.Abstractions.Models
{
    public class MerchantSessionRequest
    {
        [JsonProperty("merchantIdentifier")]
        public string MerchantIdentifier { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("initiative")]
        public string Initiative { get; set; }

        [JsonProperty("initiativeContext")]
        public string InitiativeContext { get; set; }
    }
}
