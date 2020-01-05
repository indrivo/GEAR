using System;

namespace GR.Identity.Profile.Abstractions.Models
{
    public class UserProfile
    {
        /// <summary>
        /// User id
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Reference to role profile
        /// </summary>
        public Guid RoleProfileId { get; set; }

        public RoleProfile RoleProfile { get; set; }
    }
}