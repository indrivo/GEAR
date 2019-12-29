using System;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using GR.Entities.Abstractions.Constants;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.Table;
using GR.Entities.Controls.Builders;
using GR.Entities.Controls.QueryAbstractions;

namespace GR.Entities.EntityBuilder.MsSql.Controls.Query
{
    public class MsSqlTableQueryBuilder : TableQueryBuilder
    {
        public override string AddFieldQuery(CreateTableFieldViewModel field, string tableName, string tableSchema)
        {
            var sql = new StringBuilder();
            var alterSql = new StringBuilder();
            sql.AppendFormat(" ALTER TABLE [{1}].[{0}] ", tableName, tableSchema);
            sql.AppendFormat("ADD  [{0}]", field.Name);
            var defaultValue = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.DefaultValue)?.Value;
            switch (field.DataType.Trim())
            {
                case "smallint":
                    sql.Append(" smallint");
                    if (defaultValue != null) sql.AppendFormat(" default '{0}' ", defaultValue);
                    break;

                case "int":
                    sql.Append(" int");
                    if (defaultValue != null) sql.AppendFormat(" default '{0}' ", defaultValue);
                    break;

                case "bigint":
                    sql.Append(" bigint");
                    if (defaultValue != null) sql.AppendFormat(" default '{0}' ", defaultValue);
                    break;

                case "uniqueidentifier":
                    sql.Append(" uniqueidentifier");
                    break;

                case "date":
                    sql.Append(" date");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    break;

                case "datetime":
                    sql.Append(" datetime");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    break;

                case "datetime2":
                    sql.Append(" datetime2");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    break;

                case "time":
                    sql.Append(" time");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    break;

                case "nvarchar":
                    var maxLenght = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.ContentLen)?.Value;
                    if (string.IsNullOrEmpty(maxLenght) || maxLenght.Trim() == "0") sql.AppendFormat(" nvarchar(max)");
                    else sql.AppendFormat(" nvarchar({0})", maxLenght);
                    if (defaultValue != null) sql.AppendFormat(" default '{0}' ", defaultValue);
                    break;

                case "double":
                    sql.Append(" double");
                    if (defaultValue != null) sql.AppendFormat(" default '{0}' ", defaultValue);
                    break;

