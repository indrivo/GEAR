using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Abstractions.ViewModels.UserProfileAddress
{
    public class EditUserProfileAddressViewModel : AddUserProfileAddressViewModel
    {
        [Required] public Guid Id { get; set; }

        public IEnumerable<SelectListItem> SelectedStateOrProvinceSelectListItems { get; set; } = new List<SelectListItem>();
    }
}