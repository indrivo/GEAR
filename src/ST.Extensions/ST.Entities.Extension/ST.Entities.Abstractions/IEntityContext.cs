using Microsoft.EntityFrameworkCore;
using ST.Core.Abstractions;
using ST.Entities.Abstractions.Models.Tables;

namespace ST.Entities.Abstractions
{
    public interface IEntityContext : IDbContext
    {
         DbSet<EntityType> EntityTypes { get; set; }
         DbSet<TableModel> Table { get; set; }
         DbSet<TableFieldConfigs> TableFieldConfigs { get; set; }
         DbSet<TableFieldConfigValue> TableFieldConfigValues { get; set; }
         DbSet<TableFieldGroups> TableFieldGroups { get; set; }
         DbSet<TableModelField> TableFields { get; set; }
         DbSet<TableFieldType> TableFieldTypes { get; set; }
    }
}
