using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ST.Core.Helpers;
using ST.Entities.Abstractions.Models.Tables;
using ST.Entities.Security.Abstractions;
using ST.Entities.Security.Abstractions.Enums;
using ST.Entities.Security.Abstractions.Exceptions;

namespace ST.Entities.Security.Extensions
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
            var accessService = IoC.Resolve<IEntityRoleAccessManager>();
            if (accessService == null) throw
                new EntitySecurityNotRegisteredServiceException($"{nameof(IEntityRoleAccessManager)} is not registered!");
            if (table == null) throw
                new NullReferenceException($"Table is null on {nameof(EntityAclExtensions)} for {nameof(GetUserRolesEntityAccesses)} method body");
            return await accessService.GetPermissionsAsync(roles, table.Id);
        }
    }
}
