using GR.Core.Helpers;
using GR.Identity.Abstractions;
using GR.Identity.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Identity.Razor.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class IdentityController : Controller
    {
        private UserManager<GearUser> UserManager { get; }

        private readonly ApplicationDbContext _context;

        public IdentityController(UserManager<GearUser> userManager, ApplicationDbContext context)
        {
            UserManager = userManager;
            _context = context;
        }

        /// <summary>
        ///     Get user group
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<IActionResult> GetUserGroup(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);
            if (user == null)
                return Json(new ResultModel
                {
                    IsSuccess = false,
                    Result = string.Empty
                });
            var courtId = _context.UserGroups.Where(s => s.UserId == user.Id).ToList().Select(s => s.AuthGroupId)
                .FirstOrDefault();
            return Json(new ResultModel
            {
                IsSuccess = true,
                Result = courtId
            });
        }

        /// <summary>
        ///     Get User role
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("[action]")]
        [Produces("application/json", Type = typeof(ResultModel))]
        public async Task<IActionResult> GetUserRole(string userId)
        {
            var user = await UserManager.FindByIdAsync(userId);

            if (user == null)
                return Json(new ResultModel
                {
                    IsSuccess = true,
                    Result = string.Empty
                });
            var roles = await UserManager.GetRolesAsync(user);
            if (roles == null || roles.Count <= 0)
                return Json(new ResultModel
                {
                    IsSuccess = true,
                    Result = string.Empty
                });
            var result = BuildSuccess(string.Join(",", roles));
            return Json(result);
        }

        private static ResultModel BuildSuccess(object data)
        {
            return new ResultModel
            {
                IsSuccess = true,
                Result = data
            };
        }
    }
}