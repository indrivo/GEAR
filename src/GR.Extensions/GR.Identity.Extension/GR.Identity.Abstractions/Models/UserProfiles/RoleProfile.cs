using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GR.Identity.Abstractions.Models.UserProfiles
{
    public class RoleProfile
    {
        public GearRole ApplicationRole { get; set; }
        public Profile Profile { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid ProfileId { get; set; }

        [Key]
        [Column(Order = 2)]
        public string ApplicationRoleId { get; set; }
    }
}