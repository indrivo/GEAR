using System.Collections.Generic;
using GR.Core;
using GR.Core.Helpers;

namespace GR.Entities.Security.Abstractions.Helpers
{
    public class AccessDeniedResult<T> : ResultModel<T>
    {
        /// <summary>
        /// Errors
        /// </summary>
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(GearSettings.ACCESS_DENIED_MESSAGE, GearSettings.ACCESS_DENIED_MESSAGE)
        };
    }
}
