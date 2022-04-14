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
	using static Troops;
	public sealed partial class SendTroops:DialogG,INotifyPropertyChanged
	{
		internal bool viaWater;
		internal bool isSettle;
		protected override string title => $"{(isSettle? "Setting" : Army.typeStrings[(int)type] )} => {target}"; 
		internal static SendTroops? instance;
		internal City city;
		internal City target;
		internal ArmyType type;
	//	internal ServerTime arrival;
	
		
		//internal ServerTime departure;
		internal int armyType { get => (int)type; set => type=(ArmyType)(value); }
		
		internal readonly int[] splitsItems = Enumerable.Range(1,16).ToArray();

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
				troopItems[i] = new(target:target, city:city,type:(byte)i,wantMax:false) ;

			OnPropertyChanged();
		}

		bool isRaid => type==ArmyType.raid; 
		bool isDefense => type==ArmyType.defense && !isSettle;
		int splits => isDefense || isRaid ? splitsCombo.SelectedIndex+1 : 1;

		public static void ShowInstance(City city,City target,bool isSettle,bool viaWater=false, ArmyType type = ArmyType.defense)
		{
			var rv = instance ?? new SendTroops();
			rv.city = city;
			rv.target = target;
			rv.isSettle = isSettle;
			rv.viaWater = viaWater;
			rv.type = type;
			rv.UpdateTroopItems();
			if(isSettle)
				rv.troopItems[Troops.ttMagistra].count = 1;

			rv.raidPanel.Visibility = rv.isRaid ? Visibility.Visible : Visibility.Collapsed;
			rv.OnPropertyChanged();
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
			if( type is ( >= Army.attackFirst and <= Army.attackLast) && (!city.isCastle) )
			{
				if(verbose) AppS.MessageBox($"Only castles can attack like that");
				return false;
			}
			if( (type is (  ArmyType.siege or ArmyType.assault)) &&(!target.isCastle) )
			{
				if(verbose) AppS.MessageBox($"Target must be castle");
				return false;
			}
			// check for water
			if(transport is ( ArmyTransport.land or ArmyTransport.carts) )
			{
				if(troopItems.Any(a => a.count>0 && IsTTNaval(a.type)))
				{
					if(verbose) AppS.MessageBox($"Boats must go by water");
					return false;
				}
				if(city.cont != target.cont)
				{ 
					if(verbose) AppS.MessageBox($"Land travel only to same continent");
					return false;
				}
			}
			else
			{
				// by water
				// check galley space todo
				if(!city.isOnWater || !target.isOnWater)
				{
					if(verbose) AppS.MessageBox($"Source and target must be on water");
					return false;
				}

			}
			if(!troopItems.Any(a => a.count > 0))
			{
				if(verbose) AppS.MessageBox($"Please send something");
				return false;
			}
			if(city.freeCommandSlots < splits)
			{
				if(verbose) AppS.MessageBox($"Out of command slots");
				return false;
			}

			return true;
		}

		//private bool isSettle => (transport == ArmyTransport.carts || transport==ArmyTransport.ports);


		internal SendTroopItem [] troopItems;
		private void ClearClick(object sender,RoutedEventArgs e)
		{
			for(int i = 0;i<Troops.ttCount;++i)
				troopItems[i].count = 0;
			Changed();
		}
		private void MaxClick(object sender,RoutedEventArgs e)
		{
			var t = city.troopsOwned;
			for(var i = Troops.ttZero;i<Troops.ttCount;++i)
				troopItems[i].count = t.GetCount(i);
			foreach(var i in CityStats.instance.recruitQueue)
			{
				troopItems[i.op.t].count += i.op.count; 
			}	
			Changed();
		}
		private void HomeClick(object sender,RoutedEventArgs e)
		{
			var t = city.troopsHome;
			for(var i = Troops.ttZero;i<Troops.ttCount;++i)
				troopItems[i].count = t.GetCount(i);
			Changed();
		}

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
				var splits = this.splits;

				var flags = (byte)(isRaid ? ((repeatCheckBox.IsChecked.Value ? Army.flagRepeating : Army.flagNone)
									)
									| Army.FlagSplits(splits) : Army.flagNone);
				var arrival = ServerTime.CombineDateTime(arrivalDate.SelectedDate,arrivalTime.SelectedTime);

				bool okay;
				if(arrival != default )
					okay =Army.Send(ts,flags,city,target.cid,type,transport,arrival);
				else
					okay = Army.Send(ts,flags,city,target.cid,type,transport);
				if(okay)
					Done();
			}
			else
			{
				AppS.MessageBox("Please select troops to send");
			}
		}

		//private static void ArmySend(TroopTypeCounts ts,byte flags, City city, SpotId targetCid,ArmyType type,ArmyTransport transport)
		//{
		//	using(var __lock = Sim.eventQLock.Enter)
		//	{
		//		var t = Army.FromNow(city,targetCid,transport,type,ts,isReturn: false,
		//			flags: flags);
		//		//	Assert(!t.departed);
		//		//	Assert(t.isSchedueledNotSent);
		//		new CnVEventSendTroops(t).EnqueueAlreadyLocked();
		//		//AppS.MessageBox("Send",info.ToString());
		//		//Note.Show("Send");
		//		//new CnVEventRecruit(city.c,tt).Execute();
		//	}
		//}
		

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		public static void Changed(string? member = null)
		{
			if(instance is not null)
			{
				instance.OnPropertyChanged(member);
				foreach(var i in instance.troopItems)
					i.OnPropertyChanged(member);
			}
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
		public string troopsHome => $"{city.troopsHome.GetCount(type).Format()}/{city.troopsOwned.GetCount(type).Format()}";

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null)
		{
			if (this.PropertyChanged is not null) 
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

	}
}

