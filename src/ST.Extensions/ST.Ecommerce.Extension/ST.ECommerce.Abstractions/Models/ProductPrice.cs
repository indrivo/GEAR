using System;
using ST.Core;

namespace ST.ECommerce.Abstractions.Models
{
    public class ProductPrice : BaseModel
    {
        public virtual Product Product { get; set; }
        public virtual Guid ProductId { get; set; }
        public virtual double Price { get; set; } = 0;
    }
}
