using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.Entities.Security.Razor.ViewModels
{
    public class EntityRolesPermissionsViewModel
    {
        /// <summary>
        /// Role id
        /// </summary>
        [Required]
        public Guid RoleId { get; set; }

        /// <summary>
        /// Entity id
        /// </summary>
        [Required]
        public Guid EntityId { get; set; }

        /// <summary>
        /// Permissions
        /// </summary>
        public IEnumerable<string> Permissions { get; set; }
    }
}
