using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls;

using Telerik.UI.Xaml.Controls.Grid;
//using Windows.UI.ViewManagement;
//using Windows.UI.WindowManagement;
using Microsoft.UI.Xaml;
using System.ComponentModel;
using Syncfusion.UI.Xaml.DataGrid;
using Windows.Foundation;

//using Microsoft.UI.Windowing;

namespace COTG.Views
{
	using System.Collections;

	public class UserTab:UserControl, IANotifyPropertyChanged
	{

		private const string returnReinforcement = nameof(returnReinforcement);
		public virtual TabPage defaultPage => TabPage.mainTabs;
		public static ImmutableArray<RadDataGrid> spotGrids = ImmutableArray<RadDataGrid>.Empty;
		public static ImmutableArray<RadDataGrid> dataGrids = ImmutableArray<RadDataGrid>.Empty;
		public static ImmutableArray<SfDataGrid> spotSfGrids = ImmutableArray<SfDataGrid>.Empty;

		public event PropertyChangedEventHandler PropertyChanged;
		public void CallPropertyChanged(string members = null)
		{
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(members));
		}
		public void OnPropertyChanged(string member = null)
		{
			if(PropertyChanged is not null) ((IANotifyPropertyChanged)this).IOnPropertyChanged();
		}

		public static UserTab[] userTabs;


		public static void InitUserTabs()
		{

			userTabs = new UserTab[] {
				new MainPage(),
				new IncomingTab(),
				new DonationTab(),
				new DefenseHistoryTab(),
				new BossTab(),
				new HeatTab(),
				new AttackTab(),
				new PlannerTab(),
				new QueueTab(),
				new BuildTab(),
				new SpotTab(),
				new ReinforcementsTab(),
				new OutgoingTab(),
				new HitTab(),
				new NearDefenseTab(),
				new NearRes(),
				new PlayerChangeTab(),
		};

		}

