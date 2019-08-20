using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using ST.Core;
using ST.Core.Extensions;
using ST.Core.Helpers;
using ST.Identity.Abstractions;

namespace ST.Identity.Services
{
    public class IdentityUserManager : IUserManager<ApplicationUser>
    {
        /// <summary>
        /// Inject user manager
        /// </summary>
        public UserManager<ApplicationUser> UserManager { get; }

        /// <summary>
        /// Inject role manager
        /// </summary>
        public RoleManager<ApplicationRole> RoleManager { get; }

        /// <summary>
        /// Inject context accessor
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityUserManager(UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor, RoleManager<ApplicationRole> roleManager)
        {
            UserManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            RoleManager = roleManager;
        }

        /// <summary>
        /// Get current user
        /// </summary>
        /// <returns></returns>
        public async Task<ResultModel<ApplicationUser>> GetCurrentUserAsync()
        {
            var result = new ResultModel<ApplicationUser>();
            var user = await UserManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            result.IsSuccess = user != null;
            result.Result = user;
            return result;
        }

        /// <summary>
        /// Get roles from claims
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetRolesFromClaims()
        {
            var roles = _httpContextAccessor.HttpContext.User.Claims
                .Where(x => x.Type.Equals("role") || x.Type.EndsWith("role")).Select(x => x.Value)
                .ToList();
            return roles;
        }

        /// <summary>
        /// Tenant id
        /// </summary>
        public virtual Guid? CurrentUserTenantId
        {
            get
            {
                Guid? val = _httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x => x.Type == "tenant")?.Value
                                ?.ToGuid() ?? Settings.TenantId;
                var userManager = IoC.Resolve<UserManager<ApplicationUser>>();
                if (val != Guid.Empty) return val;
                var user = userManager.GetUserAsync(_httpContextAccessor?.HttpContext?.User).GetAwaiter()
                    .GetResult();
                if (user == null) return null;
                var userClaims = userManager.GetClaimsAsync(user).GetAwaiter().GetResult();
                val = userClaims?.FirstOrDefault(x => x.Type == "tenant" && !string.IsNullOrEmpty(x.Value))?.Value?.ToGuid();

                return val;
            }
        }
    }
}