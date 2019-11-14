using System;

namespace GR.Entities.Abstractions.ViewModels.TableTypes
{
    public class EntityTypeListViewModel
    {
        /// <summary>
        ///     Name of Schema
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Add description for Schema
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Stores the time when object was created
        /// </summary>
        public DateTime Created { get; set; }
    }
}