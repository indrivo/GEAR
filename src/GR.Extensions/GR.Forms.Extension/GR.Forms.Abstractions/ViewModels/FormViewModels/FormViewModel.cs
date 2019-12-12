using System;
using System.Collections.Generic;

namespace GR.Forms.Abstractions.ViewModels.FormViewModels
{
    public class FormViewModel
    {
        public Guid Id { get; set; }
        public SettingsViewModel Settings { get; set; }
        public IDictionary<Guid, StageViewModel> Stages { get; set; }
        public IDictionary<Guid, RowViewModel> Rows { get; set; }
        public IDictionary<Guid, ColumnViewModel> Columns { get; set; }
        public IDictionary<Guid, FieldViewModel> Fields { get; set; }
    }
}