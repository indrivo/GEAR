using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Core;
using ST.Entities.Abstractions.Models.Tables;

namespace ST.Entities.Security.Models
{
    public class EntityPermission : ExtendedModel
    {
        /// <summary>
        /// Accesses
        /// </summary>
        public IEnumerable<EntityPermissionAccess> EntityPermissionAccesses { get; set; }

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
