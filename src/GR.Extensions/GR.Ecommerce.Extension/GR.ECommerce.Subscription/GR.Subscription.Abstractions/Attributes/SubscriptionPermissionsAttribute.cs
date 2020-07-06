using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Subscriptions.Abstractions.Attributes.SubscriptionPermissionValidators;
using GR.Subscriptions.Abstractions.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace GR.Subscriptions.Abstractions.Attributes
{
    public class SubscriptionPermissionsAttribute : TypeFilterAttribute
    {
        public SubscriptionPermissionsAttribute(params Type[] permissionValidators) : base(
            typeof(AuthorizeSubscriptionPermissionsAttributeExecutor))
        {
            Arguments = new object[] { new SubscriptionPermissionsAuthorizationRequirement(permissionValidators) };
        }
    }

    [Serializable]
    public class AuthorizeSubscriptionPermissionsAttributeExecutor : IAsyncResourceFilter
    {
        /// <summary>
        /// Permissions
        /// </summary>
        private readonly SubscriptionPermissionsAuthorizationRequirement _authorizationRequirement;

        /// <summary>
        /// Inject service
        /// </summary>
        private readonly ISubscriptionService<Subscription> _subscriptionService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="authorizationRequirement"></param>
        /// <param name="subscriptionService"></param>
        public AuthorizeSubscriptionPermissionsAttributeExecutor(SubscriptionPermissionsAuthorizationRequirement authorizationRequirement, ISubscriptionService<Subscription> subscriptionService)
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
                var grant = true;
                foreach (var validator in _authorizationRequirement.PermissionValidators)
                {
                    var instance = (ISubscriptionValidator)Activator.CreateInstance(validator);
                    var permission =
                        subscriptionReq.Result.SubscriptionPermissions.FirstOrDefault(x => x.Name.Equals(instance.AttributeName));
                    if (permission == null)
                    {
                        grant = false;
                        break;
                    }

                    if (instance.Validate(permission)) continue;
                    grant = false;
                    break;
                }

                if (!grant)
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
    public class SubscriptionPermissionsAuthorizationRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// Subscription name
        /// </summary>
        public IEnumerable<Type> PermissionValidators { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="permissionValidators"></param>
        public SubscriptionPermissionsAuthorizationRequirement(IEnumerable<Type> permissionValidators)
        {
            PermissionValidators = permissionValidators;
        }
    }
}