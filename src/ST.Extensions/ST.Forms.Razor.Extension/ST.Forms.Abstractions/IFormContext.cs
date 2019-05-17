using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.Forms.Abstractions.Models.FormModels;

namespace ST.Forms.Abstractions
{
    public interface IFormContext : IDbContext
    {
         DbSet<Attrs> Attrs { get; set; }
         DbSet<ColumnField> ColumnFields { get; set; }
         DbSet<Column> Columns { get; set; }
         DbSet<Config> Configs { get; set; }
         DbSet<DisabledAttr> DisabledAttrs { get; set; }
         DbSet<Field> Fields { get; set; }
         DbSet<FormType> FormTypes { get; set; }
         DbSet<Form> Forms { get; set; }
         DbSet<Meta> Meta { get; set; }
         DbSet<Option> Options { get; set; }
         DbSet<RowColumn> RowColumns { get; set; }
         DbSet<Row> Rows { get; set; }
         DbSet<Settings> Settings { get; set; }
         DbSet<StageRows> StageRows { get; set; }
         DbSet<Stage> Stages { get; set; }
         DbSet<FormFieldEvent> FormFieldEvents { get; set; }
    }
}