		protected void SpotSelectionChanged(object sender,DataGridSelectionChangedEventArgs e)
		{
			var grid = sender as RadDataGrid;
			Assert(grid != null);
			if(!isOpen)
				return;

			if(SpotTab.silenceSelectionChanges == 0)
			{
				try
				{

					var sel = grid.SelectedItems;
					Spot.selected =new HashSet<int>( sel.Select(a=> ((Spot)a).cid ));

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
				refresh = new(_Refresh) { debounceDelay = 500};// { throttled = true };
										//	ScrollViewer.SetIsVerticalScrollChainingEnabled(this,false);
//			this.Width = 500;
//			this.Height = 500;
			//	ScrollViewer.SetVerticalScrollMode(this, ScrollMode.Auto); //DependencyObjectExtensions.FindDescendant<ScrollViewer>(this).AllowFocusOnInteraction= false;
			Margin= new(8);
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
		public static  void SetupDataGrid(RadDataGrid grid)
		{
			if(!AUtil.AddIfAbsent(ref dataGrids,grid))
				return;
			grid.FontStretch = Windows.UI.Text.FontStretch.Condensed;
			grid.FontWeight = Microsoft.UI.Text.FontWeights.Light;
			
			grid.GridLinesVisibility= Telerik.UI.Xaml.Controls.Primitives.GridLinesVisibility.Both;
		//	grid.ProcessTooltips();
		//	grid.ListenForNestedPropertyChange=false;
			grid.FontSize = SettingsPage.smallFontSize;
			grid.RowHeight = SettingsPage.mediumGridRowHeight;
			grid.ProcessTooltips();

		}

		public void SetupCityDataGrid(RadDataGrid grid)
		{
			// damn, there should be a better way to check for this
			if(!AUtil.AddIfAbsent(ref spotGrids,grid) )
				return;
			grid.SelectionChanged += SpotSelectionChanged;
			
			SetupDataGrid(grid);
		}
		public void SetupCityDataGrid(SfDataGrid grid)
		{
			// damn, there should be a better way to check for this
			if(!AUtil.AddIfAbsent(ref spotSfGrids,grid))
				return;
//			grid.SelectionChanged += SpotSelectionChanged;

//			SetupDataGrid(grid);
		}
		protected void DataGridLoaded(object sender,RoutedEventArgs e)
		{
			if(sender is RadDataGrid rad)
				SetupCityDataGrid(rad);
			else if(sender is SfDataGrid sf)
				SetupCityDataGrid(sf);

		}

		public void SetPlus(bool set)
		{
			App.QueueOnUIThread(() =>
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
		public void ShowOrAdd(bool selectMe = true,bool onlyIfClosed = false)

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

			defaultPage.Add(this,selectMe);
		}

		protected void reinIn_CellTapped(object sender,Syncfusion.UI.Xaml.DataGrid.GridCellTappedEventArgs e)
		{

			try
			{
			//	Note.Show($"Cell Tap {e.Column.HeaderText??"NA"}  {e.RowColumnIndex} {e.RowColumnIndex} {e.Record.ToString} ");
			if (e.Record is City city)
			{

				switch (e.Column.MappingName)
				{
					case nameof(City.cityName):
					case nameof(City.iconUri):
					case nameof(City.remarks):
						city.DoClick();
						break;
				}

				
				return;
			}
				switch(e.Column.MappingName)
				{
					case nameof(Reinforcement.retUri):
						{
							var r = e.Record as Reinforcement;
							Note.Show($"Returning {r.troopsString} from {r.targetCity} back to {r.sourceCity} ");
							r.ReturnAsync();
							if(r.targetCity.reinforcementsIn is not null)
								r.targetCity.reinforcementsIn.Remove(r,true);
							if(r.sourceCity.reinforcementsOut is not null)
								r.sourceCity.reinforcementsOut.Remove(r,true);
							// Todo: refresh lists
							break;

						}
					
				}
			}
			catch(Exception exception)
			{
				Log(exception);
				throw;
			}



		}

		protected void CelNavigate(object sender,Syncfusion.UI.Xaml.Grids.CurrentCellRequestNavigateEventArgs e)
		{
			var uri = new Uri(e.NavigateText);


			if(uri.Scheme == ProtocolActivation.scheme && uri.LocalPath.StartsWith(ProtocolActivation.command))
			{
				var subStr = uri.LocalPath.AsSpan().Slice(ProtocolActivation.command.Length);
				if(subStr.StartsWith(returnReinforcement.AsSpan(),StringComparison.Ordinal))
				{
					e.Handled=true;
					var args = new WwwFormUrlDecoder(subStr.Slice(returnReinforcement.Length).ToString());

					Note.Show("ProtoClick");
					//			var args = Uri.Par
					//	Reinforcement.ReturnAsync(args.GetFirstValueByName("order").ParseLong().GetValueOrDefault(),args.GetFirstValueByName("pid").ParseInt().GetValueOrDefault());

				}


			}
		}

		protected void CellToolTipOpening(object sender,Syncfusion.UI.Xaml.DataGrid.GridCellToolTipOpeningEventArgs e)
		{
			var tt = e.ToolTip;
			var rec = e.Record;
			int q = 0;
		}

		public ADataGrid.ChangeContextDisposable SetupGrid(SfDataGrid grid)
		{
			var _lock0 = grid.ChangeContext();
			grid.ExpanderColumnWidth = 32;
			grid.GridContextFlyoutOpening += ContextFlyoutOpening;
			grid.RecordContextFlyout = new();
			grid.CurrentCellRequestNavigate += CelNavigate;
			grid.CellTapped += reinIn_CellTapped;
			grid.AllowFrozenGroupHeaders=false;
			grid.ColumnWidthMode = Syncfusion.UI.Xaml.Grids.ColumnWidthMode.AutoLastColumnFill;
			grid.CellToolTipOpening += CellToolTipOpening;
//			grid.LiveDataUpdateMode = Syncfusion.UI.Xaml.Data.LiveDataUpdateMode.AllowChildViewUpdate;
			return _lock0;
		}



		protected void ContextFlyoutOpening(object sender,GridContextFlyoutEventArgs e)
		{
			var flyout = e.ContextFlyout;
			flyout.Items.Clear();

			switch(e.ContextFlyoutType)
			{
				case ContextFlyoutType.RecordCell:
				{
					var info = e.ContextFlyoutInfo as GridRecordContextFlyoutInfo;
					var column = info.DataGrid.Columns[e.RowColumnIndex.ColumnIndex];
					if ( info.Record is City city)
					{
						city.AddToFlyout(flyout);
						
						break;
					}

					if ( info.Record is Reinforcement r)
					{
						flyout.AddItem("Return", () => r.ReturnAsync() );
						break;
					}
					break;
				}
			}
		}

		

		public virtual IEnumerable<SfDataGrid> GetGrids()
		{
			yield break;
		}



	}
}
