using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;

namespace GR.Entities.Abstractions.Models.Tables
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class TableModelField : BaseModel
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
        /// Is used for all tenant or only for default
        /// </summary>
        public  bool IsCommon { get; set; }

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
        public TableFieldType TableFieldType { get; set; }

        /// <summary>
        /// TableFieldType id
        /// </summary>
        public Guid TableFieldTypeId { get; set; }

        /// <summary>
        /// Lists of TableFieldConfigValues for TableModelField
        /// </summary>
        public ICollection<TableFieldConfigValue> TableFieldConfigValues { get; set; }
    }
}