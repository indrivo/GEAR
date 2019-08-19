using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Entities.Abstractions;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Security.Abstractions;
using ST.Entities.Security.Abstractions.Enums;
using ST.Entities.Security.Abstractions.Models;
using ST.Entities.Security.Data;
using ST.Identity.Abstractions;

namespace ST.Entities.Security
{
    public class EntityRoleAccessManager<TAclContext, TIdentityContext> : IEntityRoleAccessManager
        where TAclContext : EntitySecurityDbContext
        where TIdentityContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        #region Inject Services
        /// <summary>
        /// Inject context
        /// </summary>
        private readonly TAclContext _context;

        /// <summary>
        /// Inject identity context
        /// </summary>
        private readonly TIdentityContext _identityContext;

        /// <summary>
        /// Inject user manager
        /// </summary>
        private readonly IUserManager<ApplicationUser> _userManager;

        /// <summary>
        /// Inject entity context
        /// </summary>
        private readonly IEntityContext _entityContext;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="context"></param>
        /// <param name="identityContext"></param>
        /// <param name="userManager"></param>
        /// <param name="entityContext"></param>
        public EntityRoleAccessManager(TAclContext context, TIdentityContext identityContext, IUserManager<ApplicationUser> userManager, IEntityContext entityContext)
        {
            _context = context;
            _identityContext = identityContext;
            _userManager = userManager;
            _entityContext = entityContext;
        }

        /// <summary>
        /// Roles
        /// </summary>
        public IQueryable<ApplicationRole> Roles => _userManager.RoleManager
            .Roles.Where(x => !x.IsDeleted);

        /// <summary>
        /// Tables
        /// </summary>
        public IQueryable<TableModel> Tables => _entityContext.Table
            .Where(x => x.EntityType.Equals(Settings.DEFAULT_ENTITY_SCHEMA));

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<Guid, Dictionary<Guid, IEnumerable<string>>> GetAllForView()
        {
            var te = _context.EntityPermissions.ToList();
            var all = _context.EntityPermissions
                .Include(x => x.EntityPermissionAccesses)
                .GroupBy(x => x.TableModelId)
                .ToDictionary(k => k.Key,
                    v => v.ToDictionary(mk => mk.ApplicationRoleId,
                        mv => mv.EntityPermissionAccesses
                            .Select(t => t.AccessType.ToString())));
            return all;
        }

        /// <summary>
        /// Set permissions for role
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="roleId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        public virtual async Task<ResultModel> SetPermissionsForRoleOnEntityAsync(Guid? entityId, Guid? roleId, IEnumerable<string> permissions)
        {
            var result = new ResultModel();
            var nullValueError = new ErrorModel(nameof(Nullable), "Entity and role is required");
            if (entityId == null || roleId == null)
            {
                result.Errors.Add(nullValueError);
                return result;
            }

            var entity = await Tables.FirstOrDefaultAsync(x => x.Id.Equals(entityId));
            var role = await Roles.FirstOrDefaultAsync(x => x.Id.ToGuid().Equals(roleId));
            if (entity == null || role == null)
            {
                result.Errors.Add(nullValueError);
                return result;
            }

            var entityPermission = await _context.EntityPermissions
                .Include(x => x.EntityPermissionAccesses)
                .FirstOrDefaultAsync(x => x.ApplicationRoleId.Equals(roleId)
                                          && x.TableModelId.Equals(entityId));
            var grants = GetAccessTypeFromStringList(permissions);
            if (entityPermission == null)
            {
                _context.EntityPermissions.Add(new EntityPermission
                {
                    ApplicationRoleId = roleId.Value,
                    TableModelId = entityId.Value,
                    EntityPermissionAccesses = grants.Select(grant => new EntityPermissionAccess
                    {
                        AccessType = grant
                    }).ToList()
                });
            }
            else
            {
                var notChecked = entityPermission.EntityPermissionAccesses.Where(x => !grants.Contains(x.AccessType))
                    .DistinctBy(x => x.AccessType)
                    .ToList();

                var check = new List<EntityPermissionAccess>();
                foreach (var grant in grants)
                {
                    if (entityPermission.EntityPermissionAccesses.Select(x => x.AccessType)
                        .Contains(grant)) continue;
                    check.Add(new EntityPermissionAccess
                    {
                        AccessType = grant,
                        EntityPermissionId = entityPermission.Id
                    });
                }

                if (notChecked.Any())
                {
                    _context.EntityPermissionAccesses.RemoveRange(notChecked);
                }

                if (check.Any())
                {
                    await _context.AddRangeAsync(check);
                }
            }

            return await _context.SaveDependenceAsync();
        }

