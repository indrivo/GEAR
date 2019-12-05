using System.Linq;
using GR.Entities.Abstractions.Constants;
using GR.Entities.Abstractions.ViewModels.Table;

namespace GR.Entities.Abstractions.Extensions
{
    public static class FieldExtension
    {
        public static CreateTableFieldViewModel CreateSqlField(this CreateTableFieldViewModel field)
        {
            switch (field.Parameter)
            {
                case FieldType.EntityReference:
                    field.DataType = TableFieldDataType.Guid;
                    break;
                case FieldType.Boolean:
                    FieldConfigViewModel defaultBool = null;
                    foreach (var c in field.Configurations)
                    {
                        if (c.Name != FieldConfig.DefaultValue) continue;
                        defaultBool = c;
                        break;
                    }

                    if (defaultBool?.Value != null && defaultBool.Value.Trim() == "on") defaultBool.Value = "1";
                    if (defaultBool?.Value != null && defaultBool.Value.Trim() == "off") defaultBool.Value = "0";
                    break;
                case FieldType.DateTime:
                case FieldType.Date:
                case FieldType.Time:
                    FieldConfigViewModel defaultTime = null;
                    foreach (var c in field.Configurations)
                    {
                        if (c.Name != FieldConfig.DefaultValue) continue;
                        defaultTime = c;
                        break;
                    }

                    if (defaultTime?.Value != null && defaultTime.Value.Trim() == "on")
                        defaultTime.Value = "CURRENT_TIMESTAMP";
                    if (defaultTime?.Value != null && defaultTime.Value.Trim() == "off") defaultTime.Value = null;
                    break;
                case FieldType.File:
                    field.DataType = TableFieldDataType.Guid;
                    var foreignTable = field.Configurations.FirstOrDefault(s => s.Name == "ForeingTable");
                    if (foreignTable != null)
                    {
                        foreignTable.Value = "FileReferences";
                    }

                    break;
            }
            return field;
        }
    }
}
