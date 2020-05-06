using System;
using System.ComponentModel;
using System.Globalization;

namespace GR.Core.Attributes.Validation
{
    public class EuDateTimeConvertor : TypeConverter
    {
        public CultureInfo CultureInfo { get; set; }

        public override bool CanConvertFrom(ITypeDescriptorContext context,
            Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }
        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            if (!(value is string s)) return base.ConvertFrom(context, culture, value);
            if (DateTime.TryParse(s, new CultureInfo("de-DE"), DateTimeStyles.None, out DateTime date))
                return date;
            return base.ConvertFrom(context, culture, value);
        }
    }
}
