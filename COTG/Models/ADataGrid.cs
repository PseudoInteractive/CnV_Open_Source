using Microsoft.UI.Xaml.Data;


namespace CnV;

using Syncfusion.UI.Xaml.DataGrid;

using Windows.Storage;
using Converters;
using Game;
using Syncfusion.UI.Xaml.Grids;
using Views;
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

	public ref struct ChangeContextDisposable
	{
		DataGrid grid;

		public ChangeContextDisposable(DataGrid dataGrid)
		{
			grid = dataGrid;
			Assert(!dataGrid.IsListenerSuspended);
			grid.SuspendNotifyListener();
			grid.Columns.Suspend();
			if(grid.View !=null)
				grid.View.BeginInit();
		}
		public void Dispose()
		{
			grid.Columns.Resume();
			grid.ResumeNotifyListener();
			if(grid.View !=null)
				grid.View.EndInit();
		}
	}

	public static ChangeContextDisposable ChangeContext(this DataGrid grid) => new ChangeContextDisposable(grid);


	public static void AddCity(this DataGrid grid,string headerText = null,bool wantImage = true,bool wantRemarks = true,bool wantStatus = true, bool wantDefense=false,bool wantTroops=false)
	{
		Assert(grid.IsListenerSuspended);
		try
		{
			if (wantImage)
			{
				var dim = SettingsPage.mediumGridRowHeight;
				var imageDim = dim * 0.875f;
				grid.Columns.Add(new GridImageColumn()
				{
					HeaderText = "Icon",
					ImageHeight = imageDim,
					ImageWidth = imageDim,
					Width = dim,

					IsReadOnly = true,
					MappingName = nameof(City.iconUri)
				});
			}


			grid.Columns.Add(new GridTextColumn()
			{
				HeaderText = headerText,
				IsReadOnly = true,
				ColumnWidthMode=ColumnWidthMode.Auto,
				MappingName = nameof(City.cityName)
			});
			if(wantRemarks)
				grid.Columns.Add(new GridTextColumn()
				{
					IsReadOnly = true,
					HeaderText = "Remarks",
					MappingName = nameof(City.remarks)
				});
			

			if(wantStatus)
				grid.Columns.Add(new GridTextColumn()
				{
					HeaderText = "Status",
					Width = 40,
					IsReadOnly = true,
					MappingName = nameof(City.statusString)
				});
			if (wantDefense)
				grid.Columns.Add(new GridTextColumn()
				{
					HeaderText = "Total Defense",
					IsReadOnly = true,
					MappingName = nameof(City.defString)
				});
			if (wantTroops)
			{
				grid.Columns.Add(new GridTextColumn()
				{
					HeaderText = "Troops",
					IsReadOnly = true,
					MappingName = nameof(City.troopsString)
				});
			}

			
		}
		catch(Exception e)
		{
			LogEx(e);
		}
		finally
		{
		
		}
	}
	public static bool AddTime(this DataGrid grid,string mapping,string headerText = null,bool readOnly=true,string nullText=null)
	{

		Assert(grid.IsListenerSuspended);

		try
		{

			grid.Columns.Add(new GridTimeColumn()
			{
				HeaderText = headerText ?? mapping,
				IsReadOnly = readOnly,
				AllowNull=true,
				DisplayTimeFormat="H:mm:ss",
				PlaceholderText=nullText ?? "none",
				MappingName = mapping
			});
			return true;
		}
		catch(Exception e)
		{
			LogEx(e);
			return false;
		}
		finally
		{

		}
	}
	public static bool AddText(this DataGrid grid,string mapping,string headerText = null,ColumnWidthMode widthMode= ColumnWidthMode.Auto,double width = double.NaN, bool readOnly = true)
	{

		Assert(grid.IsListenerSuspended);

		try
		{

			grid.Columns.Add(new GridTextColumn()
			{
				HeaderText = headerText ?? mapping,
				Width=width,
				ColumnWidthMode=widthMode,
				UseBindingValue=false,
				MappingName=mapping,
				IsReadOnly = readOnly,
			});

			return true;
		}
		catch(Exception e)
		{
			LogEx(e);
			return false;
		}
		
	}
	public static void AddHyperLink(this DataGrid grid,string mapping, string headerText = null,string displayMapping=null,string buttonStr=null)
	{

		Assert(grid.IsListenerSuspended);

		try
		{
			headerText ??= mapping;
			
			grid.Columns.Add(new GridHyperlinkColumn()
			{
				HeaderText = headerText ,
				UseBindingValue=true,
				ValueBinding = new Binding()
					{ Path = new(mapping),Mode = BindingMode.OneTime },
				DisplayBinding = displayMapping is not null  
				?new Binding() { Path = new(displayMapping),Mode = BindingMode.OneTime}
				:new Binding() { Path = new(mapping),Mode = BindingMode.OneTime,Converter = new ConstStringConverter(),ConverterParameter=buttonStr??headerText },
			});
		}
		catch(Exception e)
		{
			LogEx(e);
		}
		finally
		{

		}
	}



	public static class Statics
	{
		public static StorageFolder folder => ApplicationData.Current.LocalFolder;
	}
}

