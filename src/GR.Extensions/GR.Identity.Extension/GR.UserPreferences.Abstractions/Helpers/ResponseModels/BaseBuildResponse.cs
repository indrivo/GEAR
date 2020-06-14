using GR.Core.Helpers;

namespace GR.UserPreferences.Abstractions.Helpers.ResponseModels
{
    public class BaseBuildResponse<T> : ResultModel<T>
    {
        /// <summary>
        /// Le type de response
        /// </summary>
        public virtual string Type { get; set; }
    }
}
