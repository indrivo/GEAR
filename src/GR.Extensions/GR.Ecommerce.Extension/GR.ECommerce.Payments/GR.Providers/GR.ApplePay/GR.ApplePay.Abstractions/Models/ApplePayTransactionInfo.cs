using Newtonsoft.Json;

namespace GR.ApplePay.Abstractions.Models
{
    public class ApplePayTransactionInfo
    {
        [JsonProperty("shop_name")]
        public virtual string ShopName { get; set; }

        [JsonProperty("product_price")]
        public virtual decimal ProductPrice { get; set; }

        [JsonProperty("shop_localisation")]
        public virtual AppleShopLocalization ShopLocalization { get; set; }
    }

    public class AppleShopLocalization
    {
        public virtual string CountryCode { get; set; }
        public virtual string CurrencyCode { get; set; }
    }
}
