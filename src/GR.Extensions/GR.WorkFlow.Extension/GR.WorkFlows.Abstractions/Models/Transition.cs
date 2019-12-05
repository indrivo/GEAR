using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.WorkFlows.Abstractions.Models
{
    public class Transition : BaseModel
    {
        /// <summary>
        /// Transition name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// From state
        /// </summary>
        public virtual State FromState { get; set; }
        [Required]
        public virtual Guid FromStateId { get; set; }

        /// <summary>
        /// To state
        /// </summary>
        public virtual State ToState { get; set; }
        [Required]
        public virtual Guid ToStateId { get; set; }

        /// <summary>
        /// Workflow reference
        /// </summary>
        public virtual WorkFlow WorkFlow { get; set; }

        [Required]
        public virtual Guid WorkflowId { get; set; }

        /// <summary>
        /// Actions
        /// </summary>
        public virtual IEnumerable<TransitionAction> TransitionActions { get; set; } = new List<TransitionAction>();

        /// <summary>
        /// Allowed roles to change state 
        /// </summary>
        public virtual IEnumerable<TransitionRole> TransitionRoles { get; set; } = new List<TransitionRole>();
    }
}
