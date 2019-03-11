using ST.Identity.Data;
using ST.Identity.Data.UserProfiles;
using ST.Identity.Services.Abstractions;
using ST.Organization.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ST.Identity.Services
{
    public class OrganizationService : IOrganizationService
    {
        /// <summary>
        /// Inject context
        /// </summary>
        private readonly ApplicationDbContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        public OrganizationService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all users for organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetAllowedUsersByOrganizationId(Guid organizationId)
        {
            return _context.Users.Where(x => x.TenantId == organizationId && !x.IsDeleted);
        }

        /// <summary>
        /// Get all tenants
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tenant> GetAllTenants()
        {
            return _context.Tenants.ToList();
        }

        public IEnumerable<ApplicationUser> GetDisabledUsersByOrganizationId(Guid organizationId)
        {
            return _context.Users.Where(x => x.TenantId == organizationId && x.IsDeleted);
        }

        /// <summary>
        /// Get tenant id
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public Tenant GetTenantById(Guid tenantId)
        {
            return _context.Tenants.FirstOrDefault(x => x.Id == tenantId);
        }

        /// <summary>
        /// Get all users for organization
        /// </summary>
        /// <param name="organization"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetUsersByOrganization(Tenant organization)
        {
            if (organization == null) return default;
            return _context.Users.Where(x => x.TenantId == organization.Id);
        }

        /// <summary>
        /// Get users by organization and role
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="roleId"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetUsersByOrganization(Guid organizationId, Guid roleId)
        {
            var role = _context.Roles.FirstOrDefault(x => x.Id.ToLower() == roleId.ToString().ToLower());
            if (role == null) return default;
            var usersId = _context.UserRoles.Where(x => x.RoleId.ToLower() == roleId.ToString().ToLower())
                .ToList()
                .Select(x => x.UserId).ToList();
            return _context.Users.Where(x => x.TenantId == organizationId && usersId.Contains(x.Id));
        }

        /// <summary>
        /// Get all users for organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationUser> GetUsersByOrganizationId(Guid organizationId)
        {
            return _context.Users.Where(x => x.TenantId == organizationId);
        }
    }
}
