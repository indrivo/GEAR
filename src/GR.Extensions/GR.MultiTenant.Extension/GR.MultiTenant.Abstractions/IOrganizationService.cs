using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using GR.Core;
using GR.Core.Abstractions;
using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Abstractions.Models.MultiTenants;
using GR.MultiTenant.Abstractions.ViewModels;

namespace GR.MultiTenant.Abstractions
{
    public interface IOrganizationService<TTenant>
        where TTenant : IBaseModel
    {
        /// <summary>
        /// Check if user is part of organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        Task<bool> IsUserPartOfOrganizationAsync(Guid? userId, Guid? tenantId);

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

        /// <summary>
        /// Filtered list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        DTResult<OrganizationListViewModel> GetFilteredList(DTParameters param);

        /// <summary>
        /// Get country list for VM states
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<SelectListItem>> GetCountrySelectList();

        /// <summary>
        /// Invite new user by email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<ResultModel> InviteNewUserByEmailAsync(InviteNewUserViewModel model);

        /// <summary>
        /// Get filtered list
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<DTResult<CompanyUsersViewModel>> LoadFilteredListCompanyUsersAsync(DTParameters param);

        /// <summary>
        /// Create new organization
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        Task<ResultModel<CreateTenantViewModel>> CreateOrganizationAsync(CreateTenantViewModel data);

        /// <summary>
        /// Send user confirm email
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task SendConfirmEmailRequest(ApplicationUser user);

        /// <summary>
        /// Check if tenant name is used
        /// </summary>
        /// <param name="tenantName"></param>
        /// <returns></returns>
        Task<bool> IsTenantNameUsedAsync(string tenantName);
    }
}