using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	using static Troops;
	public sealed partial class SendTroops:DialogG, INotifyPropertyChanged
	{
		internal bool viaWater;
		internal bool isSettle;
		internal Army prior;
		protected override string title => $"{(isSettle ? "Setting" : Army.typeStrings[(int)type])} => {target}";
		internal static SendTroops? instance;
		internal City city;
		internal City target;
		internal ArmyType type;
		//	internal ServerTime arrival;


		//internal ServerTime departure;
		internal int armyType { get => (int)type; set => type=(ArmyType)(value); }

		internal readonly int[] splitsItems = Enumerable.Range(1,16).ToArray();

		internal ArmyTransport transport => isSettle ? (viaWater ? ArmyTransport.ports : ArmyTransport.carts) : (viaWater ? ArmyTransport.water : ArmyTransport.land);
		public SendTroops() {
			this.InitializeComponent();
			instance = this;
			PropertyChanged += SendTroops_PropertyChanged;
			arrival.PropertyChanged += SendTroops_PropertyChanged;

		}

		private void SendTroops_PropertyChanged(object? sender,PropertyChangedEventArgs e) {
		
				var arrival = this.arrival.dateTime;
				var tt = travelTime;
				if(arrival == default) {
					travelInfo.Text= $"Travel time: {tt.Format()}\nArrival: {(Sim.simTime+tt)}";
				}
				else {
					var depart = (arrival-tt);
					var rv= $"Travel time: {tt.Format()}\nDepart: {depart}";
					if(depart < Sim.simTime) {
						rv += "[Can not make it]";
					}
					travelInfo.Text= rv;
				}
			
		
		}

		private void UpdateTroopItems() {
			var troopItems = new List<SendTroopItem>();
			if(prior is not null) {
				foreach(var t in prior.troops) {
					var rv = new SendTroopItem(target: target,city: city,type: t.t,count:t.c);
						troopItems.Add(rv);
						rv.PropertyChanged += SendTroops_PropertyChanged;
				}
			}
			else {
				var ttHome = city.troopsOwnedPlusRecruiting;
				for(var i = (TType)0;i<Troops.ttCount;++i) {
					if(ttHome.GetCount(i) > 0) {
						var rv = new SendTroopItem(target: target,city: city,type: i);
						troopItems.Add(rv);
						rv.PropertyChanged += SendTroops_PropertyChanged;
					}
				}
			}
			this.troopItems = troopItems.ToArray();
		}

		bool isRaid => type==ArmyType.raid;
		bool isCavernRaid => type==ArmyType.raid && target.isDungeon;
		bool isDefense => type==ArmyType.defense && !isSettle;
		int splits => isDefense || isCavernRaid ? splitsCombo.SelectedIndex+1 : 1;

		public static void ShowInstance(City city=null,City target=null,bool isSettle=false,bool viaWater=false,ArmyType type=ArmyType.nop,Army?prior=null) {
			if(city == target && prior is null) {
				AppS.MessageBox("Cannot send to self");
				return;
			}
			var rv = instance ?? new SendTroops();
			rv.prior = prior;
			if(prior is not null) {
				rv.city =prior.sourceCity;
				rv.target = prior.targetCity;
				rv.isSettle = prior.isSettle;
				rv.viaWater = prior.isViaWater;
				rv.type = prior.type;
				rv.arrival.SetDateTime(prior.arrival);
			}
			else {
				rv.city =prior?.sourceCity ?? city;
				rv.target = target;
				rv.isSettle = prior?.isSettle??isSettle;
				rv.viaWater = prior?.isViaWater ?? viaWater;
				rv.type = type;
			}
			rv.UpdateTroopItems();
			if(isSettle)
				rv.troopItems[Troops.ttMagistra].count = 1;

			rv.raidPanel.Visibility = rv.isCavernRaid ? Visibility.Visible : Visibility.Collapsed;
			rv.OnPropertyChanged();
			rv.Show(false);

		}

		internal bool IsValid(TroopTypeCounts troops,bool verbose) {
			if(city.underSiege) {
				if(verbose)
					AppS.MessageBox($"City is under siege");
				return false;
			}
			if(isSettle) {
				if(!troopItems.Any(a => a.type == Troops.ttSenator && a.count > 0)) {
					if(verbose)
						AppS.MessageBox($"Need a Magistra to settle");
					return false;
				}

				if(!Army.CheckSettleResources(city)) {
					return false;
				}
			}
			if(transport == ArmyTransport.carts && city.cartsHome < 250) {
				if(verbose) AppS.MessageBox($"Need 250 carts.  Have {city.cartsHome} home of {city.carts} ");
				return false;
			}
			if(transport == ArmyTransport.ports && city.shipsHome < 25) {
				if(verbose) AppS.MessageBox($"Need 25 trading ships.  Have {city.shipsHome} home of {city.ships} ");
				return false;
			}
			if(type is (>= Army.attackFirst and <= Army.attackLast)) {
				if(!city.isCastle) {
					if(verbose) AppS.MessageBox($"Only castles can attack like that");
					return false;
				}
				if(troops.TS() < city.minTS) {
					if(verbose) AppS.MessageBox($"Cannot send {troops.TS()}, min TS is {city.minTS}");
					return false;

				}
			}
			if((type is (ArmyType.siege or ArmyType.assault)) &&(!target.isCastle)) {
				if(verbose) AppS.MessageBox($"Target must be castle");
				return false;
			}
			// check for water
			if(transport is (ArmyTransport.land or ArmyTransport.carts)) {
				if(troopItems.Any(a => a.count>0 && IsTTNaval(a.type))) {
					if(verbose) AppS.MessageBox($"Boats must go by water");
					return false;
				}
				if(city.cont != target.cont) {
					if(verbose) AppS.MessageBox($"Land travel only to same continent");
					return false;
				}
			}
			else {
				// by water
				// check galley space todo
				if(!city.isOnWater || !target.isOnWater) {
					if(verbose) AppS.MessageBox($"Source and target must be on water");
					return false;
				}

			}
			if(!troopItems.Any(a => a.count > 0)) {
				if(verbose) AppS.MessageBox($"Please send something");
				return false;
			}
			if(city.freeCommandSlots < splits) {
				if(verbose) AppS.MessageBox($"Out of command slots");
				return false;
			}

			return true;
		}

		//private bool isSettle => (transport == ArmyTransport.carts || transport==ArmyTransport.ports);


		internal SendTroopItem[] troopItems;
		private void ClearClick(object sender,RoutedEventArgs e) {
			foreach(var ti in troopItems)
				ti.count = 0;
			Changed();
		}
		private void MaxClick(object sender,RoutedEventArgs e) {
			var t = city.troopsOwnedPlusRecruiting;
			
			foreach(var i in troopItems )
				i.count = t.GetCount(i.type);
			
			Changed();
		}
		private void HomeClick(object sender,RoutedEventArgs e) {
			var t = city.troopsHome;
			foreach(var i in troopItems )
				i.count = t.GetCount(i.type);

			Changed();
		}

		internal TimeSpanS travelTime => Army.JourneyTime(city,target.cid,transport,troops,isRaid);
		

				private void SendTroopsClick(object sender,RoutedEventArgs e) {
			TroopTypeCounts ts = troops;

			if(!IsValid(ts,true))
				return;



			if(ts.Any()) {
				var splits = this.splits;

				var flags = (byte)(isCavernRaid ? ((repeatCheckBox.IsChecked.Value ? Army.flagRepeating : Army.flagNone)
									)
									| Army.FlagSplits(splits) : Army.flagNone);
				var arrival = this.arrival.dateTime;

				bool okay;
				if(arrival != default)
					okay = Army.Send(ts,flags,city,target.cid,type,transport,arrival);
				else
					okay = Army.Send(ts,flags,city,target.cid,type,transport);
				if(okay)
					Done();
			}
			else {
				AppS.MessageBox("Please select troops to send");
			}
		}

		internal TroopTypeCounts troops {
			get {
				var ts = new TroopTypeCounts();
				foreach(var i in troopItems) {
					if(i.count > 0) {
						ts += i.tt;

					}
				}

				return ts;
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
		public void OnPropertyChanged(string? member = null) {
			if(this.PropertyChanged is not null)
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}
		public static void Changed(string? member = null) {
			if(instance is not null) {
				instance.OnPropertyChanged(member);
				foreach(var i in instance.troopItems)
					i.OnPropertyChanged(member);
			}
		}

	}


	internal class SendTroopItem:INotifyPropertyChanged
	{

		internal City city; // convienience
		internal City target;

		internal TType type;
		internal uint _count;
		internal uint count {
			get => _count;
			set {
				if(_count != value) {
					_count = value;
					OnPropertyChanged(nameof(count));
				}
			}
		}

		public SendTroopItem(City city,City target,byte type,bool wantMax=false,uint? count=null) {
			this.city=city;
			this.target=target;
			this.type=type;
			this._count = count ??( wantMax ? city.troopsHome.GetCount(type) : 0u);
		}

		internal TroopTypeCount tt => new(type,_count);
		internal ImageSource image => Troops.Image(type);
		internal TroopInfo info => TroopInfo.all[type];
		internal void MaxClick(object sender,RoutedEventArgs e) {
			count = city.troopsHome.GetCount(type);
			
		}
		public string troopsHome => $"{city.troopsHome.GetCount(type).Format()}/{city.troopsOwnedPlusRecruiting.GetCount(type).Format()}";

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null) {
			if(this.PropertyChanged is not null)
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

	}
}

