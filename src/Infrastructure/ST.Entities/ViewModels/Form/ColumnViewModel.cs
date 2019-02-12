using System;
using System.Collections.Generic;

namespace ST.Entities.ViewModels.Form
{
    public class ColumnViewModel
    {
        public IEnumerable<Guid> Fields { get; set; }
        public Guid Id { get; set; }
        public ConfigViewModel Config { get; set; }
        public string ClassName { get; set; }
    }
}