        /// <summary>
        /// Get access from a string list
        /// </summary>
        /// <param name="accesses"></param>
        /// <returns></returns>
        protected static IEnumerable<EntityAccessType> GetAccessTypeFromStringList(IEnumerable<string> accesses)
        {
            var grants = new HashSet<EntityAccessType>();
            if (accesses == null) return grants;
            foreach (var grant in accesses)
            {
                Enum.TryParse(grant, true, out EntityAccessType eGrant);
                grants.Add(eGrant);
            }

            return grants;
        }

        /// <summary>
        /// Get permissions for current user on entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<ICollection<EntityAccessType>> GetPermissionsAsync(Guid entityId)
        {
            var grant = new Collection<EntityAccessType>();
            var user = await _userManager.GetCurrentUserAsync();
            if (!user.IsSuccess) return grant;
            var roles = await _userManager.UserManager.GetRolesAsync(user.Result);
            return await GetPermissionsAsync(roles, entityId);
        }

        /// <summary>
        /// Get permissions for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<ICollection<EntityAccessType>> GetPermissionsAsync(ApplicationUser user, Guid entityId)
        {
            var grant = new Collection<EntityAccessType>();
            if (user == null) return grant;
            var roles = await _userManager.UserManager.GetRolesAsync(user);
            return await GetPermissionsAsync(roles, entityId);
        }

        /// <summary>
        /// Get access by roles and entity
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<ICollection<EntityAccessType>> GetPermissionsAsync(IEnumerable<string> userRoles, Guid entityId)
        {
            var result = new HashSet<EntityAccessType>();
            var roles = await _identityContext.Set<ApplicationRole>().Where(x => userRoles.Contains(x.Name)).ToListAsync();
            if (roles.Select(x => x.Name).Contains(Settings.ADMINISTRATOR))
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
        public virtual async Task<bool> HaveReadAccessAsync(IEnumerable<string> userRoles, Guid entityId)
        {
            var accesses = await GetPermissionsAsync(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(EntityAccessType.Read);
            return grant;
        }
        /// <summary>
        /// Have read access
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveReadAccessAsync(Guid entityId)
        {
            var accesses = await GetPermissionsAsync(entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(EntityAccessType.Read);
            return grant;
        }
        /// <summary>
        /// Have delete permanent
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveDeletePermanentAccessAsync(IEnumerable<string> userRoles, Guid entityId)
        {
            var accesses = await GetPermissionsAsync(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(EntityAccessType.DeletePermanent);
            return grant;
        }

        /// <summary>
        /// Have delete access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveDeleteAccessAsync(IEnumerable<string> userRoles, Guid entityId)
        {
            var accesses = await GetPermissionsAsync(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(EntityAccessType.Delete);
            return grant;
        }

        /// <summary>
        /// Have access 
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveRestoreAccessAsync(IEnumerable<string> userRoles, Guid entityId)
        {
            var accesses = await GetPermissionsAsync(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(EntityAccessType.Restore);
            return grant;
        }

        /// <summary>
        /// Have update access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveUpdateAccessAsync(IEnumerable<string> userRoles, Guid entityId)
        {
            var accesses = await GetPermissionsAsync(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(EntityAccessType.Update);
            return grant;
        }

        /// <summary>
        /// Have write access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveWriteAccessAsync(IEnumerable<string> userRoles, Guid entityId)
        {
            var accesses = await GetPermissionsAsync(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(EntityAccessType.Write);
            return grant;
        }

        /// <summary>
        /// Have access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <param name="accessType"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveAccessAsync(IEnumerable<string> userRoles, Guid entityId, EntityAccessType accessType = EntityAccessType.Read)
        {
            var accesses = await GetPermissionsAsync(userRoles, entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(accessType);
            return grant;
        }

        /// <summary>
        /// Have access
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="accessType"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveAccessAsync(Guid entityId, EntityAccessType accessType = EntityAccessType.Read)
        {
            var accesses = await GetPermissionsAsync(entityId);
            if (accesses == null) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.Contains(accessType);
            return grant;
        }
    }
}
