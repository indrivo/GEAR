using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Core;

namespace ST.Entities.Abstractions.Models.Tables
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

        public bool IsPartOfDbContext { get; set; }

        /// <summary>
        /// Lists of fields for TableModel
        /// </summary>
        public ICollection<TableModelFields> TableFields { get; set; }
    }
}