using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;

using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV;

using static Troops;
public sealed partial class AttackSender:DialogG, INotifyPropertyChanged
{
	internal bool ViaWater {
		get => viaWater;
		set {
			if(viaWater != value) {
				viaWater = value;
				OnPropertyChanged();
			}
		}
	}
	internal bool viaWater;

	internal ArmyType realType = ArmyType.siege;
	internal ArmyType fakeType = ArmyType.siege;

	public bool WaitReturn {
		get => waitReturn;
		set {
			if(value!=waitReturn) {
				waitReturn=value;
				OnPropertyChanged();
			}
		}
	}
	public ObservableCollection<AttackTargetItem> targets = new();
	public ObservableCollection<AttackTroopItem> sendInfo = new();

	internal bool waitReturn = true;
	protected override string title => "Attack Sender";
	internal static AttackSender? instance;
	internal City city;
	internal City cityUI {
		get => city;
		set {
			if(city != value) {
				city = value;
				UpdateTroopItems();
			}
		}
	}
	internal ServerTime arrival {
		get {
			var a = this.arrivalUI.dateTime;
			if(a.isNotZero)
				return a;
			return departure + travelTime;
		}
	}


	internal ServerTime departure {
		get {
			var arrival = this.arrivalUI.dateTime;
			if(waitReturn) {
				var _departure = city.WhenWillTroopsOfTypesReturn(sendInfo.Where(a => a.useForReal|a.useForFakes).Select(a => a.type).ToArray());
				// arrival takes precedence
				if(arrival.isNotZero) {
					return _departure.Max(arrival - travelTime).Max(Sim.simTime);

				}
				return _departure.Max(Sim.simTime);
			}


			if(arrival.isZero) {
				return Sim.simTime;
			}
			return (arrival - travelTime).Max(Sim.simTime);

		}
	}





	internal ArmyTransport transport => viaWater ? ArmyTransport.water : ArmyTransport.land;
	public AttackSender() {
		this.InitializeComponent();
		instance = this;
		arrivalUI.PropertyChanged += SendTroops_PropertyChanged;

	}

	private void SendTroops_PropertyChanged(object? sender,PropertyChangedEventArgs e) {
		OnPropertyChanged();


	}

	protected string travelTimeS {
		get {
			var depart = departure;

			var rv = $"Travel time: {travelTime.Format()}\nDepart: {depart}\nArrival: {(arrival)}";
			if(depart+10 < Sim.simTime) {
				rv += " (depart is past)";
			}
			return rv;
		}
	}

	private void UpdateTroopItems() {

		foreach(var s in sendInfo) {
			s.PropertyChanged-= SendTroops_PropertyChanged;
		}
		sendInfo.Clear();
		//foreach(var target in targets) {
		var ttHome = city.troopsOwnedPlusRecruiting;
		for(var i = (TType)0;i<Troops.ttCount;++i) {
			if(ttHome.GetCount(i) > 0) {
				var rv = new AttackTroopItem(this,i,true,true);
				sendInfo.Add(rv);
				rv.PropertyChanged += SendTroops_PropertyChanged;
			}
		}
		OnPropertyChanged();
		//}
	}

	internal bool isAttack => true;// is (>=Army.attackFirst and <= Army.attackLast);
	public static async Task<bool> ShowInstance(City city,City firstTarget,ArmyType? type = null,ServerTime arrival = default,bool? waitReturn = null,bool? viaWater = null) {
		try {
			var rv = instance ?? new AttackSender();
			if(waitReturn is not null)
				rv.waitReturn = waitReturn.Value;
			if(type is not null) {
				rv.realType = type.Value;
				rv.fakeType = type.Value;
			}
			rv.city =city;
			if(viaWater != null)
				rv.viaWater = viaWater.Value; ;

			var selected = City.GetSelectedForContextMenu(firstTarget.cid,false,city.cid,true,false);
			rv.targets.Clear();
			int counter = 0;
			foreach(var selI in selected) {
				var target = selI.AsCity();
				if(target == city)
					continue;
				if(city.IsAlly(target)) {
					if(await AppS.DoYesNoBox("Attack ally",$"Are you sure that you want to send an attack at  {target}?") != 1) {

						return false;
					}

				}
				rv.targets.Add(new(target,rv,counter++==0));
			}
			rv.UpdateTroopItems();
			//rv._departure = depart;
			//	rv.notSameAlliance.IsChecked=true;

			rv.OnPropertyChanged();
			return await rv.Show(false);
		}
		catch(Exception _ex) {
			LogEx(_ex);
			return false;
		}

	}

