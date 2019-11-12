using GR.ECommerce.Abstractions.Models;
using Microsoft.EntityFrameworkCore;

namespace GR.ECommerce.Products.Extensions
{
    internal static class EntityDbContextIndexExtension
    {
        /// <summary>
        /// Register indexes of EntitiesDbContext
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder RegisterIndexes(this ModelBuilder builder)
        {
            builder.Entity<Product>()
                .HasIndex(x => x.Name);

            return builder;
        }
    }
}
