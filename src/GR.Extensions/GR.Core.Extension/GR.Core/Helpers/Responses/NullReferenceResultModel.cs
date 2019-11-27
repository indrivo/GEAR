using System.Collections.Generic;

namespace GR.Core.Helpers.Responses
{
    public sealed class NullReferenceResultModel<T> : ResultModel<T>
    {
        public NullReferenceResultModel()
        {

        }

        public NullReferenceResultModel(string message)
        {
            Errors.Add(new ErrorModel(string.Empty, message));
        }

        public override ICollection<IErrorModel> Errors { get; set; } = new List<IErrorModel>
        {
            new ErrorModel(string.Empty, "A null reference was identified")
        };
    }
}
