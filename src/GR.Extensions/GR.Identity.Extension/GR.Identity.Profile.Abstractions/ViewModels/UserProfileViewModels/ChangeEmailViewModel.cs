using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.Identity.Profile.Abstractions.ViewModels.UserProfileViewModels
{
    public class ChangeEmailViewModel
    {
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        [RegularExpression(GlobalResources.RegularExpressions.EMAIL, ErrorMessage = "Please enter a valid email address")]
        public virtual string Email { get; set; }
    }
}