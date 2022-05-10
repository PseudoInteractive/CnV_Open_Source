using Microsoft.UI.Xaml.Data;


namespace CnV;

using Syncfusion.UI.Xaml.DataGrid;

using Windows.Storage;
using Converters;
using Game;
using Syncfusion.UI.Xaml.Grids;
using Syncfusion.UI.Xaml.Grids.ScrollAxis;
using Views;
using Microsoft.UI.Xaml;
using Windows.UI.Text;

/// <summary>
/// Tag is used to save/load the dataGrid
/// </summary>
public static partial class ADataGrid
{
	public static xDataGrid Create<T>() where T : class
	{
		var rv = new xDataGrid();
		
		rv.View.BeginInit();
		rv.SourceType = typeof(T);
		return rv;
	}

	public ref struct ChangeContextDisposable
	{
		xDataGrid? grid;

		public ChangeContextDisposable(xDataGrid? dataGrid)
		{
			grid = dataGrid;
			if (dataGrid is not null)
			{
				
				Assert(!dataGrid.IsListenerSuspended);
				grid.SuspendNotifyListener();
				grid.Columns.Suspend();
				if (grid.View != null)
					grid.View.BeginInit();
			}
			else
			{
				
			}
		}
		public void Dispose()
		{
			if (grid is not null)
			{
				grid.Columns.Resume();
				grid.ResumeNotifyListener();
				if (grid.View != null)
					grid.View.EndInit();
			}
		}
	}

	public static ChangeContextDisposable ChangeContext(this xDataGrid grid) => new ChangeContextDisposable(grid);

	public static void ScrollItemIntoView(this xDataGrid grid, object? o)
	{
		if (o is not null)
		{
			var rowIndex = grid.ResolveToRowIndex(o);
			var columnIndex = grid.ResolveToStartColumnIndex();
			if (rowIndex >= 0)
				grid.ScrollInView(new RowColumnIndex(rowIndex, columnIndex));
		}
	}

