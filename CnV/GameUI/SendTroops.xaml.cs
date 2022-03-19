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
	public sealed partial class SendTroops:DialogG,INotifyPropertyChanged
	{
		internal bool viaWater;
		internal bool isSettle;
		protected override string title => $"{(isSettle? "Setting" : Army.typeStrings[(int)type] )} => {target}"; 
		internal static SendTroops? instance;
		internal City city;
		internal City target;
		internal ArmyType type;
		internal ArmyTransport transport => isSettle ? (viaWater ? ArmyTransport.ports : ArmyTransport.carts) : (viaWater ? ArmyTransport.water : ArmyTransport.land);
		public SendTroops()
		{
			this.InitializeComponent();
			instance = this;
			
		}

		private void UpdateTroopItems()
		{
			troopItems = new SendTroopItem[Troops.ttCount];


			for(int i = 0;i<Troops.ttCount;++i)
				troopItems[i] = new(target:target, city:city,type:(byte)i,wantMax:true) ;

			OnPropertyChanged();
		}
		public static void ShowInstance(City city,City target,bool isSettle,bool viaWater=false, ArmyType type = ArmyType.defense)
		{
			var rv = instance ?? new SendTroops();
			rv.city = city;
			rv.target = target;
			rv.isSettle = isSettle;
			rv.viaWater = viaWater;
			rv.type = type;
			rv.UpdateTroopItems();
			rv.Show(false);
			
		}

		internal bool IsValid(bool verbose)
		{
			if(isSettle)
			{
				if(!troopItems.Any(a => a.type == Troops.ttSenator && a.count > 0))
				{
					if(verbose)
						AppS.MessageBox($"Need a Magistra to settle");
					return false;
				}
			}
			if(transport == ArmyTransport.carts && city.cartsHome < 250)
			{
				if(verbose) AppS.MessageBox($"Need 250 carts.  Have {city.cartsHome} home of {city.carts} ");
				return false;
			}
			if(transport == ArmyTransport.ports && city.shipsHome < 25)
			{
				if(verbose) AppS.MessageBox($"Need 25 trading ships.  Have {city.shipsHome} home of {city.ships} ");
				return false;
			}
			if(!troopItems.Any(a => a.count > 0))
			{
				if(verbose) AppS.MessageBox($"Please send something");
				return false;
			}
			return true;
		}

		//private bool isSettle => (transport == ArmyTransport.carts || transport==ArmyTransport.ports);


		internal SendTroopItem [] troopItems;

		private void SendTroopsClick(object sender,RoutedEventArgs e)
		{
			if(!IsValid(true))
				return;
			
			
			var ts = new TroopTypeCounts();
			foreach(var i in troopItems)
			{
				if(i.count > 0)
				{
					ts += i.tt;
					
				}
			}
			if(ts.Any())
			{

				using(var __lock = Sim.eventQLock.Enter)
				{
					var t = Army.FromNow(city,target.cid,transport,type,ts,false);
				//	Assert(!t.departed);
				//	Assert(t.isSchedueledNotSent);
					new CnVEventSendTroops(t).EnqueueAlreadyLocked();
					//AppS.MessageBox("Send",info.ToString());
					//Note.Show("Send");
					//new CnVEventRecruit(city.c,tt).Execute();
				}
				Hide();
			}
			else
			{
				AppS.MessageBox("Please select troops to send");
			}
		}
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.DispatchOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		public static void Changed(string? member = null)
		{
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}
	}


	internal class SendTroopItem: INotifyPropertyChanged
	{
		
		internal City city; // convienience
				internal City target;

		internal TType type;
		internal uint count;

		public SendTroopItem(City city,City target,byte type, bool wantMax)
		{
			this.city=city;
			this.target=target;
			this.type=type;
			this.count =wantMax ?city.troopsHome.GetCount(type) : 0;
		}

		internal TroopTypeCount tt => new(type,count);
		internal ImageSource image => Troops.Image(type);
		internal TroopInfo info => TroopInfo.all[type];
		internal void MaxClick(object sender,RoutedEventArgs e)
		{
			count = city.troopsHome.GetCount(type);
			OnPropertyChanged(nameof(count));
		}
		public string troopsHome => city.troopsHome.GetCount(type).Format();
		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.DispatchOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

	}
}

