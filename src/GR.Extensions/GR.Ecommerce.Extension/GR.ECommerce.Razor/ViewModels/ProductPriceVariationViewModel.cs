using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace GR.ECommerce.Razor.ViewModels
{
    public class ProductPriceVariationViewModel
    {
        [Required]
        public Guid ProductId { get; set; }
       
        public Guid? VariationId { get; set; }
        [Required] public int Quantity { get; set; } = 0;
    }
}
