using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GR.Entities.Abstractions.ViewModels.DynamicEntities;
using GR.Entities.Controls.QueryAbstractions;

namespace GR.Entities.EntityBuilder.MsSql.Controls.Query
{
    public class MsSqlQueryBuilder : EntityQueryBuilder
    {
        protected override string GetWhereString(EntityViewModel viewModel, string nullName = "null",
             string isNullName = "isnull")
        {
            var where = new StringBuilder();

            //Get Nullable or NonNullable Field Type
            var nullFields = viewModel.Fields.Where(x => x.Type == nullName || x.Type == isNullName).ToList();

            foreach (var field in viewModel.Values[0])
            {
                var tempField = nullFields.FirstOrDefault(x => x.ColumnName == field.Key);

                if (tempField != null)
                {
                    where.AppendFormat(
                        tempField.Type == nullName
                            ? "AND ([{0}] is null or [{0}]=@{0}) "
                            : "AND ([{0}] is not null or [{0}]=@{0}) ", field.Key);
                    nullFields.Remove(tempField);
                }
                else
                {
                    where.AppendFormat("AND [{0}]=@{0} ", field.Key);
                }
            }

            if (nullFields.Count <= 0) return where.ToString();
            {
                foreach (var field in nullFields)
                    where.AppendFormat(field.Type == nullName
                        ? "AND ([{0}] is null ) "
                        : "AND ([{0}] is not null ) ", field.ColumnName);
            }

            return where.ToString();
        }


        public override string DeleteByIdQuery(EntityViewModel viewModel, bool completeDelete = false)
        {
            var sql = new StringBuilder();
            sql.AppendFormat(
                completeDelete
                    ? "DELETE FROM [{0}].[{1}] WHERE Id=@Id "
                    : "UPDATE [{0}].[{1}] SET IsDeleted=1 WHERE Id=@Id", viewModel.TableSchema, viewModel.TableName);

            return sql.ToString();
        }

        public override string DeleteByIdQuery(EntityViewModel viewModel, Guid parameterId, bool completeDelete = false)
        {
            var sql = new StringBuilder();
            if (completeDelete)
                sql.AppendFormat("DELETE FROM [{0}].[{1}] WHERE Id='{2}' ", viewModel.TableName, viewModel.TableSchema,
                    parameterId);
            else
                sql.AppendFormat("UPDATE [{0}].[{1}] SET IsDeleted=1 WHERE Id='{2}' ", viewModel.TableSchema,
                    viewModel.TableName,
                    parameterId);

            return sql.ToString();
        }

        public override string DeleteByParamQuery(string tableName, string parameterName, string parameter,
            bool completeDelete = false)
        {
            var sql = new StringBuilder();

            sql.AppendFormat(
                completeDelete ? "DELETE FROM {0} WHERE [{1}]='{2}'" : "UPDATE [{1}] SET IsDeleted=1 WHERE [{1}]=@{1}",
                tableName, parameterName, parameter);

            return sql.ToString();
        }


        public override string DeleteByParamQuery(EntityViewModel viewModel, bool completeDelete = false)
        {
            var sql = new StringBuilder();

            if (viewModel.Values.Count == 0) return sql.ToString();

            var whereString = GetWhereString(viewModel);

            sql.AppendFormat(
                completeDelete
                    ? "DELETE FROM [{0}].[{1}] WHERE 1=1 {2} "
                    : "UPDATE [{0}].[{1}] SET IsDeleted=1 WHERE 1=1 {2} ", viewModel.TableSchema, viewModel.TableName,
                whereString);

            return sql.ToString();

            //        sql.AppendFormat("DELETE FROM {0} WHERE [{1}]='{2}'", viewModel.TableName, paramenterName, parameter);
            //        return sql.ToString();
        }


        public override string GetByColumnParameterAndPaginationQuery(EntityViewModel viewModel, string parameterName,
            string parameter, int perPage, int currentPage)
        {
            var sql = new StringBuilder();
            var fieldsData = new StringBuilder();

            var last = viewModel.Fields.Last();

            foreach (var item in viewModel.Fields)
                fieldsData.AppendFormat(!item.Equals(last) ? "[{0}], " : "[{0}] ", item.ColumnName);


            sql.AppendFormat(
                "SELECT {0} FROM [{1}].[{2}] WHERE {3}='{4}' ORDER BY Id OFFSET {5} * ({6} -1) ROWS FETCH NEXT {5} ROWS ONLY"
                , fieldsData, viewModel.TableSchema, viewModel.TableName, parameterName, parameter, perPage,
                currentPage);
            return sql.ToString();
        }


