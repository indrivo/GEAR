using System.Collections.Generic;

namespace ST.Entities.Models.Tables
{
    public class TableModel : ExtendedModel
    {
        /// <summary>
        /// Name of TableModel
        /// </summary>
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