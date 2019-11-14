using System.Collections.Generic;
using GR.Core.Helpers.Filters;
using GR.Entities.Abstractions.Enums;

namespace GR.Entities.Abstractions.ViewModels.DynamicEntities
{
    public class EntityViewModel
    {
        /// <summary>
        /// Sql table name
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Schema of the table
        /// </summary>
        public string TableSchema { get; set; }

        /// <summary>
        /// Fields
        /// </summary>
        public List<EntityFieldsViewModel> Fields { get; set; }

        /// <summary>
        /// Values
        /// </summary>
        public List<Dictionary<string, object>> Values { get; set; }

        /// <summary>
        /// Includes
        /// </summary>
        public List<EntityViewModel> Includes { get; set; }

        /// <summary>
        /// Order by columns
        /// </summary>
        public Dictionary<string, EntityOrderDirection> OrderByColumns { get; set; } = new Dictionary<string, EntityOrderDirection>
        {
            { nameof(Core.BaseModel.Created), EntityOrderDirection.Desc }
        };

        /// <summary>
        /// Filters
        /// </summary>
        public IList<Filter> Filters { get; set; } = new List<Filter>();
    }
}
