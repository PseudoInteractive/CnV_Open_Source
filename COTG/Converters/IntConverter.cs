using System;
using System.Globalization;
using Windows.UI.Xaml.Data;

namespace COTG.Converters
{
    public class IntConverter : IValueConverter
    {
        const string defaultFormat = "{0,6:N0}";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return String.Format( parameter != null ? parameter.ToString() : defaultFormat,value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (int.TryParse(value.ToString(), System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                return v;
            return default(int);
        }
    }

    public class FloatConverter : IValueConverter
    {
        const string defaultFormat = "{0,3:N2}";
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return String.Format(parameter != null ? parameter.ToString() : defaultFormat, value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (float.TryParse(value.ToString(), System.Globalization.NumberStyles.Number, CultureInfo.InvariantCulture, out var v))
                return v;
            return default(float);
        }
    }
}
