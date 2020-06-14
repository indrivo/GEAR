using System;
using System.Collections.Generic;
using GR.ECommerce.Abstractions.Models.Currencies;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;

namespace GR.Subscriptions.Abstractions.ViewModels
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
        /// Is current
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// Attributes
        /// </summary>
        public IEnumerable<AttributeViewModel> Attributes { get; set; } = new List<AttributeViewModel>();

        /// <summary>
        /// Variations
        /// </summary>
        public IEnumerable<SubscriptionVariationViewModel> Variations { get; set; } = new List<SubscriptionVariationViewModel>();
    }

    public class SubscriptionVariationViewModel
    {
        /// <summary>
        /// Variation id
        /// </summary>
        public virtual Guid Id { get; set; }

        /// <summary>
        /// Options
        /// </summary>
        public IEnumerable<SubscriptionVariationItemViewModel> Options { get; set; }

        /// <summary>
        /// Price
        /// </summary>
        public decimal Price { get; set; } = 0;

        /// <summary>
        /// Price with format
        /// </summary>
        public string FormattedPrice => Price.ToString("N2");

        /// <summary>
        /// Save money
        /// </summary>
        public decimal SaveMoneyValue { get; set; }
    }

    public class SubscriptionVariationItemViewModel
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