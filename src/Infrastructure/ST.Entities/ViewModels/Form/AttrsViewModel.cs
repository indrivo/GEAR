using System.Collections.Generic;

namespace ST.Entities.ViewModels.Form
{
    public class AttrsViewModel
    {
        public string ClassName { get; set; }
        public string Type { get; set; }
        public bool Required { get; set; }
        public string Value { get; set; }
        public string TableFieldId { get; set; }
        public ICollection<AttrTagViewModel> Tag { get; set; }
    }
}