using System;
using System.Linq;
using GR.Core.Extensions;
using GR.Identity.Permissions.Abstractions.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GR.Identity.Permissions.Api.Helpers
{
    public static class PermissionsOpenApiHelper
    {
        /// <summary>
        /// Apply permissions docs
        /// </summary>
        public static Action<OpenApiOperation, OperationFilterContext> PermissionDocs => (operation, context) =>
        {
            operation.Description += "\n<h3>Permissions:</h3>";

            if (context.MethodInfo.DeclaringType == null) return;
            var permsListAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizePermissionAttribute>()
                .ToList();
            var permissions = permsListAttributes.Aggregate(string.Empty, (current, permissionAttribute) => current + permissionAttribute.Arguments?[0]
                ?.Is<PermissionAuthorizationRequirement>()
                ?.RequiredPermissions?.Join(","));

            operation.Description += permissions.IsNullOrEmpty() ? "Not required" : permissions;
        };
    }
}