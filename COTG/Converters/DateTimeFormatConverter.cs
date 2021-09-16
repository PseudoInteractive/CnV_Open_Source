using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace COTG.Converters
{
    public class DateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTimeOffset dt )
            {
                return dt.ToString(parameter != null ? parameter.ToString() : AUtil.defaultDateFormat );
            }
            return value.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                if (DateTimeOffset.TryParseExact(value.ToString(), parameter != null ? parameter.ToString() : AUtil.defaultDateFormat, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out var result))
                    return result;
                if (DateTimeOffset.TryParse(value.ToString(), DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeUniversal, out result))
                    return result;

            }

            return default(DateTimeOffset);
        }
    }
}
