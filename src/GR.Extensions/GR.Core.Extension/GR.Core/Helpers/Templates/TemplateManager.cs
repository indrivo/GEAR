using System;
using System.Collections.Concurrent;
using System.IO;
using GR.Core.Extensions;
using GR.Core.Helpers.Templates.Enums;

namespace GR.Core.Helpers.Templates
{
    public static class TemplateManager
    {
        /// <summary>
        /// Templates
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> Templates = new ConcurrentDictionary<string, string>();

        /// <summary>
        /// Get template data
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        public static ResultModel<string> GetTemplateBody(string templateName, TemplateType templateType = TemplateType.Html)
        {
            var result = new ResultModel<string>();
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException($"{nameof(templateName)} is null");
            var path = $"Templates/{templateName}.{templateType.ToString().ToLower()}_template";
            Templates.TryGetValue(path, out var template);
            if (!template.IsNullOrEmpty())
            {
                result.IsSuccess = true;
                result.Result = template;
                return result;
            }

            var filePath = Path.Combine(AppContext.BaseDirectory, path);
            if (!File.Exists(filePath)) return result
                .AddError("Template not found");

            try
            {
                result.Result = File.ReadAllText(filePath);
                result.IsSuccess = true;

                Templates.TryAdd(path, result.Result);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }
    }
}
