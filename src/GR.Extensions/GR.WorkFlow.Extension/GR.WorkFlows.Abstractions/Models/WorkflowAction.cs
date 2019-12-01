using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.WorkFlows.Abstractions.Models
{
    public class WorkflowAction : BaseModel
    {
        /// <summary>
        /// Action name
        /// </summary>
        [Required]
        public virtual string Name { get; set; }

        /// <summary>
        /// System runtime name
        /// </summary>
        [Required]
        public virtual string ClassName { get; set; }

        /// <summary>
        /// Class name full path
        /// </summary>
        [Required]
        public virtual string ClassNameWithNameSpace { get; set; }

        /// <summary>
        /// Description
        /// </summary>
        public virtual string Description { get; set; }

        /// <summary>
        /// Check if is system
        /// </summary>
        public virtual bool IsSystem { get; set; }

        /// <summary>
        /// Mapped to transitions
        /// </summary>
        public virtual IEnumerable<TransitionAction> TransitionActions { get; set; }
    }
}
