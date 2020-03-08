using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GR.WebApplication.Helpers.Filters
{
    /// <summary>
    ///     Swagger authorize check filter
    /// </summary>
    // ReSharper disable once ClassNeverInstantiated.Global
    public class SwaggerAuthorizeCheckOperationFilter : IOperationFilter
    {
        /// <summary>
        ///     Apply
        /// </summary>
        /// <param name="operation"></param>
        /// <param name="context"></param>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (context.MethodInfo.DeclaringType != null)
            {
                var authAttributes = context.MethodInfo.DeclaringType.GetCustomAttributes(true)
                    .Union(context.MethodInfo.GetCustomAttributes(true))
                    .OfType<AuthorizeAttribute>();

                if (!authAttributes.Any()) return;
            }

            operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });
            operation.Responses.Add("403", new OpenApiResponse { Description = "Forbidden" });
        }
    }
}