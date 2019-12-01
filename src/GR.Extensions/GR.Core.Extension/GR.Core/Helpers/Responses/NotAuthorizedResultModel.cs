using System.Collections.Generic;

namespace GR.Core.Helpers.Responses
{
    public class NotAuthorizedResultModel<T> : ResultModel<T>
    {
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(string.Empty, "You are not authorized")
        };
    }

    public class NotAuthorizedResultModel : NotAuthorizedResultModel<object>
    {

    }
}
