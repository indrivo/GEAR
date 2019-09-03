using System;
using System.ComponentModel.DataAnnotations;

namespace ST.Identity.Razor.Users.ViewModels.UserProfileViewModels
{
    public class UserProfileEditViewModel
    {
        public string Id { get; set; }

        [MaxLength(50)]
        [Display(Name = "First name", Description = "first name", Prompt = "ex: John")]
        public string UserFirstName { get; set; }

        [MaxLength(50)]
        [Display(Name = "Last name", Description = "last name", Prompt = "ex: Doe")]
        public string UserLastName { get; set; }

        [MaxLength(20)]
        [Display(Name = "PHONE number", Description = "phone number ", Prompt = "0123456789")]
        [DataType(DataType.PhoneNumber)]
        public string UserPhoneNumber { get; set; }

        [Display(Name = "Birthday", Description = "birthday")]
        public DateTime Birthday { get; set; }

        [MaxLength(500)]
        [Display(Name = "About me", Description = "same description")]
        public string AboutMe { get; set; }
    }
}