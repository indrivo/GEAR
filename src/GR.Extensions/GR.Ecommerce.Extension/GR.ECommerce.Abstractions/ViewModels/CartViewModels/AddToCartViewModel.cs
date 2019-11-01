using GR.ECommerce.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GR.ECommerce.Razor.ViewModels
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
