﻿using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.UserPreferences.Abstractions.Helpers.ResponseModels;

namespace GR.UserPreferences.Abstractions.Helpers.PreferenceTypes
{
    public class BoolPreferenceItem : PreferenceItem
    {
        /// <summary>
        /// Type of preference item
        /// </summary>
        public override string Type => "Boolean";

        /// <summary>
        /// Default value
        /// </summary>
        public virtual bool DefaultValue { get; set; }

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public override Task<BaseBuildResponse<object>> GetConfigurationAsync(string currentValue)
        {
            if (currentValue.IsNullOrEmpty()) return Task.FromResult(new BaseBuildResponse<object>
            {
                IsSuccess = true,
                Type = Type,
                Result = DefaultValue
            });

            bool.TryParse(currentValue, out var value);
            return Task.FromResult(new BaseBuildResponse<object>
            {
                IsSuccess = true,
                Type = Type,
                Result = value
            });
        }

        /// <summary>
        /// Check if is valid configuration
        /// </summary>
        /// <returns></returns>
        public override bool Validate() => true;
    }
}
