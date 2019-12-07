using System.ComponentModel.DataAnnotations.Schema;
using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;

namespace GR.Forms.Abstractions.Models.FormModels
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