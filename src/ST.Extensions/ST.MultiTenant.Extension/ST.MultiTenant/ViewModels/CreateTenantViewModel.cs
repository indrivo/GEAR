using System.ComponentModel.DataAnnotations;

namespace ST.MultiTenant.ViewModels
{
    public class CreateTenantViewModel
    {
        /// <summary>
        /// Name of tenant
        /// </summary>
        [Required]
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
    }
}
