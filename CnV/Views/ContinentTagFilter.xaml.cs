using CnV.Game;


using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CnV.Views
{
	using Game;

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
					check.SetToolTip(tag.v.EnumName());
					instance.tagsPanel.Children.Add(check);
				}
				check.IsChecked = actuallyClearPlease ? false : Spot.tagFilter.HasFlag(tag.v);
			}
			{
				var isAll = Spot.isContinentFilterAll|actuallyClearPlease;
				for (int id = 0; id < World.continentCount; ++id)
				{
					var xy = World.ContinentIdToXY(id);
					var text = $"{xy.y}{xy.x}";
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


				string label=null;

				Spot.tagFilter = default;
				int tagCount = 0;
				// Write back tags
				foreach (var tag in TagHelper.tagsWithoutAliases)
				{
					var check = instance.tagsPanel.Children.FirstOrDefault((a) => a is ToggleButton b && b.Content as string == tag.s) as ToggleButton;
					if (check != null)
					{
						if (check.IsChecked.GetValueOrDefault())
						{
							Spot.tagFilter |= tag.v;
							++tagCount;
							if(label == null)
								label = $"{tag.s}";
						}

					}
				}

				if (tagCount > 1)
					label += $"(+{tagCount-1})";
				// Write back continents
				{
					int first = -1;
					Spot.continentFilter = 0;
					int contCount = 0;
					for (int id = 0; id < World.continentCount; ++id)
					{
						var but = instance.continentsPanel.Children[id] as ToggleButton;
						var v = but.IsChecked;
						if (v.GetValueOrDefault())
						{
							if (first == -1)
							{
								first = id;
							}
							++contCount;

							Spot.continentFilter |= Spot.ContinentFilterFlag(id);
						}
					}
						if (first == -1 )
						{
							if ( label == null)
								label = "No Filter";
							Spot.continentFilter = Spot.continentFilterAll;
						}
						else
						{
							// is just one set?
							var xy = World.ContinentIdToXY(first);
							var contLabel =$"{xy.y}{xy.x}";
							if (contCount > 1)
							{
								contLabel += $"(+{contCount-1})";
							}

							if ( label == null)
								label = contLabel;
							else
								label = $"{contLabel} {label}";

						}
				

					ShellPage.instance.ContinentFilter.Content = label;
					//	ExportCastles.instance.ContinentFilter.Content = label;
					ShellPage.CityListNotifyChange(true);
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
