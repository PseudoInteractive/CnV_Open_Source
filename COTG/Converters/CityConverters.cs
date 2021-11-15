using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG;

using Microsoft.UI.Xaml.Data;

	public class CityIconConverter : IValueConverter	
	{
		public object Convert(object value, Type targetType, object parameter, string language) => (value as Spot)?.icon;

		public object ConvertBack(object value, Type targetType, object parameter, string language) => throw new InvalidOperationException(); 
	}
public class CityNameConverter:IValueConverter
{
	public object Convert(object value,Type targetType,object parameter,string language) => (value as Spot)?.cityName;

	public object ConvertBack(object value, Type targetType, object parameter, string language)
	{
		return value.ToString();
	} 
}
public class CityStatusConverter:IValueConverter
{
	public object Convert(object value,Type targetType,object parameter,string language) => (value as City)?.statusString;

	public object ConvertBack(object value,Type targetType,object parameter,string language)
	{
		throw new InvalidOperationException();
	}
}
public class CityRemarksConverter:IValueConverter
{
	public object Convert(object value,Type targetType,object parameter,string language) => (value as City)?.remarks;

	public object ConvertBack(object value,Type targetType,object parameter,string language)
	{
		return value.ToString();
	}
}
