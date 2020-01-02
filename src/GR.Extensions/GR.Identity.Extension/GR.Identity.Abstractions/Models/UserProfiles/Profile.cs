using GR.Audit.Abstractions.Attributes;
using GR.Audit.Abstractions.Enums;
using GR.Core;
using System;
using System.ComponentModel.DataAnnotations;

namespace GR.Identity.Abstractions.Models.UserProfiles
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