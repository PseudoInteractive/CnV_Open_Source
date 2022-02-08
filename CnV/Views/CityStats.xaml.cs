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
using CommunityToolkit.WinUI.UI;
using CommunityToolkit.WinUI.UI.Controls;

//using Expander = CommunityToolkit.WinUI.UI.Controls.Expander;
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

		//public void OnPropertyChanged() =>
		//		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
		public void UpdateUI()
		{
		//	var i = CityStats.instance;
			var city = this.city;
			if(city.IsInvalid())
				return;

			var dummy = expResource.Children<TextBlock>(8);
			dummy = expBuildQuerue.Children<TextBlock>(8);
			dummy = expResource.Children<TextBlock>(8);

			var resources = city.SampleResources();
			var panels = expResource.Child<WrapPanel>().Children<StackPanel>();
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
				{ 
				var bCounts = city.GetTownHallAndBuildingCount(true);
				expBuildings.Header = $"Buildings\t[{bCounts.count}/{bCounts.max}]";

				}
			}
		}

		private void Expander_Expanding(Microsoft.UI.Xaml.Controls.Expander sender, ExpanderExpandingEventArgs args)
		{
			UpdateUI();
		}

		private void scroll_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			stackPanel.Width = e.NewSize.Width;
		}

		
		private void Expander_Expanded(object sender, EventArgs e)
		{
			UpdateUI();

		}
	}
}
