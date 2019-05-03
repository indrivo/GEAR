using System;
using System.ComponentModel.DataAnnotations;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Core;

namespace ST.Identity.Data.UserProfiles
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class Profile : BaseModel
    {
        [Required]
        public string ProfileName { get; set; }

        public bool IsSystem { get; set; }

        [Required]
        public Guid EntityId { get; set; }
    }
}
