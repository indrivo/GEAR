using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Core;

namespace ST.Entities.Abstractions.Models.Tables
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class TableModelFields : BaseModel
    {
        /// <summary>
        /// AllowNull
        /// </summary>
        public bool AllowNull { get; set; }

        /// <summary>
        /// DataType of field
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Add description for field
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Name
        /// </summary>
        [Required]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        public string Name { get; set; }

        /// <summary>
        /// Display name
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// If field is synchronized
        /// </summary>
        ///
        public bool Synchronized { get; set; }

        /// <summary>
        /// Is a system field
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Parent TableModel
        /// </summary>
        public TableModel Table { get; set; }

        /// <summary>
        /// Parent table id
        /// </summary>
        public Guid TableId { get; set; }

        /// <summary>
        /// TableFieldType
        /// </summary>
        public TableFieldTypes TableFieldType { get; set; }

        /// <summary>
        /// TableFieldType id
        /// </summary>
        public Guid TableFieldTypeId { get; set; }

        /// <summary>
        /// Lists of TableFieldConfigValues for TableModelField
        /// </summary>
        public ICollection<TableFieldConfigValues> TableFieldConfigValues { get; set; }
    }
}