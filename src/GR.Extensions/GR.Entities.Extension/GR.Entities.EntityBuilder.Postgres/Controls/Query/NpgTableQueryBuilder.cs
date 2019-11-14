using System;
using System.Linq;
using System.Text;
using Npgsql;
using GR.Entities.Abstractions.Constants;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Abstractions.ViewModels.Table;
using GR.Entities.Controls.Builders;
using GR.Entities.Controls.QueryAbstractions;

namespace GR.Entities.EntityBuilder.Postgres.Controls.Query
{
    public class NpgTableQueryBuilder : TableQueryBuilder
    {
        public override string AddFieldQuery(CreateTableFieldViewModel field, string tableName, string tableSchema)
        {
            var sql = new StringBuilder();
            var alterSql = new StringBuilder();
            sql.AppendFormat(" ALTER TABLE \"{1}\".\"{0}\" ", tableName, tableSchema);
            sql.AppendFormat("ADD COLUMN \"{0}\"", field.Name);
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
                    sql.Append(" uuid");
                    break;

                case "date":
                    sql.Append(" date");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    break;

                case "datetime":
                    sql.Append(" TIMESTAMP");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    break;

                case "datetime2":
                    sql.Append(" TIMESTAMP");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    break;

                case "time":
                    sql.Append(" time");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    break;

                case "nvarchar":
                    var maxLength = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.ContentLen)?.Value;
                    if (string.IsNullOrEmpty(maxLength) || maxLength.Trim() == "0") sql.AppendFormat(" varchar(3999)");
                    else sql.AppendFormat(" varchar({0})", maxLength);
                    if (defaultValue != null) sql.AppendFormat(" default '{0}' ", defaultValue);
                    break;

                case "double":
                    sql.Append(" double precision");
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
                    sql.Append(" boolean");
                    if (!field.AllowNull)
                    {
                        if (defaultValue != null) sql.AppendFormat(" default '{0}' ", defaultValue);
                        else sql.AppendFormat(" default '0' ");
                    }

                    break;

                case "char":
                    sql.Append(" CHAR");
                    if (defaultValue != null) sql.AppendFormat(" DEFAULT '{0}'", defaultValue);
                    break;

                default:
                    sql.AppendFormat(" VARCHAR({0})", "10");
                    if (defaultValue != null) sql.AppendFormat(" DEFAULT '{0}'", defaultValue);
                    break;
            }

            sql.Append(!field.AllowNull ? " NOT NULL" : " NULL");

