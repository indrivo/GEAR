using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.WorkFlows.Abstractions.Models
{
    public class WorkFlow : BaseModel
    {
        /// <summary>
        /// Workflow name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Enabled
        /// </summary>
        public virtual bool Enabled { get; set; }

        /// <summary>
        /// All transitions
        /// </summary>
        public virtual IEnumerable<State> States { get; set; } = new List<State>();

        /// <summary>
        /// Transitions
        /// </summary>
        public virtual IEnumerable<Transition> Transitions { get; set; } = new List<Transition>();
    }
}
