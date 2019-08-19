using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    }
}