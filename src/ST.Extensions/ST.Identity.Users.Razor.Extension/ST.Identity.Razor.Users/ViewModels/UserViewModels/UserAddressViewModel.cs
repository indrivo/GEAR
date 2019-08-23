using System;

namespace ST.Identity.Razor.Users.ViewModels.UserViewModels
{
    public class UserAddressViewModel
    {
        public Guid Id { get; set; }
        public bool IsPrimary { get; set; }
        public string CompanyName { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string District { get; set; }
    }
}