using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ST.Cache.Abstractions;
using ST.Core.Abstractions;
using ST.Core.Extensions;

namespace ST.Core.BaseControllers
{
    [Authorize]
    public abstract class BaseController<TIdentityContext, TEntityContext, TUser, TRole, TTenant, TNotify> : Controller
        where TUser : IdentityUser, IBaseModel
        where TRole : IdentityRole<string>, IBaseModel
        where TTenant : BaseModel
        where TIdentityContext : DbContext
        where TEntityContext : DbContext
    {
        /// <summary>
        /// Inject notify service
        /// </summary>
        protected readonly TNotify Notify;

        /// <summary>
        /// Inject app context
        /// </summary>
        protected readonly TIdentityContext ApplicationDbContext;

        /// <summary>
        /// Inject entities db context
        /// </summary>
        protected readonly TEntityContext Context;

        /// <summary>
        /// Inject UserManager
        /// </summary>
        protected readonly UserManager<TUser> UserManager;

        /// <summary>
        /// Inject RoleManager
        /// </summary>
        protected readonly RoleManager<TRole> RoleManager;

        /// <summary>
        /// Cache Service
        /// </summary>
        protected readonly ICacheService CacheService;


        protected BaseController(UserManager<TUser> userManager, RoleManager<TRole> roleManager, ICacheService cacheService,
            TIdentityContext applicationDbContext, TEntityContext context, TNotify notify)
        {
            UserManager = userManager;
            RoleManager = roleManager;
            CacheService = cacheService;
            ApplicationDbContext = applicationDbContext;
            Context = context;
            Notify = notify;
        }

        /// <summary>
        /// Tenant id
        /// </summary>
        protected Guid? CurrentUserTenantId
        {
            get
            {
                var tenantId = User?.Claims?.FirstOrDefault(x => x.Type == "tenant")?.Value?.ToGuid();
                if (tenantId != null) return tenantId;
                var user = UserManager.GetUserAsync(User).GetAwaiter().GetResult();
                if (user == null) return null;
                UserManager.AddClaimAsync(user, new Claim("tenant", user.TenantId.ToString())).GetAwaiter()
                    .GetResult();
                return user.TenantId;

            }
        }

        /// <summary>
        /// Get Current User async
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected async Task<TUser> GetCurrentUserAsync()
        {
            return await UserManager.GetUserAsync(User);
        }

        /// <summary>
        /// Get current user
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected TUser GetCurrentUser()
        {
            return GetCurrentUserAsync().GetAwaiter().GetResult();
        }

        /// <summary>
        /// Get User organization
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected async Task<TTenant> GetOrganizationOfUser()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return default;
            return await ApplicationDbContext.Set<TTenant>().FirstOrDefaultAsync(x => x.Id == user.TenantId);
        }
    }
}