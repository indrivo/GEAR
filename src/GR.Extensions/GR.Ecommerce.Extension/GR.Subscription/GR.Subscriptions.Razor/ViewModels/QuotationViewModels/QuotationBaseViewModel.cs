using System.ComponentModel.DataAnnotations;

namespace GR.Subscriptions.Razor.ViewModels.QuotationViewModels
{
    public class QuotationBaseViewModel
    {
        /// <summary>
        /// Company name
        /// </summary>
        [Required]
        public string CompanyName { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress]
        public string Email { get; set; }
    }
}