	public static void AddCity(this xDataGrid grid,string headerText = null,bool wantImage = true,bool wantRemarks = true,bool wantStatus = true, bool wantDefense=false,bool wantTroops=false)
	{
		Assert(grid.IsListenerSuspended);
		try
		{
			if (wantImage)
			{
				var dim = Settings.mediumGridRowHeight;
				var imageDim = dim * 0.875f;
				grid.Columns.Add(new GridImageColumn()
				{
					HeaderText = "Icon",
					ImageHeight = imageDim,
					ImageWidth = imageDim,
					Width = dim,

					IsReadOnly = true,
					MappingName = nameof(City.icon)
				});
			}


			grid.Columns.Add(new GridTextColumn()
			{
				HeaderText = headerText,
				IsReadOnly = true,
				ColumnWidthMode=ColumnWidthMode.SizeToCells,
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
	public static bool AddTime(this xDataGrid grid,string mapping,string headerText = null,bool readOnly=true,string nullText=null)
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
	public static bool AddText(this xDataGrid grid,string mapping,string headerText = null,ColumnWidthMode widthMode= ColumnWidthMode.Auto,double width = double.NaN, bool readOnly = true)
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
	public static void AddHyperLink(this xDataGrid grid,string mapping, string headerText = null,string displayMapping=null,string buttonStr=null)
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
	public static bool Register(this UserTab tab,xDataGrid grid)
	{
		if(tab is null)
			return true;
		if(tab.dataGrids.Contains(grid))
			return false;

		tab.dataGrids.Add(grid);
		return true;
	}
	//public static bool Deregister(this UserTab tab,xDataGrid grid)
	//{
	//	var a = UserTab.dataGrids.Remove(grid);
	//	return a;
	//}

	public static ADataGrid.ChangeContextDisposable SetupDataGrid(this UserTab? tab, xDataGrid grid,
		bool wantChangeContext = false, Type? sourceType = null)
	{

		if (Register(tab, grid))
		{
			
			 var _lock0 = new ADataGrid.ChangeContextDisposable(wantChangeContext ? grid : null);
			//	grid.FontFamily = App.CnVFont;
			//grid.FilterRowPosition=FilterRowPosition.FixedTop;
			//	grid.ColumnSizer.FontStretch = Windows.UI.Text.FontStretch.Condensed;
				grid.ColumnSizer.FontFamily = XamlHelper.cnvFont;
			//	grid.ColumnSizer.FontWeight = Microsoft.UI.Text.FontWeights.Normal;
			grid.ColumnSizer.FontSize = Settings.mediumFontSize;
			grid.ColumnSizer.Margin = new(16);
		//	grid.FontSize = Settings.smallFontSize;
			grid.AlternationCount = 2;
		//	grid.AllowRowHoverHighlighting = true;
			grid.RowHeight = Settings.shortGridRowHeight;
		//	grid.FontSize = Settings.smallFontSize;
			
			grid.FontWeight=Microsoft.UI.Text.FontWeights.Normal;
			grid.FontSize = Settings.smallFontSize;
			grid.FontStretch = Windows.UI.Text.FontStretch.Condensed;
			grid.FontFamily=XamlHelper.cnvFont;
			grid.SelectionMode = GridSelectionMode.Extended;
		//	grid.GridLinesVisibility = GridLinesVisibility.Both;
		
		//	grid.ShowGroupDropArea=false;
			grid.AllowResizingHiddenColumns = true;
			grid.AllowResizingColumns = true;
		//	grid.CanMaintainScrollPosition=true;
			//grid.ShowToolTip=true;
		//	grid.CellStyle = App.instance.Resources["SfTextCell"] as Style;
	//		grid.RowStyle = App.instance.Resources["sfRowStyle"] as Style;
	//		grid.HeaderStyle=App.instance.Resources["sfHeaderStyle"] as Style;
			grid.AllowEditing=false;
			grid.AllowDraggingColumns=true;
			grid.AllowSorting=true;
			grid.ColumnSizer.AutoFitMode = AutoFitMode.SmartFit;

			grid.AllowTriStateSorting=true;
//			grid.FontStretch = Windows.UI.Text.FontStretch.Condensed;
			//grid.ExpanderColumnWidth = 16;
			if (tab is not null && grid.IsCityGrid())
				grid.SelectionChanged += tab.SpotSelectionChanged;
		
			grid.GridContextFlyoutOpening += UserTab.ContextFlyoutOpening;
			grid.RecordContextFlyout = new();
			grid.RecordContextFlyout.SetXamlRoot(grid);
			grid.CurrentCellRequestNavigate += UserTab.CelNavigate;
			grid.CellTapped += ADataGrid.SfCellTapped;
		//	grid.AllowGrouping = false;
			grid.ShowToolTip=true;
			grid.IsRightTapEnabled=true;
			grid.AllowFiltering = false;

			//				grid.AllowFrozenGroupHeaders = false;
			grid.ColumnWidthMode = Syncfusion.UI.Xaml.Grids.ColumnWidthMode.SizeToCells;
//			grid.ColumnWidthMode = Syncfusion.UI.Xaml.Grids.ColumnWidthMode.SizeToCells;
			grid.CellToolTipOpening += UserTab.CellToolTipOpening;
			//if(sourceType is not null) //  || grid.ItemsSource is not null)
		//		grid.SourceType = sourceType;// ?? UserTab.GetContainerType(grid.ItemsSource);
			//grid.UseSystemFocusVisuals = true;
			grid.ShowSortNumbers = true;
			foreach(var c in 			grid.Columns ) {
				c.ShowHeaderToolTip=true;
				c.ShowToolTip=true;
				
				//c.CellStyle = App.instance.Resources["SfTextCell"] as Style;
			}
					grid.ColumnSizer.ResetAutoCalculationforAllColumns();
	
			return _lock0;

		}
		else
		{
			return new ADataGrid.ChangeContextDisposable(null);
		}
	}

	//			grid.LiveDataUpdateMode = Syncfusion.UI.Xaml.Data.LiveDataUpdateMode.AllowChildViewUpdate;
	


	public static class Statics
	{
		public static StorageFolder folder => ApplicationData.Current.LocalFolder;
	}

	public static async Task SetFocus(this xDataGrid grid, object p)
	{
		await Task.Delay(500);
		grid.CurrentItem = p;
		grid.SelectedItem = p;
		grid.ScrollItemIntoView(p);

	}
}

