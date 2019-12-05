using GR.ECommerce.Abstractions.Models;

namespace GR.ECommerce.Razor.ViewModels
{
    public class ProductAttributesViewModel: ProductAttributes
    {
        public override bool IsAvailable { get; set; }
    }
}