	TroopTypeCounts GetTroops(City target) {
		var source = city.troopsHomeAndReturningNotScheduled;

		return (TroopTypeCounts)sendInfo.Where(a => source.Any(a.type)).Select(a => new TroopTypeCount(a.type,source.GetCount(a.type)));
	}

	internal bool AreTargetsValid((TroopTypeCounts reals, TroopTypeCounts fakes) t,bool verbose) {
		if(city.underSiege) {
			if(verbose)
				AppS.MessageBox($"City is under siege");
			return (false);
		}
		if(city.isOutOfFood) {
			if(verbose)
				AppS.MessageBox($"Troops will not leave without food");
			return (false);
		}
		if(!city.isCastle) {
			if(verbose) AppS.MessageBox($"Only castles can attack like that");
			return false;
		}
		//var totalTroops = targets.Sum(a => a.troops);

		//if(arrivalTime == default && !waitReturn  ) {
		//		// check for enough troops
		//		if(!city.troopsHome.IsSuperSetOf(totalTroops)) {
		//			AppS.MessageBox($"Not enough to send {totalTroops}\n({city.troopsHome.Format()} here)");
		//			return (false);
		//		}
		//	}
		if(!t.reals.Any()) {
			if(verbose) AppS.MessageBox($"No troops specified real");
			return (false);
		}
		if(!t.fakes.Any()) {
			if(verbose) AppS.MessageBox($"No troops specified fakes");
			return (false);
		}


		if(city.freeCommandSlots < validTargets.Count()) {
			if(verbose) AppS.MessageBox($"Out of command slots");
			return (false);
		}
		return true;
	}

	internal async Task<bool> IsValid(TroopTypeCounts troops,City target,bool verbose) {




		if(troops.TS() < city.minTS) {
			if(verbose) AppS.MessageBox($"Cannot send {troops.TS()}, min TS is {city.minTS}");
			return (false);

		}

		if(realType == ArmyType.scout) {
			if(troops.GetCount(ttScout) < 0) {
				if(verbose) AppS.MessageBox($"Scouting requires scouts");
				return (false);
			}
			if(troops.Any(t => t.t is not (ttScout or ttGalley))) {
				if(verbose) AppS.MessageBox($"Some of the troops selected cannot tag along with the scouts");
				return (false);
			}

		}

		if((realType is (ArmyType.siege or ArmyType.assault)) &&(!targetCities.All(a => a.isCastle))) {
			if(verbose) AppS.MessageBox($"Target must be castle");
			return (false);
		}
		// check for water
		if(transport is ArmyTransport.land) {
			if(troops.Any(a => IsTTNaval(a.type))) {
				if(verbose) AppS.MessageBox($"Boats must go by water");
				return false;
			}
			if(city.cont != target.cont) {
				if(verbose) AppS.MessageBox($"Land travel only to same continent");
				return (false);
			}
		}
		else if(transport is ArmyTransport.water) {
			{
				// by water

				if(!city.isOnWater || !target.isOnWater) {
					if(verbose) AppS.MessageBox($"Source and target must be on water");
					return (false);
				}
				// check galley space
				var galleys = troops.GetCount(ttGalley);
				var tsLand = (int)troops.Where(t => !t.isNaval).Sum(tt => tt.ts);
				var neededGalleys = tsLand.DivideRoundUp(tsCarryPerGalley);
				if(neededGalleys > galleys) {
					if(verbose) AppS.MessageBox($"Need at least {neededGalleys} galleys to transport troops, have {galleys}");
					return (false);
				}
			}
		}
		else {
			Assert(false);

		}

		// need wings?
		var arrivalTime = arrivalUI.dateTime;
		if(arrivalTime != default) {
			var travelTime = this.travelTime;
			Assert(travelTime > 0);
			var canMakeIt = departure + travelTime <= arrivalTime + TimeSpanS.FromSeconds(10); ;
			if(!canMakeIt) {
				if(await AppS.DoYesNoBox("Arrival too soon",$"The earliest time that we can make is {departure + travelTime}, adjust send time?",no: string.Empty) != 1)
					return (false);
				var arrival = departure + travelTime +TimeSpanS.FromSeconds(10);
				arrivalUI.SetDateTime(arrival);
				arrivalTime = default;
			}
		}
		else {

		}




		return (true);
	}

