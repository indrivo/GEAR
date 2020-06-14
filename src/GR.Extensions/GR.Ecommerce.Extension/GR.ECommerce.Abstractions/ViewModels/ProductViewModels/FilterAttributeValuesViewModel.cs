using System.Collections.Generic;
using GR.ECommerce.Abstractions.Models;

namespace GR.ECommerce.Abstractions.ViewModels.ProductViewModels
{
    public class FilterAttributeValuesViewModel : ProductAttribute
    {
        public virtual IEnumerable<string> Values { get; set; }
    }
}
