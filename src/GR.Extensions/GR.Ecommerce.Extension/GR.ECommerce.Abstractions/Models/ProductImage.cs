using System;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class ProductImage : BaseModel
    {
        /// <summary>
        /// Image bytes
        /// </summary>
        public virtual byte[] Image { get; set; }
        /// <summary>
        /// Reference to product 
        /// </summary>
        public virtual Product Product { get; set; }
        public virtual Guid ProductId { get; set; }
    }
}
