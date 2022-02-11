using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV.Converters
{
	using Microsoft.UI.Xaml.Data;

	public class ConstStringConverter:IValueConverter
	{
		public object Convert(object value,Type targetType,object parameter,string language) => (parameter).ToString();

		public object ConvertBack(object value,Type targetType,object parameter,string language)
		{
			{ LogEx(new NotImplementedException("Convert")); return null; }
		}
	}
}
