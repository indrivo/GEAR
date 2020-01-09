using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GR.Audit.Contexts;
using GR.Core;
using GR.Core.Abstractions;
using GR.Forms.Abstractions;
using GR.Forms.Abstractions.Models.FormModels;
using GR.Forms.Extensions;
using Settings = GR.Forms.Abstractions.Models.FormModels.Settings;

namespace GR.Forms.Data
{
    public class FormDbContext : TrackerDbContext, IFormContext
    {
        /// <summary>
        /// Schema
        /// Do not remove this
        /// </summary>
        public const string Schema = "Forms";

        /// <inheritdoc />
        /// <summary>
        /// Options
        /// </summary>
        /// <param name="options"></param>
        public FormDbContext(DbContextOptions<FormDbContext> options)
            : base(options)
        {

        }

        /// <summary>
        /// Form Attributes
        /// </summary>
        public virtual DbSet<Attrs> Attrs { get; set; }

        /// <summary>
        /// Column fields
        /// </summary>
        public virtual DbSet<ColumnField> ColumnFields { get; set; }

        /// <summary>
        /// Form columns
        /// </summary>
        public virtual DbSet<Column> Columns { get; set; }

        /// <summary>
        /// Additional configuration 
        /// </summary>
        public virtual DbSet<Config> Configs { get; set; }

        /// <summary>
        /// Disabled attributes
        /// </summary>
        public virtual DbSet<DisabledAttr> DisabledAttrs { get; set; }

        /// <summary>
        /// Form fields
        /// </summary>
        public virtual DbSet<Field> Fields { get; set; }

        /// <summary>
        /// Form types
        /// </summary>
        public virtual DbSet<FormType> FormTypes { get; set; }

        /// <summary>
        /// Forms
        /// </summary>
        public virtual DbSet<Form> Forms { get; set; }

        /// <summary>
        /// Forms meta information
        /// </summary>
        public virtual DbSet<Meta> Meta { get; set; }

        /// <summary>
        /// Options for input fields
        /// </summary>
        public virtual DbSet<Option> Options { get; set; }

        /// <summary>
        /// Mapped columns for each row
        /// </summary>
        public virtual DbSet<RowColumn> RowColumns { get; set; }

        /// <summary>
        /// Form rows
        /// </summary>
        public virtual DbSet<Row> Rows { get; set; }

        /// <summary>
        /// Form settings
        /// </summary>
        public virtual DbSet<Settings> Settings { get; set; }

        /// <summary>
        /// Mapped rows to each stage
        /// </summary>
        public virtual DbSet<StageRows> StageRows { get; set; }

        /// <summary>
        /// Form stages
        /// </summary>
        public virtual DbSet<Stage> Stages { get; set; }

        /// <summary>
        /// Registered fields events
        /// </summary>
        public virtual DbSet<FormFieldEvent> FormFieldEvents { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// On model creating
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema(Schema);
            builder.Entity<Form>().HasOne(model => model.Type).WithMany().HasForeignKey(model => model.TypeId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Attrs>().HasOne(model => model.Row).WithMany().HasForeignKey(model => model.RowId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Row>().HasMany(model => model.Attrs).WithOne(x => x.Row).OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Field>().HasMany(model => model.Attrs).WithOne(x => x.Field).OnDelete(DeleteBehavior.Cascade);
            builder.RegisterIndexes();
            builder.Entity<Field>().Ignore(x => x.TableField);
        }

        /// <summary>
        /// Seed data
        /// </summary>
        /// <returns></returns>
        public override Task InvokeSeedAsync(IServiceProvider services)
        {
            FormDbContextSeeder<FormDbContext>.SeedAsync(this, GearSettings.TenantId).Wait();
            return Task.CompletedTask;
        }
    }
}
