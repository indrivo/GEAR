using System;
using GR.Core.Helpers;

namespace GR.DynamicEntityStorage.Abstractions.Extensions
{
    public static class UpdatePropertyExtension
    {
        /// <summary>
        /// Change object property value
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static object ChangePropValue(this object obj, string propertyName, string value)
        {
            try
            {
                Arg.NotNull(obj, typeof(object).Name);
                var prop = obj.GetType().GetProperty(propertyName);
                if (prop == null) return obj;
                var propType = prop.PropertyType;
              
                if (propType == typeof(string))
                {
                    prop.SetValue(obj, value ?? string.Empty);
                }
                else if (propType == typeof(int))
                {
                    prop.SetValue(obj, Convert.ToInt32(value));
                }
                else if (propType == typeof(DateTime))
                {
                    prop.SetValue(obj, DateTime.Parse(value));
                }
                else if (propType == typeof(bool))
                {
                    prop.SetValue(obj, Convert.ToBoolean(value));
                }
                else if (propType == typeof(Guid))
                {
                    prop.SetValue(obj, Guid.Parse(value));
                }
                else
                {
                    prop.SetValue(obj, value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return obj;
        }
    }
}
