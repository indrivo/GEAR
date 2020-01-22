using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Razor.ViewModels.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Name { get; set; }
        public string GivenName { get; set; }
        public string Picture { get; set; }
        public string LoginProvider { get; set; }
    }
}