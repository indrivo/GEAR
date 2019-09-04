﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using ST.Core.Helpers;

namespace ST.Identity.Abstractions.Extensions
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
    }
}