        public override string GetByColumnParameterAndPaginationQuery(EntityViewModel viewModel, int perPage,
            int currentPage)
        {
            var sql = new StringBuilder();
            var fieldsData = new StringBuilder();

            var last = viewModel.Fields.Last();
            foreach (var item in viewModel.Fields)
                fieldsData.AppendFormat(!item.Equals(last) ? "[{0}], " : "[{0}] ", item.ColumnName);

            var whereString = GetWhereString(viewModel);

            sql.AppendFormat(
                "SELECT {0} FROM [{1}].[{2}] WHERE 1=1 {3} ORDER BY Id OFFSET {4} * ({5} -1) ROWS FETCH NEXT {4} ROWS ONLY"
                , fieldsData, viewModel.TableSchema, viewModel.TableName, whereString, perPage, currentPage);
            return sql.ToString();
        }

        public override string GetByColumnParameterAndPaginationQuery(EntityViewModel viewModel,
            Dictionary<string, object> parameters, int perPage, int currentPage)
        {
            var sql = new StringBuilder();

            var fields = new StringBuilder();


            var lastField = viewModel.Fields.Last();
            foreach (var field in viewModel.Fields)
            {
                fields.AppendFormat("[{0}] ", field.ColumnName);
                if (lastField != field) fields.Append(", ");
            }

            sql.AppendFormat("SELECT {2} FROM [{0}].[{1}] WHERE 1=1 ", viewModel.TableSchema, viewModel.TableName,
                fields);

            var where = GetWhereString(viewModel);
            if (where.Length != 0)
                sql.AppendFormat("WHERE 1=1 {0} ", where);

            sql.AppendFormat("ORDER BY Id OFFSET {0} * ({1} -1) ROWS FETCH NEXT {0} ROWS ONLY", perPage, currentPage);
            return sql.ToString();
        }

        public override string GetPaginationByFilters(EntityViewModel viewModel, uint page, uint perPage = 10, string queryString = null)
        {
            throw new NotImplementedException();
        }

        public override string CountByFilters(EntityViewModel viewModel, string queryString = null)
        {
            throw new NotImplementedException();
        }

        public override string GetByColumnParameterQuery(EntityViewModel viewModel,
            Dictionary<string, object> parameters)
        {
            var sql = new StringBuilder();
            var fieldsData = new StringBuilder();
            var paramsData = new StringBuilder();

            var fields =
                viewModel.Fields.Where(x => x.Type != "Single" && x.Type != "Multiple" && x.Type != "EntityName")
                    .ToList();

            var last = fields.Last();
            foreach (var item in fields)
                fieldsData.AppendFormat(item.Equals(last) ? "[{0}] " : "[{0}], ", item.ColumnName);

            foreach (var param in parameters)
                paramsData.AppendFormat("AND [{0}] = @{0} ", param.Key);
            sql.AppendFormat("SELECT {0} FROM [{1}].[{2}] WHERE 1=1 {3}", fieldsData, viewModel.TableSchema,
                viewModel.TableName, paramsData);
            return sql.ToString();
        }

        public override string GetByColumnParameterQuery(EntityViewModel viewModel)
        {
            var sql = new StringBuilder();
            var fieldsData = new StringBuilder();

            var fields =
                viewModel.Fields.Where(x => x.Type != "Single" && x.Type != "Multiple" && x.Type != "EntityName")
                    .ToList();

            var last = fields.Last();

            foreach (var item in fields)
                fieldsData.AppendFormat(item.Equals(last) ? "[{0}] " : "[{0}], ", item.ColumnName);

            var whereString = GetWhereString(viewModel);
            //                paramsData.AppendFormat("AND {0} = @{0} ", param.Value);
            sql.AppendFormat("SELECT {0} FROM [{1}].[{2}] WHERE 1=1 {3}", fieldsData, viewModel.TableSchema,
                viewModel.TableName, whereString);
            return sql.ToString();
        }


        public override string GetByIdQuery(EntityViewModel viewModel)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("SELECT * FROM [{0}].[{1}] WHERE Id=@Id", viewModel.TableSchema, viewModel.TableName);
            return sql.ToString();
        }

        public override string GetCountByParameter(EntityViewModel viewModel, string parameterName, string parameter)
        {
            var sql = new StringBuilder();
            sql.AppendFormat("SELECT COUNT({0}) FROM [{1}].[{2}] WHERE [{3}]='{4}'", viewModel.Fields[0].ColumnName,
                viewModel.TableSchema, viewModel.TableName, parameterName, parameter);
            return sql.ToString();
        }

