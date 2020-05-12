using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GR.UserPreferences.Abstractions.Helpers.ResponseModels;

namespace GR.UserPreferences.Abstractions.Helpers.PreferenceTypes
{
    public class ListPreferenceItem : PreferenceItem
    {
        /// <summary>
        /// Type
        /// </summary>
        public override string Type => "List";

        /// <summary>
        /// Get configuration
        /// </summary>
        /// <param name="currentValue"></param>
        /// <returns></returns>
        public override async Task<BaseBuildResponse<object>> GetConfigurationAsync(string currentValue)
        {
            var data = await GetListValuesAsync(currentValue);

            return new BaseBuildResponse<object>
            {
                IsSuccess = true,
                Result = data,
                Type = Type
            };
        }

        /// <summary>
        /// Check if is valid
        /// </summary>
        /// <returns></returns>
        public override bool Validate()
        {
            var isValid = ResolveListItems != null;
            return isValid;
        }

        /// <summary>
        /// Task for resolve list of items
        /// </summary>
        public Func<string, Task<IEnumerable<DisplayItem>>> ResolveListItems = null;

        /// <summary>
        /// Get list of items
        /// </summary>
        /// <param name="selected"></param>
        /// <returns></returns>
        public virtual async Task<IEnumerable<DisplayItem>> GetListValuesAsync(string selected)
        {
            var items = await ResolveListItems(selected);
            return items;
        }
    }

    public class DisplayItem
    {
        public virtual string Label { get; set; }
        public virtual string Id { get; set; }
        public virtual bool Selected { get; set; }
    }
}
