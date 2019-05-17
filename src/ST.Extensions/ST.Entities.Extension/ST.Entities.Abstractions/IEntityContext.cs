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
         DbSet<TableFieldConfigValues> TableFieldConfigValues { get; set; }
         DbSet<TableFieldGroups> TableFieldGroups { get; set; }
         DbSet<TableModelFields> TableFields { get; set; }
         DbSet<TableFieldTypes> TableFieldTypes { get; set; }
    }
}
