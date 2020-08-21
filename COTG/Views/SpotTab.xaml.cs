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

namespace COTG.Views
{
    public sealed partial class SpotTab : UserTab
    {
        public DumbCollection<Spot> spotMRU { get; } = new DumbCollection<Spot>();

        public static DumbCollection<Spot> SpotMRU => instance.spotMRU;
        public static int disableSelection;

        public static SpotTab instance;
        public SpotTab()
        {
            Assert(instance == null);
            instance = this;
            this.InitializeComponent();
            selectedGrid.SelectionChanged += SpotTabSelectionChanged;

        }
        public static bool IsVisible() => instance.isVisible;

        public static int silenceChanges;

        private void SpotTabSelectionChanged(object sender, DataGridSelectionChangedEventArgs e)
        {
            if (silenceChanges == 0)
            {
                try
                {

                    var sel = selectedGrid.SelectedItems;
                    Spot.selected.EnterWriteLock();

                    Spot.selected.Clear();
                    foreach (Spot s in sel)
                    {
                        Spot.selected.Add(s.cid);
 
                    }
                }
                catch(Exception ex)
                {
                    Log(ex);
                }
                finally
                {
                    Spot.selected.ExitWriteLock();
                }
            }
        }

        private void gridPointerPress(object sender, PointerRoutedEventArgs e)
        {
            try
            {
                ++disableSelection;
                Spot.ProcessPointerPress(sender, e);
            }
            catch(Exception ex)
            {
                Log(ex);
            }
            finally
            {
                --disableSelection;
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


        public static Spot TouchSpot(int cid)
        {
            var spot = Spot.GetOrAdd(cid);
            if (disableSelection == 0)
            {
                AddToGrid(spot);
            }
            return spot;
        }
        public static void SelectSilent(Spot spot, bool selected )
        {
            App.DispatchOnUIThreadSneaky(() =>
            {
                try
                {
                    ++silenceChanges;
                    if (selected)
                        instance.selectedGrid.SelectItem(spot);
                    else
                        instance.selectedGrid.DeselectItem(spot);

                    Assert(instance.selectedGrid.SelectedItems.Contains(spot) == selected);
                }
                catch (Exception e)
                {
                    Log(e);
                }
                finally
                {
                    --silenceChanges;
                }
            });
        }
        public static void SelectOne(Spot spot)
        {
            App.DispatchOnUIThreadSneaky(() =>
            {
                try
                {
                    ++silenceChanges;
                    instance.selectedGrid.DeselectAll();
                    instance.selectedGrid.SelectItem(spot);

                }
                catch (Exception e)
                {
                    Log(e);
                }
                finally
                {
                    --silenceChanges;
                }
            });
        }

        public static void AddToGrid(Spot spot)
        {
            // Toggle Selected

            App.DispatchOnUIThreadSneaky( () =>
            {
           
                    if (!SpotMRU.Contains(spot))
                    {
                        SpotMRU.Add(spot);
                    }

                    spot.ProcessSelection();
            });

        }
  //      public static void ToggleSelected(Spot rv)
  //      {
  //          var isSelected = rv.ToggleSelected();
  ////          SelectSilent(rv, isSelected);
  //      }


    }
}
