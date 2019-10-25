using GR.ECommerce.Abstractions.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace GR.ECommerce.Razor.ViewModels
{
    public class ProductVariationViewModel
    {

        public Guid ProductId { get; set; }
        public double Price { get; set; }
        public List<ProductVariationDetail> ProductVaritionDetails { get; set; }
    }

   
}
