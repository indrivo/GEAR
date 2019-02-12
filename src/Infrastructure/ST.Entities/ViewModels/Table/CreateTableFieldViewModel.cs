using System;
using System.Collections.Generic;

namespace ST.Entities.ViewModels.Table
{
    public class CreateTableFieldViewModel
    {
        public Guid Id { get; set; }

        public Guid TableId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

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