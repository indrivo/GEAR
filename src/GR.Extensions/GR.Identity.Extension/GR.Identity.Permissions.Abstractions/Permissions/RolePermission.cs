using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.Identity.Abstractions;

namespace GR.Identity.Permissions.Abstractions.Permissions
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
        public Guid RoleId { get; set; }

        /// <summary>
        /// The role object that will be populated if using
        /// the Include method
        /// </summary>
        public GearRole Role { get; set; }

        public Permission Permission { get; set; }

        [Required]
        public Guid PermissionId { get; set; }
    }
}