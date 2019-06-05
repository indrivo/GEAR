using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.Entities.Abstractions;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Extensions;
using ST.Entities.Models.Pages;
using ST.Entities.Models.RenderTemplates;
using ST.Entities.Models.ViewModels;
using ST.Entities.Security.Data;

namespace ST.Entities.Data
{
    public class EntitiesDbContext : EntitySecurityDbContext, IEntityContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Entities";

        /// <inheritdoc />
        /// <summary>
        /// Options
        /// </summary>
        /// <param name="options"></param>
        public EntitiesDbContext(DbContextOptions<EntitiesDbContext> options)
                    : base(options)
        {

        }

        #region Table
        /// <summary>
        /// Entity types
        /// </summary>
        public DbSet<EntityType> EntityTypes { get; set; }
        /// <summary>
        /// Tables
        /// </summary>
        public DbSet<TableModel> Table { get; set; }
        /// <summary>
        /// Table configs
        /// </summary>
        public DbSet<TableFieldConfigs> TableFieldConfigs { get; set; }
        /// <summary>
        /// Table config values
        /// </summary>
        public DbSet<TableFieldConfigValue> TableFieldConfigValues { get; set; }
        /// <summary>
        /// Field groups
        /// </summary>
        public DbSet<TableFieldGroups> TableFieldGroups { get; set; }
        /// <summary>
        /// Table fields
        /// </summary>
        public DbSet<TableModelField> TableFields { get; set; }
        /// <summary>
        /// Field types
        /// </summary>
        public DbSet<TableFieldType> TableFieldTypes { get; set; }
        #endregion Table

        #region  Pages
        public DbSet<Page> Pages { get; set; }
        public DbSet<PageType> PageTypes { get; set; }
        public DbSet<PageSettings> PageSettings { get; set; }
        public DbSet<Block> Blocks { get; set; }
        public DbSet<BlockCategory> BlockCategories { get; set; }
        public DbSet<BlockAttribute> BlockAttributes { get; set; }
        public DbSet<PageScript> PageScripts { get; set; }
        public DbSet<PageStyle> PageStyles { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<ViewModel> ViewModels { get; set; }
        public DbSet<ViewModelFields> ViewModelFields { get; set; }
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
            builder.Entity<TableFieldType>().HasKey(ug => new { ug.Id });
            builder.Entity<TableFieldConfigs>().HasKey(ug => new { ug.Id });
            builder.Entity<TableFieldConfigValue>().HasKey(ug => new { ug.TableModelFieldId, ug.TableFieldConfigId });
            builder.Entity<TableModelField>().HasOne(typeof(TableFieldType), "TableFieldType").WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.RegisterIndexes();
        }

        /// <inheritdoc />
        /// <summary>
        /// Set entity
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public virtual DbSet<TEntity> SetEntity<TEntity>() where TEntity : class, IBaseModel
        {
            return Set<TEntity>();
        }
    }
}