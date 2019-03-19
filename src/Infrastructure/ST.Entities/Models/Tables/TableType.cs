
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Audit.Models;

namespace ST.Entities.Models.Tables
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class EntityType : ExtendedModel
    {
        /// <summary>
        /// Name of Schema
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Name of Schema
        /// </summary>
        public string MachineName { get; set; }

	    /// <summary>
	    /// Name of Schema
	    /// </summary>
	    public bool IsSystem { get; set; }

		/// <summary>
		/// Add description for Schema
		/// </summary>
		public string Description { get; set; }
    }
}