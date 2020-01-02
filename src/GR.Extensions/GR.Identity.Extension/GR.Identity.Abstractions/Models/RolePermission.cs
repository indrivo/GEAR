using GR.Core;
using GR.Identity.Abstractions.Models.Permmisions;
using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Abstractions.Models
{
    /// <inheritdoc />
    /// <summary>
    /// Role -&gt; Permission junction model. Represents
    /// a many-to-many relationship between these two
    /// models
    /// </summary>
    public class RolePermission : BaseModel
    {
        /// <summary>
        /// The Id of the Role
        /// </summary>
        public string RoleId { get; set; }

        /// <summary>
        /// The role object that will be populated if using
        /// the Include method
        /// </summary>
        public GearRole Role { get; set; }

        public Permission Permission { get; set; }

        /// <summary>
        /// Represents a permission object represented as
        /// a formatted string with the following format:
        /// {Service:Module:Action}
        /// e.g.
        /// "Identity:Users:Create"
        /// </summary>
        [Required]
        public string PermissionCode { get; set; }

        [Required]
        public Guid PermissionId { get; set; }
    }
}