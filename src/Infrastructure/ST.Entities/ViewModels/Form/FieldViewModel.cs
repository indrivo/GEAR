using System;
using System.Collections.Generic;

namespace ST.Entities.ViewModels.Form
{
    public class FieldViewModel
    {
        public string Tag { get; set; }
        public AttrsViewModel Attrs { get; set; }
        public ConfigViewModel Config { get; set; }
        public Guid Id { get; set; }
        public string FMap { get; set; }
        public MetaViewModel Meta { get; set; }
        public IEnumerable<OptionsViewModel> Options { get; set; }
        public string Content { get; set; }
        public int Order { get; set; }
        public Guid? TableFeldId { get; set; }
    }
}