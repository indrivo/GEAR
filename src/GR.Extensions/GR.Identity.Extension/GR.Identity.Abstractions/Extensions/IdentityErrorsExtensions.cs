using GR.Core.Helpers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections.Generic;

namespace GR.Identity.Abstractions.Extensions
{
    public static class IdentityErrorsExtensions
    {
        /// <summary>
        /// Append identity errors
        /// </summary>
        /// <param name="result"></param>
        /// <param name="errors"></param>
        /// <returns></returns>
        public static ResultModel<object> AppendIdentityErrors(this ResultModel<object> result, IEnumerable<IdentityError> errors)
        {
            foreach (var e in errors)
            {
                result.Errors.Add(new ErrorModel(e.Code, e.Description));
            }
            return result;
        }

        /// <summary>
        /// Append identity result errors to model state
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ModelStateDictionary AppendIdentityResult(this ModelStateDictionary binder, IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                binder.AddModelError(string.Empty, error.Description);
            }

            return binder;
        }

        /// <summary>
        /// Identity result to result model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public static ResultModel<T> ToResultModel<T>(this IdentityResult result)
        {
            var response = new ResultModel<T>();
            foreach (var e in result.Errors)
            {
                response.Errors.Add(new ErrorModel(e.Code, e.Description));
            }
            return response;
        }
    }
}