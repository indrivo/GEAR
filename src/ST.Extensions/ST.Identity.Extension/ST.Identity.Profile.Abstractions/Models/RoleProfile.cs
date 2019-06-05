using System;

namespace ST.Identity.Profile.Abstractions.Models
{
    public class RoleProfile
    {
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; }
        public Guid RoleId { get; set; }
    }
}
