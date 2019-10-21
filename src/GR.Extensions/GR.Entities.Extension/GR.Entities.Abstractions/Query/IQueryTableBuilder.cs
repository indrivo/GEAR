using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.Table;

namespace GR.Entities.Abstractions.Query
{
    public interface IQueryTableBuilder
    {
        string AddFieldQuery(CreateTableFieldViewModel field, string tableName, string tableSchema);
        string CheckColumnValues(string tableName, string columnName, string tableSchema);
        string CheckTableValues(string tableName, string tableSchema);
        string CreateQuery(TableModel table);
        string DropColumn(string tableName, string tableSchema, string columnName);

        string DropConstraint(string tableName, string tableSchema, string constraint,
           string columnName);

        string DropTable(string tableName, string tableSchema);
        string GetColumnsQuery(string tableName);
        string GetTablesQuery();
        string UpdateFieldQuery(CreateTableFieldViewModel field, string tableName, string tableSchema);
        string GetSchemaSqlScript(string schemaName);
        string GetDbSchemesSqlScript();
        (bool, string) IsSqlServerConnected(string connectionString);
    }
}
