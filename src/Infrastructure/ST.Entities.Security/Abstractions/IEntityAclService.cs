using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ST.Entities.Security.Enums;

namespace ST.Entities.Security.Abstractions
{
    public interface IEntityAclService
    {
        Task<ICollection<EntityAccessType>> GetAccessControl(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveReadAccess(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveDeleteAccess(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveUpdateAccess(IEnumerable<string> userRoles, Guid entityId);
        Task<bool> HaveWriteAccess(IEnumerable<string> userRoles, Guid entityId);
    }
}
