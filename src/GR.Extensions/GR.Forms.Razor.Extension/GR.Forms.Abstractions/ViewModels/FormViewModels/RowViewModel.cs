using System;
using System.Collections.Generic;

namespace GR.Forms.Abstractions.ViewModels.FormViewModels
{
    public class RowViewModel
    {
        public IEnumerable<Guid> Columns { get; set; }
        public Guid Id { get; set; }
        public ConfigViewModel Config { get; set; }
        public IDictionary<string, string> Attrs { get; set; }
    }
}