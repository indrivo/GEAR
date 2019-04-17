using System;
using ST.Shared;

namespace ST.Entities.Models.Forms
{
    public class DisabledAttr : ExtendedModel
    {
        public string Name { get; set; }
        public Guid ConfigId { get; set; }
    }
}