using ST.Audit.Attributes;
using ST.Audit.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ST.Entities.Models.Tables
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class TableFieldConfigs
    {
        /// <summary>
        /// TableFieldConfig Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// TableFieldConfig Name
        /// </summary>
        public string Name { get; set; }
	    /// <summary>
	    /// TableFieldConfig Name
	    /// </summary>
	    public string Description { get; set; }
		/// <summary>
		/// TableFieldConfig Type
		/// </summary>
		public string Type { get; set; }


        [Column(TypeName = "char(4)")]
        public string Code { get; set; }

        /// <summary>
        /// TableFieldType
        /// </summary>
        public TableFieldTypes TableFieldType { get; set; }

        /// <summary>
        /// TableFieldType id
        /// </summary>
        public Guid TableFieldTypeId { get; set; }
    }
}
