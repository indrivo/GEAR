using System.ComponentModel.DataAnnotations;

namespace GR.Localization.Abstractions.ViewModels.CountryViewModels
{
    public class AddCityViewModel
    {
        [StringLength(450)]
        [Required]
        public string CountryId { get; set; }

        [StringLength(450)]
        public string Code { get; set; }

        [Required(ErrorMessage = "The {0} field is required.")]
        [StringLength(450)]
        public string Name { get; set; }

        [StringLength(450)]
        public string Type { get; set; }
    }
}