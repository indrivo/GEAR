using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ST.Audit.Contexts;
using ST.Entities.Extensions;
using ST.Entities.Models.Forms;
using ST.Entities.Models.Pages;
using ST.Entities.Models.RenderTemplates;
using ST.Entities.Models.Tables;
using ST.Entities.Models.ViewModels;

namespace ST.Entities.Data
{
    public class EntitiesDbContext : TrackerDbContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Entities";

        /// <summary>
        /// Options
        /// </summary>
        /// <param name="options"></param>
        public EntitiesDbContext(DbContextOptions<EntitiesDbContext> options)
                    : base(options)
        {

        }

        #region Table

        public DbSet<EntityType> EntityTypes { get; set; }
        public DbSet<TableModel> Table { get; set; }
        public DbSet<TableFieldConfigs> TableFieldConfigs { get; set; }
        public DbSet<TableFieldConfigValues> TableFieldConfigValues { get; set; }
        public DbSet<TableFieldGroups> TableFieldGroups { get; set; }
        public DbSet<TableModelFields> TableFields { get; set; }
        public DbSet<TableFieldTypes> TableFieldTypes { get; set; }

        #endregion Table

        #region Forms

        public DbSet<Attrs> Attrs { get; set; }
        public DbSet<ColumnField> ColumnFields { get; set; }
        public DbSet<Column> Columns { get; set; }
        public DbSet<Config> Configs { get; set; }
        public DbSet<DisabledAttr> DisabledAttrs { get; set; }
        public DbSet<Field> Fields { get; set; }
        public DbSet<FormType> FormTypes { get; set; }
        public DbSet<Form> Forms { get; set; }
        public DbSet<Meta> Meta { get; set; }
        public DbSet<Option> Options { get; set; }
        public DbSet<RowColumn> RowColumns { get; set; }
        public DbSet<Row> Rows { get; set; }
        public DbSet<Settings> Settings { get; set; }
        public DbSet<StageRows> StageRows { get; set; }
        public DbSet<Stage> Stages { get; set; }

        #endregion Forms

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
            builder.Entity<TableFieldTypes>().HasKey(ug => new { ug.Id });
            builder.Entity<TableFieldConfigs>().HasKey(ug => new { ug.Id });
            builder.Entity<TableFieldConfigValues>().HasKey(ug => new { ug.TableModelFieldId, ug.TableFieldConfigId });
            builder.Entity<TableModelFields>().HasOne(typeof(TableFieldTypes), "TableFieldType").WithMany().OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Field>().HasOne(model => model.TableField).WithMany().HasForeignKey(model => model.TableFieldId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Form>().HasOne(model => model.Type).WithMany().HasForeignKey(model => model.TypeId).OnDelete(DeleteBehavior.Restrict);
            builder.RegisterIndexes();
        }
    }

    /// <summary>
    /// Context factory design
    /// </summary>
    public class EntitiesDbContextFactory : IDesignTimeDbContextFactory<EntitiesDbContext>
    {
        /// <inheritdoc />
        /// <summary>
        /// For creating migrations
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public EntitiesDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<EntitiesDbContext>();
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Username=postgres;Password=1111;Database=ISODMS.DEV;");
            return new EntitiesDbContext(optionsBuilder.Options);
        }
    }
}