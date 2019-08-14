using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.Entities.Security.Abstractions.Enums;

namespace ST.Entities.Security.Abstractions
{
    public interface IEntityRoleAccessManager
    {
        Task<ICollection<EntityAccessType>> GetAccessControl(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveReadAccess(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveDeletePermanentAccess(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveDeleteAccess(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveRestoreAccess(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveUpdateAccess(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveWriteAccess(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveAccess(IEnumerable<string> userRoles, Guid entityId, EntityAccessType accessType = EntityAccessType.Read);
    }
}
