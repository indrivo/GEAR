using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.Permmisions;
using Microsoft.EntityFrameworkCore;

namespace GR.Identity.Extensions
{
    public static class ApplicationDbIndexExtension
    {
        /// <summary>
        /// Register ApplicationDbContext indexes
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static ModelBuilder RegisterIndexes(this ModelBuilder builder)
        {
            builder.Entity<GearUser>()
                .HasIndex(x => x.TenantId);

            builder.Entity<GearRole>()
                .HasIndex(x => x.TenantId);

            builder.Entity<Permission>()
               .HasIndex(x => x.TenantId);

            return builder;
        }
    }
}