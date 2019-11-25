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
        public virtual Guid FromStateId { get; set; }

        /// <summary>
        /// To state
        /// </summary>
        public virtual State ToState { get; set; }
        public virtual Guid ToStateId { get; set; }

        /// <summary>
        /// Actions
        /// </summary>
        public virtual IEnumerable<TransitionAction> Actions { get; set; }
    }
}
