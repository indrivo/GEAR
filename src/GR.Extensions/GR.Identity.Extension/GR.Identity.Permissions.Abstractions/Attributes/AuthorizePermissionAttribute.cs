using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GR.Identity.Permissions.Abstractions.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// Attribute checks if User has required permission
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class AuthorizePermissionAttribute : TypeFilterAttribute
    {
        /// <inheritdoc />
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="permission">List of required permissions</param>
        public AuthorizePermissionAttribute(params string[] permission) : base(typeof(AuthorizePermissionAttributeExecutor))
        {
            Arguments = new object[] { new PermissionAuthorizationRequirement(permission) };
        }

        /// <summary>
        /// Executes permission check
        /// </summary>
        private class AuthorizePermissionAttributeExecutor : Attribute, IAsyncResourceFilter
        {
            /// <summary>
            /// Private readonly not nullable PermissionAuthorizationRequirement object.
            /// </summary>
            private readonly PermissionAuthorizationRequirement _requiredPermissions;

            /// <summary>
            /// Inject Permission service
            /// </summary>
            private readonly IPermissionService _permissionService;

            /// <inheritdoc />
            /// <summary>
            /// Public Constructor
            /// </summary>
            /// <param name="requiredPermission">PermissionAuthorizationRequirement object</param>
            /// <param name="permissionService"></param>
            public AuthorizePermissionAttributeExecutor(PermissionAuthorizationRequirement requiredPermission, IPermissionService permissionService)
            {
                _requiredPermissions = requiredPermission;
                _permissionService = permissionService;
            }

            /// <inheritdoc />
            /// <summary>
            /// On Resource Executing async method
            /// Throws UnauthorizedResult if permission check is false
            /// </summary>
            /// <param name="context">Resource executing context</param>
            /// <param name="del">Resource executing delegate</param>
            /// <returns>Task</returns>
            public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate del)
            {
                try
                {
                    var roles = context.HttpContext.User.Claims.Where(x => x.Type.Equals("role") || x.Type.EndsWith("role")).Select(x => x.Value)
                        .ToList();
                    var permissions = _requiredPermissions.RequiredPermissions.ToList();
                    var hasPermission = await _permissionService.HasPermissionAsync(roles, permissions);
                    if (!hasPermission)
                    {
                        context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                        await context.Result.ExecuteResultAsync(context);
                    }
                    else await del();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    await context.Result.ExecuteResultAsync(context);
                }
            }
        }
    }
}