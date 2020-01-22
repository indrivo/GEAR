using System;

namespace GR.Identity.Profile.Abstractions.Models
{
    public class RoleProfile
    {
        public Guid ProfileId { get; set; }
        public Profile Profile { get; set; }
        public Guid RoleId { get; set; }
    }
}