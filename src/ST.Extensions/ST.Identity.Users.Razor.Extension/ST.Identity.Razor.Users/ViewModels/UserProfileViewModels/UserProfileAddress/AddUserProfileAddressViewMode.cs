using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ST.Core.Attributes;

namespace ST.Identity.Razor.Users.ViewModels.UserProfileViewModels.UserProfileAddress
{
    public class AddUserProfileAddressViewModel
    {
        public AddUserProfileAddressViewModel()
        {
            CountrySelectListItems = new HashSet<SelectListItem>();
        }

        [StringLength(450)]
        [Required]
        [Display(Name =  "Contact Name")]
        [DisplayTranslate(Key = "user_contact_name")]
        public string ContactName { get; set; }

        [StringLength(450)]
        [Display(Name = "Phone", Prompt = "0123456789")]
        [DisplayTranslate(Key = "system_phone")]
        public string Phone { get; set; }

        [StringLength(450)]
        [Display(Name = "Address Line 1")]
        [DisplayTranslate(Key = "user_address_line_1")]
        public string AddressLine1 { get; set; }

        [StringLength(450)]
        [Display(Name = "Address Line 2")]
        [DisplayTranslate(Key = "user_address_line_2")]
        public string AddressLine2 { get; set; }

        [StringLength(450)]
        [Display(Name = "City", Prompt = "city")]
        [DisplayTranslate(Key = "system_city")]
        public string City { get; set; }

        [StringLength(450)]
        [Required]
        [Display(Name = "Zip Code", Prompt = "zip code")]
        [DisplayTranslate(Key = "user_zip_code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [DisplayTranslate(Key = "system_city")]
        public long SelectedStateOrProvinceId { get; set; }


        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        [DisplayTranslate(Key = "system_country")]
        public string SelectedCountryId { get; set; }

        [Required]
        [DisplayTranslate(Key = "system_is_default")]
        public bool IsDefault { get; set; }

        public IEnumerable<SelectListItem> CountrySelectListItems { get; set; }
    }
}