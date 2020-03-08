using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;

namespace GR.Core.Razor.Helpers
{
    /// <summary>
    /// Swagger Versioning
    /// </summary>
    public static class SwaggerVersioning
    {
        /// <summary>
        /// Provide a custom strategy for selecting actions.
        /// </summary>
        /// <returns>A lambda that returns true/false based on document name and ApiDescription</returns>
        public static bool DocInclusionPredicate(string version, ApiDescription apiDescription)
        {
            var values = apiDescription.RelativePath
                .Split('/')
                .Select(v => v.Replace("v{version}", version))
                .ToList();

            apiDescription.RelativePath = string.Join("/", values);

            var versionParameter = apiDescription.ParameterDescriptions
                .SingleOrDefault(p => p.Name == "version");

            if (versionParameter != null)
                apiDescription.ParameterDescriptions.Remove(versionParameter);
            else
            {
                if (values.Count < 2) return true;
                var regex = new Regex(@"v\d+");
                var match = regex.Match(values[1]);
                if (!match.Success) return true;
                values[1] = version;
                apiDescription.RelativePath = string.Join("/", values);
            }

            return true;
        }
    }
}