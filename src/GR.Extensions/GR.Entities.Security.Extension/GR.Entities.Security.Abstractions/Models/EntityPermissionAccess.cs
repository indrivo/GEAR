using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.Entities.Security.Abstractions.Enums;

namespace GR.Entities.Security.Abstractions.Models
{
    public class EntityPermissionAccess : BaseModel
    {
        /// <summary>
        /// Entity access type
        /// </summary>
        [Required]
        public EntityAccessType AccessType { get; set; }

        /// <summary>
        /// Entity permission
        /// </summary>
        public EntityPermission EntityPermission { get; set; }

        public Guid EntityPermissionId { get; set; }
    }
}
