using COTG.Draw;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static COTG.Draw.CityView;
using static COTG.Game.City;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
	public sealed partial class CityBuild : UserControl
	{
		public static CityBuild instance = new CityBuild();
		public CityBuild()
		{
			this.InitializeComponent();
		}

		private void Upgrade_Click(object sender, RoutedEventArgs e)
		{
			JSClient.view.InvokeScriptAsync("upgradeBuilding",new[] { (selected.x-span0).ToString(), (selected.y-span0).ToString(), (GetBuilding(selected).bl + 1).ToString() });
		}

		private void Downgrade_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Destroy_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}
