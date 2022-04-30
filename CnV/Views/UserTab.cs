global using xDataGrid = Syncfusion.UI.Xaml.DataGrid.SfDataGrid;
using Microsoft.UI.Xaml.Controls;

//using Windows.UI.ViewManagement;
//using Windows.UI.WindowManagement;
using Microsoft.UI.Xaml;
using Syncfusion.UI.Xaml.DataGrid;
using Windows.Foundation;


//using Microsoft.UI.Windowing;

namespace CnV.Views;

using System;
using System.Collections.ObjectModel;

using Syncfusion.UI.Xaml.Grids;

public partial class UserTab:UserControl, IANotifyPropertyChanged
{

	private const string returnReinforcement = nameof(returnReinforcement);
	public virtual TabPage? defaultPage => TabPage.mainTabs;

	public readonly record struct DataGridProxy(UserTab tab,xDataGrid sf)
	{
		public ObservableCollection<object> SelectedItems() => sf.SelectedItems;
	}
	public ImmutableArray<xDataGrid> myDataGrids = ImmutableArray<xDataGrid>.Empty;

//	public static Dictionary<xDataGrid,UserTab> dataGrids = new();

	internal List<xDataGrid> dataGrids = new(); // grids owned by this tab

	public virtual IEnumerable<xDataGrid> GetDataGrids()
	{
		yield break;
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	public void CallPropertyChanged(string? members = null)
	{
		PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(members));
	}
	public void OnPropertyChanged(string? member = null)
	{
		if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged(member);
	}

	public static UserTab[]? userTabs;


	public static void InitUserTabs()
	{

		userTabs = new UserTab[] {
				new BuildTab(),
				new MainPage(),
				new PlayerTab(),
				new DonationTab(),
				new IncomingTab(),
				new HitHistoryTab(),
				new AttackTab(),
				new PlannerTab(),
				new SpotTab(),
				new ReinforcementsTab(),
				new OutgoingTab(),
				new AllianceTab(),
				new NearDefenseTab(),
				new NearRes(),
				new ChartDialog(),
				new TimelineView(),
				new NPCHistory(),
		};

	}

