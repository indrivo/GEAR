using System;
using System.Collections.Generic;

namespace ST.Entities.ViewModels.Form
{
    public class RowViewModel
    {
        public IEnumerable<Guid> Columns { get; set; }
        public Guid Id { get; set; }
        public ConfigViewModel Config { get; set; }
        public AttrsViewModel Attrs { get; set; }
    }
}