using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels
{
    public class UserProfileEditViewModel
    {
        [Required]
        [MaxLength(50)]
        [Display(Name = "First name", Description = "first name", Prompt = "ex: John")]
        public virtual string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        [Display(Name = "Last name", Description = "last name", Prompt = "ex: Doe")]
        public virtual string LastName { get; set; }

        [Required]
        [MaxLength(20)]
        [Display(Name = "Phone number", Description = "phone number ", Prompt = "0123456789")]
        [RegularExpression(@"^\+[0-9]?()[0-9](\s|\S)(\d[0-9]{6,9})$", ErrorMessage = "This is not a valid phone number, ex: +373069999999")]
        [DataType(DataType.PhoneNumber)]
        public virtual string PhoneNumber { get; set; }

        [Display(Name = "Birthday", Description = "birthday")]
        public virtual DateTime Birthday { get; set; } = DateTime.Now;

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public virtual string Email { get; set; }

        [MaxLength(500)]
        [Display(Name = "About me", Description = "same description")]
        public virtual string AboutMe { get; set; }
    }
}