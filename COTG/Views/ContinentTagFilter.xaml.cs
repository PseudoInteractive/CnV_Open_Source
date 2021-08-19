using COTG.Game;

using Cysharp.Text;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace COTG.Views
{
	public sealed partial class ContinentTagFilter : ContentDialog
	{
		public static ContinentTagFilter instance;
		public ContinentTagFilter()
		{
			this.InitializeComponent();
			instance = this;
		}
		
		public static async Task<bool> Show(bool actuallyClearPlease=false)
		{
			if (instance == null)
				new ContinentTagFilter();

			foreach (var tag in TagHelper.tagsWithoutAliases)
			{
				var check = instance.tagsPanel.Children.FirstOrDefault((a) => a is ToggleButton b && b.Content as string == tag.s) as ToggleButton;
				if (check == null)
				{
					check = new ToggleButton() { Content = tag.s };
					instance.tagsPanel.Children.Add(check);
				}
				check.IsChecked = actuallyClearPlease ? false : Spot.tagFilter.HasFlag(tag.v);
			}
			{
				var isAll = Spot.isContinentFilterAll|actuallyClearPlease;
				for (int id = 0; id < World.continentCount; ++id)
				{
					var xy = World.PackedContinentToXY(id);
					var text = ZString.Format("{0}{1}", xy.y, xy.x);
					var check = instance.continentsPanel.Children.FirstOrDefault((a) => a is ToggleButton b && b.Content as string == text) as ToggleButton;
					if (check == null)
					{
						check = new ToggleButton() { Content = text };
						instance.continentsPanel.Children.Add(check);

					}
					check.IsChecked = !isAll && Spot.TestContinentFilterPacked(id);
				}
			}

			var rv = actuallyClearPlease ? ContentDialogResult.Primary : await instance.ShowAsync2();
			if (rv == ContentDialogResult.Primary)
			{


				Spot.tagFilter = default;
				// Write back tags
				foreach (var tag in TagHelper.tagsWithoutAliases)
				{
					var check = instance.tagsPanel.Children.FirstOrDefault((a) => a is ToggleButton b && b.Content as string == tag.s) as ToggleButton;
					if (check != null)
					{
						if (check.IsChecked.GetValueOrDefault())
						{
							Spot.tagFilter |= tag.v;
						}

					}
				}
				// Write back continents
				{
					var any = false;
					int first = 0;
					Spot.continentFilter = 0;
					for (int id = 0; id < World.continentCount; ++id)
					{
						var but = instance.continentsPanel.Children[id] as ToggleButton;
						var v = but.IsChecked;
						if (v.GetValueOrDefault())
						{
							if (!any)
							{
								first = id;
							}

							any = true;
							Spot.continentFilter |= Spot.ContinentFilterFlag(id);
						}
					}
					string label;
					if (!any)
					{
						Spot.continentFilter = Spot.continentFilterAll;

						label = "Cont/Tag";
					}
					else
					{
						// is just one set?
						var xy = World.PackedContinentToXY(first);
						if ((Spot.continentFilter & (Spot.continentFilter - 1ul)) == 0)
						{
							label = ZString.Format("{0}{1}", xy.y, xy.x);
						}
						else
						{
							label = ZString.Format("{0}{1}+", xy.y, xy.x);
						}
					}
					ShellPage.instance.ContinentFilter.Content = label;
					//	ExportCastles.instance.ContinentFilter.Content = label;
					CityList.NotifyChange();
					ShellPage.RefreshTabs.Go();

				}

				return true;
			}
			else
			{
				return false;
			}

		}

		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void TagsFilterClear(object sender, RoutedEventArgs e)
		{
			foreach (var but in instance.tagsPanel.Children)
			{
				var t = but as ToggleButton;
				if (t != null)
					t.IsChecked = false;
			}
		}

		private void ContinentFilterClear(object sender, RoutedEventArgs e)
		{
			foreach (var but in instance.continentsPanel.Children)
			{
				var t = but as ToggleButton;
				if (t != null)
					t.IsChecked = false;
			}

		}
	}
}
