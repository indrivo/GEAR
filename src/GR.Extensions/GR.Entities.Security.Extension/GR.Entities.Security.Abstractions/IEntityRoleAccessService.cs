using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Security.Abstractions.Enums;
using GR.Identity.Abstractions;

namespace GR.Entities.Security.Abstractions
{
    public interface IEntityRoleAccessService
    {
        /// <summary>
        /// Get all active roles
        /// </summary>
        IQueryable<GearRole> Roles { get; }

        /// <summary>
        /// Get all tables
        /// </summary>
        IQueryable<TableModel> Tables { get; }
        /// <summary>
        /// Get permissions for user on specific entity
        /// </summary>
        /// <param name="user"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<ICollection<EntityAccessType>> GetPermissionsAsync(GearUser user, Guid entityId);
        /// <summary>
        /// Get permissions for specific roles
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<ICollection<EntityAccessType>> GetPermissionsAsync(IEnumerable<string> userRoles, Guid entityId);
        /// <summary>
        /// Check if is read access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<bool> HaveReadAccessAsync(IEnumerable<string> userRoles, Guid entityId);

        /// <summary>
        /// Check if is read access
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<bool> HaveReadAccessAsync(Guid entityId);
        /// <summary>
        /// Check if is delete permanent access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<bool> HaveDeletePermanentAccessAsync(IEnumerable<string> userRoles, Guid entityId);
        /// <summary>
        /// Check if is logical delete access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<bool> HaveDeleteAccessAsync(IEnumerable<string> userRoles, Guid entityId);
        /// <summary>
        /// Check if is restore access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<bool> HaveRestoreAccessAsync(IEnumerable<string> userRoles, Guid entityId);
        /// <summary>
        /// Check if is update access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<bool> HaveUpdateAccessAsync(IEnumerable<string> userRoles, Guid entityId);
        /// <summary>
        /// Check if is write access
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<bool> HaveWriteAccessAsync(IEnumerable<string> userRoles, Guid entityId);
        /// <summary>
        /// Check if is access on specific permission
        /// </summary>
        /// <param name="userRoles"></param>
        /// <param name="entityId"></param>
        /// <param name="accessType"></param>
        /// <returns></returns>
        Task<bool> HaveAccessAsync(IEnumerable<string> userRoles, Guid entityId, EntityAccessType accessType = EntityAccessType.Read);

        /// <summary>
        /// Check if is access on specific permission
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="accessType"></param>
        /// <returns></returns>
        Task<bool> HaveAccessAsync(Guid entityId, EntityAccessType accessType = EntityAccessType.Read);

        /// <summary>
        /// Set permissions for role
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="roleId"></param>
        /// <param name="permissions"></param>
        /// <returns></returns>
        Task<ResultModel> SetPermissionsForRoleOnEntityAsync(Guid? entityId, Guid? roleId,
            IEnumerable<string> permissions);

        /// <summary>
        /// Get data structure
        /// </summary>
        /// <returns></returns>
        Dictionary<Guid, Dictionary<Guid, IEnumerable<string>>> GetAllForView();

        /// <summary>
        /// Get permissions for current user
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        Task<ICollection<EntityAccessType>> GetPermissionsAsync(Guid entityId);

        /// <summary>
        /// Check access by entity name
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="accessTypes"></param>
        /// <returns></returns>
        Task<bool> HaveAccessAsync(string entityName, IEnumerable<EntityAccessType> accessTypes);

        /// <summary>
        /// Clear entity permissions from cache
        /// </summary>
        /// <param name="entityId"></param>
        void ClearEntityPermissionsFromCache(Guid entityId);
    }
}