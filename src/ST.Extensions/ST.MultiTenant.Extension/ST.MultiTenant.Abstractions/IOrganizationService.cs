using System;
using System.Collections.Generic;
using ST.Core.Abstractions;
using ST.Identity.Abstractions;

namespace ST.MultiTenant.Abstractions
{
    public interface IOrganizationService<TTenant>
        where TTenant : IBaseModel
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
        IEnumerable<ApplicationUser> GetUsersByOrganization(TTenant organization);

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
        IEnumerable<TTenant> GetAllTenants();

        /// <summary>
        /// Get tenant by id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        TTenant GetTenantById(Guid tenantId);
    }
}
