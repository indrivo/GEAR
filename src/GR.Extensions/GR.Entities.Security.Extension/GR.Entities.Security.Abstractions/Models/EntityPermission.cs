using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Core;

namespace GR.Entities.Security.Abstractions.Models
{
    public class EntityPermission : BaseModel
    {
        /// <summary>
        /// Accesses
        /// </summary>
        public ICollection<EntityPermissionAccess> EntityPermissionAccesses { get; set; }

        /// <summary>
        /// Reference to roles
        /// </summary>
        [Required]
        public Guid ApplicationRoleId { get; set; }

        /// <summary>
        /// Table id
        /// </summary>
        [Required]
        public Guid TableModelId { get; set; }
    }
}
