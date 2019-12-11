using System.Collections.Generic;
using System.Linq;
using GR.ECommerce.Abstractions.Models;
using GR.Subscriptions.Abstractions.Models;

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
    }
}
