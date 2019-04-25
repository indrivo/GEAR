using System;

namespace ST.DynamicEntityStorage.Abstractions.Extensions
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
                var propType = obj.GetType().GetProperty(propertyName).PropertyType;

                if (propType == typeof(string))
                {
                    obj.GetType().GetProperty(propertyName).SetValue(obj, value ?? string.Empty);
                }
                else if (propType == typeof(int))
                {
                    obj.GetType().GetProperty(propertyName).SetValue(obj, Convert.ToInt32(value));
                }
                else if (propType == typeof(DateTime))
                {
                    obj.GetType().GetProperty(propertyName).SetValue(obj, DateTime.Parse(value));
                }
                else if (propType == typeof(bool))
                {
                    obj.GetType().GetProperty(propertyName).SetValue(obj, Convert.ToBoolean(value));
                }
                else
                {
                    obj.GetType().GetProperty(propertyName).SetValue(obj, value);
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
