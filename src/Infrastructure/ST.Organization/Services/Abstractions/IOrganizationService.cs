using ST.Identity.Data.UserProfiles;
using System;
using System.Collections.Generic;
using ST.Identity.Data.MultiTenants;

namespace ST.MultiTenant.Services.Abstractions
{
    public interface IOrganizationService
    {
        /// <summary>
        /// Get users for organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        IEnumerable<ApplicationUser> GetUsersByOrganizationId(Guid organizationId);

        /// <summary>
        /// Get users for organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        IEnumerable<ApplicationUser> GetUsersByOrganization(Tenant organization);

        /// <summary>
        /// Get users by role
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        IEnumerable<ApplicationUser> GetUsersByOrganization(Guid organizationId, Guid roleId);

        /// <summary>
        /// Get allowed users
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        IEnumerable<ApplicationUser> GetAllowedUsersByOrganizationId(Guid organizationId);

        /// <summary>
        /// Get  disabled users
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        IEnumerable<ApplicationUser> GetDisabledUsersByOrganizationId(Guid organizationId);

        /// <summary>
        /// Get all tenants
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tenant> GetAllTenants();

        /// <summary>
        /// Get tenant by id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Tenant GetTenantById(Guid tenantId);
    }
}
