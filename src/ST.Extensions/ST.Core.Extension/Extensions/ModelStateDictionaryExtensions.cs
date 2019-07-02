using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ST.Core.Helpers;

namespace ST.Core.Extensions
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
    }
}
