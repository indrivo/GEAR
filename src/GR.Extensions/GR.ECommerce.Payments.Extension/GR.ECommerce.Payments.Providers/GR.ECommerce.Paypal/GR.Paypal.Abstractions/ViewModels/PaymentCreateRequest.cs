using Newtonsoft.Json;

namespace GR.Paypal.Abstractions.ViewModels
{
    public class PaymentCreateRequest
    {
        public virtual string Intent { get; set; }

        [JsonProperty("experience_profile_id")]
        public virtual string ExperienceProfileId { get; set; }

        public virtual Payer Payer { get; set; }

        public virtual Transaction[] Transactions { get; set; }

        [JsonProperty("note_to_payer")]
        public virtual string Notes { get; set; }

        [JsonProperty("redirect_urls")]
        public virtual RedirectUrls RedirectUrls { get; set; }
    }

    public class Payer
    {
        [JsonProperty("payment_method")]
        public virtual string PaymentMethod { get; set; }
    }

    public class RedirectUrls
    {
        [JsonProperty("return_url")]
        public virtual string ReturnUrl { get; set; }

        [JsonProperty("cancel_url")]
        public virtual string CancelUrl { get; set; }
    }

    public class Transaction
    {
        public virtual Amount Amount { get; set; }
        public virtual string Description { get; set; }
        public virtual string Custom { get; set; }
        [JsonProperty("invoice_number")]
        public virtual string InvoiceNumber { get; set; }
        [JsonProperty("payment_options")]
        public virtual PaymentOptions PaymentOptions { get; set; }
        [JsonProperty("soft_descriptor")]
        public virtual string SoftDescriptor { get; set; }
        [JsonProperty("item_list")]
        public virtual ItemList Items { get; set; }
    }

    public class Amount
    {
        public virtual string Total { get; set; }
        public virtual string Currency { get; set; }
        public virtual Details Details { get; set; }
    }

    public class Details
    {
        public virtual string Subtotal { get; set; }
        public virtual string Tax { get; set; }
        public virtual string Shipping { get; set; }
        [JsonProperty("handling_fee")]
        public virtual string HandlingFee { get; set; }
        [JsonProperty("shipping_discount")]
        public virtual string ShippingDiscount { get; set; }
        public virtual string Insurance { get; set; }
    }

    public class PaymentOptions
    {
        [JsonProperty("allowed_payment_method")]
        public virtual string AllowedPaymentMethod { get; set; }
    }

    public class ItemList
    {
        public virtual Item[] Items { get; set; }
        [JsonProperty("shipping_address")]
        public virtual ShippingAddress ShippingAddress { get; set; }
    }

    public class ShippingAddress
    {
        [JsonProperty("recipient_name")]
        public virtual string RecipientName { get; set; }
        public virtual string Line1 { get; set; }
        public virtual string Line2 { get; set; }
        public virtual string City { get; set; }
        [JsonProperty("country_code")]
        public virtual string CountryCode { get; set; }
        [JsonProperty("postal_code")]
        public virtual string PostalCode { get; set; }
        public virtual string Phone { get; set; }
        public virtual string State { get; set; }
    }

    public class Item
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual string Quantity { get; set; }
        public virtual string Price { get; set; }
        public virtual string Tax { get; set; }
        public virtual string Sku { get; set; }
        public virtual string Currency { get; set; }
    }
}