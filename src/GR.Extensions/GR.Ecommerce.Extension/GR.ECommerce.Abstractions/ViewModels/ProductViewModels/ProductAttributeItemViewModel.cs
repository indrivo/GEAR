using System;

namespace GR.ECommerce.Abstractions.ViewModels.ProductViewModels
{
    public class ProductAttributeItemViewModel
    {
        public virtual string Label { get; set; }
        public virtual Guid AttributeId { get; set; }
        public virtual string Value { get; set; }
        public virtual bool IsAvailable { get; set; }
        public virtual bool IsPublished { get; set; }
        public virtual bool ShowInFilters { get; set; }
    }
}
