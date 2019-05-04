using System;
using System.Collections.Generic;

namespace ST.Entities.ViewModels.Form
{
    public class StageViewModel
    {
        public Guid Id { get; set; }
        public SettingsViewModel Settings { get; set; }
        public IEnumerable<Guid> Rows { get; set; }
    }
}