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

		public NotifyCollection<Spot> spotMRU { get; } = new NotifyCollection<Spot>();

        public static NotifyCollection<Spot> SpotMRU => instance.spotMRU;
        public static int disableSelection;
	    public static SpotTab instance;
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
        public static bool IsVisible() => instance.isFocused;

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
 


        public static Spot TouchSpot(int cid, VirtualKeyModifiers mod,bool updateSelected=false,bool pin=false)
        {
            var spot = Spot.GetOrAdd(cid);
            if(pin)
            {
               spot.SetPinned(true);
            }
            
            AddToGrid(spot, mod, updateSelected);
            
            return spot;
        }
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


        public static void AddToGrid(Spot spot, VirtualKeyModifiers mod, bool updateSelection = true, bool scrollIntoView=true)
        {
            // Toggle Selected
            if (disableSelection == 0)
            {

                AppS.DispatchOnUIThreadLow(() =>
           {
               var id = SpotMRU.c.IndexOf(spot);
               if (id != 0)
               {
                   if (id > 0)
                   {
                       SpotMRU.RemoveAt(id,true);
                   }
                   else if (SpotMRU.Count >= Settings.mruSize)
                   {
                       // not in list
                       var counter = SpotMRU.Count;
                       while (--counter >= 0)
                       {
                           if (!Spot.selected.Contains(SpotMRU[counter].cid) && SpotMRU[counter].pinned == false)
                           {
                               SpotMRU.RemoveAt(counter,true);
                               break;
                           }

                       }
                   }

                   SpotMRU.Insert(0, spot,true);

               }

               if (updateSelection)
                   CityUI.ProcessSelection(spot as City,mod,false,scrollIntoView);
           });
            }

        }
        

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			bool first = true;
			var pinned = false;
			foreach (var cid in Spot.GetSelectedForContextMenu(0,false,0,false))
			{
				if(first)
				{
					pinned = !Spot.GetOrAdd(cid).pinned;
					first=false;
				}
				Spot.GetOrAdd(cid).SetPinned(pinned);
			}
		}

	
		//      public static void ToggleSelected(Spot rv)
		//      {
		//          var isSelected = rv.ToggleSelected();
		////          SelectSilent(rv, isSelected);
		//      }


	}
}
