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
	public class TroopTypeComboItem
	{
		public byte type { get; set; }

		//public byte type { get => _type;
		//	set {
		//		if(_type != value) {

		//			_type=value;
		//			OnPropertyChanged();
				
		//		}
		//	} 
		//}
		public ImageSource image => Troops.Image(type.Max(0),32);
	}
	public sealed partial class TroopsControl:UserControl, INotifyPropertyChanged
	{
		public static readonly DependencyProperty troopsProperty = DependencyProperty.Register(
		  "troops",
		  typeof(TroopTypeCounts),
		  typeof(TroopsControl),
		  new PropertyMetadata(new TroopTypeCounts())
		);

		public string label { get; set; }

		public City city { get; set; }

		internal static TroopTypeComboItem[] troopTypeIds = Enumerable.Range(0,Troops.ttCount).Select(a => new TroopTypeComboItem() { type=(byte)a  } ).ToArray();
		public TroopTypeCounts troops
		{
			get { return (TroopTypeCounts)GetValue(troopsProperty); }
			set { SetValue(troopsProperty,value); }
		}

		internal TroopTypeCounts ttCounts {
			get {
				var rv = new TroopTypeCounts();
				foreach(var ti in troopItems) {
					if(ti.count > 0)
						rv += ti.tt;
				}
				return rv;
			}
			set {
				troopItems.Clear();
				foreach(var tt in value) {
					troopItems.Add(CreateTroopItem(tt.type,tt.count));
				}
			
			}
		}
		RecruitTroopItem CreateTroopItem(byte type, uint count) {
			var rv = new RecruitTroopItem() { count=(int)count,type=type,city=city };
			rv.PropertyChanged+=TroopsPropertyChanged;
			return rv;
		}

		private void TroopsPropertyChanged(object? sender,PropertyChangedEventArgs e) {
			WriteBackTroops();
			OnPropertyChanged();
		}

		internal ObservableCollection<RecruitTroopItem> troopItems = new();
		public TroopsControl() {
			this.InitializeComponent();
			
		}
		private void WriteBackTroops() {
			troops = ttCounts;
		}

		private void AddClick(object sender,RoutedEventArgs e) {
			troopItems.Add(CreateTroopItem(Troops.ttVanq,1));
		}

		public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged(string? member = null)
	{
		if (this.PropertyChanged is not null)
			AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
	}
	}
}
