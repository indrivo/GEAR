﻿using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;

namespace ST.Identity.Abstractions
{
    public interface IPermissionService
    {
        /// <summary>
        /// Has permission
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<bool> HasClaim(Guid userId, string permission);
        /// <summary>
        /// Has permission
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        Task<bool> HasClaim(ApplicationUser user, string permission);
        /// <summary>
        /// Has permission
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        bool HasClaim(ClaimsPrincipal user, string permission);
        /// <summary>
        /// Get user claims
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<Claim>> GetUserClaims(Guid userId);
        /// <summary>
        /// Set object value in cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        Task SetObjectAsync<T>(string key, T obj);
        /// <summary>
        /// Get object from cache
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> GetObjectAsync<T>(string key);
        /// <summary>
        /// Get roles permissions
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<RolePermissionViewModel>> RolesPermissionsAsync();
        /// <summary>
        /// Refresh cache
        /// </summary>
        /// <returns></returns>
        Task RefreshCache();
        /// <summary>
        /// Check if user have permission
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="userPermissions"></param>
        /// <returns></returns>
        Task<bool> HasPermission(IList<string> roles, IList<string> userPermissions);

        /// <summary>
        /// Refresh cache by role
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        Task RefreshCacheByRole(string roleName, bool delete = false);
    }
}