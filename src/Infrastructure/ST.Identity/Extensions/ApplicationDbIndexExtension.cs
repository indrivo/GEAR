using Microsoft.EntityFrameworkCore;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;

namespace ST.Identity.Extensions
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
            builder.Entity<ApplicationUser>()
                .HasIndex(x => x.TenantId);

            builder.Entity<ApplicationRole>()
                .HasIndex(x => x.TenantId);

            builder.Entity<AuthGroup>()
               .HasIndex(x => x.TenantId);

            builder.Entity<Profile>()
                .HasIndex(x => x.TenantId);

            builder.Entity<Permission>()
               .HasIndex(x => x.TenantId);

            return builder;
        }
    }
}
