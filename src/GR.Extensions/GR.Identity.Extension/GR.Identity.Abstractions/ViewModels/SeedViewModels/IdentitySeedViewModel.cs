using System.Collections.Generic;
using GR.Identity.Abstractions.ViewModels.RoleViewModels;
using GR.Identity.Abstractions.ViewModels.UserViewModels;

namespace GR.Identity.Abstractions.ViewModels.SeedViewModels
{
    public class IdentitySeedViewModel
    {
        /// <summary>
        /// List of system roles
        /// </summary>
        public virtual ICollection<RolesViewModel> ApplicationRoles { get; set; }

        /// <summary>
        /// List of system users
        /// </summary>
        public virtual ICollection<UserSeedViewModel> ApplicationUsers { get; set; }
    }
}
