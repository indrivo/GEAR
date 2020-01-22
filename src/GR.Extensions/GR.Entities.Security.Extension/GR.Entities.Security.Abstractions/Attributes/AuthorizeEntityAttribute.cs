using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Entities.Security.Abstractions.Enums;
using GR.Entities.Security.Abstractions.Helpers;

namespace GR.Entities.Security.Abstractions.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AuthorizeEntityAttribute : TypeFilterAttribute
    {
        public AuthorizeEntityAttribute(params EntityAccessType[] accessType) : base(
            typeof(AuthorizeEntityPermissionAttributeExecutor))
        {
            Arguments = new object[] { new EntityPermissionAuthorizationRequirement(accessType) };
        }
    }

    [Serializable]
    public class AuthorizeEntityPermissionAttributeExecutor : IAsyncResourceFilter
    {
        /// <summary>
        /// Permissions
        /// </summary>
        private readonly EntityPermissionAuthorizationRequirement _authorizationRequirement;

        /// <summary>
        /// Inject service
        /// </summary>
        private readonly IEntityRoleAccessService _entityRoleAccessService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="entityRoleAccessService"></param>
        /// <param name="authorizationRequirement"></param>
        public AuthorizeEntityPermissionAttributeExecutor(IEntityRoleAccessService entityRoleAccessService,
            EntityPermissionAuthorizationRequirement authorizationRequirement)
        {
            _entityRoleAccessService = entityRoleAccessService;
            _authorizationRequirement = authorizationRequirement;
        }

        /// <summary>
        /// On executing context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="next"></param>
        /// <returns></returns>
        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var req = context.HttpContext.Request;
            req.EnableRewind();
            var reader = new StreamReader(req.Body);
            var body = await reader.ReadToEndAsync();
            req.Body.Position = 0;
            var responseBody = context.HttpContext.Response;

            var data = body.Deserialize<RequestData>();
            if (data == null)
            {
                await responseBody.WriteAsync(new ResultModel
                {
                    Errors = new List<IErrorModel>
                        {
                            new ErrorModel(nameof(BadRequestResult), nameof(BadRequestResult))
                        }
                }.SerializeAsJson());
            }
            else if (!await _entityRoleAccessService.HaveAccessAsync(data.EntityName,
             _authorizationRequirement.RequiredPermissions))
            {
                await responseBody.WriteAsync(AccessDeniedResult<object>.Instance.SerializeAsJson());
            }
            else await next();
        }
    }

    /// <inheritdoc />
    /// <summary>
    /// Requirements class
    /// </summary>
    public class EntityPermissionAuthorizationRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// List of Required Permissions
        /// </summary>
        public IEnumerable<EntityAccessType> RequiredPermissions { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requiredPermissions">List of required Permissions</param>
        public EntityPermissionAuthorizationRequirement(IEnumerable<EntityAccessType> requiredPermissions)
        {
            RequiredPermissions = requiredPermissions;
        }
    }
}