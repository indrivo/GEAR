using System;
using System.Collections.Generic;

namespace GR.Forms.Abstractions.ViewModels.FormViewModels
{
    public class StageViewModel
    {
        public Guid Id { get; set; }
        public SettingsViewModel Settings { get; set; }
        public IEnumerable<Guid> Rows { get; set; }
    }
}