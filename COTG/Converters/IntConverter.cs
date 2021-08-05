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
            if ( value.ToString().TryParseInt( out var v))
                return v;
            return default(int);
        }
    }
	public class TimeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return ((TimeSpan)value).Format();
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			return TimeSpan.Zero;
		}
	}
	public class NullableIntToDoubleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			var _value = (int?)value;
			return (_value.HasValue ? (double)_value.Value : double.NaN );
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			if (double.TryParse(value.ToString(), System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var v) && !double.IsNaN(v) )
			{
				return v.RoundToInt();
			}
			else
			{
				return null;
			}
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
            if (float.TryParse(value.ToString(), System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var v))
                return v;
            return default(float);
        }
    }
	public class PercentConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, string language)
		{
			return $"{(((float)value)*100.0f):N3}%";
		}

		public object ConvertBack(object value, Type targetType, object parameter, string language)
		{
			var s = value.ToString().TrimEnd('%');

			if (float.TryParse(s, System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo, out var v))
				return v*(0.01f);
			return default(float);
		}
	}
}
