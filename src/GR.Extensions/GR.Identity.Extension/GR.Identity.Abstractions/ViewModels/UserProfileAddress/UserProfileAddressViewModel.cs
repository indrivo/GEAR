using System;

namespace GR.Identity.Abstractions.ViewModels.UserProfileAddress
{
    public class UserProfileAddressViewModel
    {
        public Guid Id { get; set; }
        public bool IsPrimary { get; set; }
        public string ContactName { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string District { get; set; }
    }
}