
namespace ST.Entities.Models.Tables
{
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