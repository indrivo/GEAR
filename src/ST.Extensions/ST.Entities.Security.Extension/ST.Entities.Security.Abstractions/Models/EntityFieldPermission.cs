using System;
using System.ComponentModel.DataAnnotations;
using ST.Core;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Security.Abstractions.Enums;

namespace ST.Entities.Security.Abstractions.Models
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
