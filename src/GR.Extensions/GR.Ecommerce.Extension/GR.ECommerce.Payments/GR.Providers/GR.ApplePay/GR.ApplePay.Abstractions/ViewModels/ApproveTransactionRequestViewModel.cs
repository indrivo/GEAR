using System;
using System.ComponentModel.DataAnnotations;

namespace GR.ApplePay.Abstractions.ViewModels
{
    public class ApproveTransactionRequestViewModel
    {
        [Required]
        public virtual Guid OrderId { get; set; }
        [Required]
        public virtual ApplePayToken Token { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public virtual string CustomerEmail { get; set; }
        [Required]
        public virtual ApplePayAddress BillingDetails { get; set; }
        [Required]
        public virtual ApplePayAddress ShippingDetails { get; set; }
    }

    public class ApplePayToken
    {
        public virtual string PaymentData { get; set; }
        public virtual ApplePaymentMethod PaymentMethod { get; set; } = new ApplePaymentMethod();
        [Required]
        public virtual string TransactionIdentifier { get; set; }
    }

    public class ApplePaymentMethod
    {
        public virtual string DisplayName { get; set; }
        [Required]
        public virtual string Network { get; set; }
        public virtual string PaymentPass { get; set; }
        [Required]
        public virtual string Type { get; set; }
    }

    public class ApplePayAddress
    {
        public string[] AddressLines { get; set; }
        public string AdministrativeArea { get; set; }
        public string Country { get; set; }
        public string CountryCode { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string EmailAddress { get; set; }
        public string FamilyName { get; set; }
        public string GivenName { get; set; }
        public string Locality { get; set; }
        public string PhoneNumber { get; set; }
        public string PhoneticFamilyName { get; set; }
        public string PhoneticGivenName { get; set; }
        public string PostalCode { get; set; }
        public string SubAdministrativeArea { get; set; }
        public string SubLocality { get; set; }
    }
}
