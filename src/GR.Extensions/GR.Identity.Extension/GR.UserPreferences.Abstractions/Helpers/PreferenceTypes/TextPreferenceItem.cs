using System.Threading.Tasks;
using GR.UserPreferences.Abstractions.Helpers.ResponseModels;

namespace GR.UserPreferences.Abstractions.Helpers.PreferenceTypes
{
    public class TextPreferenceItem : PreferenceItem
    {
        /// <summary>
        /// Type of preference item
        /// </summary>
        public override string Type => "Text";

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public override Task<BaseBuildResponse<object>> GetConfigurationAsync(string currentValue)
        => Task.FromResult(new BaseBuildResponse<object>
        {
            IsSuccess = true,
            Type = Type,
            Result = currentValue
        });

        /// <summary>
        /// Check if is valid configuration
        /// </summary>
        /// <returns></returns>
        public override bool Validate() => true;
    }
}
