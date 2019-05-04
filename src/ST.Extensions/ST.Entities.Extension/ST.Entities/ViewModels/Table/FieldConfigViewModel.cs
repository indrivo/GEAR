using System;

namespace ST.Entities.ViewModels.Table
{
    public class FieldConfigViewModel
    {
        public string Value { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public Guid ConfigId { get; set; }

        public string ConfigCode { get; set; }
    }
}