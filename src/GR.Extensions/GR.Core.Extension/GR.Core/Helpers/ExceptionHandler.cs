using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace GR.Core.Helpers
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Returns ResultModel of type T with exception message passed from enumeration exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exceptionMessageEnum"></param>
        /// <returns></returns>
        public static ResultModel<T> ToErrorModel<T>(this Enum exceptionMessageEnum)
        {
            return new ResultModel<T>
            {
                IsSuccess = false,
                Errors = new List<IErrorModel>
                {
                    new ErrorModel
                    {
                        Key = string.Empty,
                        Message = exceptionMessageEnum.GetEnumDescription()
                    },
                }
            };
        }

        /// <summary>
        /// Returns ResultModel of type T with exception message passed from enumeration exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exceptionMessageEnum"></param>
        /// <returns></returns>
        public static ResultModel ToErrorModel(this Enum exceptionMessageEnum)
        {
            return new ResultModel
            {
                IsSuccess = false,
                Errors = new List<IErrorModel>
                {
                    new ErrorModel
                    {
                        Key = string.Empty,
                        Message = exceptionMessageEnum.GetEnumDescription()
                    },
                }
            };
        }

        /// <summary>
        /// Returns ResultModel of type T with exception message passed from enumeration exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exceptionMessage"></param>
        /// <returns></returns>
        public static ResultModel<T> ToErrorModel<T>(object exceptionMessage)
        {
            return new ResultModel<T>
            {
                IsSuccess = false,
                Errors = new List<IErrorModel>
                {
                    new ErrorModel
                    {
                        Key = string.Empty,
                        Message = exceptionMessage.ToString()
                    }
                }
            };
        }

        /// <summary>
        /// Returns ResultModel of type ModelStateDictionary with exception message passed from enumeration exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="modelStateErrors"></param>
        /// <returns></returns>
        public static ResultModel<T> ToErrorModel<T>(this ModelStateDictionary modelStateErrors)
        {
            return new ResultModel<T>
            {
                IsSuccess = false,
                Errors = modelStateErrors.Select(item => new ErrorModel
                {
                    Key = item.Key,
                    Message = item.Value.ValidationState.ToString()
                }).Cast<IErrorModel>().ToList()
            };
        }
    }
}
