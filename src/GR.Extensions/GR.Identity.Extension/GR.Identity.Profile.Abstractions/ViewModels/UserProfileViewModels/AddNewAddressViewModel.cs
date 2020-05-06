using System;
using System.ComponentModel.DataAnnotations;
using GR.Core.Attributes;
using GR.Identity.Abstractions.Helpers;

namespace GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels
{
    public class AddNewAddressViewModel
    {
        /// <summary>
        /// Contact name
        /// </summary>
        [StringLength(450)]
        [Required]
        [Display(Name = "Contact Name")]
        [DisplayTranslate(Key = IdentityResources.Translations.CONTACT_NAME)]
        public string ContactName { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        [StringLength(450)]
        [Display(Name = nameof(Phone), Prompt = "0123456789")]
        [DisplayTranslate(Key = IdentityResources.Translations.PHONE)]
        public string Phone { get; set; }

        /// <summary>
        /// Primary address
        /// </summary>
        [StringLength(450)]
        [Required]
        [Display(Name = "Address Line 1")]
        [DisplayTranslate(Key = IdentityResources.Translations.ADDRESS_LINE1)]
        public string AddressLine1 { get; set; }

        /// <summary>
        /// Second address
        /// </summary>
        [StringLength(450)]
        [Display(Name = "Address Line 2")]
        [DisplayTranslate(Key = IdentityResources.Translations.ADRESS_LINE2)]
        public string AddressLine2 { get; set; }

        /// <summary>
        /// Zip code
        /// </summary>
        [StringLength(450)]
        [Required]
        [Display(Name = "Zip Code")]
        [DisplayTranslate(Key = IdentityResources.Translations.ZIP_CODE)]
        public string ZipCode { get; set; }

        /// <summary>
        /// City id
        /// </summary>
        [Required(ErrorMessage = "The {0} field is required.")]
        [DisplayTranslate(Key = IdentityResources.Translations.CITY)]
        public Guid CityId { get; set; }

        /// <summary>
        /// Country id, ex: MD
        /// </summary>
        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        [DisplayTranslate(Key = IdentityResources.Translations.COUNTRY)]
        public string CountryId { get; set; }

        /// <summary>
        /// If true this address will be chosen primary
        /// </summary>
        [Required]
        [DisplayTranslate(Key = IdentityResources.Translations.IS_DEFAULT)]
        public bool IsDefault { get; set; }

        /// <summary>
        /// Region
        /// </summary>
        public string Region { get; set; }
    }
}