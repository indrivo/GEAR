using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.WorkFlows.Abstractions.Models
{
    public class WorkFlowEntityContract : BaseModel
    {
        /// <summary>
        /// Entity name
        /// </summary>
        public virtual string EntityName { get; set; }

        /// <summary>
        /// Workflow reference
        /// </summary>
        public virtual WorkFlow WorkFlow { get; set; }

        [Required]
        public virtual Guid WorkFlowId { get; set; }

        /// <summary>
        /// Entry states
        /// </summary>
        public virtual IEnumerable<EntryState> EntryStates { get; set; } = new List<EntryState>();
    }
}
