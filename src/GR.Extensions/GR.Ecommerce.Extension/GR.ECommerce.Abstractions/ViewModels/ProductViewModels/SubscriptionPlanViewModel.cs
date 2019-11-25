using System;
using System.Collections.Generic;
using GR.ECommerce.Abstractions.Models.Currencies;

namespace GR.ECommerce.Abstractions.ViewModels.ProductViewModels
{
    public sealed class SubscriptionPlanViewModel
    {
        /// <summary>
        /// Product id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Currency
        /// </summary>
        public Currency Currency { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Short description
        /// </summary>
        public string ShortDescription { get; set; }

        /// <summary>
        /// Attributes
        /// </summary>
        public IEnumerable<AttributeViewModel> Attributes { get; set; } = new List<AttributeViewModel>();

        /// <summary>
        /// Variations
        /// </summary>
        public IEnumerable<VariationViewModel> Variations { get; set; } = new List<VariationViewModel>();
    }
}
