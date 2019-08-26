using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        public string ContactName { get; set; }

        [StringLength(450)]
        [Display(Name = "Phone", Prompt = "0123456789")]
        public string Phone { get; set; }

        [StringLength(450)]
        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }

        [StringLength(450)]
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }

        [StringLength(450)]
        [Display(Name = "City")]
        public string City { get; set; }

        [StringLength(450)]
        [Required]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        public long SelectedStateOrProvinceId { get; set; }


        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public Guid SelectedCountryId { get; set; }

        [Display(Name = "Country")]
        public IEnumerable<SelectListItem> CountrySelectListItems { get; set; }
    }
}