using System;
using System.ComponentModel.DataAnnotations;
using ST.Core;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Security.Enums;

namespace ST.Entities.Security.Models
{
    public class EntityPermission : ExtendedModel
    {
        /// <summary>
        /// Entity access type
        /// </summary>
        [Required]
        public EntityAccessType AccessType { get; set; } = EntityAccessType.FullControl;

        /// <summary>
        /// Reference to roles
        /// </summary>
        [Required]
        public Guid ApplicationRoleId { get; set; }

        /// <summary>
        /// Reference to entity
        /// </summary>
        public TableModel TableModel { get; set; }
        [Required]
        public Guid TableModelId { get; set; }
    }
}
