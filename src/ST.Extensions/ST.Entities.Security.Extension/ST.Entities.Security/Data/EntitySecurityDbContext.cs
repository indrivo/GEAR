using Microsoft.EntityFrameworkCore;
using ST.Audit.Contexts;
using ST.Core.Abstractions;
using ST.Entities.Security.Abstractions;
using ST.Entities.Security.Abstractions.Models;

namespace ST.Entities.Security.Data
{
    public class EntitySecurityDbContext : TrackerDbContext, IEntitySecurityDbContext
    {
        /// <summary>
        /// Entities schema
        /// </summary>
        public const string Schema = "EntitySecurity";

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public EntitySecurityDbContext(DbContextOptions options) : base(options)
        {

        }

        /// <summary>
        /// Entity permissions
        /// </summary>
        public virtual DbSet<EntityPermission> EntityPermissions { get; set; }

        /// <summary>
        /// Entity field permissions
        /// </summary>
        public virtual DbSet<EntityFieldPermission> EntityFieldPermissions { get; set; }

        /// <summary>
        /// Entity access by role
        /// </summary>
        public virtual DbSet<EntityPermissionAccess> EntityPermissionAccesses { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.Entity<EntityPermission>()
                .HasIndex(x => x.ApplicationRoleId);

            builder.Entity<EntityPermission>()
                .HasIndex(x => x.TableModelId);

            builder.Entity<EntityFieldPermission>()
                .HasIndex(x => x.ApplicationRoleId);

            builder.Entity<EntityFieldPermission>()
                .HasIndex(x => x.TableModelFieldId);
        }

        public DbSet<T> SetEntity<T>() where T : class, IBaseModel
        {
            return Set<T>();
        }
    }
}
