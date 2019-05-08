using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ST.Entities.Abstractions.Models.Tables;

namespace ST.Entities.ViewModels.Table
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

        public string SelectedTypeId { get; set; }

        public string EntityType { get; set; }

        public IEnumerable<EntityType> EntityTypes { get; set; }

        public IEnumerable<TableModelFields> TableFields { get; set; }

        public string Description { get; set; }
        public  Guid? TenantId { get; set; }
    }
}