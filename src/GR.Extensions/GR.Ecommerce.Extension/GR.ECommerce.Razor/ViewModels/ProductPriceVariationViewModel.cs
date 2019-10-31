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
        [Required]
        public Guid OptionId { get; set; }
        [Required]
        public IEnumerable<Guid> ListVariationDetailsId { get; set; }
    }
}
