using System.Collections.Generic;

namespace GR.Core.Helpers.Responses
{
    public class InvalidParametersResultModel<T> : ResultModel<T>
    {
        public InvalidParametersResultModel() { }
        public InvalidParametersResultModel(string message)
        {
            Errors.Add(new ErrorModel(string.Empty, message));
        }
        public sealed override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(string.Empty, "Invalid parameters")
        };
    }

    public class InvalidParametersResultModel : ResultModel
    {
        public InvalidParametersResultModel() { }
        public InvalidParametersResultModel(string message)
        {
            Errors.Add(new ErrorModel(string.Empty, message));
        }

        public sealed override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(string.Empty, "Invalid parameters")
        };
    }
}
