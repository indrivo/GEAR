using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using GR.Audit.Contexts;
using Microsoft.EntityFrameworkCore;
using GR.Entities.Abstractions.Models.Tables;
using GR.PageRender.Abstractions;
using GR.PageRender.Abstractions.Helpers;
using GR.PageRender.Abstractions.Models.Pages;
using GR.PageRender.Abstractions.Models.PagesACL;
using GR.PageRender.Abstractions.Models.RenderTemplates;
using GR.PageRender.Abstractions.Models.ViewModels;
using GR.PageRender.Extensions;

namespace GR.PageRender.Data
{
    public class DynamicPagesDbContext : TrackerDbContext, IDynamicPagesContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        // ReSharper disable once MemberCanBePrivate.Global
        public const string Schema = "Pages";

        private const string ParentSchema = "Entities";

        public static bool IsMigrationMode { get; set; } = false;

        // ReSharper disable once SuggestBaseTypeForParameter
        public DynamicPagesDbContext(DbContextOptions<DynamicPagesDbContext> options) : base(options)
        {
        }

        #region DbSets
        /// <summary>
        /// Pages
        /// </summary>
        public virtual DbSet<Page> Pages { get; set; }
        /// <summary>
        /// Page types
        /// </summary>
        public virtual DbSet<PageType> PageTypes { get; set; }
        /// <summary>
        /// Page settings
        /// </summary>
        public virtual DbSet<PageSettings> PageSettings { get; set; }
        /// <summary>
        /// UI Blocks
        /// </summary>
        public virtual DbSet<Block> Blocks { get; set; }
        /// <summary>
        /// UI block categories
        /// </summary>
        public virtual DbSet<BlockCategory> BlockCategories { get; set; }
        /// <summary>
        /// UI block attributes
        /// </summary>
        public virtual DbSet<BlockAttribute> BlockAttributes { get; set; }
        /// <summary>
        /// Page scripts
        /// </summary>
        public virtual DbSet<PageScript> PageScripts { get; set; }
        /// <summary>
        /// Page styles
        /// </summary>
        public virtual DbSet<PageStyle> PageStyles { get; set; }
        /// <summary>
        /// UI templates
        /// </summary>
        public virtual DbSet<Template> Templates { get; set; }
        /// <summary>
        /// List view models
        /// </summary>
        public virtual DbSet<ViewModel> ViewModels { get; set; }
        /// <summary>
        /// List view model fields
        /// </summary>
        public virtual DbSet<ViewModelFields> ViewModelFields { get; set; }
        /// <summary>
        /// Page ACL
        /// </summary>
        public virtual DbSet<RolePagesAcl> RolePagesAcls { get; set; }
        /// <summary>
        /// View model fields configuration codes
        /// </summary>
        public virtual DbSet<ViewModelFieldCode> ViewModelFieldCodesCodes { get; set; }
        /// <summary>
        /// View model field configurations
        /// </summary>
        public virtual DbSet<ViewModelFieldConfiguration> ViewModelFieldConfigurations { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Entity types
        /// </summary>
        [NotMapped]
        public DbSet<EntityType> EntityTypes { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Tables
        /// </summary>
        [NotMapped]
        public DbSet<TableModel> Table { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Table configs
        /// </summary>
        [NotMapped]
        public DbSet<TableFieldConfigs> TableFieldConfigs { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Table config values
        /// </summary>
        [NotMapped]
        public DbSet<TableFieldConfigValue> TableFieldConfigValues { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Field groups
        /// </summary>
        [NotMapped]
        public DbSet<TableFieldGroups> TableFieldGroups { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Table fields
        /// </summary>
        [NotMapped]
        public DbSet<TableModelField> TableFields { get; set; }
        /// <inheritdoc />
        /// <summary>
        /// Field types
        /// </summary>
        [NotMapped]
        public DbSet<TableFieldType> TableFieldTypes { get; set; }
        #endregion

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
            builder.Entity<ViewModelFieldCode>().HasKey(x => x.Code);
            builder.Entity<ViewModelFieldConfiguration>().HasKey(x => new
            {
                x.ViewModelFieldCodeId,
                x.ViewModelFieldId
            });

            builder.Entity<TableFieldType>().HasKey(ug => new { ug.Id });
            builder.Entity<TableFieldConfigs>().HasKey(ug => new { ug.Id });
            builder.Entity<TableFieldConfigValue>().HasKey(ug => new { ug.TableModelFieldId, ug.TableFieldConfigId });
            builder.Entity<TableModelField>().HasOne(typeof(TableFieldType), "TableFieldType").WithMany().OnDelete(DeleteBehavior.Restrict);

            #region Entity base dependecies
            //This block eliminate the base entities to be included on migration
            if (IsMigrationMode)
            {
                builder.Ignore<EntityType>();
                builder.Ignore<TableModel>();
                builder.Ignore<TableFieldConfigs>();
                builder.Ignore<TableFieldConfigValue>();
                builder.Ignore<TableFieldGroups>();
                builder.Ignore<TableModelField>();
                builder.Ignore<TableFieldType>();
            }

            #endregion
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            DynamicPagesDbContextSeeder<DynamicPagesDbContext>.SeedAsync(this).Wait();
            PageSeeder.SyncWebPagesAsync().Wait();
            TemplateManager.SeedAsync().Wait();
            return Task.CompletedTask;
        }
    }
}