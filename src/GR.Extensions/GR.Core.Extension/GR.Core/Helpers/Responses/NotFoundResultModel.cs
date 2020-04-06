using System.Collections.Generic;
using GR.Core.Helpers.ErrorCodes;

namespace GR.Core.Helpers.Responses
{
    public class NotFoundResultModel<T> : ResultModel<T>
    {
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(ResultModelCodes.NotFound, "Entry not found")
        };
    }

    public class NotFoundResultModel : ResultModel
    {
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(ResultModelCodes.NotFound, "Entry not found")
        };
    }
}
