using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ST.Core.Helpers
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Returns ResultModel of type T with exception message passed from enumeration exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exceptionMessageEnum"></param>
        /// <returns></returns>
        public static ResultModel<T> ReturnErrorModel<T>(Enum exceptionMessageEnum)
        {
            return new ResultModel<T>
            {
                IsSuccess = false,
                Errors = new List<IErrorModel>
                {
                    new ErrorModel
                    {
                        Key = string.Empty,
                        Message = EnumHelper.GetEnumDescription(exceptionMessageEnum)
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
        public static ResultModel ReturnErrorModel(Enum exceptionMessageEnum)
        {
            return new ResultModel
            {
                IsSuccess = false,
                Errors = new List<IErrorModel>
                {
                    new ErrorModel
                    {
                        Key = string.Empty,
                        Message = EnumHelper.GetEnumDescription(exceptionMessageEnum)
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
        public static ResultModel<T> ReturnErrorModel<T>(object exceptionMessage)
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
    }
}
