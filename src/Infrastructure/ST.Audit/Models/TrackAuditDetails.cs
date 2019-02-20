using System;
using System.ComponentModel.DataAnnotations.Schema;
using ST.BaseRepository;
using ST.Organization.Models;

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

        /// <summary>
        /// Tenant id
        /// </summary>
        public Guid? TenantId { get; set; }
    }
}