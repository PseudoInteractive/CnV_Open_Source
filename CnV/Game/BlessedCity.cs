using CnV;

using CnV.Helpers;
using CnV.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace CnV
{
	using Microsoft.UI.Xaml.Media;

	using Services;

	using System.Collections.Concurrent;

	using Views;

	// these are transient, not cached, owned by their grid.
	public class DonationOrder : CNotifyPropertyChanged, IEquatable<DonationOrder?>
	{
	//	public static List<DonationOrder> all = new List<DonationOrder>();
		public City senderCity;
		public bool isTempleDonation;
	
		public City target { get; set; }

		public TimeSpanS travelTime; // distance to sending city

		public string nameAndRemarks => target.nameAndRemarks;
		public int cid =>  target.cid;
		public int cont =>  target.cont;
		public string cityName => target.FormatName(true,true,false);
		public ImageSource icon => target.icon;

		public string xy => target.xy;
		public string player => target.playerName;
		public string virtue => target.blessData.virtue.EnumName(); // only for temple
		public ServerTime blessedUntil => target.blessData.blessedUntil; // only for temple
		public DateTime BlessedUntil => blessedUntil; // only for temple
													  //public Resources resoucesToSend;

		internal static ConcurrentHashSet<WeakReference<DonationOrder>> all = new();

		public Resources resoucesToSend {
			get {
				var res = senderCity.sampleResources.SubSat(Settings.tradeSendReserve);
				var desired = isTempleDonation ? target.templeMissing : target.requestedResources;
				res = res.Min(desired);
				return res.LimitToTransport(tradeTransport);
			}
		}
		public DonationOrder(City target,TimeSpanS travelTime,City senderCity,bool isTempleDonation) {
			this.senderCity=senderCity;
			this.target=target;
			this.isTempleDonation = isTempleDonation;
			this.travelTime = travelTime;
			if(isTempleDonation) {
				Assert(resoucesToSend.iron==0);
				Assert(resoucesToSend.food==0);
			}
			all.Add(new(this,false));
			//Note.Show($"Orders: {all.Count}");
			
		}
		public static DonationOrder GetOrAdd(City target,TimeSpanS travelTime,City senderCity,bool isTempleDonation) {
			foreach(var i in all.ToArray()) {
				if( i.TryGetTarget(out var t)) {

					if(t.senderCity == senderCity&&t.target==target && t.isTempleDonation==isTempleDonation)
						return t;
				}
				else {
					all.Remove(i);
				}
			}
			return new(target,travelTime,senderCity,isTempleDonation);
		}
		internal static void RefreshAll() {
			
			var initial = all.Count;
			foreach(var i in all.ToArray()) {
				if( i.TryGetTarget(out var t)) {
				
					t.OnPropertyChangedImmediate();
				}
				else {
					all.Remove(i);
				}
			}
		//	Note.Show($"Orders: {initial}=>{all.Count}");
		}
		public int tradeTransport => DonationTab.ViaWater ? (senderCity.shipsHome - Settings.tradeSendReserveShips).Max(0)*10_000 : (senderCity.cartsHome - Settings.tradeSendReserveCarts).Max(0)*1000;
		public int wood => resoucesToSend.wood;
		public int stone => resoucesToSend.stone;

		public string goldReward => (resoucesToSend.sum * target.resourcePaymentRate).RoundToInt().Format();

		public int iron => resoucesToSend.iron;

		public int food => resoucesToSend.food;
		public PalacePriority pri => target.blessData.priority;
		public string Pri => pri.EnumTitle();
		
		public int level => target.palaceLevel;
		public string notes => target.blessData.notes;
		public DateTimeOffset travelT => (ServerTime.epoch + travelTime); // year and day are off format for UI

		public int priToScore => (int)pri;

		public float templeSortScore => (float)-(priToScore * (1024) - (blessedUntil-Sim.simTime).TotalHours
										 - travelTime.TotalMinutes*0.5f);
		
		public float marketSortScre => travelTime.TotalSeconds; // least to greatest todo: also take into acccount resources needed

		//public static async void Refresh()
		//{
		//	try
		//	{
		//		var cities = new List<DonationOrder>();
		//	//	using var data = await Post.SendForJson("overview/bleover.php");
		//	//	foreach (var city in data.RootElement.GetProperty("a").EnumerateArray())
		//	//	{
		//	//		var bc = new BlessedCity();
		//	//		var cid = city[10].GetInt32();

		//	//		bc.spot = Spot.GetOrAdd(cid);
		//	//	//	bc.spot._cityName = city[0].GetString();
		//	////		bc.spot.pid = Player.NameToId(city[2].GetString());
		//	//		bc.virtue = city[3].GetString();
		//	//		bc.level = city[4].GetAsInt();
		//	//		bc.blessedUntil = city[5].GetString().ParseDateTime(true);
		//	//		bc.wood = city[6].GetInt32();
		//	//		bc.stone = city[7].GetInt32();
		//	//		bc.pri = city[8].GetInt32();
		//	//		bc.notes = city[9].GetString();
		//	//		cities.Add(bc);
		//	//	}

		//		all = cities;

		//	}
		//	catch (Exception e)
		//	{
		//		Debug.LogEx(e);
		//	}

		//}

		//public static List<DonationOrder> GetForCity(City? city)
		//{
			
		//	var rv = new List<DonationOrder>();
		//	if (city == null)
		//	{
		//		return rv;
		//	}


		//	var p = city.player;
			
		//	var cid = city.cid;
		//	var worldC = cid.CidToWorld();
		//	var cont = cid.CidToContinentDigits();
		//	if(wantTempleDonations) {
		//		Assert(false);
		//	}
		//	else {
		//		foreach(var blc in City.allianceCities) {
		//			if(!blc.requestedResources.any)
		//				continue;

		//			// only on same continent or if both are on water and we have ships
		//			TimeSpanS travelTime;
		//			if(DonationTab.viaWater) {
		//				if(!blc.isOnWater )
		//					continue;
		//				travelTime = p.ShipTravelTime(worldC.DistanceToCidD(blc.cid));
					
		//			}
		//			else {
		//				// must be same continent
		//				if(blc.cont != cont)
		//					continue;
		//				travelTime = p.CartTravelTime(worldC.DistanceToCidD(blc.cid));
					
		//			}
		//			if(travelTime > blc.requestedResourceMaxTravel)
		//				continue;
					
		//			rv.Add(new(target:blc,travelTime:travelTime,senderCity:this));
		//		}
		//	}

		//	rv.SortSmall( b => b.sortScore);
		//	return rv;
		//}

		public async void Send()
		{
			// Sender City might be null here
			try
			{
				var source = senderCity;

				
					await	SendResDialogue.ShowInstance(source,target,resoucesToSend,DonationTab.ViaWater,palaceDonation:isTempleDonation);
					await Task.Delay(500);
				// Need to update all of them
//					UpdateResourcesToSend();
					RefreshAll();	
				//OnPropertyChanged();
						// Todo: apply limit to resources again in case of pause before sending
						//var trade = new TradeOrder(source: source.c,target: target.c,departure: Sim.simTime,viaWater: DonationTab.viaWater,isTempleTrade: wantTempleDonations,
						//	resources: resoucesToSend.Min(source.sampleResources).LimitToTranspost(tradeTransport) );

						//new CnVEventTrade(source.c,trade: trade).EnqueueAsap();


						//{
						//	Note.Show($"Sent {trade.resources.Format()} from {source} to {target}");
						//}
				//	}


			//	source.OnPropertyChanged();
			//	target.OnPropertyChanged();
			}
			catch (Exception e)
			{
				LogEx(e);
			}
		}






		/*        {
    "a": [
        [
        0	"44.01.07",
        1	"C44 (468:453)",
        2	"HARE",
        3	"Ibria",
        4	1,
        5	"12:07:04 ",
        6	0,
        7	0,
        8	3,
        9	"overfill",
        10	29688276,
        11	1596283624
        ]
    ],
    "b": []
    }*/

		public void ProcessTap(string mappingName)
		{
			var i = this;
			switch (mappingName)
			{
				case nameof(i.virtue):
				case nameof(i.Pri):
				case nameof(i.travelT):
				case nameof(i.wood):
				case nameof(i.stone):
				case nameof(i.food):
				case nameof(i.iron):
				case nameof(i.goldReward):
				{
						i.Send();
						break;
				//	var sender = senderCity;

				//	if (sender != null)
				//	{
				//		var inst = DonationTab.instance;
				//		var wood = (sender.wood - DonationTab.reserveWood.Max(1000)).Max0();
				//		var stone = (sender.stone - DonationTab.reserveStone.Max(1000)).Max0();
				//		if (wood + stone <= 0)
				//		{
				//			Note.Show("Not enough res");
				//			return;

				//		}

				//		var useShips =
				//			DonationTab.viaWater; //;sender.cid.CidToContinent() != i.cid.CidToContinent();
				//		if (useShips)
				//		{

				//			var shipReserve = (DonationTab.reserveShipsPCT * sender.ships).RoundToInt()
				//				.Max(DonationTab.reserveShips);
				//			var ships = sender.shipsHome - shipReserve - 1;
				//			if (ships <= 0)
				//			{
				//				Note.Show("Not enough ships");
				//				return;
				//			}

				//			if (wood + stone > ships * 10000)
				//			{
				//				var desiredC = (wood + stone) / 10000 + 1;
				//				var ratio = (float) ships / desiredC;
				//				wood = (int) (wood * ratio);
				//				stone = (int) (stone * ratio);
				//			}

				//		//	sender.shipsHome -= (ushort) ((wood + stone + 9999) / 10000);

				//		}
				//		else
				//		{
				//			var cartReserve = (DonationTab.reserveCartsPCT * sender.carts).RoundToInt()
				//				.Max(DonationTab.reserveCarts);
				//			var carts = sender.cartsHome - cartReserve - 1;
				//			if (carts <= 0)
				//			{
				//				Note.Show("Not enough carts");
				//				return;
				//			}

				//			if (wood + stone > carts * 1000)
				//			{
				//				var desiredC = (wood + stone) / 1000 + 1;
				//				var ratio = (float) carts / desiredC;

				//				var maxWood = wood;
				//				var maxStone = stone;
				//				wood = (int) (wood * ratio);
				//				stone = (int) (stone * ratio);
				//				float denom = (wood + stone);
				//				var desRatio =
				//					Settings.donationsProportionateToWhatsNeeded && (i.wood > 0 || i.stone > 0)
				//						? (float) i.wood / (i.wood + i.stone).Max(1)
				//						: DonationTab.woodStoneRatio;
				//				if (desRatio >= 0)
				//				{
				//					while (wood > 1000 && stone < maxStone - 1000
				//					                   &&  (wood - 1000) / denom > desRatio)
				//					{
				//						wood -= 1000;
				//						stone += 1000;
				//					}

				//					while (stone > 1000 && wood < maxWood - 1000
				//					                    && (wood + 1000) / denom < desRatio)
				//					{
				//						wood += 1000;
				//						stone -= 1000;
				//					}
				//				}
				//			}

				//		//	sender.cartsHome -= (ushort) ((wood + stone + 999) / 1000);
				//		}

				//			//sender.wood -= wood;
				//			//sender.stone -= stone;
				//		i.Send();
				//	//	i.wood -= wood;
				//	//	i.stone -= stone;
				//		DonationTab.ClearBlessedCity();
				//		//	BlessedCity.senderCity = null;
				//		//      var tempSource = DonationTab.instance.donationGrid.ItemsSource;
				//		//   DonationTab.instance.donationGrid.ItemsSource = tempSource;
				//		//     DonationTab.instance.donationGrid.ItemsSource = null;
				//		//     DonationTab.instance.donationGrid.ItemsSource = tempSource; // Force a refresh -  We set null in between, (might be needed)


				//	}
					}
				//	break;
				case nameof(i.xy):
				case nameof(i.icon):
					Spot.ProcessCoordClick(i.cid, false, AppS.keyModifiers);
					break;
				//            case nameof(Dungeon.plan):
				//                Raiding.SendRaids(i);
				//                break;

				//        }
			}
		}

		public override bool Equals(object? obj) {
			return Equals(obj as DonationOrder);
		}

		public bool Equals(DonationOrder? other) {
			return other is not null&&
				   EqualityComparer<City>.Default.Equals(senderCity,other.senderCity)&&
				   isTempleDonation==other.isTempleDonation&&
				   EqualityComparer<City>.Default.Equals(target,other.target);
		}

		public override int GetHashCode() {
			return HashCode.Combine(senderCity,isTempleDonation,target);
		}

		public static bool operator ==(DonationOrder? left,DonationOrder? right) {
			return EqualityComparer<DonationOrder>.Default.Equals(left,right);
		}

		public static bool operator !=(DonationOrder? left,DonationOrder? right) {
			return !(left==right);
		}
	}

}
