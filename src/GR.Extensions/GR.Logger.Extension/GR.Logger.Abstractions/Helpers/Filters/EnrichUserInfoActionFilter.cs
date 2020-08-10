using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog.Context;

namespace GR.Logger.Abstractions.Helpers.Filters
{
    public class EnrichUserInfoActionFilter : IAsyncActionFilter
    {
        #region Injectable

        /// <summary>
        /// Inject accessor
        /// </summary>
        private readonly IHttpContextAccessor _httpContextAccessor;

        #endregion

        public EnrichUserInfoActionFilter(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var httpUser = _httpContextAccessor.HttpContext.User;

            if (httpUser.Identity.IsAuthenticated)
            { 
                LogContext.PushProperty("UserName", httpUser.Identity.Name);
                LogContext.PushProperty("AuthenticationType", httpUser.Identity.AuthenticationType);
                LogContext.PushProperty("Claims", httpUser.Claims);
            }
            else
            {
                LogContext.PushProperty("UserName", "-");
            }

            await next();
        }
    }
}
