using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	public class TroopTypeComboItem {
		public byte type { get; set; }
		public ImageSource image => Troops.Image(type.Max(0),32);
	}
	public sealed partial class TroopsControl:UserControl
	{
		public static readonly DependencyProperty troopsProperty = DependencyProperty.Register(
		  "troops",
		  typeof(TroopTypeCounts),
		  typeof(TroopsControl),
		  new PropertyMetadata(new())
		);

		public City city { get; set; }

		internal static TroopTypeComboItem[] troopTypeIds = Enumerable.Range(0,Troops.ttCount).Select(a => new TroopTypeComboItem() { type=(byte)a } ).ToArray();
		public TroopTypeCounts troops
		{
			get => ttCounts;
			set {
				troopItems.Clear();
				foreach(var tt in value) {
					troopItems.Add(new() { count=(int)tt.count,type=tt.type,city=city });
				}
			
			}
		}

		internal TroopTypeCounts ttCounts {
			get {
				var rv = new TroopTypeCounts();
				foreach(var ti in troopItems) {
					if(ti.count >= 0)
						rv += ti.tt;
				}
				return rv;
			}
		}

		internal ObservableCollection<RecruitTroopItem> troopItems = new();
		public TroopsControl() {
			this.InitializeComponent();
		}

		private void AddClick(object sender,RoutedEventArgs e) {
			troopItems.Add(new() { city=city,count=1,type=Troops.ttVanq });
		}
	}
}
