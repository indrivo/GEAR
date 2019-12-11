using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GR.Core.Helpers
{
    /// <summary>
    /// <see cref="ErrorModel"/> extension methods
    /// </summary>
    public static class ErrorModelExtensions
    {
        /// <summary>
        /// Transforms <see cref="ModelError"/> to <see cref="ErrorModel"/>.
        /// </summary>
        public static ErrorModel ToErrorModel(this ModelError errorMessage)
        {
            if (errorMessage == null)
                throw new ArgumentNullException(nameof(errorMessage));

            ErrorModel errorModel = errorMessage.ErrorMessage;
            return errorModel;
        }
    }
}
