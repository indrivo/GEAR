namespace GR.Core.Helpers.Responses
{
    public class SuccessResultModel<T> : ResultModel<T>
    {
        public SuccessResultModel()
        {

        }

        public SuccessResultModel(T result)
        {
            Result = result;
        }

        public override bool IsSuccess { get; set; } = true;
    }
}
