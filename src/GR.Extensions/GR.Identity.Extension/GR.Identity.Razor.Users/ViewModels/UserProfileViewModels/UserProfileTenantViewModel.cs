using System;

namespace GR.Identity.Razor.Users.ViewModels.UserProfileViewModels
{
    public class UserProfileTenantViewModel
    {
        /// <summary>
        /// Name of tenant
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description for this tenant
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The url of site web
        /// </summary>
        public string SiteWeb { get; set; }

        /// <summary>
        /// Address
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// </summary>
        public Guid? TenantId { get; set; }
    }
}