using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GR.Core.Extensions
{
    public static class StringInjectExtension
    {
        public static string Inject(this string formatString, object injectionObject)
        {
            return formatString.Inject(GetPropertyHash(injectionObject));
        }
        public static string Inject(this string formatString, IDictionary dictionary)
        {
            return formatString.Inject(new Hashtable(dictionary));
        }
        public static string Inject(this string formatString, Hashtable attributes)
        {
            string result = formatString;
            if (attributes == null || formatString == null)
                return result;

            foreach (string attributeKey in attributes.Keys)
            {
                result = result.InjectSingleValue(attributeKey, attributes[attributeKey]);
            }
            return result;
        }

        public static string InjectSingleValue(this string formatString, string key, object replacementValue)
        {
            string result = formatString;
            Regex attributeRegex = new Regex("{(" + key + ")(?:}|(?::(.[^}]*)}))");
            foreach (Match m in attributeRegex.Matches(formatString))
            {
                string replacement = m.ToString();
                if (m.Groups[2].Length > 0) //matched {foo:SomeFormat}
                {
                    //do a double string.Format - first to build the proper format string, and then to format the replacement value
                    string attributeFormatString = string.Format(CultureInfo.InvariantCulture, "{{0:{0}}}", m.Groups[2]);
                    replacement = string.Format(CultureInfo.CurrentCulture, attributeFormatString, replacementValue);
                }
                else //matched {foo}
                {
                    replacement = (replacementValue ?? string.Empty).ToString();
                }
                //perform replacements, one match at a time
                result = result.Replace(m.ToString(), replacement);  //attributeRegex.Replace(result, replacement, 1);
            }
            return result;
        }

        private static Hashtable GetPropertyHash(object properties)
        {
            Hashtable values = null;
            if (properties != null)
            {
                values = new Hashtable();
                PropertyDescriptorCollection props = TypeDescriptor.GetProperties(properties);
                foreach (PropertyDescriptor prop in props)
                {
                    values.Add(prop.Name, prop.GetValue(properties));
                }
            }
            return values;
        }
    }
}
