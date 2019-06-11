using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Data;
using ST.Entities.Security.Models;
using ST.PageRender.Abstractions;
using ST.PageRender.Abstractions.Models.Pages;
using ST.PageRender.Abstractions.Models.PagesACL;
using ST.PageRender.Abstractions.Models.RenderTemplates;
using ST.PageRender.Abstractions.Models.ViewModels;
using ST.PageRender.Extensions;

namespace ST.PageRender.Data
{
    public class DynamicPagesDbContext : EntitiesDbContext, IDynamicPagesContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public new const string Schema = "Pages";

        private const string ParentSchema = "Entities";

        // ReSharper disable once SuggestBaseTypeForParameter
        public DynamicPagesDbContext(DbContextOptions<EntitiesDbContext> options) : base(options)
        {
        }

        public virtual DbSet<Page> Pages { get; set; }
        public virtual DbSet<PageType> PageTypes { get; set; }
        public virtual DbSet<PageSettings> PageSettings { get; set; }
        public virtual DbSet<Block> Blocks { get; set; }
        public virtual DbSet<BlockCategory> BlockCategories { get; set; }
        public virtual DbSet<BlockAttribute> BlockAttributes { get; set; }
        public virtual DbSet<PageScript> PageScripts { get; set; }
        public virtual DbSet<PageStyle> PageStyles { get; set; }
        public virtual DbSet<Template> Templates { get; set; }
        public virtual DbSet<ViewModel> ViewModels { get; set; }
        public virtual DbSet<ViewModelFields> ViewModelFields { get; set; }
        public virtual DbSet<RolePagesAcl> RolePagesAcls { get; set; }

        /// <summary>
        /// Entity types
        /// </summary>
        [NotMapped]
        public override DbSet<EntityType> EntityTypes { get; set; }
        /// <summary>
        /// Tables
        /// </summary>
        [NotMapped]
        public override DbSet<TableModel> Table { get; set; }
        /// <summary>
        /// Table configs
        /// </summary>
        [NotMapped]
        public override DbSet<TableFieldConfigs> TableFieldConfigs { get; set; }
        /// <summary>
        /// Table config values
        /// </summary>
        [NotMapped]
        public override DbSet<TableFieldConfigValue> TableFieldConfigValues { get; set; }
        /// <summary>
        /// Field groups
        /// </summary>
        [NotMapped]
        public override DbSet<TableFieldGroups> TableFieldGroups { get; set; }
        /// <summary>
        /// Table fields
        /// </summary>
        [NotMapped]
        public override DbSet<TableModelField> TableFields { get; set; }
        /// <summary>
        /// Field types
        /// </summary>
        [NotMapped]
        public override DbSet<TableFieldType> TableFieldTypes { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.RegisterIndexes();
            builder.Entity<RolePagesAcl>().HasKey(x => new { x.RoleId, x.PageId });

            builder.Entity<EntityType>().ToTable(nameof(EntityTypes), ParentSchema);
            builder.Entity<TableModel>().ToTable(nameof(Table), ParentSchema);
            builder.Entity<TableFieldConfigs>().ToTable(nameof(TableFieldConfigs), ParentSchema);
            builder.Entity<TableFieldConfigValue>().ToTable(nameof(TableFieldConfigValues), ParentSchema);
            builder.Entity<TableFieldGroups>().ToTable(nameof(TableFieldGroups), ParentSchema);
            builder.Entity<TableModelField>().ToTable(nameof(TableFields), ParentSchema);
            builder.Entity<TableFieldType>().ToTable(nameof(TableFieldTypes), ParentSchema);
            builder.Entity<EntityPermission>().ToTable(nameof(EntityPermissions), ParentSchema);
            builder.Entity<EntityFieldPermission>().ToTable(nameof(EntityFieldPermissions), ParentSchema);
            builder.Entity<EntityPermissionAccess>().ToTable(nameof(EntityPermissionAccesses), ParentSchema);

            builder.Ignore<EntityType>();
            builder.Ignore<TableModel>();
            builder.Ignore<TableFieldConfigs>();
            builder.Ignore<TableFieldConfigValue>();
            builder.Ignore<TableFieldGroups>();
            builder.Ignore<TableModelField>();
            builder.Ignore<TableFieldType>();
            builder.Ignore<EntityPermission>();
            builder.Ignore<EntityFieldPermission>();
            builder.Ignore<EntityPermissionAccess>();
        }
    }
}