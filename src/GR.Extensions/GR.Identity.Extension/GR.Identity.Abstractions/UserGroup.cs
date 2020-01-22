using GR.Core;
using System;

namespace GR.Identity.Abstractions
{
    public class UserGroup : BaseModel
    {
        public AuthGroup AuthGroup { get; set; }
        public Guid AuthGroupId { get; set; }
        public GearUser User { get; set; }
        public string UserId { get; set; }
    }
}