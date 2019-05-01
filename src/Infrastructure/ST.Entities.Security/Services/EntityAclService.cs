using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ST.Audit.Extensions;
using ST.Core;
using ST.Core.Extensions;
using ST.Entities.Security.Abstractions;
using ST.Entities.Security.Data;
using ST.Entities.Security.Enums;
using ST.Identity.Abstractions;

namespace ST.Entities.Security.Services
{
    public class EntityAclService<TAclContext, TIdentityContext> : IEntityAclService
        where TAclContext : EntitySecurityDbContext
        where TIdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        /// <summary>
        /// Inject context
        /// </summary>
        private readonly TAclContext _context;

        /// <summary>
        /// Inject identity context
        /// </summary>
        private readonly TIdentityContext _identityContext;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="identityContext"></param>
        public EntityAclService(TAclContext context, TIdentityContext identityContext)
        {
            _context = context;
            _identityContext = identityContext;
        }

        /// <summary>
        /// Get access by roles and entity
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<ICollection<EntityAccessType>> GetAccessControl(IEnumerable<string> userRoles, Guid entityId)
        {
            var result = new HashSet<EntityAccessType>();
            var roles = await _identityContext.Set<ApplicationRole>().Where(x => userRoles.Contains(x.Name)).ToListAsync();
            if (roles.Select(x => x.Name).Contains(Settings.SuperAdmin))
            {
                result.Add(EntityAccessType.FullControl);
                return result;
            }

            foreach (var role in roles)
            {
                var config = await _context
                    .EntityPermissions
                    .Include(x => x.EntityPermissionAccesses)
                    .FirstOrDefaultAsync(x => x.ApplicationRoleId == role.Id.ToGuid() && x.TableModelId == entityId);
                if (config == null) continue;
                result.AddRange(config.EntityPermissionAccesses.Select(x => x.AccessType).ToList());
            }

            return result;
        }

        /// <summary>
        /// Have read access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveReadAccess(IEnumerable<string> userRoles, Guid entityId)
        {
            var accesses = await GetAccessControl(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                         || accesses.Contains(EntityAccessType.Owner)
                         || accesses.Contains(EntityAccessType.Read);
            return grant;
        }

        /// <summary>
        /// Have delete access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveDeleteAccess(IEnumerable<string> userRoles, Guid entityId)
        {
            var accesses = await GetAccessControl(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(EntityAccessType.Owner)
                        || accesses.Contains(EntityAccessType.Delete);
            return grant;
        }

        /// <summary>
        /// Have update access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveUpdateAccess(IEnumerable<string> userRoles, Guid entityId)
        {
            var accesses = await GetAccessControl(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(EntityAccessType.Owner)
                        || accesses.Contains(EntityAccessType.Update);
            return grant;
        }

        /// <summary>
        /// Have write access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveWriteAccess(IEnumerable<string> userRoles, Guid entityId)
        {
            var accesses = await GetAccessControl(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(EntityAccessType.Owner)
                        || accesses.Contains(EntityAccessType.Write);
            return grant;
        }
    }
}
