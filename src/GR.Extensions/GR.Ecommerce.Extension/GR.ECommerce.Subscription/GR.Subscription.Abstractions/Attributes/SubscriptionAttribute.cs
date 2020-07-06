using System;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Subscriptions.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GR.Subscriptions.Abstractions.Attributes
{
    public class SubscriptionAttribute : TypeFilterAttribute
    {
        public SubscriptionAttribute(string subscriptionName) : base(
            typeof(AuthorizeSubscriptionAttributeExecutor))
        {
            Arguments = new object[] { new SubscriptionNameAuthorizationRequirement(subscriptionName) };
        }
    }

    [Serializable]
    public class AuthorizeSubscriptionAttributeExecutor : IAsyncResourceFilter
    {
        /// <summary>
        /// Permissions
        /// </summary>
        private readonly SubscriptionNameAuthorizationRequirement _authorizationRequirement;

        /// <summary>
        /// Inject service
        /// </summary>
        private readonly ISubscriptionService<Subscription> _subscriptionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authorizationRequirement"></param>
        /// <param name="subscriptionService"></param>
        public AuthorizeSubscriptionAttributeExecutor(SubscriptionNameAuthorizationRequirement authorizationRequirement, ISubscriptionService<Subscription> subscriptionService)
        {
            _authorizationRequirement = authorizationRequirement;
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// On executing context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var denyResult = new ResultModel();
            denyResult.AddError(GearSettings.ACCESS_DENIED_MESSAGE, GearSettings.ACCESS_DENIED_MESSAGE);
            var responseBody = context.HttpContext.Response;
            var subscriptionReq = await _subscriptionService.GetLastSubscriptionForUserAsync();
            if (!subscriptionReq.IsSuccess)
            {
                await responseBody.WriteAsync(denyResult.SerializeAsJson());
            }
            else if (!subscriptionReq.Result.IsValid)
            {
                await responseBody.WriteAsync(denyResult.SerializeAsJson());
            }
            else
            {
                if (subscriptionReq.Result.Name != _authorizationRequirement.SubscriptionName)
                {
                    await responseBody.WriteAsync(denyResult.SerializeAsJson());
                }
                else
                {
                    await next();
                }
            }
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Requirements class
    /// </summary>
    public class SubscriptionNameAuthorizationRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Subscription name
        /// </summary>
        public string SubscriptionName { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="subscriptionName"></param>
        public SubscriptionNameAuthorizationRequirement(string subscriptionName)
        {
            SubscriptionName = subscriptionName;
        }
    }
}
