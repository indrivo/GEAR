using System.Collections.Generic;

namespace ST.Entities.ViewModels.DynamicEntities
{
    public class EntityViewModel
    {
        /// <summary>
        /// Sql table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// SChema of the table
        /// </summary>
        public string TableSchema { get; set; }


	    /// <summary>
	    /// Has configuration list
	    /// </summary>
	    public bool HasConfig { get; set; }

		/// <summary>
		/// Chema of the table
		/// </summary>
		public List<EntityFieldsViewModel> Fields { get; set; }

	    public List<Dictionary<string, object>> Values { get; set; }

        public List<EntityViewModel> Includes { get; set; }
    }
}
