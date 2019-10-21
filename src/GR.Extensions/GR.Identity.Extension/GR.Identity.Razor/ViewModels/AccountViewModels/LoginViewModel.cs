using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Razor.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        //[RequiredTranslate(Key = "name")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}