using GR.Core;
using GR.Core.Helpers;
using GR.Core.Helpers.ErrorCodes;
using GR.Core.Razor.BaseControllers;
using GR.Core.Razor.Helpers.Filters;
using GR.Identity.Abstractions.Helpers.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR.Identity.Permissions.Api.Controllers
{
    [GearAuthorize(GearAuthenticationScheme.IdentityWithBearer)]
    [JsonApiExceptionFilter]
    [Route(DefaultApiRouteTemplate)]
    public class PermissionsApiController : BaseGearController
    {
        /// <summary>
        /// Access denied message
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult AccessDeniedResult()
        => Json(new ResultModel().AddError(ResultModelCodes.Forbidden, GearSettings.ACCESS_DENIED_MESSAGE));
    }
}