	//private bool isSettle => (transport == ArmyTransport.carts || transport==ArmyTransport.ports);


	//private void ClearClick(object sender,RoutedEventArgs e) {
	//	foreach(var ti in troopItems)
	//		ti.count = 0;
	//	Changed();
	//}
	//private void MaxClick(object sender,RoutedEventArgs e) {
	//	var t = prior?.troops?? city.troopsOwned;

	//	foreach(var i in troopItems)
	//		i.count = (int)t.GetCount(i.type);

	//	Changed();
	//}

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
	//internal string AttackStrength() {
	//	return $"Attack Strength:\n{troops.AttackStrength(city.player,target.isDungeonOrBoss ? Cavern.Get(target.c).combatWeakness : CombatCategory.invalid):N0}";
	//}
	//internal string RaidCarry() {
	//	return $"Carry: {troops.carry/(float)Cavern.Get(target.c).GetLoot((uint)splits,troops.carry):P}";
	//}

	//private void HomeClick(object sender,RoutedEventArgs e) {
	//	var t = prior?.troops?? city.troopsHome;
	//	foreach(var i in troopItems)
	//		i.count = (int)t.GetCount(i.type);

	//	Changed();
	//}

	internal TimeSpanS travelTime {
		get {
			if(!validTargets.Any())
				return default;
			var t = GetTroopCounts();

			return validTargets.Max(a => Army.JourneyTime(city,a.target.cid,transport,a.isReal ? t.reals : t.fakes,false));
		}
	}
	//internal TimeSpanS travelTimeWithHorms => new((travelTime*useHorns.Switch(1.0f,0.5f)).CeilToInt());


	private async void SendTroopsClick(object sender,RoutedEventArgs e) {
		try {   //if( prior is not null ) {
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
			if(!validTargets.Any()) {

				AppS.MessageBox("Please add or set target cities","");
				return;
			}
			var troops = GetTroopCounts();
			if(!troops.hasEnough) {
				AppS.MessageBox("Not enough troops for minTS","");
				return;
			}
			var valid = AreTargetsValid((troops.reals, troops.fakes),true);
			if(!valid)
				return;
			foreach(var t in validTargets) {
				if(!await IsValid(t.isReal ? troops.reals : troops.fakes,t.target,true)) {
					return;
				}
			}



			var flags = (
											Army.flagNone);
			if(notSameAlliance.IsChecked.Value)
				flags|= Army.flagNotSameAlliance;
			//var arrival = this.arrivalUI.dateTime;
			//if(arrival == default)
			//	arrival = departure + travelTimeWithHorms;
			foreach(var t in validTargets) {
				var _t = t;
				await SendTroops.ShowInstance(city,t.target,false,_viaWater:viaWater,t.isReal ? realType : fakeType,null,t.isReal ? troops.reals : troops.fakes,TimingSetting.arrival, arrival,false,waitReturn,notSameAlliance: notSameAlliance.IsChecked);
			}
			AppS.MessageBox("Attacks Sent! (hopefully)","");
			//	Done();
		}
		catch(Exception ex) { LogEx(ex); }
	}

