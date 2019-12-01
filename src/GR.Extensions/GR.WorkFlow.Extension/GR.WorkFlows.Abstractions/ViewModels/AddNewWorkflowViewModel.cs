using System.ComponentModel.DataAnnotations;

namespace GR.WorkFlows.Abstractions.ViewModels
{
    public class AddNewWorkflowViewModel
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
    }
}
