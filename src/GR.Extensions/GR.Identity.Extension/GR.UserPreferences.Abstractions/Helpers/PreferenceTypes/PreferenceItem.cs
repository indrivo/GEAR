using System;
using System.Threading.Tasks;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.UserPreferences.Abstractions.Helpers.ResponseModels;

namespace GR.UserPreferences.Abstractions.Helpers.PreferenceTypes
{
    public abstract class PreferenceItem
    {
        /// <summary>
        /// Type
        /// </summary>
        public abstract string Type { get; }

        /// <summary>
        /// Key
        /// </summary>
        public virtual string Key { get; set; }

        /// <summary>
        /// Is required
        /// </summary>
        public virtual bool IsRequired { get; set; }

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <returns></returns>
        public abstract Task<BaseBuildResponse<object>> GetConfigurationAsync(string currentValue);

        /// <summary>
        /// Is Valid value
        /// </summary>
        public virtual Func<string, ResultModel> IsValidValue { get; set; } = x => new SuccessResultModel<object>()
            .ToBase();

        /// <summary>
        /// Validate
        /// </summary>
        /// <returns></returns>
        public abstract bool Validate();
    }
}
