using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace GR.Core.Helpers.JsonConvertors
{
    public class StringEnumWithDefaultConverter : JsonConverter
    {
        private Dictionary<Type, Dictionary<string, object>> _fromValueMap;
        private Dictionary<Type, Dictionary<object, string>> _toValueMap;

        public override bool CanConvert(Type objectType)
        {
            Type type = IsNullableType(objectType) ? Nullable.GetUnderlyingType(objectType) : objectType;
            return type != null && type.IsEnum;
        }

        public override object ReadJson(
          JsonReader reader,
          Type objectType,
          object existingValue,
          JsonSerializer serializer)
        {
            bool flag = IsNullableType(objectType);
            Type type = flag ? Nullable.GetUnderlyingType(objectType) : objectType;
            InitMap(type);
            if (reader.TokenType == JsonToken.String)
            {
                string str = reader.Value.ToString();
                object obj = FromValue(type, str);
                if (obj != null)
                    return obj;
            }
            if (flag)
                return null;
            string str1 = Enum.GetNames(type).SingleOrDefault(n => string.Equals(n, "Unknown", StringComparison.OrdinalIgnoreCase));
            if (str1 == null)
                throw new JsonSerializationException(string.Format("Unable to parse '{0}' to enum {1}. Consider adding Unknown as fail-back value.", reader.Value, type));
            return Enum.Parse(type, str1);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Type type = value.GetType();
            InitMap(type);
            string str = ToValue(type, value);
            writer.WriteValue(str);
        }

        private bool IsNullableType(Type t)
        {
            return t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        private void InitMap(Type enumType)
        {
            if (_fromValueMap == null)
                _fromValueMap = new Dictionary<Type, Dictionary<string, object>>();
            if (_toValueMap == null)
                _toValueMap = new Dictionary<Type, Dictionary<object, string>>();
            if (_fromValueMap.ContainsKey(enumType))
                return;
            Dictionary<string, object> dictionary1 = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            Dictionary<object, string> dictionary2 = new Dictionary<object, string>();
            foreach (FieldInfo field in enumType.GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                string name = field.Name;
                object index1 = Enum.Parse(enumType, name);
                EnumMemberAttribute customAttribute = field.GetCustomAttribute<EnumMemberAttribute>();
                if (customAttribute != null)
                {
                    string index2 = customAttribute.Value;
                    dictionary1[index2] = index1;
                    dictionary2[index1] = index2;
                }
                else
                    dictionary2[index1] = name;
                dictionary1[name] = index1;
            }
            _fromValueMap[enumType] = dictionary1;
            _toValueMap[enumType] = dictionary2;
        }

        private string ToValue(Type enumType, object obj)
        {
            return _toValueMap[enumType][obj];
        }

        private object FromValue(Type enumType, string value)
        {
            Dictionary<string, object> fromValue = _fromValueMap[enumType];
            return fromValue.ContainsKey(value) ? fromValue[value] : null;
        }
    }
}
