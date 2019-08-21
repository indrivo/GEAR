using System;
using System.ComponentModel.DataAnnotations;
using ST.Core;
using ST.Entities.Security.Abstractions.Enums;

namespace ST.Entities.Security.Abstractions.Models
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
