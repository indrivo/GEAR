using GR.Core.Helpers;
using GR.Documents.Abstractions.Helpers;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GR.Documents.Abstractions.Extensions
{
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Add commerce error to model state
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static ModelStateDictionary AddError(this ModelStateDictionary modelState, ErrorKey error)
        {
            Arg.NotNull(modelState, nameof(ModelStateDictionary));
            Arg.NotNull<ErrorKey>(error, nameof(ErrorKey));
            modelState.AddModelError(error.Key, error.Value);
            return modelState;
        }
    }
}
