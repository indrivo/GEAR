using System.ComponentModel.DataAnnotations;

namespace GR.ECommerce.Abstractions.ViewModels.ProductTypeViewModels
{
    public class CreateProductTypeViewModel
    {
        [Required]
        public virtual string Name { get; set; }
        [Required]
        public virtual string DisplayName { get; set; }
    }
}