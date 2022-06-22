using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	using static Troops;
	public sealed partial class AttackSender:DialogG, INotifyPropertyChanged
	{
		internal bool ViaWater {
			get => viaWater;
			set {
				if(viaWater != value) {
					viaWater = value;
					UpdateTravelTime();
				}
			}
		}
		internal bool viaWater;
		internal bool isSettle;
		internal bool useHorns;
		public bool UseHorns {
			get => useHorns;
			set {
				if(value!=useHorns) {
					useHorns=value;
					UpdateTravelTime();
				}
			}
		}
		public bool WaitReturn {
			get => waitReturn;
			set {
				if(value!=waitReturn) {
					waitReturn=value;
					UpdateTravelTime();
				}
			}
		}
		public ObservableCollection<AttackTargetItem> targets = new();

		internal bool waitReturn;
		internal bool isReturn => prior != null;
		internal Army prior;
		protected override string title => $"{(isReturn ? "Return " : String.Empty)}{(isSettle ? "Settle" : target.isBoss ? "NPC Hit" : Army.typeStrings[(int)type])} => {target}";
		internal static AttackSender? instance;
		internal City city;
		internal City target;
		internal ArmyType type;
		internal ServerTime arrival {
			get {
				var a = this.arrivalUI.dateTime;
				if(a.isNotZero)
					return a;
				return departure + travelTimeWithHorms;
			}
		}


		internal ServerTime departure {
			get {
				var arrival = this.arrivalUI.dateTime;
				if(waitReturn ) {
					var _departure = city.WhenWillEnoughTroopsReturnToSend(troops);
					// arrival takes precedence
					if(arrival.isNotZero) {
						return _departure.Max(arrival - travelTimeWithHorms).Max(Sim.simTime);

					}
					return  _departure.Max(Sim.simTime);
				}
				
				
				if(arrival.isZero) {
					return Sim.simTime;
				}
				return (arrival - travelTimeWithHorms).Max(Sim.simTime);

			}
		}
		internal int armyType { get => isAttack ? (int)type - Army.armyTypeAttackStart : 0; set {
				if(isAttack) 
					type=(ArmyType)(value+Army.armyTypeAttackStart);
			}

		}

		internal readonly int[] splitsItems = Enumerable.Range(1,16).ToArray();

		internal ArmyTransport transport => isSettle ? (viaWater ? ArmyTransport.ports : ArmyTransport.carts) : (viaWater ? ArmyTransport.water : ArmyTransport.land);
		public AttackSender() {
			this.InitializeComponent();
			instance = this;
			PropertyChanged += SendTroops_PropertyChanged;
			arrivalUI.PropertyChanged += SendTroops_PropertyChanged;

		}

		private void SendTroops_PropertyChanged(object? sender,PropertyChangedEventArgs e) {
			UpdateTravelTime();
			OnPropertyChanged();

		}

		private void UpdateTravelTime() {
			var depart = departure;

			var rv =  $"Travel time: {travelTimeWithHorms.Format()}\nDepart: {depart}\nArrival: {(arrival)}";
			if(depart+10 < Sim.simTime) {
				rv += " (depart is past)";
			}
			travelInfo.Text= rv;
		}

		private void UpdateTroopItems(TroopTypeCounts? troops) {
			var troopItems = new List<SendTroopItem>();
			if(troops is not null) {
				foreach(var t in troops.Value) {
					var rv = new SendTroopItem(target: target,city: city,type: t.t,count: (int)t.c,prior: prior);
					troopItems.Add(rv);
					rv.PropertyChanged += SendTroops_PropertyChanged; // memory leak
				}
			}
			else {
				var ttHome = city.troopsOwnedPlusRecruiting;
				for(var i = (TType)0;i<Troops.ttCount;++i) {
					if(ttHome.GetCount(i) > 0) {
						var rv = new SendTroopItem(target: target,city: city,type: i,prior: prior);
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
		int splits => isCavernRaid ? splitsCombo.SelectedIndex+1 : 1;
		internal bool isAttack => type.IsAttack();// is (>=Army.attackFirst and <= Army.attackLast);
		public static async Task<bool> ShowInstance(City city = null,City target = null,bool isSettle = false,bool viaWater = false,ArmyType type = ArmyType.nop,Army? prior = null,TroopTypeCounts? troops = null,ServerTime arrival = default, bool ? useHorns=null, bool? waitReturn=null) {
			try {
				if(city == target && prior is null) {
					AppS.MessageBox("Cannot send to self");
					return (false);
				}
				if(isSettle) {
					Assert(type == ArmyType.defense);
				}
				var isAttack = type.IsAttack();// is (>=Army.attackFirst and <= Army.attackLast);
				if(prior != null) {
					target = prior.targetCity;
				}
				else {
					if(isAttack && city.IsAlly(target) ) {
						if(await AppS.DoYesNoBox("Attack ally", "Are you sure that you want to send an attack at an ally?") != 1) {

							return false;
						}
					}
				}

				var rv = instance ?? new AttackSender();
				rv.prior = prior;
				rv.armyTypeCombo.Visibility = Visibility.Collapsed;
				if((type == ArmyType.defense && !isSettle)||((prior is not null&&prior.isDefense))) {
					if(useHorns is not null)
						rv.useHorns = useHorns.Value;
					rv.useHornsCheckbox.Visibility = Visibility.Visible;

				}
				else {
					rv.useHorns=false;
					rv.useHornsCheckbox.Visibility = Visibility.Collapsed;

				}
				if((type == ArmyType.defense||isAttack) && !isSettle) {
				
					if(waitReturn is not null)
						rv.waitReturn = waitReturn.Value;
					rv.waitReturnCheckbox.Visibility = Visibility.Visible;

				}
				else {

					rv.waitReturn=false;
					rv.waitReturnCheckbox.Visibility = Visibility.Collapsed;
				}
				//rv._departure = depart;
				rv.notSameAlliance.IsChecked=true;
				rv.notSameAlliance.Visibility = Visibility.Collapsed;
			

				if(prior is not null) {
					rv.arrivalUI.Visibility = Visibility.Visible; // reset
					rv.city =prior.sourceCity;
					rv.target = prior.targetCity;
					rv.isSettle = prior.isSettle;
					rv.viaWater = prior.isViaWater;
					rv.type = prior.type;
					rv.UpdateTroopItems(prior.troops);
					rv.arrivalUI.Clear();
//					rv.arrivalUI.SetDateTime(prior.arrival);
					//rv.arrival.Visibility = !wantReturn ? Visibility.Visible : Visibility.Collapsed;
					rv.buttoGo.Content = "Return";
					rv.buttoGo.IsEnabled = prior.sourceCity.outgoing.Contains(prior);
					//	rv.isReturn = wantReturn;
				
					
				}
				else {
					//	Assert(wantReturn == false);

					var isRaid = type == ArmyType.raid;
					rv.arrivalUI.Visibility = isRaid||isSettle ? Visibility.Collapsed : Visibility.Visible;
					rv.city =city;
					rv.target = target;
					rv.isSettle = isSettle;
					rv.viaWater = viaWater;
					rv.UpdateTroopItems(troops);
					
					rv.arrivalUI.Clear();
					if(isAttack && rv.type is (>=Army.attackFirst and <= Army.attackLast)) {
						// leave it
					}
					else {
						if(rv.type != type) {
							rv.type = type;
							//	rv.arrival.Clear();
						}
					}




					if(isRaid||isSettle) {
						//					rv.arrival.Clear();
					}
					else if(arrival.isNotZero)
						rv.arrivalUI.SetDateTime(arrival);
					//				else if(depart.isNotZero)
					//					rv.arrivalUI.SetDateTime(depart + rv.travelTimeWithHorms);

					if(isAttack) {
						rv.armyTypeCombo.Visibility = Visibility.Visible;
						rv.notSameAlliance.Visibility = Visibility.Visible;
					}

					rv.buttoGo.Content = "Send";
					rv.buttoGo.IsEnabled=true;


				}
				


				if(isSettle) {
					foreach(var i in rv.troopItems) {
						if(i.type ==ttMagistra)
							i.count = 1;
					}
				}
				rv.raidPanel.Visibility = rv.isCavernRaid ? Visibility.Visible : Visibility.Collapsed;
				rv.OnPropertyChanged();
				return await rv.Show(false);
			}
			catch(Exception _ex) {
				LogEx(_ex);
				return false;
			}

		}

		internal async Task<(bool okay, int usedHorns)> IsValid(TroopTypeCounts troops,bool verbose) {
			var usedHorns = 0;
			if(!isReturn) {
				if(isSettle) {
					if(!troopItems.Any(a => a.type == Troops.ttSenator && a.count > 0)) {
						if(verbose)
							AppS.MessageBox($"Need a Magistra to settle");
						return (false, 0);
					}

					if(!Army.CheckSettleResources(city)) {
						return (false, 0);
					}
				}

				if(city.underSiege ) {
					if(verbose)
						AppS.MessageBox($"City is under siege");
					return (false, 0);
				}
				if(city.isOutOfFood ) {
					if(verbose)
						AppS.MessageBox($"Troops will not leave without food");
					return (false, 0);
				}
				if(transport == ArmyTransport.carts && city.cartsHome < 250) {
					if(verbose) AppS.MessageBox($"Need 250 carts.  Have {city.cartsHome} home of {city.carts} ");
					return (false, 0);
				}
				if(transport == ArmyTransport.ports && city.shipsHome < 25) {
					if(verbose) AppS.MessageBox($"Need 25 trading ships.  Have {city.shipsHome} home of {city.ships} ");
					return (false, 0);
				}
				if(type is (>= Army.attackFirst and <= Army.attackLast)) {
					if(!city.isCastle) {
						if(verbose) AppS.MessageBox($"Only castles can attack like that");
						return (false, 0);
					}
					if(troops.TS() < city.minTS) {
						if(verbose) AppS.MessageBox($"Cannot send {troops.TS()}, min TS is {city.minTS}");
						return (false, 0);

					}
				}

				if(type == ArmyType.scout) {
					if(troops.GetCount(ttScout) < 0) {
						if(verbose) AppS.MessageBox($"Scouting requires scouts");
						return (false, 0);
					}
					if(troops.Any( t=> t.t is not( ttScout or ttGalley) ))
					{
						if(verbose) AppS.MessageBox($"Some of the troops selected cannot tag along with the scouts");
						return (false, 0);
					}

				}

				if((type is (ArmyType.siege or ArmyType.assault)) &&(!target.isCastle)) {
					if(verbose) AppS.MessageBox($"Target must be castle");
					return (false, 0);
				}
				// check for water
				if(transport is (ArmyTransport.land or ArmyTransport.carts)) {
					if((troopItems.Any(a => a.count>0 && IsTTNaval(a.type))) &&!target.isOnWater) {
						if(verbose) AppS.MessageBox($"Boats must go by water");
						return (false, 0);
					}
					if(city.cont != target.cont) {
						if(verbose) AppS.MessageBox($"Land travel only to same continent");
						return (false, 0);
					}
				}
				else {
					// by water

					if(!city.isOnWater || !target.isOnWater) {
						if(verbose) AppS.MessageBox($"Source and target must be on water");
						return (false, 0);
					}
					if(transport is ArmyTransport.water) {
						Assert(!isSettle);
						// check galley space
						var galleys = troops.GetCount(ttGalley);
						var tsLand = (int)troops.Where(t => !t.isNaval).Sum(tt => tt.ts);
						var neededGalleys = tsLand.DivideRoundUp(tsCarryPerGalley);
						if(neededGalleys > galleys) {
							if(verbose) AppS.MessageBox($"Need at least {neededGalleys} galleys to transport troops");
							return (false, 0);
						}
					}
					else {
						Assert(isSettle);
					}
				}
			}
				// need wings?
				var arrivalTime = arrivalUI.dateTime;
			if(arrivalTime != default) {
				var travelTime = this.travelTime;
				Assert(travelTime > 0);
				var canMakeIt = departure + travelTime <= arrivalTime;
				if(!canMakeIt) {
					if(useHorns) {
						var timeAvailable = (arrivalTime -departure);
						if(timeAvailable > 0) {
							var speedupNeeded = (double)travelTime/(double)timeAvailable;
							Assert(speedupNeeded >= 1);
							if(speedupNeeded <= 2.125f) {
								canMakeIt = true;


								//		1 + horns*tsPerHorn /ts  = speedupNeeded 
								//  horns*tsPerHorn /ts  = ts * (speedupNeeded-1)/tsPerHorn
								var ts = (int)troops.TS(); // TODO: 
								var art = Artifact.GetForPlayerRank(Artifact.ArtifactType.Horn);
								var tsPerHorn = art.r[13];
								usedHorns = ((float)(ts *(speedupNeeded.Min(2.0f)-1)/tsPerHorn)).CeilToInt(); ;
								Note.Show($"{usedHorns} required for {ts} TS");

							}
						}
					}
				}
				if(!canMakeIt) {
					if(await AppS.DoYesNoBox("Arrival too soon",$"The earliest time that we can make is {departure + (useHorns.Switch(1.0f,0.5f)*travelTime).CeilToInt()}, send now?",no: string.Empty) != 1)
						return (false, 0);
					arrivalUI.Clear(); ;
					arrivalTime = default;
				}
			}
			else {
				if(useHorns) {
					var ts = (int)troops.TS(); // TODO: 
					var art = Artifact.GetForPlayerRank(Artifact.ArtifactType.Horn);
					var tsPerHorn = art.r[13];
					usedHorns = ts.DivideRoundUp(tsPerHorn);
					Note.Show($"{usedHorns} required for {ts} TS");
				}
			}
				if(arrivalTime == default && !waitReturn && (prior==null) ) {
					// check for enough troops
					if(!city.troopsHome.IsSuperSetOf(troops)) {
						AppS.MessageBox($"Not enough to send {troops}\n({city.troopsHome.Format()} here)");
						return (false, 0);
					}
				}
				if(!troopItems.Any(a => a.count > 0)&& (prior==null)) {
					if(verbose) AppS.MessageBox($"Please send something");
					return (false, 0);
				}
			

			if(city.freeCommandSlots < splits) {
				if(verbose) AppS.MessageBox($"Out of command slots");
				return (false, 0);
			}



			return (true, usedHorns);
		}

		//private bool isSettle => (transport == ArmyTransport.carts || transport==ArmyTransport.ports);


		internal SendTroopItem[] troopItems;
		private void ClearClick(object sender,RoutedEventArgs e) {
			foreach(var ti in troopItems)
				ti.count = 0;
			Changed();
		}
		private void MaxClick(object sender,RoutedEventArgs e) {
			var t = prior?.troops?? city.troopsOwned;

			foreach(var i in troopItems)
				i.count = (int)t.GetCount(i.type);

			Changed();
		}

		//private void ClampToGalleys() {
		//	if(prior is not null)
		//		return;
		//	var troops = this.troops;
		//	var galleys = troops.GetCount(ttGalley);
		//	if(galleys > 0 ){
		//		var tsCarry = tsCarryPerGalley*galleys;
		//		var tsLand = troops.Where(tt => !tt.isNaval).Sum(tt => tt.ts);
		//		if(tsLand > tsCarry ) {
		//			var ratio = tsCarry / (double) tsLand;
		//		}
		//	}
		//}
		internal string AttackStrength() {
			return $"Attack Strength:\n{troops.AttackStrength(city.player,target.isDungeonOrBoss ? Cavern.Get(target.c).combatWeakness : CombatCategory.invalid):N0}";
		}
		internal string RaidCarry() {
			return $"Carry: {troops.carry/(float)Cavern.Get(target.c).GetLoot((uint)splits,troops.carry):P}";
		}

		private void HomeClick(object sender,RoutedEventArgs e) {
			var t = prior?.troops?? city.troopsHome;
			foreach(var i in troopItems)
				i.count = (int)t.GetCount(i.type);

			Changed();
		}

		internal TimeSpanS travelTime => Army.JourneyTime(city,target.cid,transport,troops,isCavernRaid);
		internal TimeSpanS travelTimeWithHorms => new((travelTime*useHorns.Switch(1.0f,0.5f)).CeilToInt());


		private async void SendTroopsClick(object sender,RoutedEventArgs e) {
			//if( prior is not null ) {
			//	if(prior.sourceCity.outgoing.Contains(prior))
			//	{
			//		CnVEventReturnTroops.TryReturn(prior);
			//	}
			//	else {
			//		AppS.MessageBox("Cannot be returned this way.");
			//	}
			//	Done();
			//	return;
			//}
			TroopTypeCounts ts = troops;

			var valid = await IsValid(ts,true);
			if(!valid.okay)
				return;

			var usedHorns = valid.usedHorns;
				if(usedHorns > 0) {
					if(await AppS.DoYesNoBox($"Use {usedHorns} Horns?",$"This will require {usedHorns} horns, are you sure?") != 1)
						return;
					// can addord it?
					var art = Artifact.GetForPlayerRank(Artifact.ArtifactType.Horn);

					var artifact = art.id;

					var needed = usedHorns- Player.active.ArtifactCount(artifact);
					if(!Artifact.Get(artifact).IsOkayToUse(usedHorns))
						return;
					try {
						SocketClient.DeferSendStart();

						if(needed > 0) {
							new CnVEventPurchaseArtifacts((ushort)artifact,(ushort)needed,Player.active.id).EnqueueAsap();
						}
						if(!AppS.isTest)
							(new CnVEventUseArtifacts(city.c) { artifactId = (ushort)artifact,count = (ushort)usedHorns,flags=CnVEventUseArtifacts.Flags.noEffect }).EnqueueAsap();

					}
					catch(Exception _ex) {
						LogEx(_ex);

					}
					finally {
						SocketClient.DeferSendEnd();
					}
				}

			Assert(ts.Any());


			bool okay;


			if(isReturn) {
				CnVEventReturnTroops.TryReturn(prior,ts.isEmpty || ts.IsSuperSetOf(prior.troops) ? default : ts, useHorns);
				okay = true;
			}
			else {
				

				var splits = this.splits;

				var flags = (byte)(isCavernRaid ? repeatCheckBox.IsChecked.Value.Switch(Army.flagNone,Army.flagRepeating)| Army.FlagSplits(splits) :
												Army.flagNone);
				if(notSameAlliance.IsChecked.Value)
					flags|= Army.flagNotSameAlliance;
				//var arrival = this.arrivalUI.dateTime;
				//if(arrival == default)
				//	arrival = departure + travelTimeWithHorms;

				okay =  Army.Send(ts,flags,city,target.cid,type,transport,arrival,departure);

			}
			if(okay)
				Done();
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
	public class AttackTargetItem
	{
		public AttackTargetItem() { }
		public City city { get; set; }


	}
	
}

