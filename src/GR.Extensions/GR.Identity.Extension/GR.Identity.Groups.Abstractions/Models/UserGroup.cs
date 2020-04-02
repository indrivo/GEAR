using System;
using GR.Core;
using GR.Identity.Abstractions;

namespace GR.Identity.Groups.Abstractions.Models
{
    public class UserGroup : BaseModel
    {
        public virtual Group Group { get; set; }
        public virtual Guid GroupId { get; set; }

        public virtual Guid UserId { get; set; }
        public virtual GearUser User { get; set; }
    }
}