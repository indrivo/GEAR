using System.Collections.Generic;

namespace GR.Core.Helpers.Responses
{
    public class InvalidParametersResultModel<T> : ResultModel<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public InvalidParametersResultModel()
        {
            Errors = new List<IErrorModel>
            {
                new ErrorModel(string.Empty, "Invalid parameters")
            };
        }
    }
}
