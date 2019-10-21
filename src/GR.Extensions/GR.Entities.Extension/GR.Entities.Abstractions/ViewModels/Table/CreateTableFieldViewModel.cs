using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GR.Entities.Abstractions.ViewModels.Table
{
    public class CreateTableFieldViewModel
    {
        public Guid Id { get; set; }

        public Guid TableId { get; set; }

        [Required]
        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public bool Synchronized { get; set; }

        public string DataType { get; set; }

        public bool AllowNull { get; set; }

        public Guid TableFieldTypeId { get; set; }

        public string TableFieldCode { get; set; }

        public string Parameter { get; set; }

        public List<FieldConfigViewModel> Configurations { get; set; }

        public List<string> EntitiesList { get; set; }
    }
}