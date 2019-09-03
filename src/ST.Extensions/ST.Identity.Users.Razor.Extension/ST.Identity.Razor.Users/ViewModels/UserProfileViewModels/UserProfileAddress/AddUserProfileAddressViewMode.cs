using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST.Core.Attributes;
using ST.Identity.Abstractions.Helpers;

namespace ST.Identity.Razor.Users.ViewModels.UserProfileViewModels.UserProfileAddress
{
    public class AddUserProfileAddressViewModel
    {
        [StringLength(450)]
        [Required]
        [Display(Name = "Contact Name")]
        [DisplayTranslate(Key = Resources.Translations.CONTACT_NAME)]
        public string ContactName { get; set; }

        [StringLength(450)]
        [Display(Name = nameof(Phone), Prompt = "0123456789")]
        [DisplayTranslate(Key = Resources.Translations.PHONE)]
        public string Phone { get; set; }

        [StringLength(450)]
        [Display(Name = "Address Line 1")]
        [DisplayTranslate(Key = Resources.Translations.ADDRESS_LINE1)]
        public string AddressLine1 { get; set; }

        [StringLength(450)]
        [Display(Name = "Address Line 2")]
        [DisplayTranslate(Key = Resources.Translations.ADRESS_LINE2)]
        public string AddressLine2 { get; set; }

        [StringLength(450)]
        [Display(Name = "City", Prompt = "city")]
        [DisplayTranslate(Key = Resources.Translations.CITY)]
        public string City { get; set; }

        [StringLength(450)]
        [Required]
        [Display(Name = "Zip Code", Prompt = "zip code")]
        [DisplayTranslate(Key = Resources.Translations.ZIP_CODE)]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [DisplayTranslate(Key = Resources.Translations.CITY)]
        public long SelectedStateOrProvinceId { get; set; }


        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        [DisplayTranslate(Key = Resources.Translations.COUNTRY)]
        public string SelectedCountryId { get; set; }

        [Required]
        [DisplayTranslate(Key = Resources.Translations.IS_DEFAULT)]
        public bool IsDefault { get; set; }

        public IEnumerable<SelectListItem> CountrySelectListItems { get; set; } = new List<SelectListItem>();
    }
}