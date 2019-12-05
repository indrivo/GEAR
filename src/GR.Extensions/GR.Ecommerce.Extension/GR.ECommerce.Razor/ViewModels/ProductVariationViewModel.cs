using GR.ECommerce.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.ECommerce.Razor.ViewModels
{
    public class ProductVariationViewModel
    {
       
        public Guid? VariationId { get; set; }
        [Required]
        public Guid ProductId { get; set; }
        public decimal Price { get; set; } = 0;
        public IEnumerable<ProductVariationDetail> ProductVariationDetails { get; set; } = new List<ProductVariationDetail>();
    }
}
