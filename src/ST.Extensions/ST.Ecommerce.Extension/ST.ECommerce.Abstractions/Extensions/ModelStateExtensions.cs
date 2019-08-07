using Microsoft.AspNetCore.Mvc.ModelBinding;
using ST.Core.Helpers;
using ST.ECommerce.Abstractions.Helpers;

namespace ST.ECommerce.Abstractions.Extensions
{
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Add commerce error to model state
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static ModelStateDictionary AddCommerceError(this ModelStateDictionary modelState, ErrorKey error)
        {
            Arg.NotNull(modelState, nameof(ModelStateDictionary));
            Arg.NotNull<ErrorKey>(error, nameof(ErrorKey));
            modelState.AddModelError(error.Key, error.Value);
            return modelState;
        }
    }
}
