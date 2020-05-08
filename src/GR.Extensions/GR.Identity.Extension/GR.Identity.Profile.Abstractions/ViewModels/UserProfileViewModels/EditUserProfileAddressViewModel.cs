using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels
{
    public class EditUserProfileAddressViewModel : AddUserProfileAddressViewModel
    {
        [Required] public Guid Id { get; set; }

        public IEnumerable<SelectListItem> SelectedStateOrProvinceSelectListItems { get; set; } = new List<SelectListItem>();
    }
}