using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using static CnV.View;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI;
//using CommunityToolkit.WinUI.UI;
//using CommunityToolkit.WinUI.UI.Controls;
using static CnV.City;
//using Expander = CommunityToolkit.WinUI.UI.Controls.cer;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	public sealed partial class CityStats:UserControl
	{
		public static CityStats instance;
		public CityStats()
		{
			this.InitializeComponent();
			instance = this;
		}
		public City city => City.GetBuild();


		//public SolidColorBrush ResourceForeground(int resId) => new SolidColorBrush(Windows.UI.Color.FromArgb(255,(byte)(31+64*resId),128,128) );
		//public string ResourceStr(int resId) => $"{city?.resources[resId]:N0}";

		//public event PropertyChangedEventHandler PropertyChanged;
	
		// invalidate the cache
		private static City lastDisplayed;
		public static void CityBuildingsChange(City _city)
		{
			if(_city == lastDisplayed)
				lastDisplayed = null;
			}
		//public void OnPropertyChanged() =>
		//		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
			public void UpdateUI()
		{
			try
			{
				//	var i = CityStats.instance;
				var city = this.city;
				if(city.IsInvalid())
					return;
				// building counts
				var wasDisplayed = lastDisplayed == city;
				if(!wasDisplayed)
					lastDisplayed = city;
				var bdd = !wasDisplayed  ?  GetBuildingCounts(city) : default;


				AppS.DispatchOnUIThreadIdle(() =>
				{
					
					
					try
					{
						var t = CnVServer.simTime;
						ShellPage.instance.timeDisplay.Text = t.FormatWithYear();

						if(expResource.IsExpanded)
						{
							var resources = city.SampleResources();
							var panels = expResource.Child<CommunityToolkit.WinUI.UI.Controls.WrapPanel>().Children<StackPanel>();
							for(var r = 0; r< CnV.Resources.idCount; r++)
							{
								var ch = panels.ElementAt(r).Children<TextBlock>();
								Assert(ch.Count==2);
								var txt = ch.ElementAt(0);
								var prod = ch.ElementAt(1);

							//				var txt = r switch { 0 => res0, 1 => res1, 2 => res2, _ => res3 };
							//				var prod = r switch { 0 => prod0, 1 => prod1, 2 => prod2, _ => prod3 };

							var res = resources[r];
								var storage = city.stats.storage[r];
								txt.UpdateLazy($"{res:N0}", (res >= storage ? Colors.Red : res >= storage*3/4 ? Colors.Orange : res == 0 ? Colors.Gray : Colors.Green));
								var p = city.stats.production[r];
								prod.UpdateLazy($"{CnV.Resources.names[r]}/h:{p:+#,#;-#,#;' --'}", (p switch
								{
									> 0 => Colors.Gray,
									< 0 => Colors.Yellow,
									_ => Colors.DarkGray

								}));
							}
						}
						if(!wasDisplayed)
						{
							var txt= (expBuildings.Header as DependencyObject).Child<TextBlock>(1);
							txt.UpdateLazy($"Buildings: [{bdd.buildingCount}/{bdd.townHallLevel*10}] cs={city.stats.cs:N0}  ");

							buildingCountGrid.SelectedItems.Clear();
							var bd = new List<BuildingCountAndBrush>();
							foreach(var i in bdd.counts)
							{
								if(i.Value > 0)
								{
									bd.Add(new BuildingCountAndBrush()
									{ count = i.Value, image = CityBuild.GetBuildingImage(i.Key, BuildingCountAndBrush.width) });
								}
							}

							bd.Add(new BuildingCountAndBrush()
							{ count = bdd.buildingCount+bdd.towerCount, image = CityBuild.GetBuildingImage(Building.bidTownHall, BuildingCountAndBrush.width) });

							// var button = sender as Button; button.Focus(FocusState.Programmatic);

							buildingCountGrid.ItemsSource = bd;
						}
					}
					catch(Exception e)
					{
						LogEx(e, report: false);
					}
				});

			}
			catch(Exception e)
			{
				LogEx(e, report: false);
			}
		}
		private void Expander_Expanding(Microsoft.UI.Xaml.Controls.Expander sender, ExpanderExpandingEventArgs args)
		{
			UpdateUI();
		}

		private void scroll_SizeChanged(object sender, ScrollViewerViewChangedEventArgs e)
		{
			if(e.IsIntermediate)
				return;
			ScrollSizeChanged();
		}
		private void scroll_SizeChanged4(object sender, SizeChangedEventArgs e)
		{
			ScrollSizeChanged();

		}
		private void ScrollSizeChanged()
		{
			DebounceA.Q(runOnUIThread: true, debounceT: 50, action: () =>
			{
				var baseSize = ((scroll.ActualWidth)/scroll.ZoomFactor).Max(0);
				stackPanel.Width = (baseSize -8).Max(0);
				//var expanderWidth = (baseSize -14).Max(0);
				//foreach(var ch in stackPanel.Children<Expander>())
				//{
				//	ch.Width = expanderWidth;
				//}
			});
		}
		

		private void Expander_Expanded(object sender, EventArgs e)
		{
			UpdateUI();

		}

		private (int buildingCount, int towerCount, int townHallLevel, Dictionary<BuildingId, int> counts) GetBuildingCounts(City build)
		{
			var buildingCounts = new Dictionary<BuildingId, int>();
			int buildingCount = 0;
			int towerCount = 0;
			try
			{
				var buildings = CityBuild.isPlanner ? build.GetLayoutBuildings() : build.postQueueBuildings;



				foreach(var bdi in buildings)
				{
					var id = bdi.id;
					if(id == 0 || !bdi.isBuilding || id == Building.bidTownHall || id == Building.bidWall)
					{
						continue;
					}

					{
						if(bdi.isTower)
							++towerCount;
						else
							++buildingCount;

						if(!buildingCounts.TryGetValue(id, out var counter))
						{
							buildingCounts.Add(id, 0);
							counter = 0;
						}

						buildingCounts[id] = counter + 1;
					}
				}
				return (buildingCount, towerCount, buildings[Building.bspotTownHall].bl, buildingCounts);

			}
			catch(Exception ex)
			{
				LogEx(ex);
			}
			return (buildingCount, towerCount, -1, buildingCounts);
		}

		
		
		
	}
	public class BuildingCountAndBrush
	{
		public const int width = 32;
		public Microsoft.UI.Xaml.Media.Imaging.BitmapImage image { get; set; }
		public int count { get; set; }

	}

}
