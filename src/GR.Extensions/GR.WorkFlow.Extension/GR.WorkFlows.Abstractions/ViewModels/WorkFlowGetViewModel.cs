using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class WorkFlowGetViewModel
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
        /// States
        /// </summary>
        public IEnumerable<StateGetViewModel> States { get; set; } = new List<StateGetViewModel>();

        /// <summary>
        /// Transitions
        /// </summary>
        public IEnumerable<TransitionGetViewModel> Transitions { get; set; } = new List<TransitionGetViewModel>();
    }
}
