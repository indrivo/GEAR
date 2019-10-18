using System;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class ProductPrice : BaseModel
    {
        public virtual Product Product { get; set; }
        public virtual Guid ProductId { get; set; }
        public virtual double Price { get; set; } = 0;
    }
}
