using System;
using System.ComponentModel.DataAnnotations;
using GR.Core;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Security.Abstractions.Enums;

namespace GR.Entities.Security.Abstractions.Models
{
    public class EntityFieldPermission : BaseModel
    {
        /// <summary>
        /// Reference to entity field
        /// </summary>
        [Required]
        public Guid TableModelFieldId { get; set; }

        /// <summary>
        /// Field access type
        /// </summary>
        [Required]
        public FieldAccessType FieldAccessType { get; set; } = FieldAccessType.FullControl;

        /// <summary>
        /// Reference to roles
        /// </summary>
        [Required]
        public Guid ApplicationRoleId { get; set; }
    }
}
