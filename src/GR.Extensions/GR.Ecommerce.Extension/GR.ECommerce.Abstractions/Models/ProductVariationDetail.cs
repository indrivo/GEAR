using System;
using System.Collections.Generic;
using System.Text;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class ProductVariationDetail : BaseModel
    {
        public virtual string Value { get; set; }

        public virtual ProductVariation ProductVariation { get; set; }
        public virtual Guid ProductVariationId { get; set; }

        public virtual ProductOption ProductOption { get; set; }
        public virtual Guid ProductOptionId { get; set; }
    }
}
