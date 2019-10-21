using GR.Identity.Abstractions.Models.MultiTenants;

namespace GR.MultiTenant.Abstractions.ViewModels
{
    public class OrganizationListViewModel : Tenant
    {
        /// <summary>
        /// Company users
        /// </summary>
        public int Users { get; set; } = 0;
    }
}
