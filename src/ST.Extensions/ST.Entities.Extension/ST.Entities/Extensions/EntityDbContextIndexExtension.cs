using Microsoft.EntityFrameworkCore;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Models.Pages;
using ST.Entities.Models.ViewModels;

namespace ST.Entities.Extensions
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

            builder.Entity<TableModelFields>()
               .HasIndex(x => x.TenantId);

            builder.Entity<Page>()
               .HasIndex(x => x.TenantId);

            builder.Entity<Block>()
                .HasIndex(x => x.TenantId);

            builder.Entity<BlockCategory>()
               .HasIndex(x => x.TenantId);

            builder.Entity<ViewModel>()
                .HasIndex(x => x.TenantId);

            builder.Entity<ViewModelFields>()
              .HasIndex(x => x.TenantId);

            return builder;
        }
    }
}
