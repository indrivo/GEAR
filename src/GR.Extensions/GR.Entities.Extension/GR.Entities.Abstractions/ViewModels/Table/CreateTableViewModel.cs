using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Entities.Abstractions.Models.Tables;

namespace GR.Entities.Abstractions.ViewModels.Table
{
    public class CreateTableViewModel
    {
        /// <summary>
        /// Entity name
        /// </summary>
        [Required(AllowEmptyStrings = false)]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        public string Name { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Select a schema for this table model")]
        [Display(Name = "Schema")]

        public Guid SelectedTypeId { get; set; }

        public string EntityType { get; set; }

        public IEnumerable<EntityType> EntityTypes { get; set; }

        public IEnumerable<TableModelField> TableFields { get; set; }

        /// <summary>
        /// Table description
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Tenant id
        /// </summary>
        public  Guid? TenantId { get; set; }

        /// <summary>
        /// Is common 
        /// </summary>
        public bool IsCommon { get; set; }
    }
}