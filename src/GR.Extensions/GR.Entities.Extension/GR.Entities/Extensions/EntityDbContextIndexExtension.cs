using Microsoft.EntityFrameworkCore;
using GR.Entities.Abstractions.Models.Tables;

namespace GR.Entities.Extensions
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
            builder.Entity<TableModel>()
                .HasIndex(x => x.TenantId);

            builder.Entity<EntityType>()
                .HasIndex(x => x.TenantId);

            builder.Entity<TableModelField>()
               .HasIndex(x => x.TenantId);

            return builder;
        }
    }
}
