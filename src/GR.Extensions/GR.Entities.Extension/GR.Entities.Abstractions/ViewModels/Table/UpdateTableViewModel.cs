using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using GR.Entities.Abstractions.Models.Tables;

namespace GR.Entities.Abstractions.ViewModels.Table
{
    public class UpdateTableViewModel
    {
        public Guid Id { get; set; }

        [RegularExpression(@"^\S*$", ErrorMessage = "No white space allowed")]
        public string Name { get; set; }

        public string EntityType { get; set; }

        public IEnumerable<TableModelField> TableFields { get; set; }

        public string Description { get; set; }

        public DateTime Created { get; set; }

        public string ModifiedBy { get; set; }

        public string TabName { get; set; }

        public bool IsSystem { get; set; }

        public List<TableFieldGroups> Groups { get; set; }

        public bool IsPartOfDbContext { get; set; }

        public  Guid? TenantId { get; set; }
    }
}