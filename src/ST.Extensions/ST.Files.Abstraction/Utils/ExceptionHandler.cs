using System;
using System.Collections.Generic;
using System.ComponentModel;
using ST.Core.Helpers;

namespace ST.Files.Abstraction.Utils
{
    public static class ExceptionHandler
    {
        /// <summary>
        /// Returns ResultModel of type T with exception message passed from enumeration exception
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="exceptionMessageEnum"></param>
        /// <returns></returns>
        public static ResultModel<T> ReturnErrorModel<T>(ExceptionMessagesEnum exceptionMessageEnum)
        {
            return new ResultModel<T>
            {
                IsSuccess = false,
                Errors = new List<IErrorModel>
                {
                    new ErrorModel
                    {
                        Key = string.Empty,
                        Message = GetEnumDescription(exceptionMessageEnum)
                    },
                }
            };
        }

        /// <summary>
        /// Get enumerator description
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string GetEnumDescription(this Enum value)
        {
            var type = value.GetType();
            var name = Enum.GetName(type, value);
            if (name != null)
            {
                var field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                        Attribute.GetCustomAttribute(field,
                            typeof(DescriptionAttribute)) as DescriptionAttribute;
                    return attr?.Description;
                }
            }

            return null;
        }
    }
}
