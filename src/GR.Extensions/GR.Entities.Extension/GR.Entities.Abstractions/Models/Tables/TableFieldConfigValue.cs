using System;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;

namespace GR.Entities.Abstractions.Models.Tables
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class TableFieldConfigValue
    {
        /// <summary>
        /// TableModelField
        /// </summary>
        public TableModelField TableModelField { get; set; }

        /// <summary>
        /// TableModelField Id
        /// </summary>
        public Guid TableModelFieldId { get; set; }

        /// <summary>
        /// TableFieldConfig
        /// </summary>
        public TableFieldConfigs TableFieldConfig { get; set; }

        /// <summary>
        /// TableFieldConfig Id
        /// </summary>
        public Guid TableFieldConfigId { get; set; }

        /// <summary>
        /// TableFieldConfigValue Value
        /// </summary>
        public string Value { get; set; }
    }
}
