using ST.Identity.Abstractions.Models.MultiTenants;

namespace ST.MultiTenant.Abstractions.ViewModels
{
    public class OrganizationListViewModel : Tenant
    {
        /// <summary>
        /// Company users
        /// </summary>
        public int Users { get; set; } = 0;
    }
}
