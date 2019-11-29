using System.ComponentModel.DataAnnotations;
using GR.WorkFlows.Abstractions.Models;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class TransitionGetViewModel
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
        /// <summary>
        /// To state
        /// </summary>
        public virtual State ToState { get; set; }

        /// <summary>
        /// Workflow reference
        /// </summary>
        public virtual WorkFlow WorkFlow { get; set; }
    }
}
