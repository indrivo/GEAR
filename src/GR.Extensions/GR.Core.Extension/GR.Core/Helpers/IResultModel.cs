using System;
using System.Collections.Generic;

namespace GR.Core.Helpers
{
    public interface IResultModel<T>
    {
        /// <summary>
        /// Bool indicating that the request resulted with success.
        /// If False than the <see cref="ErrorModel"/> will 
        /// contain a Error message that produced this error.
        /// </summary>

        bool IsSuccess { get; set; }

        /// <summary>
        /// This property will contain error keys if any.
        /// </summary>

        ICollection<IErrorModel> Errors { get; set; }

        /// <summary>
        /// The result of the response, if there is no errors.
        /// </summary>

        T Result { get; set; }

        Guid? KeyEntity { get; set; }
    }
}
