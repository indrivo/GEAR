using System.Collections.Generic;

namespace GR.Core.Helpers.Responses
{
    public class NotFoundResultModel<T> : ResultModel<T>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NotFoundResultModel()
        {
            Errors = new List<IErrorModel>
            {
                new ErrorModel(string.Empty, "Entry not found")
            };
        }
    }
}
