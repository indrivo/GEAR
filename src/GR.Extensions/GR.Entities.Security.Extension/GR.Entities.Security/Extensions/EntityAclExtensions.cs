using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Entities.Abstractions.Models.Tables;
using GR.Entities.Security.Abstractions;
using GR.Entities.Security.Abstractions.Enums;
using GR.Entities.Security.Abstractions.Exceptions;

namespace GR.Entities.Security.Extensions
{
    public static class EntityAclExtensions
    {
        /// <summary>
        /// Entity access control by defined roles
        /// </summary>
        /// <param name="table"></param>
        /// <param name="roles"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<EntityAccessType>> GetUserRolesEntityAccesses(this TableModel table, IEnumerable<string> roles)
        {
            var accessService = IoC.Resolve<IEntityRoleAccessService>();
            if (accessService == null) throw
                new EntitySecurityNotRegisteredServiceException($"{nameof(IEntityRoleAccessService)} is not registered!");
            if (table == null) throw
                new NullReferenceException($"Table is null on {nameof(EntityAclExtensions)} for {nameof(GetUserRolesEntityAccesses)} method body");
            return await accessService.GetPermissionsAsync(roles, table.Id);
        }
    }
}
