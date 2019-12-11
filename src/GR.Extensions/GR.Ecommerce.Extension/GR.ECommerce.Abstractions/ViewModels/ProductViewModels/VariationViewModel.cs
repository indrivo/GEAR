using System;
using System.Collections.Generic;

namespace GR.ECommerce.Abstractions.ViewModels.ProductViewModels
{
    public class VariationViewModel
    {
        /// <summary>
        /// Variation id
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        public IEnumerable<VariationItemViewModel> Options { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; } = 0;

        /// <summary>
        /// Price with format
        /// </summary>
        public string FormattedPrice => Price.ToString("N2");
    }

    public class VariationItemViewModel
    {
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }
    }
}
