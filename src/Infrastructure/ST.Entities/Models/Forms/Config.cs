using System.Collections.Generic;
using ST.Audit.Models;

namespace ST.Entities.Models.Forms
{
    public class Config : ExtendedModel
    {
        public bool Fieldset { get; set; }
        public string Legend { get; set; }
        public bool InputGroup { get; set; }
        public string Width { get; set; }
        public IEnumerable<DisabledAttr> DisabledAttrs { get; set; }
        public string Label { get; set; }
        public bool HideLabel { get; set; }
        public bool Editable { get; set; }
    }
}