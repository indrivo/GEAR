using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.WorkFlows.Abstractions.Models
{
    public class State : BaseModel
    {
        /// <summary>
        /// Transition name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Is start state
        /// </summary>
        public virtual bool IsStartState { get; set; }

        /// <summary>
        /// Is end state
        /// </summary>
        public virtual bool IsEndState { get; set; }

        /// <summary>
        /// Reference to workflow
        /// </summary>
        public virtual WorkFlow WorkFlow { get; set; }
        public virtual Guid WorkFlowId { get; set; }

        /// <summary>
        /// Additional settings
        /// </summary>
        public virtual string AdditionalSettings { get; set; }
    }
}