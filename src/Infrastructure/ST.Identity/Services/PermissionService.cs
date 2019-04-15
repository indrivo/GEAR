﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Mapster;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using ST.Identity.Abstractions;
using ST.Identity.Data;
using ST.Identity.Data.Permissions;
using ST.Identity.Data.UserProfiles;

namespace ST.Identity.Services
{
    public class PermissionService<TContext> : IPermissionService where TContext: ApplicationDbContext
    {
        /// <summary>
        /// Inject sign in manager
        /// </summary>
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Inject distributed cache
        /// </summary>
        private readonly IDistributedCache _cache;

        /// <summary>
        /// Inject context
        /// </summary>
        private readonly TContext _context;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="signInManager"></param>
        /// <param name="cache"></param>
        /// <param name="context"></param>
        public PermissionService(SignInManager<ApplicationUser> signInManager, IDistributedCache cache,
            TContext context)
        {
            _signInManager = signInManager;
            _cache = cache;
            _context = context;
        }

        /// <inheritdoc />
        /// <summary>
        /// Has permission for user id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<bool> HasClaim(Guid userId, string permission)
        {
            var claims = await GetUserClaims(userId);
            return claims.Select(x => x.Type).Contains(permission);
        }

        /// <inheritdoc />
        /// <summary>
        /// Has permission for user
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public async Task<bool> HasClaim(ApplicationUser user, string permission)
        {
            if (user == null) return false;
            var claims = await _signInManager.UserManager.GetClaimsAsync(user);
            return claims.Select(x => x.Type).Contains(permission);
        }

        /// <inheritdoc />
        /// <summary>
        /// Has permission from ClaimsPrincipal
        /// </summary>
        /// <param name="user"></param>
        /// <param name="permission"></param>
        /// <returns></returns>
        public bool HasClaim(ClaimsPrincipal user, string permission)
        {
            return user != null && user.Claims.Select(x => x.Type).Contains(permission);
        }

        /// <inheritdoc />
        /// <summary>
        /// Get user claims
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Claim>> GetUserClaims(Guid userId)
        {
            if (userId == Guid.Empty) return default;
            var user = await _signInManager.UserManager.Users.FirstOrDefaultAsync(x => x.Id.Equals(userId.ToString()));
            if (user == null) return default;
            return await _signInManager.UserManager.GetClaimsAsync(user);
        }

        /// <inheritdoc />
        /// <summary>
        /// Set object async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public async Task SetObjectAsync<T>(string key, T obj)
        {
            if (!string.IsNullOrEmpty(key))
            {
                try
                {
                    var data = JsonConvert.SerializeObject(obj);
                    if (!string.IsNullOrEmpty(data))
                    {
                        await _cache.SetStringAsync(key, data);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Get object async
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetObjectAsync<T>(string key)
        {
            if (string.IsNullOrEmpty(key)) return default;
            try
            {
                var data = await _cache.GetStringAsync(key);
                return string.IsNullOrEmpty(data) ? default : JsonConvert.DeserializeObject<T>(data);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return default;
        }

        /// <inheritdoc />
        /// <summary>
        /// Get roles permissions
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<RolePermissionViewModel>> RolesPermissionsAsync()
        {
            var result = new List<RolePermissionViewModel>();
            var roles = await _context.Roles.ToListAsync();
            foreach (var role in roles)
            {
                var data = role.Adapt<RolePermissionViewModel>();
                var permissions = await _context.RolePermissions.Where(x => x.RoleId.Equals(role.Id))
                    .Select(c => _context.Permissions.FirstOrDefault(g => g.Id.Equals(c.PermissionId)))
                    .ToListAsync();
                data.Permissions = permissions;
                result.Add(data);
            }

            return result;
        }

        /// <inheritdoc />
        /// <summary>
        /// Refresh Permission for roles
        /// </summary>
        /// <returns></returns>
        public async Task RefreshCache()
        {
            var roles = await RolesPermissionsAsync();
            var store = roles.ToDictionary(role => role.Name, role => role.Permissions.Select(x => x.PermissionKey));
            await SetObjectAsync(CacheKeyName, store);
        }


        /// <inheritdoc />
        /// <summary>
        /// Refresh cache by role 
        /// </summary>
        /// <param name="roleName"></param>
        /// <param name="delete"></param>
        /// <returns></returns>
        public async Task RefreshCacheByRole(string roleName, bool delete = false)
        {
            var storeDictionary = await GetObjectAsync<Dictionary<string, IEnumerable<string>>>(CacheKeyName);
            if (delete)
            {
                if (storeDictionary.ContainsKey(roleName))
                {
                    storeDictionary.Remove(roleName);
                }

                return;
            }

            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name.Equals(roleName));

            if (role == null)
            {
                return;
            }

            var data = role.Adapt<RolePermissionViewModel>();
            var permissions = await _context.RolePermissions.Where(x => x.RoleId.Equals(role.Id))
                .Select(c => _context.Permissions.FirstOrDefault(g => g.Id.Equals(c.PermissionId)))
                .ToListAsync();

            data.Permissions = permissions;


            if (storeDictionary.ContainsKey(data.Name))
            {
                storeDictionary[data.Name] = data.Permissions.Select(x => x.PermissionKey);
            }
            else
            {
                storeDictionary.Add(data.Name, data.Permissions.Select(x => x.PermissionKey));
            }

            await SetObjectAsync(CacheKeyName, storeDictionary);
        }

        /// <inheritdoc />
        /// <summary>
        /// Check if user have permission
        /// </summary>
        /// <param name="roles"></param>
        /// <param name="userPermissions"></param>
        /// <returns></returns>
        public async Task<bool> HasPermission(IList<string> roles, IList<string> userPermissions)
        {
            var match = new List<string>();
            if (!userPermissions.Any() || !roles.Any()) return false;
            var data = await GetObjectAsync<Dictionary<string, IEnumerable<string>>>(CacheKeyName);

            try
            {
                foreach (var role in data)
                {
                    if (!roles.Contains(role.Key)) continue;
                    foreach (var perm in userPermissions)
                    {
                        if (!role.Value.Contains(perm)) continue;
                        if (!match.Contains(perm)) match.Add(perm);
                    }

                    if (match.Count.Equals(userPermissions.Count)) return true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        /// <summary>
        /// Name of stored role permission in cache
        /// </summary>
        private const string CacheKeyName = "PermissionRole";
    }
}