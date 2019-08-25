using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.Core.Abstractions;
using ST.Core.Helpers;
using ST.Identity.Abstractions;
using ST.Identity.Abstractions.Models.MultiTenants;

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

        /// <summary>
        /// Get user organization
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        TTenant GetUserOrganization(ApplicationUser user);

        /// <summary>
        /// Check if exist any user with this email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        Task<bool> CheckIfUserExistAsync(string email);

        /// <summary>
        /// Send email for confirmation
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task SendInviteToEmailAsync(ApplicationUser user);

        /// <summary>
        /// Create new Organization User
        /// </summary>
        /// <param name="user"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        Task<ResultModel> CreateNewOrganizationUserAsync(ApplicationUser user, IEnumerable<string> roles);

        /// <summary>
        /// Get tenant by current user
        /// </summary>
        /// <returns></returns>
        Task<ResultModel<Tenant>> GetTenantByCurrentUserAsync();

        /// <summary>
        /// Return list of available roles
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ApplicationRole>> GetRoles();

        /// <summary>
        /// Get default image
        /// </summary>
        /// <returns></returns>
        byte[] GetDefaultImage();
    }
}