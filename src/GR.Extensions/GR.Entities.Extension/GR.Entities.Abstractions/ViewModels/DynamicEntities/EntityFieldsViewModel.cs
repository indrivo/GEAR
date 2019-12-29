using System.Collections.Generic;

namespace GR.Entities.Abstractions.ViewModels.DynamicEntities
{
    public class EntityFieldsViewModel
    {

        /// <summary>
        /// ColumnName
        /// </summary>
        public string ColumnName { get; set; }

        //public object Value { get; set; }

        /// <summary>
        /// Type
        /// </summary>
        public string Type { get; set; }

	    /// <summary>
	    /// Type
	    /// </summary>
	    public bool IsSystem { get; set; }

	    public List<EntityConfigViewModel> Configurations { get; set; }
	}
}
