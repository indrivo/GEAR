using System.ComponentModel.DataAnnotations;

namespace GR.ECommerce.Abstractions.ViewModels.BrandViewModels
{
    public class CreateBrandViewModel
    {
        /// <summary>
        /// Brand name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        [Required]
        public virtual string DisplayName { get; set; }
    }
}