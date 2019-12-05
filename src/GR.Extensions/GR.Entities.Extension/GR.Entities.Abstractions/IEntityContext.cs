using Microsoft.EntityFrameworkCore;
using GR.Core.Abstractions;
using GR.Entities.Abstractions.Models.Tables;

namespace GR.Entities.Abstractions
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
