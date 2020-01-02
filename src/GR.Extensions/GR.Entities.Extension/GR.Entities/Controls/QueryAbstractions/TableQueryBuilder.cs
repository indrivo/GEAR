using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.Query;
using GR.Entities.Abstractions.ViewModels.Table;

namespace GR.Entities.Controls.QueryAbstractions
{
    public abstract class TableQueryBuilder : IQueryTableBuilder
    {
        public abstract string AddFieldQuery(CreateTableFieldViewModel field, string tableName, string tableSchema);

        public abstract string CheckColumnValues(string tableName, string columnName, string tableSchema);

        public abstract string CheckTableValues(string tableName, string tableSchema);

        public abstract string CreateQuery(TableModel table);

        public abstract string DropColumn(string tableName, string tableSchema, string columnName);

        public abstract string DropConstraint(string tableName, string tableSchema, string constraint,
            string columnName);

        public abstract string DropTable(string tableName, string tableSchema);

        public abstract string GetColumnsQuery(string tableName);

        public abstract string GetTablesQuery();

        public abstract string UpdateFieldQuery(CreateTableFieldViewModel field, string tableName, string tableSchema);

        public abstract string GetSchemaSqlScript(string schemaName);

        public abstract string GetDbSchemesSqlScript();

        public abstract (bool, string) IsSqlServerConnected(string connectionString);
    }
}