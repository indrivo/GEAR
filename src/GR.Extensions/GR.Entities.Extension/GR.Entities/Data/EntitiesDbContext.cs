using GR.Core;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Extensions;
using GR.Entities.Security.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace GR.Entities.Data
{
    public class EntitiesDbContext : EntitySecurityDbContext, IEntityContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this, is used on audit
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public new const string Schema = "Entities";

        /// <inheritdoc />
        /// <summary>
        /// Options
        /// </summary>
        /// <param name="options"></param>
        // ReSharper disable once SuggestBaseTypeForParameter
        public EntitiesDbContext(DbContextOptions<EntitySecurityDbContext> options)
                    : base(options)
        {
        }

        /// <summary>
        /// Entity types
        /// </summary>
        public virtual DbSet<EntityType> EntityTypes { get; set; }

        /// <summary>
        /// Tables
        /// </summary>
        public virtual DbSet<TableModel> Table { get; set; }

        /// <summary>
        /// Table configs
        /// </summary>
        public virtual DbSet<TableFieldConfigs> TableFieldConfigs { get; set; }

        /// <summary>
        /// Table config values
        /// </summary>
        public virtual DbSet<TableFieldConfigValue> TableFieldConfigValues { get; set; }

        /// <summary>
        /// Field groups
        /// </summary>
        public virtual DbSet<TableFieldGroups> TableFieldGroups { get; set; }

        /// <summary>
        /// Table fields
        /// </summary>
        public virtual DbSet<TableModelField> TableFields { get; set; }

        /// <summary>
        /// Field types
        /// </summary>
        public virtual DbSet<TableFieldType> TableFieldTypes { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.Entity<TableFieldType>().HasKey(ug => new { ug.Id });
            builder.Entity<TableFieldConfigs>().HasKey(ug => new { ug.Id });
            builder.Entity<TableFieldConfigValue>().HasKey(ug => new { ug.TableModelFieldId, ug.TableFieldConfigId });
            builder.Entity<TableModelField>().HasOne(typeof(TableFieldType), "TableFieldType").WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.RegisterIndexes();
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            EntitiesDbContextSeeder<EntitiesDbContext>
                .SeedAsync(this, GearSettings.TenantId)
                .Wait();
            return Task.CompletedTask;
        }
    }
}