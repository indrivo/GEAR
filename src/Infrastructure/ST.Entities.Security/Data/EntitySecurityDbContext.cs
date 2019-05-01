using Microsoft.EntityFrameworkCore;
using ST.Audit.Contexts;
using ST.Entities.Security.Models;

namespace ST.Entities.Security.Data
{
    public class EntitySecurityDbContext : TrackerDbContext
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        protected EntitySecurityDbContext(DbContextOptions options) : base(options)
        {

        }

        /// <summary>
        /// Entity permissions
        /// </summary>
        public DbSet<EntityPermission> EntityPermissions { get; set; }

        /// <summary>
        /// Entity field permissions
        /// </summary>
        public DbSet<EntityFieldPermission> EntityFieldPermissions { get; set; }

        /// <summary>
        /// Entity access by role
        /// </summary>
        public DbSet<EntityPermissionAccess> EntityPermissionAccesses { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<EntityPermission>()
                .HasIndex(x => x.ApplicationRoleId);

            builder.Entity<EntityPermission>()
                .HasIndex(x => x.TableModelId);

            builder.Entity<EntityFieldPermission>()
                .HasIndex(x => x.ApplicationRoleId);

            builder.Entity<EntityFieldPermission>()
                .HasIndex(x => x.TableModelFieldId);
        }
    }
}
