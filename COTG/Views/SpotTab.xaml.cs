using COTG.Game;
using COTG.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using Windows.UI.Xaml.Controls;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Grid;
using static COTG.Debug;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data;
using System.Collections.Specialized;
using Windows.Foundation;
using Microsoft.Toolkit.Uwp;
using Windows.UI.Xaml.Input;
using COTG.Services;
using System.Collections;
using COTG.JSON;
using Windows.UI.Input;
using Telerik.UI.Xaml.Controls.Input;
using COTG.Helpers;
using Windows.UI.Xaml.Navigation;
using Windows.System;
using System.Linq;

namespace COTG.Views
{
    public sealed partial class SpotTab : UserTab
    {
        public DumbCollection<Spot> spotMRU { get; } = new DumbCollection<Spot>();

        public static DumbCollection<Spot> SpotMRU => instance.spotMRU;
        public static int disableSelection;
		static bool loaded;
        public static SpotTab instance;
        public SpotTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();
            selectedGrid.SelectionChanged += SpotTabSelectionChanged;
			selectedGrid.OnKey = Spot.OnKeyDown;
			selectedGrid.ProcessTooltips();
		}
        public static bool IsVisible() => instance.isVisible;

        public static int silenceSelectionChanges;

        private void SpotTabSelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            if (silenceSelectionChanges == 0)
            {
                try
                {

                    var sel = selectedGrid.SelectedItems;
                    var newSel = new HashSet<int>();
                    foreach (Spot s in sel)
                    {
                        newSel.Add(s.cid);

                    }

                    //          Spot.selected.EnterWriteLock();

                    Spot.selected = newSel;
                                    }
                catch (Exception ex)
                {
                    LogEx(ex);
                }
                finally
                {
          //          Spot.selected.ExitWriteLock();
                }
            }
        }

        private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                ++disableSelection;
                ++silenceSelectionChanges;
                Spot.ProcessPointerPress(this,sender, e);
            }
            catch (Exception ex)
            {
                LogEx(ex);
            }
            finally
            {
                --disableSelection;
                --silenceSelectionChanges;
            }
        }
        //private void gridPointerMoved(object sender, PointerRoutedEventArgs e)
        //{
        //    Spot.ProcessPointerMoved(sender, e);
        //}
        private void gridPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Spot.ProcessPointerExited();
        }


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
        //    App.DispatchOnUIThreadSneaky(() =>
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
        //    App.DispatchOnUIThreadSneaky(() =>
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

                App.DispatchOnUIThreadSneaky(() =>
           {
               var id = SpotMRU.IndexOf(spot);
               if (id != 0)
               {
                   if (id > 0)
                   {
                       SpotMRU.RemoveAt(id);
                   }
                   else if (SpotMRU.Count >= SettingsPage.mruSize)
                   {
                       // not in list
                       var counter = SpotMRU.Count;
                       while (--counter >= 0)
                       {
                           if (!Spot.selected.Contains(SpotMRU[counter].cid) && SpotMRU[counter].pinned == false)
                           {
                               SpotMRU.RemoveAt(counter);
                               break;
                           }

                       }
                   }

                   SpotMRU.Insert(0, spot);

               }

               if (updateSelection)
                   spot.ProcessSelection(mod,false,scrollIntoView);
           });
            }

        }
        public static void LoadFromPriorSession( )
        {
			if(!loaded)
			{
				loaded = true;
				SettingsPage.pinned = SettingsPage.pinned.ArrayRemoveDuplicates();

				foreach (var m in SettingsPage.pinned)
				{
				  var spot=  TouchSpot(m, VirtualKeyModifiers.None, false,true);
				}
			}
		}

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			bool first = true;
			var pinned = false;
			foreach (var cid in Spot.GetSelectedForContextMenu(0,false))
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
