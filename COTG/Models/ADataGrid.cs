using Microsoft.UI.Xaml.Data;


namespace COTG;

using Syncfusion.UI.Xaml.DataGrid;

using Windows.Storage;

using DataGrid = Syncfusion.UI.Xaml.DataGrid.SfDataGrid;
/// <summary>
/// Tag is used to save/load the dataGrid
/// </summary>
public static partial class ADataGrid
{
	public static DataGrid Create<T>() where T : class
	{
		var rv = new DataGrid();
		rv.View.BeginInit();
		rv.SourceType = typeof(T);
		return rv;
	}



	public static void AddCity(this DataGrid grid,string cityMap,string headerText = null,bool wantImage = true,bool wantRemarks = true,bool wantStatus = true)
	{
		grid.SuspendNotifyListener();
		grid.Columns.Suspend();
		grid.View.BeginInit();
		
		try
		{
			if(wantImage)
				grid.Columns.Add(new GridImageColumn()
				{
					HeaderText="I",
					UseBindingValue = true,Width = SettingsPage.mediumGridRowHeight,IsReadOnly = true,
					DisplayBinding = new Binding()
					{ Path=cityMap, Source = cityMap,Mode = BindingMode.OneTime,Converter = new CityIconConverter(),},
					ValueBinding = new Binding()
					{ Source = cityMap,Mode = BindingMode.OneTime,Converter = new CityIconConverter(),}
				});



			grid.Columns.Add(new GridTextColumn()
			{

				UseBindingValue = true,IsReadOnly = true,
				HeaderText = headerText ?? cityMap,
				DisplayBinding = new Binding()
				{ Source = cityMap,Mode = BindingMode.OneTime,Converter = new CityNameConverter() },
				ValueBinding = new Binding()
				{ Source = cityMap,Mode = BindingMode.OneTime,Converter = new CityNameConverter() }
			});
			if(wantRemarks)
				grid.Columns.Add(new GridTextColumn()
				{
					MappingName = "Remarks",
					UseBindingValue = true,IsReadOnly = true,HeaderText = "Remarks",Width = 80,
					DisplayBinding = new Binding()
					{ Source = cityMap,Mode = BindingMode.OneTime,Converter = new CityRemarksConverter() }
				});
			if(wantStatus)
				grid.Columns.Add(new GridTextColumn()
				{
					HeaderText = "Status",
					UseBindingValue = true,IsReadOnly = true,
					ValueBinding = new Binding()
					{ Source = cityMap,Mode = BindingMode.OneTime,Converter = new CityStatusConverter() },
					DisplayBinding = new Binding()
					{ Source = cityMap,Mode = BindingMode.OneTime,Converter = new CityStatusConverter() }
				});
		}
		catch(Exception e)
		{
			LogEx(e);
		}
		finally
		{
			grid.Columns.Resume();
			grid.View.EndInit();
			grid.ResumeNotifyListener();
		}
	}


	public static class Statics
	{
		public static StorageFolder folder => ApplicationData.Current.LocalFolder;
	}
}

