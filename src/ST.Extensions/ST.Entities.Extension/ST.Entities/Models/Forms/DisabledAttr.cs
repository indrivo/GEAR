using System;
using ST.Core;

namespace ST.Entities.Models.Forms
{
    public class DisabledAttr : BaseModel
    {
        public string Name { get; set; }
        public Guid ConfigId { get; set; }
    }
}