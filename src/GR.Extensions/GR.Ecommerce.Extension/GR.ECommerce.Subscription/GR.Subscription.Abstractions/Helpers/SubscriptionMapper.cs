using System;
using System.Collections.Generic;
using System.Linq;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.Models.Currencies;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;
using GR.Subscriptions.Abstractions.Models;
using GR.Subscriptions.Abstractions.ViewModels;

namespace GR.Subscriptions.Abstractions.Helpers
{
    public static class SubscriptionMapper
    {
        /// <summary>
        /// Map product attributes to subscription
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public static IEnumerable<SubscriptionPermission> Map(IEnumerable<ProductAttributes> attributes)
        {
            var attrs = attributes?.ToList() ?? new List<ProductAttributes>();
            foreach (var attr in attrs)
            {
                yield return new SubscriptionPermission
                {
                    Name = attr?.ProductAttribute?.Name,
                    Value = attr?.Value
                };
            }
        }

        /// <summary>
        /// Map
        /// </summary>
        /// <param name="product"></param>
        /// <param name="currency"></param>
        /// <param name="activeSubscription"></param>
        /// <returns></returns>
        public static SubscriptionPlanViewModel Map(Product product, Currency currency, Subscription activeSubscription)
        {
            var subscription = new SubscriptionPlanViewModel
            {
                Id = product.Id,
                Currency = currency,
                DisplayName = product.DisplayName,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                IsCurrent = activeSubscription?.Order?.ProductOrders?.Any(x => x.ProductId.Equals(product.Id)) ?? false,
                Attributes = product.ProductAttributes?.Select(x => new AttributeViewModel
                {
                    Name = x.ProductAttribute?.Name,
                    DisplayName = x.ProductAttribute?.DisplayName,
                    Value = x.Value
                }),
                Variations = product.ProductVariations.Select(x => new SubscriptionVariationViewModel
                {
                    Id = x.Id,
                    Price = x.Price,
                    Options = x.ProductVariationDetails?.Select(y => new SubscriptionVariationItemViewModel
                    {
                        Name = y.ProductOption?.Name,
                        Value = y.Value
                    }),
                    SaveMoneyValue = CalculateSaveMoney(product.ProductVariations, x)
                })
            };


            return subscription;
        }

        /// <summary>
        /// Calculate save money
        /// </summary>
        /// <param name="variations"></param>
        /// <param name="productVariation"></param>
        /// <returns></returns>
        public static decimal CalculateSaveMoney(IEnumerable<ProductVariation> variations, ProductVariation productVariation)
        {
            var preVariation = variations
                .Where(x => x.Id != productVariation.Id && x.Price < productVariation.Price)
                .OrderBy(x => x.Price)
                .FirstOrDefault();
            if (preVariation == null) return 0;

            var preMonths = preVariation.ProductVariationDetails
                .FirstOrDefault(x => x.ProductOption.Name.Equals("Period"));

            var currentMonths = productVariation.ProductVariationDetails
                .FirstOrDefault(x => x.ProductOption.Name.Equals("Period"));
            if (preMonths == null || currentMonths == null) return 0;

            int.TryParse(preMonths.Value, out var preCount);
            int.TryParse(currentMonths.Value, out var currentCount);
            if (preCount == 0 || currentCount == 0) return 0;

            var saveMoney = productVariation.Price - preCount * (currentCount / preCount) * preVariation.Price;
            return Math.Abs(saveMoney);
        }
    }
}