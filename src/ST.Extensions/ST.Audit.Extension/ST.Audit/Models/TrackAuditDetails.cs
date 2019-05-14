using System;
using System.ComponentModel.DataAnnotations.Schema;
using ST.Core;

namespace ST.Audit.Models
{
    [Table("TrackAuditDetails")]
    public class TrackAuditDetails : BaseModel
    {
        /// <summary>
        /// Track audit id
        /// </summary>
        public Guid TrackAuditId { get; set; }

        /// <summary>
        /// Property Name
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Property Type
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// Value
        /// </summary>
        public string Value { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Ignore version field
        /// </summary>
        [NotMapped]
        public override int Version { get; set; }
    }
}