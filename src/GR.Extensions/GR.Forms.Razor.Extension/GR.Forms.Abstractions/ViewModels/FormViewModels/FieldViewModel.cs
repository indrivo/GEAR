using System;
using System.Collections.Generic;

namespace GR.Forms.Abstractions.ViewModels.FormViewModels
{
    public class FieldViewModel
    {
        public string Tag { get; set; }
        public IDictionary<string, string> Attrs { get; set; }
        public ConfigViewModel Config { get; set; }
        public Guid Id { get; set; }
        public string FMap { get; set; }
        public MetaViewModel Meta { get; set; }
        public IEnumerable<OptionsViewModel> Options { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
    }
}