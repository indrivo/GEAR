using System;
using System.ComponentModel.DataAnnotations;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class AddNewStateViewModel
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
        /// Reference to workflow
        /// </summary>
        public virtual Guid WorkFlowId { get; set; }

        /// <summary>
        /// Settings
        /// </summary>
        public virtual object AdditionalSettings { get; set; }
    }
}
