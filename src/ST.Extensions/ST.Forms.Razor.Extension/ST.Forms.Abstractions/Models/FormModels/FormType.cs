using System.ComponentModel.DataAnnotations.Schema;
using ST.Audit.Abstractions.Attributes;
using ST.Audit.Abstractions.Enums;
using ST.Core;

namespace ST.Forms.Abstractions.Models.FormModels
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class FormType : BaseModel
    {
        public string Name { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "char(4)")]
        public string Code { get; set; }
        public bool IsSystem { get; set; }
    }
}