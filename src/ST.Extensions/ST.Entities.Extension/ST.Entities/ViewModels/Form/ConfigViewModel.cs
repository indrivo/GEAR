using System.Collections.Generic;

namespace ST.Entities.ViewModels.Form
{
    public class ConfigViewModel
    {
        public bool Fieldset { get; set; }
        public string Legend { get; set; }
        public bool InputGroup { get; set; }
        public string Width { get; set; }
        public IEnumerable<string> DisabledAttrs { get; set; }
        public string Label { get; set; }
        public bool HideLabel { get; set; }
        public bool Editable { get; set; }
    }
}