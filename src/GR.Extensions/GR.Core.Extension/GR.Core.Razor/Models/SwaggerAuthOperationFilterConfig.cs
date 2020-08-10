using System;
using System.Collections.Generic;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace GR.Core.Razor.Models
{
    public class SwaggerAuthOperationFilterConfig
    {
        /// <summary>
        /// Open api operations
        /// </summary>
        public IList<Action<OpenApiOperation, OperationFilterContext>> OpenApiOperations { get; set; } = new List<Action<OpenApiOperation, OperationFilterContext>>();
    }
}
