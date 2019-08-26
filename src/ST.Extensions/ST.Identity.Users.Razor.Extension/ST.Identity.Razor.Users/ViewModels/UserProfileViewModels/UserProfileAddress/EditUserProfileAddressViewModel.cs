using System;
using System.ComponentModel.DataAnnotations;

namespace ST.Identity.Razor.Users.ViewModels.UserProfileViewModels.UserProfileAddress
{
    public class EditUserProfileAddressViewModel : AddUserProfileAddressViewModel
    {
        [Required]
        public Guid Id { get; set; }
    }
}