using System;
using System.Linq;
using System.Threading.Tasks;
using GR.AccountActivity.Abstractions.Helpers;
using GR.Core;
using GR.Identity.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;

namespace GR.AccountActivity.Abstractions.ActionFilters
{
    public class ActivityTrackerFilter : IAsyncActionFilter
    {
        #region Injectable

        /// <summary>
        /// Inject activity service
        /// </summary>
        protected readonly IUserActivityService ActivityService;

        /// <summary>
        /// Inject user manager
        /// </summary>
        protected readonly IUserManager<GearUser> UserManager;

        #endregion

        public ActivityTrackerFilter(IUserActivityService activityService, IUserManager<GearUser> userManager)
        {
            ActivityService = activityService;
            UserManager = userManager;
        }

        /// <summary>
        /// On action execution
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public virtual async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var controller = context.RouteData.Values["controller"].ToString();
            if (!context.HttpContext.User.Identity.IsAuthenticated || controller == "AccountActivity" || controller == "AccountActivityApi")
            {
                await next();
                return;
            }

            var roles = UserManager.GetRolesFromClaims()?.ToList();
            if (roles?.Any(x => x.Equals(GlobalResources.Roles.ADMINISTRATOR)) ?? false)
            {
                await next();
                return;
            }

            //Disable check track new device on new registration period
            var userReq = (await UserManager.GetCurrentUserAsync()).Result;
            if (DateTime.Now - userReq.Created < AccountActivityResources.TimeToDisableTrackingDevice)
            {
                await next();
                return;
            }

            var deviceCheck = await ActivityService.FindDeviceAsync(context.HttpContext);

            if (!deviceCheck.IsSuccess)
            {
                var addReq = await ActivityService.AddNewDeviceAsync(context.HttpContext);
                await RedirectToNotConfirmedDeviceViewAsync(context, addReq.Result);
                return;
            }

            if (!deviceCheck.Result.IsConfirmed)
            {
                await RedirectToNotConfirmedDeviceViewAsync(context, deviceCheck.Result.Id);
                return;
            }

            await next();
        }

        /// <summary>
        /// Redirect
        /// </summary>
        /// <param name="context"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
        protected static async Task RedirectToNotConfirmedDeviceViewAsync(ActionExecutingContext context, Guid deviceId)
        {
            var authHeader = context.HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authHeader?.StartsWith("Bearer ") ?? false)
            {
                context.Result = new JsonResult(new NotConfirmedDeviceResultModel());
            }
            else context.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new
                    {
                        controller = "AccountActivity",
                        action = "NotConfirmedDevice",
                        deviceId
                    })
                );

            await context.Result.ExecuteResultAsync(context);
        }
    }
}
