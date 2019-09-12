﻿using ST.Core.Helpers;
using System;
using System.IO;
using ST.Core.Razor.Enums;

namespace ST.Core.Razor.Helpers
{
    internal static class TemplateManager
    {
        /// <summary>
        /// Get template data
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="templateType"></param>
        /// <returns></returns>
        public static ResultModel<string> GetTagHelperTemplate(string templateName, TemplateType templateType = TemplateType.Html)
        {
            var result = new ResultModel<string>();
            if (string.IsNullOrEmpty(templateName)) throw new ArgumentNullException($"{nameof(templateName)} is null");
            try
            {
                result.Result = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, $"Templates/{templateName}.{templateType.ToString().ToLower()}"));
                result.IsSuccess = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;
        }
    }
}