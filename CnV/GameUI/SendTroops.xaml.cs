using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using System.Diagnostics.Metrics;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	using static Troops;
	public sealed partial class SendTroops:DialogG, INotifyPropertyChanged
	{
		internal bool? ViaWater {
			get => _viaWater;
			set {
				if(_viaWater != value) {
					_viaWater = value;
					OnPropertyChanged();
				}
			}
		}
		 bool? _viaWater;
		internal bool sendViaWater {
			 get {
					if(prior is not null)
						return prior.isViaWater;
					if(_viaWater is not null)
						return _viaWater.Value;
					if(city.isOnWater && target.isOnWater && (troops.Any(a => a.isNaval) )||(isSettle && city.cont != target.cont) ) {
						return true;
					}
					return false;
				}
			
		}
		internal bool isSettle;
		bool _useHorns;
		public bool UseHorns {
			get => ((isDefense)||(prior is not null&&(prior.isDefense||prior.isRaid))) ?  _useHorns : false;
			set {
				if(value!=_useHorns) {
					_useHorns=value;
					OnPropertyChanged();
				}
			}
		}
		public bool WaitReturn {
			get => _waitReturn;
			set {
				if(value!=waitReturn) {
					_waitReturn=value;
					OnPropertyChanged();
				}
			}
		}
		bool returnAllRaids;
		public bool ReturnAllRaids {
			get => returnAllRaids & isRaid & isReturn;
			set {
				if(value!=returnAllRaids) {
					returnAllRaids=value;
					OnPropertyChanged();
				}
			}
		}
		private bool _waitReturn;
		internal bool waitReturn => isReturn ? false : timingSetting switch 	{
			TimingSetting.onReturn=>true,
			TimingSetting.arrival => _waitReturn,
			_=>false
		};

		internal bool isReturn => prior != null;
		internal Army prior;
		protected override string title => $"{(isReturn ? "Return " : String.Empty)}{(isSettle ? "Settle" : target.isBoss ? "NPC Hit" : Army.typeStrings[(int)type])} => {Cavern.Get(target.cid).Format(' ', true) }";
		internal static SendTroops? instance;
		internal City city;
		internal City target;
		internal ArmyType type;

		TimingSetting timingSetting;// => (TimingSetting)timingSelection;

		internal ServerTime arrival {
			get {
				switch(timingSetting) {
				
					case TimingSetting.arrival: {
							var rv = arrivalUI.dateTime;
							if(rv.isNotZero)
								return rv;
							break;

						}

				}
				return departure + travelTimeWithHorms;
			}
		
		}
		internal ServerTime arrivalOrBest => arrival.Max(Sim.simTime+travelTimeWithHorms);
		

		internal ServerTime departure {
			get {
				switch(timingSetting) {
//					case TimingSetting.now: return Sim.simTime;
					case TimingSetting.onReturn: return city.WhenWillEnoughTroopsReturnToSend(troops);
					case TimingSetting.departure: {
							var rv = departureUI.dateTime;
							if(rv.isNotZero)
								return rv;
							break;
						}
					case TimingSetting.arrival: {
							var rv = arrivalUI.dateTime;
							var ready=  city.WhenWillEnoughTroopsReturnToSend(troops);
							if(rv.isNotZero) {
								rv -= travelTimeWithHorms;
								if(waitReturn)
									return ready.Max(rv);
								else
									return rv;
							}
							else  if(waitReturn)
								return ready;

							break;

						}

				}
				return Sim.simTime;
			}
		}
		internal ServerTime departureOrBest => departure.Max(Sim.simTime);


		internal int uiArmyType { get => isAttack ? (int)type - Army.armyTypeAttackStart : 0; set {
				if(isAttack) 
					type=(ArmyType)(value+Army.armyTypeAttackStart);
			}

		}

		internal readonly int[] splitsItems = Enumerable.Range(1,16).ToArray();

		internal ArmyTransport transport => isSettle ? (sendViaWater ? ArmyTransport.ports : ArmyTransport.carts) : (sendViaWater ? ArmyTransport.water : ArmyTransport.land);
		public SendTroops() {
			this.InitializeComponent();
			instance = this;
		//	arrivalUI.PropertyChanged += PropagatePropertyChanged;

		}

	
		private void PropagatePropertyChanged(object? sender,PropertyChangedEventArgs e) {
			PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(null));
			//PropertyChanged?.Invoke(this,e);

		}

		private string travelInfoS { get {
				var depart = departure;
				string rv;
				if(isReturn) {
					rv = $" Travel time: {travelTimeWithHorms.Format()}\n{(isRaid? "Stop raiding at" : isDefense ? "Leave at" : "Stop sieging at")}: {depart}\nHome by: {(arrival)}";
					if(prior.returnTimeIfAborted >= arrival ) {
						rv += " (cannot be home in time)";
					}
				}
				else {
					rv = $" Travel time: {travelTimeWithHorms.Format()}\nDepart: {depart}\nArrival: {(arrival)}";
					if(depart+10 < Sim.simTime) {
						rv += " (depart is past)";
					}
				}
				return rv;
			}
		}

		private void UpdateTroopItems(TroopTypeCounts? troops) {
			var troopItems = new List<SendTroopItem>();
			if(troops is not null) {
				foreach(var t in troops.Value) {
					var rv = new SendTroopItem(target: target,city: city,dialog:this,type: t.t,count: (int)t.c,prior: prior);
					troopItems.Add(rv);
					rv.PropertyChanged += PropagatePropertyChanged; // memory leak
				}
			}
			else {
				var ttHome = city.troopsOwnedPlusRecruiting;
				for(var i = (TType)0;i<Troops.ttCount;++i) {
					if(ttHome.GetCount(i) > 0) {
						var rv = new SendTroopItem(target: target,city: city,dialog:this,type: i,prior: prior);
						troopItems.Add(rv);
						rv.PropertyChanged += PropagatePropertyChanged;
					}
				}
			}
			this.troopItems = troopItems.ToArray();
		}
		int timingSelection {
			get => (int)timingSetting;
		   set {
				if( (int)timingSetting != value && value >= 0 ) {
					timingSetting = (TimingSetting)value;
					OnPropertyChanged();
				}
			}
		}
		bool isRaid => type==ArmyType.raid;
		bool isCavernRaid => type==ArmyType.raid && target.isDungeon;
		bool isDefense => type==ArmyType.defense && !isSettle;
		int splits => isCavernRaid ? splitsCombo.SelectedIndex+1 : 1;
		internal bool isAttack => type.IsAttack();// is (>=Army.attackFirst and <= Army.attackLast);
		public static async Task<bool> ShowInstance(City city = null,City target = null,bool isSettle = false,bool ? _viaWater=null,ArmyType type = ArmyType.nop,Army? prior = null,TroopTypeCounts? troops = null,
			TimingSetting? timing=null,
			ServerTime arrival = default, bool ? useHorns=null, bool? waitReturn=null, bool ? notSameAlliance=null,
			bool? returnAllRaids= null) {
			try {
				if(city == target && prior is null) {
					AppS.MessageBox("Cannot send to self");
					return (false);
				}
				if(isSettle) {
					Assert(type == ArmyType.defense);
				}
				if(prior != null) {
					target = prior.targetCity;
					type = prior.type;
					isSettle = prior.isSettle;
				}
				var isAttack = type.IsAttack();// is (>=Army.attackFirst and <= Army.attackLast);

				if(prior is null) {
					if(isAttack && city.IsAlly(target)) {
						if(await AppS.DoYesNoBox("Attack ally","Are you sure that you want to send an attack at an ally?") != 1) {

							return false;
						}
					}
				}

				var rv = instance ?? new SendTroops();
				if(returnAllRaids is not null)
					rv.returnAllRaids = returnAllRaids.Value;
			//	rv.troopList.Visibility = prior is not null && prior.isRaid  ? Visibility.Collapsed : Visibility.Visible; 
				//				rv.arrivalUI.ResetRecentTimesComboBox();
				if(timing is not null)
					rv.timingSetting = timing.Value;
				if(_viaWater is not null) {
					rv._viaWater = _viaWater;
				}
				var isReturn = prior is not null;
				rv.prior = prior;
				

				rv.armyTypeCombo.Visibility = Visibility.Collapsed;
				var isDefense = type == ArmyType.defense&& !isSettle;
				if((isDefense)||((prior is not null&&(prior.isDefense||prior.isRaid)))) {
					if(useHorns is not null)
						rv._useHorns = useHorns.Value;
					rv.useHornsCheckbox.Visibility = Visibility.Visible;

				}
				else {
	//				rv.useHorns=false;
					rv.useHornsCheckbox.Visibility = Visibility.Collapsed;

				}
				if(waitReturn is not null)
					rv._waitReturn = waitReturn.Value;
		//		if((type == ArmyType.defense||isAttack) && !isSettle) {

		////			rv.waitReturnCheckbox.Visibility = Visibility.Visible;

		//		}
		//		else {

		//			rv.waitReturn=false;
		//			rv.waitReturnCheckbox.Visibility = Visibility.Collapsed;
		//		}
				//rv._departure = depart;
				if(notSameAlliance is not null)
					rv.notSameAlliance.IsChecked=notSameAlliance.Value;
				rv.notSameAlliance.Visibility = Visibility.Collapsed;
				var isRaid = type == ArmyType.raid;
				if(isRaid) {
					rv.useHornsCheckbox.Content = "Quadriga";
					ToolTipService.SetToolTip(rv.useHornsCheckbox,"If set, Quadriga will be used to speed up raid return if needed");
				}
				else {
					rv.useHornsCheckbox.Content = "Horns";
					ToolTipService.SetToolTip(rv.useHornsCheckbox,"If set, horns will be used to speed up defense travel as needed");
				}

				if(prior is not null) {

					//rv.arrivalUI.Visibility = Visibility.Visible; // reset
					rv.city =prior.sourceCity;
					rv.target = prior.targetCity;
					rv.isSettle = prior.isSettle;
				//	rv._viaWater = prior.isViaWater;
					rv.type = prior.type;
					rv.UpdateTroopItems(prior.troops);
				//	rv.arrivalUI.Clear();
//					rv.arrivalUI.SetDateTime(prior.arrival);
					//rv.arrival.Visibility = !wantReturn ? Visibility.Visible : Visibility.Collapsed;
					rv.buttoGo.Content = "Return";
					rv.buttoGo.IsEnabled = prior.sourceCity.outgoing.Contains(prior);
					//	rv.isReturn = wantReturn;
					Assert(rv.isReturn);
					//rv.timingArrivalTimed.IsEnabled = true;
					//rv.timingDepartureTimed.Visibility = Visibility.Visible;
					//rv.timingNow.Visibility = Visibility.Visible;
					//rv.timingOnReturn.Visibility = Visibility.Collapsed;

					
				}
				else {
					//	Assert(wantReturn == false);

		//			rv.timingArrivalTimed.Visibility = (isAttack | isDefense) ? Visibility.Visible :  Visibility.Collapsed;
		//			rv.timingDepartureTimed.Visibility = (isAttack | isDefense) ? Visibility.Visible :  Visibility.Collapsed;
		//			rv.timingNow.Visibility = Visibility.Visible;
		//			rv.timingOnReturn.Visibility = Visibility.Visible;

				//	rv.arrivalUI.Visibility = isRaid||isSettle ? Visibility.Collapsed : Visibility.Visible;
					rv.city =city;
					rv.target = target;
					rv.isSettle = isSettle;
					//rv.viaWater = viaWater;
					rv.UpdateTroopItems(troops);
					
				//	rv.arrivalUI.Clear();
					if(isAttack && rv.type is (>=Army.attackFirst and <= Army.attackLast)) {
						// leave it
						rv.type = type;
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
				var isRaidReturn = isRaid && isReturn;
				var isNormalRaid = isRaid && !isReturn;
		//		rv.troopList.Visibility =isRaidReturn ? Visibility.Collapsed : Visibility.Visible;
			//	rv.troopString.Visibily = isRaidReturn ? Visibility.Visible : Visibility.Collapsed;
				rv.raidPanel.Visibility = (rv.isCavernRaid && !isReturn).AsVisability();

				rv.departureWaitReturn.Visibility = rv.arrivalWaitReturn.Visibility = (!isReturn).AsVisability();
				rv.timingNow.Header = isReturn ? "Return Now" : "Now";
				rv.timingOnReturn.Header = isReturn ?  string.Empty : "Wait troops";

				rv.timingDepartureTimed.Header = isNormalRaid|| rv.isSettle ? string.Empty : isReturn ? "Stop by" : "Depart at"; 
				rv.timingArrivalTimed.Header =isNormalRaid || rv.isSettle ?  string.Empty : isReturn ? "Home by" : "Arrive at";

				if((uint)rv.timingSelection < rv.timingPivot.Items.Count && (rv.timingPivot.Items[rv.timingSelection] as PivotItem).Header.ToString().IsNullOrEmpty())
					rv.timingSelection=0;
				rv.returnAllRaidsCheckBox.Visibility = isRaidReturn.AsVisability();
				rv.OnPropertyChanged();
				var result = await rv.Show(false);
				rv.troopItems = Array.Empty<SendTroopItem>();
				return result;
			}
			catch(Exception _ex) {
				LogEx(_ex);
				return false;
			}

		}

		internal (bool okay, int usedHorns) IsValid(TroopTypeCounts troops,bool verbose) {
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
			var arrivalTimeUI = this.arrivalUI.dateTime;
			
			if(timingSetting == TimingSetting.arrival  && arrivalTimeUI.isNotZero ) {
				var travelTime = this.travelTime;
				Assert(travelTime > 0);
				var depart = waitReturn ? city.WhenWillEnoughTroopsReturnToSend(troops) : Sim.simTime;
				
				// for returns, this is when we are back home, for attacks this is when we arrive
				var bestTimeNoHorns = prior is not null ? prior.returnTimeIfAborted : depart + travelTime;
				var bestTimeHorns = prior is not null ? bestTimeNoHorns : depart + travelTime*0.5f;
					


				var canMakeItWithoutHorns = bestTimeNoHorns <= arrivalTimeUI;
				var canMakeItWitHorns = bestTimeHorns <= arrivalTimeUI;
				var willMakeIt = canMakeItWithoutHorns;
				if(!canMakeItWithoutHorns) {
					if(UseHorns) {
						var timeAvailable = (arrivalTimeUI -depart);
						if(timeAvailable > 0) {
							var speedupNeeded = (double)travelTime/(double)timeAvailable;
							Assert(speedupNeeded >= 1);
							if(speedupNeeded <= 2.125f) {
								willMakeIt = true;


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
				if(!willMakeIt) {
					AppS.MessageBox("Arrival too soon",$"The earliest time that we can make is {departure + (UseHorns.Switch(1.0f,0.5f)*travelTime).CeilToInt()}, departing at {departure}");
					return (false, 0);
					
				}
			}
			else {
				if(UseHorns) {
					var ts = (int)troops.TS(); // TODO: 
					var art = Artifact.GetForPlayerRank(Artifact.ArtifactType.Horn);
					var tsPerHorn = art.r[13];
					usedHorns = ts.DivideRoundUp(tsPerHorn);
					Note.Show($"{usedHorns} required for {ts} TS");
				}
			}
				if(departure <= Sim.simTime + 10 && (prior==null) ) {
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

			if(!isReturn) {
				if(city.freeCommandSlots < splits) {
					if(verbose) AppS.MessageBox($"Out of command slots");
					return (false, 0);
				}
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
		internal TimeSpanS travelTimeWithHorms => new((travelTime*UseHorns.Switch(1.0f,0.5f)).CeilToInt());


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

			var valid =  IsValid(ts,true);
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
				var depart = departure;
				CnVEventReturnTroops.TryReturn(prior,ts.isEmpty || ts.IsSuperSetOf(prior.troops) || (isRaid && isReturn) ? default : ts,
					useHorns: UseHorns,
					returnBy:(depart > Sim.simTime + 10) ? arrival : default ,allRaids: ReturnAllRaids  );
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

				okay =  Army.Send(ts,flags,city,target.cid,type,transport,arrivalOrBest,departureOrBest);

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

		private void splitsChanged(object sender,SelectionChangedEventArgs e) {
			OnPropertyChanged();
		}
		internal bool canEditTroops => !(isRaid&&isReturn);
		internal Visibility editTroopsVisibility => canEditTroops.AsVisability();
		internal Visibility isAttackVisibility => isAttack.AsVisability();
		internal Visibility isAttackOrRaidVisibility => (isAttack||isRaid).AsVisability();
	}


	internal class SendTroopItem:INotifyPropertyChanged
	{
		internal SendTroops dialog;
		internal City city; // convienience
		internal City target;
		internal Army prior;
		internal TType type;
		internal int _count;
		internal int count {
			get => _count;
			set {
				if(_count != value && value >=0) {
					_count = value;
					OnPropertyChanged();
				}
			}
		}

		public SendTroopItem(City city,City target, SendTroops dialog,byte type,bool wantMax = false,int? count = null,Army? prior = null) {
			this.city=city;
			this.target=target;
			this.type=type;
			this.prior = prior;
			this._count = count ??(wantMax ? (int)city.troopsHome.GetCount(type) : 0);
			this.dialog =dialog;
		}

		internal TroopTypeCount tt => new(type,(uint)_count.Max(0));
		internal ImageSource image => Troops.Image(type);
		internal TroopInfo info => TroopInfo.all[type];
		internal void MaxClick(object sender,RoutedEventArgs e) {
			count = (int)(prior?.troops ?? city.troopsHome).GetCount(type);

		}
		public string troopsHome => $"{(prior?.troops ?? city.troopsHome).GetCount(type).Format()}/{city.troopsOwned.GetCount(type).Format()}";

		public event PropertyChangedEventHandler? PropertyChanged;
		public void OnPropertyChanged(string? member = null) {
			if(this.PropertyChanged is not null)
				AppS.QueueOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
		}

	}
		public enum TimingSetting {
			now,
			onReturn,
			departure,
			arrival
		}
}

