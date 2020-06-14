using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GR.Core.Extensions;
using GR.Core.Helpers;
using GR.Core.Helpers.Responses;
using GR.UserPreferences.Abstractions.Helpers.PreferenceTypes;
using GR.UserPreferences.Abstractions.Helpers.ResponseModels;

namespace GR.UserPreferences.Abstractions.Helpers
{
    public class DefaultUserPreferenceProvider
    {
        /// <summary>
        /// Items
        /// </summary>
        private static readonly IList<PreferenceItem> Items = new List<PreferenceItem>();

        /// <summary>
        /// Check if exist
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public virtual bool Exist(string key)
            => Items.Any(x => x.Key.ToLowerInvariant().Equals(key.ToLowerInvariant()));

        /// <summary>/
        /// Register new preference item
        /// </summary>
        /// <typeparam name="TConf"></typeparam>
        /// <param name="key"></param>
        /// <param name="conf"></param>
        public void RegisterPreferenceItem<TConf>(string key, TConf conf)
            where TConf : PreferenceItem
        {
            if (key.IsNullOrEmpty() || conf == null) return;
            if (Exist(key)) return;
            var isValid = conf.Validate();
            if (!isValid) return;
            Items.Add(conf);
        }

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <param name="key"></param>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public virtual async Task<BaseBuildResponse<object>> GetConfigurationAsync(string key, string currentValue)
        {
            var response = new BaseBuildResponse<object>();
            if (key.IsNullOrEmpty())
            {
                response.AddError("Invalid parameters");
                return response;
            }
            var item = Items.FirstOrDefault(x => x.Key.Equals(key));
            if (item == null)
            {
                response.AddError("Preference item configuration was not found");
                return response;
            }

            var conf = await item.GetConfigurationAsync(currentValue);
            return conf;
        }

        /// <summary>
        /// Get available keys
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<string> GetAvailableKeys() => Items.Select(x => x.Key).ToList();

        /// <summary>
        /// Validate value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual ResultModel ValidateValueByKey(string key, string value)
        {
            var response = new ResultModel();
            var conf = Items.FirstOrDefault(x => x.Key.Equals(key));
            if (conf == null) return new NotFoundResultModel();
            if (!conf.IsRequired) return conf.IsValidValue(value);
            if (!value.IsNullOrEmpty()) return conf.IsValidValue(value);
            response.AddError("Value is required");
            return response;
        }
    }
}