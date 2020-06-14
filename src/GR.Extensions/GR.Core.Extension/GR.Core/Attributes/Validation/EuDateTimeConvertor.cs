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
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context,
            CultureInfo culture, object value)
        {
            try
            {
                if (!(value is string s)) return base.ConvertFrom(context, culture, value);
                return DateTime.TryParse(s, new CultureInfo("de-DE"), DateTimeStyles.None, out var date)
                    ? date
                    : base.ConvertFrom(context, culture, value);
            }
            catch (Exception)
            {
                return value;
            }
        }
    }
}