                case "decimal":
                    var precision = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.Precision)?.Value;
                    var scale = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.Scale)?.Value;
                    if (string.IsNullOrEmpty(scale) && string.IsNullOrEmpty(precision))
                        sql.AppendFormat(" decimal(18,2)");
                    else sql.AppendFormat(" decimal({0},{1})", precision, scale);
                    if (defaultValue != null) sql.AppendFormat(" default '{0}' ", defaultValue);
                    break;

                case "bool":
                    sql.Append(" bit");
                    if (!field.AllowNull)
                    {
                        if (defaultValue != null) sql.AppendFormat(" default '{0}' ", defaultValue);
                        else sql.AppendFormat(" default '0' ");
                    }

                    break;

                case "char":
                    sql.Append(" char");
                    if (defaultValue != null) sql.AppendFormat(" default '{0}'", defaultValue);
                    break;

                default:
                    sql.AppendFormat(" nvarchar({0})", "10");
                    if (defaultValue != null) sql.AppendFormat(" default '{0}'", defaultValue);
                    break;
            }

            if (!field.AllowNull) sql.Append(" NOT NULL");

            if (field.Parameter == FieldType.EntityReference || field.Parameter == FieldType.File)
            {
                var foreingTableName =
                    field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.ForeingTable)?.Value;
                var foreingTableSchemaName =
                   field.Configurations.FirstOrDefault(x => x.ConfigCode.Equals("9999"))?.Value;
                var constraintName = foreingTableName + "_" + field.Name;
                if (foreingTableName != null)
                    sql.AppendFormat(" CONSTRAINT FK_{0} FOREIGN KEY REFERENCES {3}.{1}({2})", constraintName,
                        foreingTableName, "Id", foreingTableSchemaName);
            }

            sql.AppendFormat("{0}", alterSql);
            return sql.ToString();
        }

        public override string CheckColumnValues(string tableName, string columnName, string tableSchema)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("  SELECT * FROM {2}.{0} WHERE {1} is not null", tableName, columnName, tableSchema);
            return sql.ToString();
        }

        public override string CheckTableValues(string tableName, string tableSchema)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(" SELECT TOP 1 * FROM {1}.{0}", tableName, tableSchema);
            return sql.ToString();
        }

        public override string CreateQuery(TableModel table)
        {
            var sql = new StringBuilder();
            var alterSql = new StringBuilder();
            var baseModelFields = BaseModelBuilder.CreateBaseModel(table.EntityType);
            sql.AppendFormat("CREATE TABLE [{1}].[{0}] (", table.Name, table.EntityType);
            foreach (var item in baseModelFields)
            {
                sql.AppendFormat(" [{0}]", item.Name);
                switch (item.DataType.Trim())
                {
                    case "uniqueidentifier":
                        sql.Append(" uniqueidentifier");
                        break;

                    case "datetime":
                        sql.Append(" datetime");
                        break;

                    case "nvarchar":
                        sql.AppendFormat(" nvarchar({0})", item.MaxLenght);
                        break;

                    case "bit":
                        sql.Append(" bit");
                        break;

                    default:
                        sql.AppendFormat(" nvarchar({0})", item.MaxLenght);
                        break;
                }

                if (!item.AllowNull) sql.Append(" NOT NULL");

                sql.Append(",");
            }

            sql.AppendFormat(" CONSTRAINT PK_{0} PRIMARY KEY ({1})", table.Name, "Id");
            sql.AppendFormat("{0})", alterSql);
            return sql.ToString();
        }

        public override string DropColumn(string tableName, string tableSchema, string columnName)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("ALTER TABLE {2}.{0} DROP COLUMN {1}", tableName, columnName, tableSchema);
            return sql.ToString();
        }

        public override string DropConstraint(string tableName, string tableSchema, string constraint, string columnName)
        {
            var sql = new StringBuilder();
            var contraint = "FK_" + constraint + "_" + columnName;
            sql.AppendFormat("ALTER TABLE {2}.{0} DROP constraint  {1}", tableName, contraint, tableSchema);
            return sql.ToString();
        }

        public override string DropTable(string tableName, string tableSchema)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("DROP TABLE {1}.{0}", tableName, tableSchema);
            return sql.ToString();
        }

        public override string GetColumnsQuery(string tableName)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(
                "select COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, NUMERIC_PRECISION, DATETIME_PRECISION,IS_NULLABLE from INFORMATION_SCHEMA.COLUMNS where TABLE_NAME = '{0}'",
                tableName);
            return sql.ToString();
        }

        public override string GetTablesQuery()
        {
            var sql = new StringBuilder();
            sql.AppendFormat("SELECT * FROM INFORMATION_SCHEMA.TABLES");
            //  sql.AppendFormat("USE [BPMNS.db] GO SELECT * FROM INFORMATION_SCHEMA.TABLES", dataBase);
            return sql.ToString();
        }

        public override string UpdateFieldQuery(CreateTableFieldViewModel field, string tableName, string tableSchema)
        {
            var sql = new StringBuilder();
            var alterSql = new StringBuilder();
            sql.AppendFormat(" ALTER TABLE [{1}].[{0}] ", tableName, tableSchema);
            sql.AppendFormat(" ALTER COLUMN [{0}] ", field.Name);
            var defaultValue = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.DefaultValue)?.Value;
            switch (field.DataType.Trim())
            {
                case "smallint":
                    sql.Append(" smallint");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                case "int":
                    sql.Append(" int");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                case "bigint":
                    sql.Append(" bigint");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                case "uniqueidentifier":
                    sql.Append(" uniqueidentifier");
                    break;

                case "date":
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    else sql.AppendFormat(" default drop");
                    break;

                case "datetime":
                    sql.Append(" datetime");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    else sql.AppendFormat(" default drop");
                    break;

                case "datetime2":
                    sql.Append(" datetime2");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    else sql.AppendFormat(" default drop");
                    break;

                case "time":
                    sql.Append(" time");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    else sql.AppendFormat(" default drop");
                    break;

                case "nvarchar":
                    var maxLenght = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.ContentLen)?.Value;
                    sql.AppendFormat(" nvarchar({0})", maxLenght);
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                case "double":
                    sql.Append(" double");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                case "decimal":
                    var precision = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.Precision)?.Value;
                    var scale = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.Scale)?.Value;
                    sql.AppendFormat(" decimal({0},{1})", precision, scale);
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                case "bool":
                    sql.Append(" bit");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                case "char":
                    sql.Append(" char");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                default:
                    sql.AppendFormat(" nvarchar({0})", "10");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;
            }

            if (!field.AllowNull) sql.Append(" NOT NULL");

            if (field.Parameter == FieldType.EntityReference)
            {
                var foreingTableName =
                    field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.ForeingTable)?.Value;
                //var foreingTableSchemaName =
                //    field.Configurations.FirstOrDefault(x => x.ConfigCode.Equals("9999"))?.Value;
                sql.AppendFormat(" FOREIGN KEY REFERENCES {2}.{0}({1})", foreingTableName, "Id", tableSchema);
            }

            sql.AppendFormat("{0}", alterSql);
            return sql.ToString();
        }

        /// <summary>
        /// Get sql script for add new schema 
        /// </summary>
        /// <param name="schemaName"></param>
        /// <returns></returns>
        public override string GetSchemaSqlScript(string schemaName)
        {
            var sql = $"CREATE SCHEMA {schemaName}";
            return sql;
        }

        /// <summary>
        /// Get all schema
        /// </summary>
        /// <returns></returns>
        public override string GetDbSchemesSqlScript()
        {
            return "SELECT schema_name FROM information_schema.schemata";
        }

        /// <summary>
        /// Check if Sql server is available
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public override (bool, string) IsSqlServerConnected(string connectionString)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return (true, "");
                }
                catch (Exception e)
                {
                    return (false, e.ToString());
                }
                finally
                {
                    // not really necessary
                    connection.Close();
                }
            }
        }
    }
}