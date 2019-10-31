using GR.ECommerce.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace GR.ECommerce.Razor.ViewModels
{
    public class AddToCartViewModel
    {
        [Required]
        public Guid ProductId { get; set; }
        [Required]
        public int Quantity { get; set; }
        [Required]
        public Guid VariationId { get; set; }
    }
}
