using Microsoft.EntityFrameworkCore;
using GR.PageRender.Abstractions.Models.Pages;
using GR.PageRender.Abstractions.Models.ViewModels;

namespace GR.PageRender.Extensions
{
    internal static class DynamicPagesIndexExtensions
    {
        public static ModelBuilder RegisterIndexes(this ModelBuilder builder)
        {
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
