using System;
using System.ComponentModel.DataAnnotations;
using GR.ECommerce.Abstractions.Models;

namespace GR.ECommerce.Abstractions.ViewModels.CartViewModels
{
    public class AddToCartViewModel: Cart
    {
        [Required]
        public virtual Guid ProductId { get; set; }
        [Required]
        public virtual int Quantity { get; set; }
      
        public virtual Guid? VariationId { get; set; }
    }
}
