//using Windows.UI.Core;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;

//using Windows.UI.Input;
using Windows.System;

namespace CnV.Views
{
	using Game;

	public sealed partial class SpotTab : UserTab
    {
		public override TabPage defaultPage => TabPage.secondaryTabs;

	//	public static ObservableCollection<Spot> spotMRU { get; } = new ObservableCollection<Spot>();

        public static ObservableCollection<Spot> spotMRU = new ObservableCollection<Spot>();
        public ObservableCollection<object> selection = new ObservableCollection<object>();
        public static int disableSelection;
	    public static SpotTab? instance;
        public SpotTab()
        {
            Assert(instance == null);
            instance = this;
			this.InitializeComponent();
			
			//			selectedGrid.SelectionChanged += SpotSelectionChanged;
			//	selectedGrid.OnKey = Spot.OnKeyDown;
			//			selectedGrid.ProcessTooltips();
			//			spotGrids.Add(selectedGrid);
		}
        public static bool IsVisible() =>  instance is not null && instance.isFocused;

        public static int silenceSelectionChanges;



		//private void gridPointerPress(object sender, PointerRoutedEventArgs e)
  //      {
  //          try
  //          {
  //              ++disableSelection;
  //              ++silenceSelectionChanges;
  //              Spot.GridPressed(sender, e);
  //          }
  //          catch (Exception ex)
  //          {
  //              LogEx(ex);
  //          }
  //          finally
  //          {
  //              --disableSelection;
  //              --silenceSelectionChanges;
  //          }
  //      }
        //private void gridPointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    Spot.ProcessPointerMoved(sender, e);
        //}
 


       
        //public static void SelectedToGrid()
        //{
        //    ++silenceChanges;
        //    AppS.DispatchOnUIThreadLow(() =>
        //    {
        //        try
        //        {

        //            var sel = new HashSet<int>(Spot.selected);
                  

        //            foreach (Spot i in instance.selectedGrid.SelectedItems.ToArray())
        //            {
        //                if (!sel.Remove(i.cid))
        //                    instance.selectedGrid.DeselectItem(i);
        //            }
        //            foreach (var i in sel)
        //            {
        //                instance.selectedGrid.selectionService.SelectRowUnit(Spot.GetOrAdd(i), true, false);
        //            }

        //            // Todo: optimize this
        //            // now do raiding grid
        //            sel = new HashSet<int>(Spot.selected);

        //            foreach (Spot i in MainPage.CityGrid.SelectedItems.ToArray())
        //            {
        //                if (!sel.Remove(i.cid))
        //                    instance.selectedGrid.DeselectItem(i);
                      
        //            }
        //            foreach (var i in sel)
        //            {
        //                instance.selectedGrid.selectionService.SelectRowUnit(Spot.GetOrAdd(i), true, false);
        //            }


        //            //    var sel = instance.selectedGrid.SelectedItems;
        //            //                    sel.A

        //        }
        //        catch (Exception e)
        //        {
        //            Log(e);
        //        }
        //        finally
        //        {
        //            --silenceChanges;
        //        }
        //    });

        //}
        //public static void SelectOne(Spot spot)
        //{
        //    AppS.DispatchOnUIThreadLow(() =>
        //    {
        //        try
        //        {
        //            ++silenceChanges;
        //            var sel = instance.selectedGrid.SelectedItems;
        //            sel.Clear();
        //            sel.Add(spot);
        //            //                    instance.selectedGrid.DeselectAll();
        //            //                    instance.selectedGrid.SelectItem(spot);

        //        }
        //        catch (Exception e)
        //        {
        //            Log(e);
        //        }
        //        finally
        //        {
        //            --silenceChanges;
        //        }
        //    });
        //}


