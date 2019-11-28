using System;
using System.ComponentModel.DataAnnotations;

namespace GR.WorkFlows.Abstractions.Models
{
    public class TransitionAction
    {
        /// <summary>
        /// Reference to action
        /// </summary>
        public virtual Transition Transition { get; set; }
        [Required]
        public virtual Guid TransitionId { get; set; }

        /// <summary>
        /// Reference to action
        /// </summary>
        public virtual WorkflowAction Action { get; set; }
        public virtual Guid ActionId { get; set; }
    }
}