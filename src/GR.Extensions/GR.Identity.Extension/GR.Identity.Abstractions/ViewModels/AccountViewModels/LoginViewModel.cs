using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Abstractions.ViewModels.AccountViewModels
{
    public class LoginViewModel
    {
        [Required]
        public virtual string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public virtual string Password { get; set; }

        [Display(Name = "Remember me?")]
        public virtual bool RememberMe { get; set; }
    }
}