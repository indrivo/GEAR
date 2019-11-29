using System.Collections.Generic;

namespace GR.Core.Helpers.Responses
{
    public class NotFoundResultModel<T> : ResultModel<T>
    {
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(string.Empty, "Entry not found")
        };
    }

    public class NotFoundResultModel : ResultModel
    {
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(string.Empty, "Entry not found")
        };
    }
}
