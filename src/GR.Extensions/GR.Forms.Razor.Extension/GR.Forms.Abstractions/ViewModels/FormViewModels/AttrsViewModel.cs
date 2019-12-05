using System.Collections.Generic;

namespace GR.Forms.Abstractions.ViewModels.FormViewModels
{
    public class AttrsViewModel
    {
        public string ClassName { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public string Value { get; set; }
        public ICollection<AttrTagViewModel> Tag { get; set; }
    }
}