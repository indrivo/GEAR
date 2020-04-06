using GR.Identity.Abstractions;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.LdapAuth.Abstractions.Models
{
    public class LdapUser : GearUser, ILdapEntry
    {

        public string ObjectSid { get; set; }


        public string ObjectGuid { get; set; }


        public string ObjectCategory { get; set; }


        public string ObjectClass { get; set; }


        public string Name { get; set; }


        public string CommonName { get; set; }


        public string DistinguishedName { get; set; }


        public string SamAccountName { get; set; }


        public int SamAccountType { get; set; }


        public string[] MemberOf { get; set; }


        public bool IsDomainAdmin { get; set; }


        public bool MustChangePasswordOnNextLogon { get; set; }


        public string UserPrincipalName { get; set; }


        public string DisplayName { get; set; }


        [Required(ErrorMessage = "You must enter your first name!")]
        public override string FirstName { get; set; }


        [Required(ErrorMessage = "You must enter your last name!")]
        public override string LastName { get; set; }


        public string FullName => $"{this.FirstName} {this.LastName}";


        [Required(ErrorMessage = "You must enter your email address!")]
        [EmailAddress(ErrorMessage = "You must enter a valid email address.")]
        public string EmailAddress { get; set; }


        public string Description { get; set; }


        public string Phone { get; set; }


        public LdapAddress Address { get; set; }
    }

    public class LdapAddress
    {
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string StateName { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
    }
}