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
using System.Collections;
using System.Collections.ObjectModel;

using CommunityToolkit.WinUI.UI.Controls;

using Syncfusion.UI.Xaml.Grids;

using Windows.Storage;

using static CnV.Views.UserTab;

public partial class UserTab:Page, IANotifyPropertyChanged {

	private const string returnReinforcement = nameof(returnReinforcement);
	public virtual TabPage? defaultPage => TabPage.mainTabs;

	public readonly record struct DataGridProxy(UserTab tab,xDataGrid sf) {
		public ObservableCollection<object> SelectedItems() => sf.SelectedItems;
	}
	public ImmutableArray<xDataGrid> myDataGrids = ImmutableArray<xDataGrid>.Empty;

	//	public static Dictionary<xDataGrid,UserTab> dataGrids = new();

	internal List<xDataGrid> dataGrids = new(); // grids owned by this tab

	public virtual IEnumerable<xDataGrid> GetDataGrids() {
		yield break;
	}

	public event PropertyChangedEventHandler? PropertyChanged;
	public void CallPropertyChanged(string? members = null) {
		PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(members));
	}
	public void OnPropertyChanged(string? member = null) {
		if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged(member);
	}
	

	public static void InitUserTabs()
	{

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
				
					
				var sel = City.gridCitySelected;
				var prior = Spot.selected;
				Spot.selected = (sel.Select(a => ((Spot)a).cid)).Concat(prior.Where(a=> !City.gridCitySource.Contains(a.AsCity()) )).ToImmutableHashSet();
				SpotTab.SyncSelectionToUI(syncRecentGrid: true,syncCityGrid: false);
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
		grid.SelectAll(false);
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
															//Margin= new(8);
															//Loaded+=OnLoaded;
		userTabs.InterlockedAdd(this);
	}
	protected void DataGridLoaded(object sender,RoutedEventArgs e)
	{
		var dataGrid = (xDataGrid)sender;
		using var __ = ADataGrid.SetupDataGrid(this,dataGrid,true);
	}
	
	protected ADataGrid.ChangeContextDisposable SetupDataGrid(xDataGrid grid,bool wantChangeContext = false,Type? sourceType = null,IEnumerable source=null)
	{
		return ADataGrid.SetupDataGrid(this,grid,wantChangeContext,sourceType,source);

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


	//public void SetPlus(bool set)
	//{
	//	AppS.QueueOnUIThread(() =>
	//	{
	//		(var tp, var tvi) = GetViewItem();
	//		if(tvi!=null)
	//		{
	//			var h = tvi.Header as string;
	//			var hasNumber = ((h[0] >= '0') && (h[0] <= '9'));
	//			if(!set)
	//			{
	//				if(hasNumber)
	//				{
	//					tvi.Header = h.Substring(2);
	//				}
	//			}
	//			else
	//			{
	//				if(hasNumber)
	//				{
	//					tvi.Header=(int.Parse(h.Substring(0,1))+1).Min(9) + h.Substring(1);
	//				}
	//				else
	//				{
	//					tvi.Header = "1 " + h;
	//				}
	//			}
	//		}
	//	});
	//}


	internal static (TabPage tabPage, TabViewItem tabViewItem, UserTab tab, bool tabExists, bool hasFocus) GetViewItem(TabInfo t)
	{
		foreach(var tabPage in TabPage.tabPages)
		{
			foreach(TabViewItem ti in tabPage.Tabs.TabItems)
			{
				if( ti.Tag as TabInfo == t) {
					return (tabPage, ti, ti.GetTab(),true, ti.GetTab()?.isFocused == true  );

				}
			}
		}
		return (null, null,null,false,false);
	}
		internal static (TabPage tabPage, TabViewItem tabViewItem, UserTab tab, bool tabExists, bool hasFocus) GetViewItem(UserTab t)
	{
		foreach(var tabPage in TabPage.tabPages)
		{
			foreach(TabViewItem ti in tabPage.Tabs.TabItems)
			{
				if( ti.GetTab() == t) {
					return (tabPage, ti, ti.GetTab(),true, ti.GetTab()?.isFocused == true );

				}
			}
		}
		return (null, null,null,false,false);
	}
	internal static ImmutableArray<UserTab> userTabs = ImmutableArray<UserTab>.Empty;
	
	
	//public IEnumerable<TabPage> GetTabPages()
	//{
	//	foreach(var tabPage in TabPage.tabPages)
	//	{
	//		foreach(TabViewItem ti in tabPage.Tabs.TabItems)
	//		{
	//			if(ti.Content == this)
	//			{
	//				yield return tabPage;
	//				break;
	//			}
	//		}
	//	}
	//}

	
	public Task Close()
	{
		if(!isOpen) {
		//	Assert(false);
			return Task.CompletedTask;
		}
		return TabPage.Close(this);
	}
	
	
	public virtual async Task Closed()
	{
		isFocused = false;
		isOpen = false;
		try {
			await VisibilityChanged(false,longTerm: true);
		}catch(Exception ex) {
			LogEx(ex);
		}
		userTabs.InterlockedRemove(this);
		foreach(var grid in dataGrids) {
			if(!(grid.Tag is string s && s == "details"))
				grid.Dispose();

			//		AppS.QueueOnUIThreadIdle(d.SaveAndDispose);
			}
			
		dataGrids.Clear();
//		System.GC.Collect(2,GCCollectionMode.Default,true,true);
	}

	// Callback
	public virtual void Opened()	
		{
		Assert(isOpen==true);
	}

	internal static Task ShowOrAdd<T>(bool selectMe = true,bool onlyIfClosed = false,TabPage page = null, UserTab tab=null) where T: UserTab {
		return ShowOrAdd(TabInfo.Get(typeof(T)),selectMe,onlyIfClosed,page,tab);
	}	
	
	internal static Task ShowOrAdd(TabInfo ti,bool selectMe = true,bool onlyIfClosed = false,TabPage page=null,UserTab tab=null)

	{
		System.GC.Collect(2,GCCollectionMode.Default,true,true);
		return AppS.DispatchOnUIThreadTask(() =>
	{
	
		var info = tab is not null ? GetViewItem(tab) : GetViewItem(ti);
		if(onlyIfClosed && info.tabExists)
			return;

		// already here?
		if(info.tabExists )
		{
			if(selectMe)
				info.tabPage.Tabs.SelectedItem = info.tabViewItem;
			return;
		}
		// Todo
		(page??(ti.page 
		switch { 
			TabInfo.TabPageId.main=>TabPage.mainTabs,
			TabInfo.TabPageId.secondary=>TabPage.secondaryTabs,
			TabInfo.TabPageId.chat=>ChatTab.tabPage,
		}
		)).Add(ti,selectMe,tab);
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
			//	if(e.Column.MappingName is (nameof(Player.avatarImage)))
					e.ToolTip.Content = XamlHelper.MakeMarkdown( player.toolTipMd );
				

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
			else  if(e.Record is Army army)
			{
				
					e.ToolTip.Content = army.WorldToolTip().tip;

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
//		var info = e.ContextFlyoutInfo as GridColumnContextFlyoutInfo;
		switch(e.ContextFlyoutType)
		{
			case ContextFlyoutType.RecordCell:
				{
					var info = e.ContextFlyoutInfo as GridRecordContextFlyoutInfo;
					var column = info?.DataGrid.Columns[e.RowColumnIndex.ColumnIndex];
//					var c = info.DataGrid.ge
					if(info.Record is City city)
					{
						e.Handled = true;
						var c = flyout.Target;
						CityUI.ProcessClick(city.cid,AppS.keyModifiers.ClickMods(isRight:true,bringIntoView:true,noFlyout:true,setFocus:true));
				//		Assert(false);
	//					city.AddToFlyout(flyout);

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
					else if(info.Record is Army army)
					{
						if(army.sourceCity.isSubOrMine || (army.targetCity.isSubOrMine && army.isDefense ) ) {
							flyout.AddItem("Return",() => {
								if(army.isDefense || army.isSieging) {
									SendTroops.ShowInstance(prior: army);
								}
								else {
									CnVEventReturnTroops.TryReturn(army);
								}
							});
						}

						break;
					}
					break;
				}
			case ContextFlyoutType.Header: {
					var info = e.ContextFlyoutInfo as GridColumnContextFlyoutInfo;
					var dg = info.DataGrid;
					Assert(info is not null);
					if(info is not null) {
						flyout.AddItem("Reset columns",() => {
							var sizer = dg.ColumnSizer;
							sizer.ResetAutoCalculationforAllColumns();
							sizer.Refresh();
						}
						);
						flyout.AddItem("Toggle Filter",() => {
							if(dg.FilterRowPosition == FilterRowPosition.Top) {
								dg.FilterRowPosition = FilterRowPosition.None;
								dg.AllowFiltering = true;
							//	dg.filter
							}
							else if(dg.AllowFiltering) {
								dg.AllowFiltering = false;

							}
							else {
								dg.FilterRowPosition = FilterRowPosition.Top;
								dg.AllowFiltering = true;

							}
						});

					}
					break;
			}
		}
	}

	//internal void DetailsViewUpdateColumnWidths(object sender,GridDetailsViewExpandedEventArgs e) {
	//	var send2 = e.OriginalSender;
	//	if(sender is xDataGrid grid)
	//		grid.ResetAutoColumns();
	//	if(send2 != sender && send2 is xDataGrid grid2)
	//		grid2.ResetAutoColumns();
	//}

}

internal record class TabInfo(bool persist,Type t,string name,TabInfo.TabPageId page,Symbol symbol = default,char fontIcon = default) {

	internal enum TabPageId {
		main,
		secondary,
		chat
	}

	internal static TabInfo[] all = new TabInfo[] {
		new TabInfo(false,typeof( BuildTab),"Build",TabPageId.main,Symbol.Repair),
			new(   false, typeof( MainPage),"Raid",TabPageId.main,Symbol.ReShare),
			new(   false, typeof( PlayerTab),"Player",TabPageId.main,default,'\uE902'),
			new(   false, typeof( PalaceTab),"Palace",TabPageId.main, Symbol.Like ),
			new(   false, typeof( DonationTab),"RssSender",TabPageId.main,Symbol.Share),
			new(   false, typeof( IncomingTab),"Incoming",TabPageId.main,fontIcon:'\uF0EF'),
			new(   false, typeof( HitHistoryTab),"Hits",TabPageId.secondary,fontIcon:'\uEA69'),
			//	typeof( AttackTab),"Build",TabPageId.main),
			new(   false, typeof( PlannerTab),"Planner",TabPageId.main,Symbol.Map),
			new(   false, typeof( SpotTab),"Recent",TabPageId.secondary,fontIcon:'\uF738'),
			new(   false, typeof( ReinforcementsTab),"Reinforcements",TabPageId.main,Symbol.AddFriend ),
			new(    false,typeof( OutgoingTab),"Outgoing",TabPageId.main,fontIcon:'\uE189'),
			new(    false,typeof( AllianceTab),"Alliance",TabPageId.main,fontIcon:'\uE902'),
			new(    false,typeof( NearDefenseTab),"NearDefense",TabPageId.main,fontIcon:'\uEA18'),
			new(    false,typeof( NearRes),"Near Res",TabPageId.main,Symbol.Download ),
			new(    false,typeof( ChartDialog),"Chart",TabPageId.secondary,fontIcon:'\uEA69'),
			new(    false,typeof( TimelineView),"Timeline",TabPageId.main,Symbol.Calendar ),
			new(    false,typeof( NPCHistory),"Boss Hits",TabPageId.main, Symbol.Play),
			new(    true,typeof( ChatTab),"Chat",TabPageId.chat, Symbol.VideoChat),
		};
		internal static TabInfo Get(Type t) {
		return all.First(i => i.t == t);
	}
	internal bool isOpen => UserTab.GetViewItem(this).tabExists;
	}

public  static partial class UserTabHelpers
{
	//internal static UserTab GetTab(this xDataGrid grid)
	//{
	//	return UserTab.dataGrids[grid];
	//}
	internal static UserTab GetTab(this TabViewItem t) => (t.Content as Frame).Content as UserTab;
	internal static Frame GetFrame(this TabViewItem t) => (t.Content as Frame);
	internal static void Clear(this TabViewItem t) => (t.Content as Frame).Content = new Page();
	internal static void Set(this TabViewItem t, UserTab tab) => (t.Content as Frame).Content = tab ?? new Page();
	internal static bool IsCityGrid(this xDataGrid grid) => object.ReferenceEquals(grid.ItemsSource,City.gridCitySource);

	
}

