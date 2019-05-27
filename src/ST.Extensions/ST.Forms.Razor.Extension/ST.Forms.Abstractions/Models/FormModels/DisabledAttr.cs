using System;
using ST.Core;

namespace ST.Forms.Abstractions.Models.FormModels
{
    public class DisabledAttr : BaseModel
    {
        public string Name { get; set; }
        public Guid ConfigId { get; set; }
    }
}