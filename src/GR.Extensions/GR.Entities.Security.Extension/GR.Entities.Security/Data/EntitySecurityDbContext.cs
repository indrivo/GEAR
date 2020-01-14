using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Contexts;
using GR.Core.Abstractions;
using GR.Entities.Security.Abstractions;
using GR.Entities.Security.Abstractions.Models;

namespace GR.Entities.Security.Data
{
    public class EntitySecurityDbContext : TrackerDbContext, IEntitySecurityDbContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this, is used on audit 
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public const string Schema = "Entities";

        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="options"></param>
        public EntitySecurityDbContext(DbContextOptions<EntitySecurityDbContext> options) : base(options)
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

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override  Task InvokeSeedAsync(IServiceProvider services)
        {
            return Task.CompletedTask;
        }
    }
}
