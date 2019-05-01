using System;
using System.ComponentModel.DataAnnotations;
using ST.Core;
using ST.Entities.Security.Enums;

namespace ST.Entities.Security.Models
{
    public class EntityPermissionAccess : ExtendedModel
    {
        /// <summary>
        /// Entity access type
        /// </summary>
        [Required]
        public EntityAccessType AccessType { get; set; }

        public EntityPermission EntityPermission { get; set; }
        public Guid EntityPermissionId { get; set; }
    }
}