	//internal TroopTypeCounts troops {
	//	get {
	//		var ts = new TroopTypeCounts();
	//		foreach(var i in troopItems) {
	//			if(i.count > 0) {
	//				ts += i.tt;

	//			}
	//		}

	//		return ts;
	//	}
	//}

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

	public void CallPropertyChanged() {
		if(this.PropertyChanged is not null) {

			PropertyChanged?.Invoke(this,new(null));
		}

	}

	public void OnPropertyChanged() {
		if(this.PropertyChanged is not null)
			AppS.QueueOnUIThread(CallPropertyChanged);
	}
	//public static void Changed(string? member = null) {
	//	if(instance is not null) {
	//		instance.OnPropertyChanged(member);
	//		foreach(var i in instance.validTargets)
	//			i.OnPropertyChanged(member);
	//	}
	//}
	public IEnumerable<City> targetCities => validTargets.Select(a => a.target);
	public IEnumerable<AttackTargetItem> validTargets => targets.Where(a => a.target.IsValid());
	public TroopTypeCounts troopsToSend => city.troopsOwned;
	public (int reals, int fakes) realsAndFakes => (validTargets.Count(a => a.isReal), validTargets.Count(a => a.isFake));

	public (TroopTypeCounts reals, TroopTypeCounts fakes, bool hasEnough) GetTroopCounts() {
		var tsSend = troopsToSend;
		var counts = realsAndFakes;
		var troopsForFakes = sendInfo.Where(a => a.useForFakes).
			Aggregate(new TroopTypeCounts(),(sum,a) =>
					sum+ new TroopTypeCount(a.type,tsSend.GetCount(a.type)));
		var fakesTs = troopsForFakes.TS();
		var minTs = city.minTS;
		var fraction = (float)minTs / fakesTs;
		// Todo senators
		var fakeTroops = troopsForFakes.MulRoundUp(fraction);
		var leftOvers = (tsSend - fakeTroops*counts.fakes);//.Where(a => a.useForReal ) /counts.reals;
		var reals = sendInfo.Where(a => a.useForReal).
				Aggregate(new TroopTypeCounts(),(sum,a) =>
						sum+ new TroopTypeCount(a.type,leftOvers.GetCount(a.type)));
		return (reals, fakeTroops, fraction <= 1.0f && fakeTroops.TS() >= minTs && reals.TS() >= minTs);


	}

	private void RemoveTarget(object sender,RoutedEventArgs e) {
		var s = sender as Button;
		var ctx = s.DataContext as AttackTargetItem;
		Assert(ctx is not null);
		targets.Remove(ctx);
		OnPropertyChanged();

	}
	internal int uiRealType {

		get => (int)realType - Army.armyTypeAttackStart;
		set => realType=(ArmyType)(value+Army.armyTypeAttackStart);


	}
	internal int uiFakeType {

		get => (int)fakeType - Army.armyTypeAttackStart;
		set => fakeType=(ArmyType)(value+Army.armyTypeAttackStart);


	}

	private void AddTarget(object sender,RoutedEventArgs e) {
		targets.Add(new(City.invalid,this,false));

	}

	internal override bool closeOnCitySwitch => false;

	internal JsonAttack AsJsonAttack() {
		JsonAttack rv = new();
		rv.attacker = city.cid.CidToString();
		rv.t = arrival;
		rv.real = realType.EnumNameCode();
		rv.fake = fakeType.EnumNameCode();
		var ts = validTargets.ToArray();
		var t = new JsonAttack.Target[ts.Length];
		for(int i = 0;i < ts.Length;i++) {
			t[i].c = ts[i].target.cid.CidToString();
			t[i].real = ts[i].isReal;
		}
		rv.targets = t;
		return rv;
	}

