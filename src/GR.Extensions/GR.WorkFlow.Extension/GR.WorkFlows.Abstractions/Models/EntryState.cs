using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.WorkFlows.Abstractions.Models
{
    public class EntryState : BaseModel
    {
        /// <summary>
        /// Contract reference
        /// </summary>
        public virtual WorkFlowEntityContract Contract { get; set; }
        [Required]
        public virtual Guid ContractId { get; set; }

        /// <summary>
        /// Entry id
        /// </summary>
        public virtual string EntryId { get; set; }

        /// <summary>
        /// Reference to state
        /// </summary>
        public virtual State State { get; set; }
        [Required]
        public virtual Guid StateId { get; set; }

        /// <summary>
        /// Message
        /// </summary>
        public virtual string Message { get; set; }

        /// <summary>
        /// Entry state histories 
        /// </summary>
        public virtual ICollection<EntryStateHistory> EntryStateHistories { get; set; } = new List<EntryStateHistory>();
    }
}
