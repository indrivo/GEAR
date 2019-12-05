using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;

namespace GR.Entities.Abstractions.Models.Tables
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class TableModel : BaseModel
    {
        /// <summary>
        /// Name of TableModel
        /// </summary>
        [Required]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        public string Name { get; set; }

        /// <summary>
        /// Add description for TableModel
        /// </summary>
        public string EntityType { get; set; }

        /// <summary>
        /// Add description for TableModel
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Is a system table
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Return value for check
        ///     if other tenants can access data from this entity
        /// </summary>
        public bool IsCommon { get; set; }

        /// <summary>
        /// Is part of system db context
        /// </summary>
        public bool IsPartOfDbContext { get; set; }

        /// <summary>
        /// Lists of fields for TableModel
        /// </summary>
        public ICollection<TableModelField> TableFields { get; set; }
    }
}