using GR.ECommerce.Abstractions.Models;

namespace GR.ECommerce.Abstractions.ViewModels.ProductViewModels
{
    public class ProductAttributesViewModel: ProductAttributes
    {
        public override bool IsAvailable { get; set; }
    }
}
