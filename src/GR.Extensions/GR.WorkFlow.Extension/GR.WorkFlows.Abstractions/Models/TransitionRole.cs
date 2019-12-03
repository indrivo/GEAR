using System;

namespace GR.WorkFlows.Abstractions.Models
{
    public class TransitionRole
    {
        /// <summary>
        /// Role id
        /// </summary>
        public virtual Guid RoleId { get; set; }

        /// <summary>
        /// Transition reference
        /// </summary>
        public virtual Transition Transition { get; set; }
        public virtual Guid TransitionId { get; set; }
    }
}
