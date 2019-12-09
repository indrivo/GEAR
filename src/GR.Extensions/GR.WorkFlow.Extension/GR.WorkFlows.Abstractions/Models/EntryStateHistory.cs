using System;
using GR.Core;

namespace GR.WorkFlows.Abstractions.Models
{
    public class EntryStateHistory : BaseModel
    {
        /// <summary>
        /// Entry state reference
        /// </summary>
        public virtual EntryState EntryState { get; set; }
        public virtual Guid EntryStateId { get; set; }

        /// <summary>
        /// From state 
        /// </summary>
        public virtual State FromState { get; set; }
        public virtual Guid FromStateId { get; set; }

        /// <summary>
        /// From state 
        /// </summary>
        public virtual State ToState { get; set; }
        public virtual Guid ToStateId { get; set; }

        /// <summary>
        /// Change state message
        /// </summary>
        public string Message { get; set; }
    }
}
