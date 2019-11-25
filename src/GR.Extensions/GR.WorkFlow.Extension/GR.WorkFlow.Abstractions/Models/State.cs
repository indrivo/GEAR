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
        /// Reference to workflow
        /// </summary>
        public virtual WorkFlow WorkFlow { get; set; }
        public virtual Guid WorkFlowId { get; set; }
    }
}