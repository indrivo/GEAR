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
        /// All transitions
        /// </summary>
        public virtual IEnumerable<State> States { get; set; }

        /// <summary>
        /// Transitions
        /// </summary>
        public virtual IEnumerable<Transition> Transitions { get; set; }
    }
}
