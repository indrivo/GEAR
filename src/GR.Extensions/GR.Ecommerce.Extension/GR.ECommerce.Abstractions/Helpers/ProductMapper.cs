using System.Linq;
using GR.ECommerce.Abstractions.Models;
using GR.ECommerce.Abstractions.Models.Currencies;
using GR.ECommerce.Abstractions.ViewModels.ProductViewModels;

namespace GR.ECommerce.Abstractions.Helpers
{
    public static class ProductMapper
    {
        /// <summary>
        /// Map
        /// </summary>
        /// <param name="product"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        public static SubscriptionPlanViewModel Map(Product product, Currency currency)
        {
            var subscription = new SubscriptionPlanViewModel
            {
                Id = product.Id,
                Currency = currency,
                DisplayName = product.DisplayName,
                Description = product.Description,
                ShortDescription = product.ShortDescription,
                Attributes = product.ProductAttributes?.Select(x => new AttributeViewModel
                {
                    Name = x.ProductAttribute?.Name, 
                    Value = x.Value
                }),
                Variations = product.ProductVariations.Select(x => new VariationViewModel
                {
                    Id = x.Id,
                    Price = x.Price,
                    Options = x.ProductVariationDetails?.Select(y => new VariationItemViewModel
                    {
                        Name = y.ProductOption?.Name,
                        Value = y.Value
                    })
                })
            };


            return subscription;
        }
    }
}
