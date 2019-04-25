using System;
using System.ComponentModel.DataAnnotations;
using ST.Audit.Attributes;
using ST.Audit.Enums;
using ST.Shared;

namespace ST.Identity.Data.UserProfiles
{
    [TrackEntity(Option = TrackEntityOption.AllFields)]
    public class Profile : ExtendedModel
    {
        [Required]
        public string ProfileName { get; set; }

        public bool IsSystem { get; set; }

        [Required]
        public Guid EntityId { get; set; }
    }
}
