using ST.Audit.Attributes;
using ST.Audit.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using ST.Core;

namespace ST.Entities.Models.Forms
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class FormType : ExtendedModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "char(4)")]
        public string Code { get; set; }
        public bool IsSystem { get; set; }
    }
}