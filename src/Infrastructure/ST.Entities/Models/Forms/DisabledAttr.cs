using System;
using ST.BaseRepository;

namespace ST.Entities.Models.Forms
{
    public class DisabledAttr : BaseModel
    {
        public string Name { get; set; }
        public Guid ConfigId { get; set; }
    }
}