        public static void AddToGrid(Spot spot, bool moveToFront=true)
        {
			if(!spot.isValid) {
				Assert(false);
				return;
			}
			AppS.DispatchOnUIThread(() => {
			
			
			if(disableSelection == 0) {

				{
					var id = spotMRU.IndexOf(spot);
					if(id != 0) {
						if(id > 0) {
								if(moveToFront) {
									if(id > 10) {
									
										spotMRU.Move(id,0);
									}
									else {
										if(instance is not null) {
											instance.selectedGrid.ScrollItemIntoView(spot,true);
										}
									}
								}
						}

						else {
							Assert(id==-1);
							if(spotMRU.Count >= Settings.mruSize) {
								// not in list
								var counter = spotMRU.Count;
								while(--counter>=0) {
									if(!Spot.selected.Contains(spotMRU[counter].cid) && spotMRU[counter].pinned == false) {
										// only removes 1
										spotMRU.RemoveAt(counter);
										if(spotMRU.Count < Settings.mruSize)
											break;
									}

								}
							}
							spotMRU.Insert(0,spot);
						}
					}


					
				}
		
            }	});

        }
        

		//private void Button_Click(object sender, RoutedEventArgs e)
		//{
		//	bool first = true;
		//	var pinned = false;
		//	foreach (var cid in Spot.GetSelectedForContextMenu(0,false,0,false))
		//	{
		//		if(first)
		//		{
		//			pinned = !Spot.GetOrAdd(cid).pinned;
		//			first=false;
		//		}
		//		Spot.GetOrAdd(cid).SetPinned(pinned);
		//	}
		//}
		public override async Task Closed()
		{ 
			await base.Closed();
			instance = null;
		}
		internal static void SyncSelectionToUI(bool syncRecentGrid=true, bool syncCityGrid=true) {
			
			++SpotTab.silenceSelectionChanges;
			try {
				var i = instance;
				var selCities = City.selected.Select(a => a.AsCity());

				// Sync spot grid
				if(i is not null && syncRecentGrid) {
					// Add any that are missing
					foreach(var  c in selCities) {
					if(!spotMRU.Contains(c))
						SpotTab.AddToGrid(c,false);
					}
					selCities.ToHashSet<object>().SyncSet(i.selection);
				}
				// Sync selected cities in gridCityGrid
				if(syncCityGrid)
					selCities.Where(c => City.gridCitySource.Contains(c)).ToHashSet<object>().SyncSet(City.gridCitySelected);
			}
			catch(Exception ex) {
				LogEx(ex);
			}
			finally
				{
				--SpotTab.silenceSelectionChanges;
			}
		}
		private void selectedGrid_SelectionChanged(object sender,Syncfusion.UI.Xaml.Grids.GridSelectionChangedEventArgs e) {
			if(silenceSelectionChanges==0) {
				++silenceSelectionChanges;
				try {
					City.selected = selection.Select(i => (i as City).cid).ToHashSet();
					SyncSelectionToUI(syncRecentGrid: false,syncCityGrid: true);
				}
				catch(Exception ex) 
				{
				LogEx(ex);
				}finally 
				{
					--silenceSelectionChanges; 
				}
			}
		}

		private void ClearPinned(object sender,RoutedEventArgs e) {
			foreach(var p in Settings.pinned) {
				var c = p.AsCity();
				Assert(c.pinned);
				c.pinned=false;
				c.OnPropertyChanged();
			}
			Settings.pinned = Array.Empty<SpotId>();
		}
		private async void PinSelected(object sender,RoutedEventArgs e) {
			var i = await AppS.DoYesNoBox("Replace or Append Pinned","Should current list of pinned spots be replaced or appended to?",yes: "Replace",no: "Append");
			if(i==-1)
				return;
			if(i==1)
				ClearPinned(default,default);

			foreach(var p in City.selected) {
				var c = p.AsCity();
				c.SetPinned(true);
			}
		}

		private void SelectPinned(object sender,RoutedEventArgs e) {
			foreach(var p in Settings.pinned) {
				City.selected.Add(p);
				
			}
			SyncSelectionToUI();
		}

		private void ClearSelected(object sender,RoutedEventArgs e) {
			City.selected.Clear();
			SyncSelectionToUI();
		}

        private void OnLoaded(object sender,RoutedEventArgs e) {
			base.SetupDataGrid(selectedGrid,true,typeof(City),spotMRU);
        }

        //      public static void ToggleSelected(Spot rv)
        //      {
        //          var isSelected = rv.ToggleSelected();
        ////          SelectSilent(rv, isSelected);
        //      }


    }
}
