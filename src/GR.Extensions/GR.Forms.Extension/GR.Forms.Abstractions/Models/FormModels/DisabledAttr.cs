using System;
using GR.Core;

namespace GR.Forms.Abstractions.Models.FormModels
{
    public class DisabledAttr : BaseModel
    {
        public string Name { get; set; }
        public Guid ConfigId { get; set; }
    }
}