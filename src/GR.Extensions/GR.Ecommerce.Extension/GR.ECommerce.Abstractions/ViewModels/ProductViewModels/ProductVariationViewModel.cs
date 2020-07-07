using System;
using System.Collections.Generic;

namespace GR.ECommerce.Abstractions.ViewModels.ProductViewModels
{
    public class ProductVariationViewModel
    {
        public virtual Guid VariationId { get; set; }
        public virtual Guid ProductId { get; set; }
        public virtual decimal Price { get; set; }
        public virtual IEnumerable<ProductVariationDetailsViewModel> VariationDetails { get; set; } = new List<ProductVariationDetailsViewModel>();
    }

    public class ProductVariationDetailsViewModel
    {
        public virtual string Value { get; set; }
        public virtual string Option { get; set; }
        public virtual Guid OptionId { get; set; }
    }
}