            if (field.Parameter == FieldType.EntityReference || field.Parameter == FieldType.File)
            {
                var foreignTableName =
                    field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.ForeingTable)?.Value;
                var foreignSchemaTableName = field.Configurations.FirstOrDefault(x => x.ConfigCode == "9999")?.Value;
                var constraintName = foreignTableName + "_" + field.Name;
                if (foreignTableName != null)
                    sql.AppendFormat(" ,ADD CONSTRAINT FK_{0} FOREIGN KEY (\"{3}\") REFERENCES \"{4}\".\"{1}\"({2})", constraintName,
                        foreignTableName, "\"Id\"", field.Name, foreignSchemaTableName);
            }

            sql.AppendFormat("{0}", alterSql);
            return sql.ToString();
        }

        public override string CheckColumnValues(string tableName, string tableSchema, string columnName)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(" SELECT * FROM \"{1}\".\"{0}\" WHERE \"{2}\" IS NOT NULL", tableName, tableSchema, columnName);
            return sql.ToString();
        }

        public override string CheckTableValues(string tableName, string tableSchema)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(" SELECT * FROM \"{1}\".\"{0}\" LIMIT 1", tableName, tableSchema);
            return sql.ToString();
        }

        public override string CreateQuery(TableModel table)
        {
            var sql = new StringBuilder();
            var alterSql = new StringBuilder();
            var baseModelFields = BaseModelBuilder.CreateBaseModel(table.EntityType);
            sql.AppendFormat("CREATE TABLE \"{1}\".\"{0}\" (", table.Name, table.EntityType);
            foreach (var item in baseModelFields)
            {
                sql.AppendFormat(" \"{0}\"", item.Name);
                switch (item.DataType.Trim())
                {
                    case "uniqueidentifier":
                        sql.Append(" uuid");
                        break;

                    case "datetime":
                        sql.Append(" TIMESTAMP");
                        break;

                    case "nvarchar":
                        sql.AppendFormat(" varchar({0})", item.MaxLenght);
                        break;

                    case "bit":
                        sql.Append(" boolean");
                        break;

                    default:
                        sql.AppendFormat(" varchar({0})", item.MaxLenght);
                        break;
                }

                if (!item.AllowNull) sql.Append(" NOT NULL");

                sql.Append(",");
            }

            sql.AppendFormat(" CONSTRAINT PK_{0} PRIMARY KEY ({1})", table.Name, "\"Id\"");
            sql.AppendFormat("{0})", alterSql);
            return sql.ToString();
        }
        /// <summary>
        /// Drop column
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="columnName"></param>
        /// <param name="tableSchema"></param>
        /// <returns></returns>
        public override string DropColumn(string tableName, string columnName, string tableSchema)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("ALTER TABLE \"{2}\".\"{0}\" DROP COLUMN \"{1}\"", tableName, columnName, tableSchema);
            return sql.ToString();
        }

        public override string DropConstraint(string tableName, string constraint, string columnName, string tableSchema)
        {
            var sql = new StringBuilder();
            constraint = "FK_" + constraint + "_" + columnName;
            sql.AppendFormat("ALTER TABLE \"{2}\".\"{0}\" DROP constraint  {1}", tableName, constraint, tableSchema);
            return sql.ToString();
        }

        public override string DropTable(string tableName, string tableSchema)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("DROP TABLE \"{1}\".\"{0}\"", tableName, tableSchema);
            return sql.ToString();
        }

        public override string GetColumnsQuery(string tableName)
        {
            throw new NotImplementedException();
        }

        public override string GetTablesQuery()
        {
            throw new NotImplementedException();
        }

        public override string UpdateFieldQuery(CreateTableFieldViewModel field, string tableName, string tableSchema)
        {
            var sql = new StringBuilder();
            var alterSql = new StringBuilder();
            sql.AppendFormat(" ALTER TABLE \"{1}\".\"{0}\" ", tableName, tableSchema);
            sql.AppendFormat(" ALTER COLUMN \"{0}\" TYPE", field.Name);
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
                    sql.Append(" uuid");
                    break;

                case "date":
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    else sql.AppendFormat(" default drop");
                    break;

                case "datetime":
                    sql.Append(" TIMESTAMP");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    else sql.AppendFormat(" default drop");
                    break;

                case "datetime2":
                    sql.Append(" TIMESTAMP");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    else sql.AppendFormat(" default drop");
                    break;

                case "time":
                    sql.Append(" time");
                    if (defaultValue != null) sql.AppendFormat(" default {0} ", defaultValue);
                    else sql.AppendFormat(" default drop");
                    break;

                case "nvarchar":
                    var maxLength = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.ContentLen)?.Value;
                    if (string.IsNullOrEmpty(maxLength) || maxLength.Trim() == "0") sql.AppendFormat(" varchar(3999)");
                    else sql.AppendFormat(" varchar({0})", maxLength);
                    if (defaultValue != null) sql.AppendFormat(" default '{0}' ", defaultValue);
                    break;

                case "double":
                    sql.Append(" double precision");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                case "decimal":
                    var precision = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.Precision)?.Value;
                    var scale = field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.Scale)?.Value;
                    sql.AppendFormat(" decimal({0},{1})", precision, scale);
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                case "bool":
                    sql.Append(" boolean");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                case "char":
                    sql.Append(" char");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;

                default:
                    sql.AppendFormat(" varchar({0})", "10");
                    if (defaultValue != null) sql.AppendFormat(" default'{0}'", defaultValue);
                    break;
            }
            sql.AppendFormat(", ALTER COLUMN \"{0}\" ", field.Name);
            sql.Append(!field.AllowNull ? "SET NOT NULL" : "DROP NOT NULL");

            //if (field.Parameter == FieldType.EntityReference)
            //{
            //    var foreignTableName =
            //        field.Configurations.FirstOrDefault(x => x.Name == FieldConfig.ForeingTable)?.Value;
            //    var foreignTableSchemaName =
            //        field.Configurations.FirstOrDefault(x => x.ConfigCode == "9999");
            //    sql.AppendFormat(" FOREIGN KEY REFERENCES \"{2}\".\"{0}\"({1})", foreignTableName, "\"Id\"", foreignTableSchemaName);
            //}

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
            return "SELECT \"schema_name\" FROM \"information_schema\".schemata";
        }

        /// <summary>
        /// Check if Npg server is available
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        public override (bool, string) IsSqlServerConnected(string connectionString)
        {
            using (var connection = new NpgsqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return (true, "");
                }
                catch (Exception e)
                {
                    if (e.Data.Contains("SqlState"))
                    {
                        if (e.Data["SqlState"].ToString() == "3D000")
                        {
                            return (true, "");
                        }
                    }
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