	internal async void ApplyAttackString(object sender,RoutedEventArgs e) {
		var value = attackString.Text;
		try {

			// Toddo
			var js = JsonSerializer.Deserialize<JsonAttack>(value,JSON.jsonSerializerOptions);
			var source = js.attacker.FromCoordinate();
			if(source > 0) {
				var i = (source == city.cid) ? 1 : await AppS.DoYesNoBox("Set Attacking City?",$"Select yes to set attackert to {source.AsCity()}, select No to keep the current city");
				if(i==-1)
					return;
				if(i==1) {
					cityUI = source.AsCity();
					Enum.TryParse<ArmyType>(js.real,ignoreCase:true,out realType);
					Enum.TryParse<ArmyType>(js.fake,ignoreCase:true,out fakeType);
				}
				js.t = new(js.t.Ticks,DateTimeKind.Unspecified);
				arrivalUI.SetDateTime(js.t);
			}
			targets.Clear();

			foreach(var t in js.targets) {
				var target = t.c.FromCoordinate().AsCity();
				if(target == city || target.IsInvalid())
					continue;

				targets.Add(new(target,this,t.real));
			}

			

		}
		catch(Exception ex) {
			Note.Show("Invalid Json");
		}
		OnPropertyChanged();
	}


	private void Copy(object sender,RoutedEventArgs e) {
		attackString.SelectAll();
		attackString.CopySelectionToClipboard();
	}

	private void Paste(object sender,RoutedEventArgs e) {
		attackString.SelectAll();
		attackString.PasteFromClipboard();

	}

	private void GetAttackString(object sender,RoutedEventArgs e) {
		attackString.Text = JsonSerializer.Serialize(AsJsonAttack(),JSON.jsonSerializerOptionsPretty);
	}
}


internal class JsonAttack
{
	public string attacker { get; set; }
	public DateTime t { get; set; }
	public Target[] targets { get; set; }

	public string real { get; set; }
	public string fake { get; set; }

	internal struct Target
	{

		public string c { get; set; }
		public bool real { get; set; }
	}

}

public class AttackTargetItem:ANotifyPropertyChanged
{
	internal City targetUI {
		get => target;
		set {
			if(target != value) {
				target = value;
				attackSender.OnPropertyChanged();
			}
		}
	}
	internal City target;
	internal AttackSender attackSender;
	internal bool isReal;
	internal bool isFake => !isReal;
	internal City source => attackSender.city;
	public AttackTargetItem(Spot target,AttackSender attackSender,bool isReal) {
		this.target=target;
		this.attackSender=attackSender;
		this.isReal= isReal;

	}

	TroopTypeCounts troops => attackSender.GetTroopCounts() switch { var rf when isFake => rf.fakes, var rf => rf.reals };



	//internal string AttackStrength() {
	//	return $"Attack Strength:\n{troops.AttackStrength(city.player,target.isDungeonOrBoss ? Cavern.Get(target.c).combatWeakness : CombatCategory.invalid):N0}";
	//}


}
public class AttackTroopItem:ANotifyPropertyChanged
{


	public TType type;
	internal bool useForReal;
	internal bool useForFakes;

	internal City source => sender.city;
	internal AttackSender sender;

	public AttackTroopItem(AttackSender attack,TType type,bool useforReal,bool useForFakes) {
		this.type=type;
		this.sender=attack;
		this.useForReal=useforReal;
		this.useForFakes=useForFakes;
	}

	//	internal TroopTypeCount tt => new(type,(uint)_count.Max(0));
	internal ImageSource image => Troops.Image(type);
	internal TroopInfo info => TroopInfo.all[type];


	public string troopsHome => $"{source.troopsHome.GetCount(type).Format()}/{source.troopsOwned.GetCount(type).Format()}";



}