        public override string GetCountByParameters(EntityViewModel viewModel)
        {
            var sql = new StringBuilder();
            var whereString = GetWhereString(viewModel);

            sql.AppendFormat("SELECT COUNT({0}) FROM [{1}].[{2}] WHERE 1=1 {3} ", viewModel.Fields[0].ColumnName,
                viewModel.TableSchema,
                viewModel.TableName, whereString);
            return sql.ToString();
        }

        public override string GetCountByParameter(EntityViewModel viewModel, Dictionary<string, object> parameters)
        {
            var sql = new StringBuilder();

            var where = new StringBuilder();
            if (parameters.Count >= 1)
            {
                var lastWhereKey = parameters.Last().Key;
                foreach (var param in parameters)
                {
                    where.AppendFormat("[{0}]=@{0}", param.Key);
                    if (param.Key != lastWhereKey)
                        where.Append(" AND ");
                }
            }

            sql.AppendFormat("SELECT COUNT(Id) FROM [{0}].[{1}] ",
                viewModel.TableSchema, viewModel.TableName);
            if (where.Length != 0)
                sql.AppendFormat("WHERE {0} ", where);
            return sql.ToString();
        }

        public override string InsertQuery(EntityViewModel viewModel)
        {
            var parameters = new StringBuilder();
            var values = new StringBuilder();
            var sql = new StringBuilder();

            var last = viewModel.Fields.Last();
            foreach (var item in viewModel.Fields)
                if (!item.Equals(last))
                {
                    parameters.AppendFormat("[{0}],", item.ColumnName);
                    values.AppendFormat("@{0},", item.ColumnName);
                }
                else
                {
                    parameters.AppendFormat("[{0}] ", item.ColumnName);
                    values.AppendFormat("@{0} ", item.ColumnName);
                }

            sql.AppendFormat("INSERT INTO [{0}].[{1}]({2}) VALUES({3})", viewModel.TableSchema, viewModel.TableName,
                parameters, values);
            return sql.ToString();
        }

        public override string UpdateQuery(EntityViewModel viewModel, Guid parameterId)
        {
            var sql = new StringBuilder();
            var fieldsData = new StringBuilder();
            var where = new StringBuilder();
            sql.AppendFormat("UPDATE {0} SET ", viewModel.TableName);
            where.AppendFormat(" WHERE Id='{0}'", parameterId);
            var last = viewModel.Fields.Last();
            foreach (var item in viewModel.Fields)
                fieldsData.AppendFormat(!item.Equals(last) ? "[{0}]=@{1}, " : "[{0}]=@{1}", item.ColumnName,
                    item.ColumnName);
            sql.Append(fieldsData);
            sql.Append(where);
            return sql.ToString();
        }

        public override string UpdateQuery(EntityViewModel viewModel)
        {
            var sql = new StringBuilder();
            var fieldsData = new StringBuilder();
            var where = new StringBuilder();
            sql.AppendFormat("UPDATE [{0}].[{1}] SET ", viewModel.TableSchema, viewModel.TableName);
            where.AppendFormat(" WHERE Id=@Id");
            var last = viewModel.Fields.Last();

            foreach (var item in viewModel.Fields)
                fieldsData.AppendFormat(!item.Equals(last) ? "[{0}]=@{1}, " : "[{0}]=@{1}", item.ColumnName,
                    item.ColumnName);
            sql.Append(fieldsData);
            sql.Append(where);
            return sql.ToString();
        }

        public override string GetByIncludeParam(EntityViewModel parentTable, EntityViewModel childTable,
            string fieldName)
        {
            var sql = new StringBuilder();
            var idIn = new StringBuilder();
            var fields = new StringBuilder();
            if (parentTable.Fields.Any())
            {
                var last = parentTable.Fields.Last();
                foreach (var field in parentTable.Fields)
                {
                    fields.AppendFormat("[{0}]", field.ColumnName);
                    if (field != last) fields.Append(", ");
                }
            }

            idIn.AppendFormat("SELECT {2} FROM [{0}].[{1}] WHERE Id = @Id ", parentTable.TableSchema,
                parentTable.TableName, fieldName);
            sql.AppendFormat("SELECT {3} FROM [{0}].[{1}] WHERE Id IN ({2})", childTable.TableSchema,
                childTable.TableName, idIn, fields);
            return sql.ToString();
        }
    }
}