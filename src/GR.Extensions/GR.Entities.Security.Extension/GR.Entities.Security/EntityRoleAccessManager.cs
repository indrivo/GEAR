﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Entities.Abstractions;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Security.Abstractions;
using GR.Entities.Security.Abstractions.Enums;
using GR.Entities.Security.Abstractions.Models;
using GR.Entities.Security.Data;
using GR.Identity.Abstractions;

namespace GR.Entities.Security
{
    public class EntityRoleAccessManager<TAclContext, TIdentityContext> : IEntityRoleAccessManager
        where TAclContext : EntitySecurityDbContext
        where TIdentityContext : IdentityDbContext<GearUser, GearRole, string>
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
        private readonly IUserManager<GearUser> _userManager;

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
        public EntityRoleAccessManager(TAclContext context, TIdentityContext identityContext, IUserManager<GearUser> userManager, IEntityContext entityContext)
        {
            _context = context;
            _identityContext = identityContext;
            _userManager = userManager;
            _entityContext = entityContext;
        }

        /// <summary>
        /// Roles
        /// </summary>
        public IQueryable<GearRole> Roles => _userManager.RoleManager
            .Roles.Where(x => !x.IsDeleted);

        /// <summary>
        /// Tables
        /// </summary>
        public IQueryable<TableModel> Tables => _entityContext.Table
            .Where(x => x.EntityType.Equals(GearSettings.DEFAULT_ENTITY_SCHEMA) || x.IsPartOfDbContext);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<Guid, Dictionary<Guid, IEnumerable<string>>> GetAllForView()
        {
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

            return await _context.PushAsync();
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
            if (GearApplication.AppState.InstallOnProgress) return new List<EntityAccessType>
            {
                EntityAccessType.FullControl
            };
            var user = await _userManager.GetCurrentUserAsync();
            IEnumerable<string> roles = new List<string> { GlobalResources.Roles.ANONIMOUS_USER };
            if (user.IsSuccess)
            {
                roles = _userManager.GetRolesFromClaims();
            }

            return await GetPermissionsAsync(roles, entityId);
        }

        /// <summary>
        /// Get permissions by entity name
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public virtual async Task<ICollection<EntityAccessType>> GetPermissionsAsync(string entityName)
        {
            var defResult = new Collection<EntityAccessType>();
            if (string.IsNullOrEmpty(entityName)) return defResult;
            var table = await Tables.FirstOrDefaultAsync(x =>
                x.Name.Equals(entityName));
            if (table == null) return defResult;
            var user = await _userManager.GetCurrentUserAsync();
            IEnumerable<string> roles = new List<string> { GlobalResources.Roles.ANONIMOUS_USER };
            if (user.IsSuccess)
            {
                roles = _userManager.GetRolesFromClaims();
            }

            return await GetPermissionsAsync(roles, table.Id);
        }

        /// <summary>
        /// Get permissions for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<ICollection<EntityAccessType>> GetPermissionsAsync(GearUser user, Guid entityId)
        {
            IEnumerable<string> roles = new List<string> { GlobalResources.Roles.ANONIMOUS_USER };
            if (user != null)
            {
                roles = _userManager.GetRolesFromClaims();
            }

            return await GetPermissionsAsync(roles, entityId);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get access by roles and entity
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public virtual async Task<ICollection<EntityAccessType>> GetPermissionsAsync(IEnumerable<string> userRoles, Guid entityId)
        {
            var result = new List<EntityAccessType>();
            var roles = _identityContext.Set<GearRole>().Where(x => userRoles.Contains(x.Name)).ToList();
            if (roles.Select(x => x.Name).Contains(GlobalResources.Roles.ADMINISTRATOR))
            {
                result.Add(EntityAccessType.FullControl);
                return result;
            }

            var toCheck = new List<Guid> { entityId };

            var table = await _entityContext.Table.FirstOrDefaultAsync(x => x.Id.Equals(entityId));
            if (table != null)
            {
                if (!table.IsCommon)
                {
                    var tenantTables =
                        _entityContext.Table.Where(x => x.Name.Equals(table.Name) && !x.Id.Equals(table.Id))
                            .Select(x => x.Id).ToList();
                    if (tenantTables.Any())
                    {
                        toCheck.AddRange(tenantTables);
                    }
                }
            }

            if (table == null) return result;

            var permissionGroup = await _context.EntityPermissions
                .Include(x => x.EntityPermissionAccesses)
                .Where(x => roles.Select(b => b.Id.ToGuid())
                                .Contains(x.ApplicationRoleId) && toCheck
                                .Any(v => v
                                    .Equals(x.TableModelId)))
                .Select(j => j.EntityPermissionAccesses
                    .Select(p => p.AccessType))
                .ToListAsync();

            foreach (var permissions in permissionGroup)
            {
                result.AddRange(permissions);
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

        /// <summary>
        /// Have access
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="accessTypes"></param>
        /// <returns></returns>
        public virtual async Task<bool> HaveAccessAsync(string entityName, IEnumerable<EntityAccessType> accessTypes)
        {
            if (string.IsNullOrEmpty(entityName)) return false;
            var accesses = await GetPermissionsAsync(entityName);
            if (accesses == null || !accesses.Any()) return false;
            var grant = accesses.Contains(EntityAccessType.FullControl)
                        || accesses.All(accessTypes.Contains);
            return grant;
        }
    }
}
