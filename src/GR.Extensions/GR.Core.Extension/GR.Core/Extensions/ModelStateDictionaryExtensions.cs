using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using GR.Core.Helpers;

namespace GR.Core.Extensions
{
    public static class ModelStateDictionaryExtensions
    {
        /// <summary>
        /// Append errors to Model State
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static ModelStateDictionary AppendResultModelErrors(this ModelStateDictionary modelState, ICollection<IErrorModel> errors)
        {
            foreach (var error in errors)
            {
                modelState.AddModelError(error.Key, error.Message);
            }
            return modelState;
        }

        /// <summary>
        /// </summary>
        /// <param name="modelState"></param>
        /// <returns></returns>
        public static IEnumerable<IErrorModel> ToResultModelErrors(this ModelStateDictionary modelState)
        {
            foreach (var stateError in modelState.Values)
            {
                foreach (var error in stateError.Errors)
                {
                    yield return new ErrorModel(string.Empty, error.ErrorMessage);
                }
            }
        }
    }
}
