using System;
using System.Collections.Generic;
using GR.Localization.Abstractions.Helpers;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GR.Localization.Api.Helpers
{
    public static class LocalizationOpenApiHelper
    {
        /// <summary>
        /// Apply permissions docs
        /// </summary>
        public static Action<OpenApiOperation, OperationFilterContext> LocalizationDocs => (operation, context) =>
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = LocalizationResources.XLocalizationIdentifier,
                In = ParameterLocation.Header,
                Description = "The language in which the messages will be translated (optional)",
                Required = false,
                Schema = new OpenApiSchema
                {
                    Type = "String",
                    Default = new OpenApiString("en")
                }
            });
        };
    }
}
