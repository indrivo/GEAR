using System.Collections.Generic;
using GR.Core.Helpers;

namespace GR.Identity.Abstractions.Helpers.Responses
{
    public class UserNotFoundResult<T> : ResultModel<T>
    {
        public UserNotFoundResult()
        {
            Errors = new List<IErrorModel>
            {
                new ErrorModel(string.Empty, "User not found")
            };
        }
    }
}
