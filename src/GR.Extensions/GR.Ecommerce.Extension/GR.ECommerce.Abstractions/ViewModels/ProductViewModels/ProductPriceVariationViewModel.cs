using System;
using System.ComponentModel.DataAnnotations;

namespace GR.ECommerce.Abstractions.ViewModels.ProductViewModels
{
    public class ProductPriceVariationViewModel
    {
        [Required]
        public Guid ProductId { get; set; }
       
        public Guid? VariationId { get; set; }
        [Required] public int Quantity { get; set; } = 0;
    }
}
