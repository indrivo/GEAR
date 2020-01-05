using GR.Core;
using System;

namespace GR.Identity.Profile.Abstractions.Models
{
    /// <inheritdoc />
    public class Profile : BaseModel
    {
        /// <summary>
        /// Profile name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Profile description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Describe profile level
        /// </summary>
        public ProfileLevel ProfileLevel { get; set; }

        /// <summary>
        /// Reference to dynamic table
        /// </summary>
        public Guid TableId { get; set; }
    }

    public enum ProfileLevel
    {
        High,
        Medium,
        Low
    }
}