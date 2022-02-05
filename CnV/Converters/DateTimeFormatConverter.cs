using System;
using System.Globalization;
using Microsoft.UI.Xaml.Data;

namespace CnV.Converters
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

    public class SmallTimeConverter : IValueConverter
    {
	    public object Convert(object value, Type targetType, object parameter, string language)
	    {

			if (value is DateTimeOffset dateTime)
			{
				if(targetType == typeof(DateTimeOffset))
				{
					return dateTime;
				}

				else
				{
					return dateTime.Format();
				}
			}

			if (value is ServerTime src)
			{
				if (targetType == typeof(DateTimeOffset))
				{
					return src.dateTimeNullable;
				}
				
				else
				{
					return src.Format();
				}
			}
			return value;

	    }

	    public object ConvertBack(object value,Type targetType,object parameter,string language)
	    {
		    if (typeof(ServerTime) == targetType)
		    {
			    if(value is ServerTime st)
			    {
				    return st;
			    }
				else if ( value is DateTimeOffset dn)
			    {
				    return new ServerTime(dn);
			    }
			    else if (value is null)
			    {
				    return ServerTime.zero;
			    }
			    else
			    {
				    if (AUtil.TryParseTime(value.ToString(), out var rv))
					    return new ServerTime(rv);
				    else
					    return ServerTime.zero;

			    }
		    }
			else if (typeof(DateTimeOffset) == targetType)
		    {

			    if(value is DateTimeOffset dn)
			    {
				    return dn;
			    }
			    else if(value is ServerTime st)
			    {
				    return st.dateTime;
			    }
			    else if(value is null)
			    {
				    return null;
			    }
				else
			    {
				    if (AUtil.TryParseTime(value.ToString(), out var rv))
					    return rv;
				    else
					    return null;

			    }
			}
		    else
		    {
			    Assert(false);
				return value; // Todo
		    }
	    }
    }
}
