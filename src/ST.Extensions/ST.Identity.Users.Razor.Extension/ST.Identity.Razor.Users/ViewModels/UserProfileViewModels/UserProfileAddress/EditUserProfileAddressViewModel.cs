using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ST.Identity.Razor.Users.ViewModels.UserProfileViewModels.UserProfileAddress
{
    public class EditUserProfileAddressViewModel : AddUserProfileAddressViewModel
    {
        [Required] public Guid Id { get; set; }

        public IEnumerable<SelectListItem> SelectedStateOrProvinceSelectListItems { get; set; } = new List<SelectListItem>();
    }
}