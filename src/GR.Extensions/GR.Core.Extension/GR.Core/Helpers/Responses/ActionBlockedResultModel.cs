using System.Collections.Generic;

namespace GR.Core.Helpers.Responses
{
    public class ActionBlockedResultModel<T> : ResultModel<T>
    {
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(string.Empty, "The action was blocked because you do not have permission to continue")
        };
    }
}