	internal void SpotSelectionChanged(object sender,GridSelectionChangedEventArgs? e)
	{
		if(!isFocused)
			return;
		var grid = (xDataGrid)sender;
		Assert(grid.IsCityGrid());
		if(SpotTab.silenceSelectionChanges == 0)
		{
			try
			{
				var sel = grid.SelectedItems;
				Spot.selected =new HashSet<int>(sel.Select(a => ((Spot)a).cid));
			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			finally
			{
				//          Spot.selected.ExitWriteLock();
			}
		}
	}
	public void SelectAllWorkAround(xDataGrid grid)
	{
		grid.SelectAll();
		SpotSelectionChanged(grid,null);
	}

	public virtual Task VisibilityChanged(bool visible,bool longTerm)
	{
		if(visible)
		{
			//		Assert(isFocused);
		}
		//			Log($"VisibilityChanged: {visible} {this}");
		return Task.CompletedTask;

	}
	//public virtual Task Reset()
	//{
	//	if(visible)
	//	{
	//		await VisibilityChanged(false);  // close enough default behaviour
	//		await VisibilityChanged(true);  // close enough default behaviour
	//	}
	//}
	// If this is not an xaml island, the newXamlRoot will be null
	//	public virtual void XamlTreeChanged(TabPage newPage) { } // The tab was dragged somewhere else
	public bool isFocused; // if set, the tab is open/focused/visible.  There is one focused tab per page
	public bool isOpen; // if this is set, the tab exists on 1 tab page
	public bool hasAnnouncedActive; // true after active has been called at least once
									//User pressed F5 or refresh button

	//		static DateTimeOffset nextCityRefresh = DateTimeOffset.UtcNow;
	public Debounce refresh;

	public UserTab()
	{
		if(refresh == null)
			refresh = new(_Refresh) { debounceDelay = 500 };// { throttled = true };
															//	ScrollViewer.SetIsVerticalScrollChainingEnabled(this,false);
															//			this.Width = 500;
															//			this.Height = 500;
															//	ScrollViewer.SetVerticalScrollMode(this, ScrollMode.Auto); //DependencyObjectExtensions.FindDescendant<ScrollViewer>(this).AllowFocusOnInteraction= false;
		Margin= new(8);
		//Loaded+=OnLoaded;

	}
	protected void DataGridLoaded(object sender,RoutedEventArgs e)
	{
		var dataGrid = (xDataGrid)sender;
		using var __ = ADataGrid.SetupDataGrid(this,dataGrid,false);
	}
	
	protected ADataGrid.ChangeContextDisposable SetupDataGrid(xDataGrid grid,bool wantChangeContext = false,Type? sourceType = null)
	{
		return ADataGrid.SetupDataGrid(this,grid,wantChangeContext,sourceType);

	}
	public Task VisibilityMaybeChanged()
	{
		return VisibilityChanged(isFocused,longTerm: false);

	}
	protected virtual async Task _Refresh()
	{
		if(isFocused && isOpen)
		{
			isFocused=false;
			await VisibilityChanged(false,longTerm: false);  // close enough default behaviour

			isFocused=true;
			await VisibilityChanged(true,longTerm: false);  // close enough default behaviour
		}
	}

	//public void SetupDataGrid(SfDataGrid grid)
	//{
	//	if(!allDataGrids.AddIfAbsent(new(this,rad:grid) ))
	//		return;
	//	grid.Padding = new (0,0,32,32);
	//	grid.FontStretch = Windows.UI.Text.FontStretch.Condensed;
	//	grid.FontWeight = Microsoft.UI.Text.FontWeights.Light;
	//	grid.UseSystemFocusVisuals=true;
	//	grid.GridLinesVisibility= Telerik.UI.Xaml.Controls.Primitives.GridLinesVisibility.Both;
	////	grid.ProcessTooltips();
	////	grid.ListenForNestedPropertyChange=false;
	//	grid.FontSize = Settings.smallFontSize;
	//	grid.RowHeight = Settings.mediumGridRowHeight;
	//	grid.ProcessTooltips();
	//	if (object.ReferenceEquals(grid.ItemsSource, City.gridCitySource) )
	//	{
	//		grid.SelectionChanged += (a, b) => SpotSelectionChanged(((SfDataGrid)a).SelectedItems);
	//	}

	//}


	public void SetPlus(bool set)
	{
		AppS.QueueOnUIThread(() =>
		{
			(var tp, var tvi) = GetViewItem();
			if(tvi!=null)
			{
				var h = tvi.Header as string;
				var hasNumber = ((h[0] >= '0') && (h[0] <= '9'));
				if(!set)
				{
					if(hasNumber)
					{
						tvi.Header = h.Substring(2);
					}
				}
				else
				{
					if(hasNumber)
					{
						tvi.Header=(int.Parse(h.Substring(0,1))+1).Min(9) + h.Substring(1);
					}
					else
					{
						tvi.Header = "1 " + h;
					}
				}
			}
		});
	}

	public (TabPage tabPage, TabViewItem tabViewItem) GetViewItem()
	{
		foreach(var tabPage in TabPage.tabPages)
		{
			foreach(TabViewItem ti in tabPage.Tabs.TabItems)
			{
				if(ti.Content == this)
					return (tabPage, ti);
			}
		}
		return (null, null);
	}
	public IEnumerable<TabPage> GetTabPages()
	{
		foreach(var tabPage in TabPage.tabPages)
		{
			foreach(TabViewItem ti in tabPage.Tabs.TabItems)
			{
				if(ti.Content == this)
				{
					yield return tabPage;
					break;
				}
			}
		}
	}

	public void Show()
	{
		if(!isOpen)
		{
			ShowOrAdd(true);
		}
		else
		{
			if(!isFocused)
				TabPage.Show(this);
			//    else
			//      tab.Refresh();
		}

	}
	public virtual void Close()
	{
		if(!isOpen)
			return;
		TabPage.Close(this);
	}
	public void ShowOrAdd(bool selectMe = true,bool onlyIfClosed = false,TabPage page = null)

	{
		AppS.DispatchOnUIThread(() =>
	{
		if(onlyIfClosed && isOpen)
			return;

		// already here?
		var existing = GetViewItem();
		if(existing.tabPage != null)
		{
			if(selectMe)
				existing.tabPage.Tabs.SelectedItem = existing.tabViewItem;
			return;
		}

		(page??defaultPage).Add(this,selectMe);
	});
	}



	public static void CelNavigate(object sender,Syncfusion.UI.Xaml.Grids.CurrentCellRequestNavigateEventArgs e)
	{
		try
		{
			e.Handled = true;
			if(e.NavigateText.Contains("://"))
			{
				var uri = new Uri(e.NavigateText);

				if(uri.Scheme == ProtocolActivation.scheme && uri.LocalPath.StartsWith(ProtocolActivation.command))
				{
					var subStr = uri.LocalPath.AsSpan().Slice(ProtocolActivation.command.Length);
					if(subStr.StartsWith(returnReinforcement.AsSpan(),StringComparison.Ordinal))
					{
						var args = new WwwFormUrlDecoder(subStr.Slice(returnReinforcement.Length).ToString());

						Note.Show("ProtoClick");
						//			var args = Uri.Par
						//	Reinforcement.ReturnAsync(args.GetFirstValueByName("order").ParseLong().GetValueOrDefault(),args.GetFirstValueByName("pid").ParseInt().GetValueOrDefault());

					}


				}
			}
		}
		catch(Exception ex)
		{
			// invalid link, not a problem - we are just using it as a button
		}
	}

	public static void CellToolTipOpening(object sender,Syncfusion.UI.Xaml.DataGrid.GridCellToolTipOpeningEventArgs e)
	{
		///			var tt = e.ToolTip;
		//			var rec = e.Record;
		
		
		
			var tt = ToolTipService.GetToolTip(e.Column) as string;
		if(!tt.IsNullOrEmpty() )
			e.ToolTip.Content = tt;
		else
		{
			if(e.Record is Player player)
			{
				if(e.Column.MappingName is ( nameof(Player.avatarImage)  ) )
					e.ToolTip.Content = new Image { Source = player.avatarImage };

			}
			else if(e.Record is City city)
			{
				if(e.Column.MappingName is ( "nameAndRemarks"  ) || e.Column.MappingName is ( nameof(City.icon)  ) )
					e.ToolTip.Content = city.toolTip;
//				else if(e.Column.MappingName is ( nameof(City.icon)  ) )
	//				e.ToolTip.Content = new Image { Source = city.icon };

			}
			else  if(e.Record is BattleReport b)
			{
				if(e.Column.MappingName is ( "sourceCity" or "sXY" ) )
					e.ToolTip.Content = b.sourceCity.toolTip;
				else if(e.Column.MappingName is ( "targetCity" or "tXY" ))
					e.ToolTip.Content = b.targetCity.toolTip;

			}

		}
		
	}

	public static Type GetContainerType(object container)
	{
		if(container is NotifyCollection<City>)
		{
			return typeof(City);

		}

		if(container is NotifyCollectionBase)
		{
			var type = container.GetType();
			Assert(type.IsGenericType);
			Assert(type.GenericTypeArguments.Length == 1);
			return type.GenericTypeArguments[0];

		}

		Assert(false); // Todo: Test
		return typeof(object);
	}





	public static void ContextFlyoutOpening(object sender,GridContextFlyoutEventArgs e)
	{
		var flyout = e.ContextFlyout;
		flyout.Items.Clear();

		switch(e.ContextFlyoutType)
		{
			case ContextFlyoutType.RecordCell:
				{
					var info = e.ContextFlyoutInfo as GridRecordContextFlyoutInfo;
					var column = info?.DataGrid.Columns[e.RowColumnIndex.ColumnIndex];
					if(info.Record is City city)
					{
						city.AddToFlyout(flyout);

						break;
					}
					else if(info.Record is Player p)
					{
						//var me = Player.me;

						p.AddToFlyout(flyout);
						break;
					}
					else if(info.Record is Alliance a)
					{
						//var me = Player.me;

						a.AddToFlyout(flyout);
						break;
					}
					else if(info.Record is Reinforcement r)
					{
						flyout.AddItem("Return",() => r.ReturnAsync());
						break;
					}
					break;
				}
		}
	}



}


public static class UserTabHelpers
{
	//internal static UserTab GetTab(this xDataGrid grid)
	//{
	//	return UserTab.dataGrids[grid];
	//}

	internal static bool IsCityGrid(this xDataGrid grid) => object.ReferenceEquals(grid.ItemsSource,City.gridCitySource);
}

