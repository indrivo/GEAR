using System;
using System.ComponentModel.DataAnnotations;

namespace ST.ECommerce.Abstractions.Models
{
    public class ProductOrder
    {
        public virtual Product Product { get; set; }
        [Required]
        public virtual Guid ProductId { get; set; }
        public virtual Order Order { get; set; }
        [Required]
        public virtual Guid OrderId { get; set; }
        [Required]
        public virtual double Price { get; set; }
    }
}
