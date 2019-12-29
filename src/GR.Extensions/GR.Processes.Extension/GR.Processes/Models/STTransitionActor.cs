using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.Procesess.Models
{
    public class STTransitionActor : BaseModel
    {
        /// <summary>
        /// Name of actor
        /// </summary>
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// System role
        /// </summary>
        [Required]
        public Guid RoleId { get; set; }

        /// <summary>
        /// Process transition 
        /// </summary>
        public STProcessTransition ProcessTransition { get; set; }
        public Guid ProcessTransitionId { get; set; }

        /// <summary>
        /// Additional settings defined by user
        /// </summary>
        public string ActorSettings { get; set; }
    }
}
