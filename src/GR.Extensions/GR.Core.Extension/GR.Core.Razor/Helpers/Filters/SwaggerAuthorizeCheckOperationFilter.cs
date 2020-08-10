using System;
using System.Collections.Generic;
using System.Linq;
using GR.Core.Extensions;
using GR.Core.Razor.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GR.Core.Razor.Helpers.Filters
{
    /// <summary>
    ///     Swagger authorize check filter
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SwaggerAuthorizeCheckOperationFilter : IOperationFilter
    {
        #region Injectable

        /// <summary>
        /// Inject config
        /// </summary>
        private readonly SwaggerAuthOperationFilterConfig _operationFilterConfig;

        #endregion

        public SwaggerAuthorizeCheckOperationFilter(SwaggerAuthOperationFilterConfig operationFilterConfig)
        {
            _operationFilterConfig = operationFilterConfig;
        }

        /// <summary>
        ///     Apply
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var authAttributes = new List<AuthorizeAttribute>();
            var filterDescriptor = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var allowAnonymous = filterDescriptor.Select(filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);
            operation.ExternalDocs = new OpenApiExternalDocs
            {
                Description = "Postman collection",
                Url = new Uri(GearApplication.SystemConfig.EntryUri, "/postman")
            };
            if (context.MethodInfo.DeclaringType != null)
            {
                authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                    .Union(context.MethodInfo.GetCustomAttributes(true))
                    .OfType<AuthorizeAttribute>()
                    .ToList();

                if (!authAttributes.Any()) return;
            }

            if (!allowAnonymous)
            {
                operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
                operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
            }

            operation.Responses.Add("500", new OpenApiResponse { Description = "Internal server error" });

            if (!allowAnonymous)
            {
                if (operation.Parameters == null)
                    operation.Parameters = new List<OpenApiParameter>();

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Description = "Access token, use then authentication is made with token",
                    Required = false,
                    Schema = new OpenApiSchema
                    {
                        Type = "String",
                        Default = new OpenApiString("Bearer ")
                    }
                });

                // Padlock
                if (operation.Security == null) operation.Security = new List<OpenApiSecurityRequirement>();
                operation.Security.Add(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Name = "Bearer"
                        }, new List<string>()
                    }
                });
            }

            //Description
            var roles = authAttributes.Aggregate(string.Empty, (current, authAttribute) => current + authAttribute.Roles);
            roles = roles.IsNullOrEmpty() ? "Any" : roles;
            operation.Description = $"<h3>Roles:</h3> {roles}";

            if (!_operationFilterConfig.OpenApiOperations.Any()) return;
            foreach (var func in _operationFilterConfig.OpenApiOperations)
            {
                func?.Invoke(operation, context);
            }
        }
    }
}