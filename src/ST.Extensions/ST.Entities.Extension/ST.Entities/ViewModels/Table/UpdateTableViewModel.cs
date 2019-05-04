using System;
using System.Collections.Generic;
using ST.Entities.Abstractions.Models.Tables;

namespace ST.Entities.ViewModels.Table
{
    public class UpdateTableViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string EntityType { get; set; }

        public IEnumerable<TableModelFields> TableFields { get; set; }

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