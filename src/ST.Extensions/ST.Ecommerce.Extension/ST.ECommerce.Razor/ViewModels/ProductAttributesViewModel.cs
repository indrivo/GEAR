using ST.ECommerce.Abstractions.Models;

namespace ST.ECommerce.Razor.ViewModels
{
    public class ProductAttributesViewModel: ProductAttributes
    {
        public override bool IsAvailable { get; set; }
    }
}
