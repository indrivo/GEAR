using System.Collections.Generic;

namespace GR.Core.Helpers.Responses
{
    public class InvalidParametersResultModel<T> : ResultModel<T>
    {
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(string.Empty, "Invalid parameters")
        };
    }


    public class InvalidParametersResultModel : ResultModel
    {
        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(string.Empty, "Invalid parameters")
        };
    }
}
