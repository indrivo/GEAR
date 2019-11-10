using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using GR.Core;

namespace GR.ECommerce.Abstractions.Models
{
    public class CartItem : BaseModel
    {
        [Required]
        public Guid CartId { get; set; }
        public Cart Cart { get; set; }

        [Required]
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        
        public Guid? ProductVariationId { get; set; }
        public ProductVariation ProductVariation { get; set; }


        public int Amount { get; set; }
    }
}
