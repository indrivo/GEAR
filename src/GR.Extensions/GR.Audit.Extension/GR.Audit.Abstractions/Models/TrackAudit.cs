using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using GR.Audit.Abstractions.Enums;
using GR.Core;

namespace GR.Audit.Abstractions.Models
{
    [Table("TrackAudits")]
    public class TrackAudit : BaseModel
    {
        /// <summary>
        /// Context
        /// </summary>
        public string DatabaseContextName { get; set; }

        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Track event type
        /// </summary>
        public TrackEventType TrackEventType { get; set; }

        /// <summary>
        /// Id of record
        /// </summary>
        public Guid RecordId { get; set; }

        /// <summary>
        /// Full name of record type
        /// </summary>
        public string TypeFullName { get; set; }

        /// <summary>
        /// Track Audit details
        /// </summary>
        public ICollection<TrackAuditDetails> AuditDetailses { get; set; }
    }
}