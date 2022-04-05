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
	using Services;
	using Views;

	// these are transient, not cached, owned by their grid.
	public class BlessedCity
	{
		public static List<BlessedCity> all = new List<BlessedCity>();
		public static City? senderCity;
		public Spot? spot;
		public int cid => spot is not null ? spot.cid : 0;
		public int cont => spot is not null ? spot.cont : 0;
		public string city => spot?.cityName;
		public string xy => spot?.xy;
		public string player => spot?.playerName;
		public string virtue { get; set; }
		public DateTimeOffset blessedUntil { get; set; }
		public int wood { get; set; }
		public int stone { get; set; }
		public int pri;
		public string Pri => DonationTab.instance.priorityNames[pri];

		public int level { get; set; }
		public string notes { get; set; }
		public float travelMinutes; // distance to sending city
		public string travelTime => TimeSpan.FromMinutes(travelMinutes).Format();

		public int priToScore => pri == 4 ? 0 : pri + 1;

		public float sortScore =>
			(float) -(priToScore * 1e20f - (CnVServer.simDateTime - blessedUntil).TotalHours
			                             - travelMinutes ); // todo: refine this, also take into acccount resources needed

		public static async void Refresh()
		{
			try
			{
				var cities = new List<BlessedCity>();
			//	using var data = await Post.SendForJson("overview/bleover.php");
			//	foreach (var city in data.RootElement.GetProperty("a").EnumerateArray())
			//	{
			//		var bc = new BlessedCity();
			//		var cid = city[10].GetInt32();

			//		bc.spot = Spot.GetOrAdd(cid);
			//	//	bc.spot._cityName = city[0].GetString();
			////		bc.spot.pid = Player.NameToId(city[2].GetString());
			//		bc.virtue = city[3].GetString();
			//		bc.level = city[4].GetAsInt();
			//		bc.blessedUntil = city[5].GetString().ParseDateTime(true);
			//		bc.wood = city[6].GetInt32();
			//		bc.stone = city[7].GetInt32();
			//		bc.pri = city[8].GetInt32();
			//		bc.notes = city[9].GetString();
			//		cities.Add(bc);
			//	}

				all = cities;

			}
			catch (Exception e)
			{
				Debug.LogEx(e);
			}

		}

		public static List<BlessedCity> GetForCity(City? city)
		{
			senderCity = city;
			if (city == null)
			{
				return all;
			}

			var rv = new List<BlessedCity>();
			var cid = city.cid;
			var worldC = cid.CidToWorld();
			var cont = cid.CidToContinentDigits();
			foreach (var blc in all)
			{
				// only on same continent or if both are on water and we have ships
				if (DonationTab.viaWater)
				{
					if ( (!blc.spot.isOnWater || city.shipsHome == 0))
						continue;
				}
				else
				{
					if (blc.cont != cont )
						continue;
				}

				blc.travelMinutes = (float) ( worldC.DistanceToCidD(blc.spot.cid) * Troops.cartTravel);
				rv.Add(blc);
			}

			rv.SortSmall( b => b.sortScore);
			return rv;
		}

		public static async Task SendDonation(int senderCid, int targetCid, int woodToSend, int stoneToSend,
			bool useShips )
		{
			// Sender City might be null here
			try
			{
				var city = senderCid.AsCity();
				var target = targetCid.AsCity();
				var sendType = useShips ? 2 : 1;
			//	AppS.UpdateKeyStates();
				var pid = World.CidToPlayerOrMe(senderCid);

				var secret = $"JJx452Tdd{pid}sRAssa";
				var reqF =
					$"{{\"a\":{woodToSend},\"b\":{stoneToSend},\"c\":0,\"d\":0,\"cid\":{senderCid},\"rcid\":{targetCid},\"t\":\"{sendType}\"}}"; // t==1 is land, t==2 is water
				Note.Show(
					$"Sent {woodToSend:N0} wood and {stoneToSend:N0} stone in {((woodToSend + stoneToSend + 999) / (sendType == 1 ? 1000 : 10000)):N0} {(sendType == 1 ? "carts" : "ships")} from {City.Get(senderCid).nameMarkdown}");
				int count = AppS.IsKeyPressedShiftAndControl() ? 4 : 1;
				var _sender = senderCid;
				for (int i = 0; i < count; ++i)
				{
					await Post.Get("includes/sndTtr.php",
						$"cid={_sender}&f=" + HttpUtility.UrlEncode(Aes.Encode(reqF, secret), Encoding.UTF8), pid);
					await Task.Delay(450);
				}
				city.OnPropertyChanged();
				target.OnPropertyChanged();
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

		public void ProcessTap(string headerText)
		{
			var i = this;
			switch (headerText)
			{
				case nameof(i.virtue):
				{
					var sender = BlessedCity.senderCity;

					if (sender != null)
					{
						var inst = DonationTab.instance;
						var wood = (sender.wood - DonationTab.reserveWood.Max(1000)).Max0();
						var stone = (sender.stone - DonationTab.reserveStone.Max(1000)).Max0();
						if (wood + stone <= 0)
						{
							Note.Show("Not enough res");
							return;

						}

						var useShips =
							DonationTab.viaWater; //;sender.cid.CidToContinent() != i.cid.CidToContinent();
						if (useShips)
						{

							var shipReserve = (DonationTab.reserveShipsPCT * sender.ships).RoundToInt()
								.Max(DonationTab.reserveShips);
							var ships = sender.shipsHome - shipReserve - 1;
							if (ships <= 0)
							{
								Note.Show("Not enough ships");
								return;
							}

							if (wood + stone > ships * 10000)
							{
								var desiredC = (wood + stone) / 10000 + 1;
								var ratio = (float) ships / desiredC;
								wood = (int) (wood * ratio);
								stone = (int) (stone * ratio);
							}

						//	sender.shipsHome -= (ushort) ((wood + stone + 9999) / 10000);

						}
						else
						{
							var cartReserve = (DonationTab.reserveCartsPCT * sender.carts).RoundToInt()
								.Max(DonationTab.reserveCarts);
							var carts = sender.cartsHome - cartReserve - 1;
							if (carts <= 0)
							{
								Note.Show("Not enough carts");
								return;
							}

							if (wood + stone > carts * 1000)
							{
								var desiredC = (wood + stone) / 1000 + 1;
								var ratio = (float) carts / desiredC;

								var maxWood = wood;
								var maxStone = stone;
								wood = (int) (wood * ratio);
								stone = (int) (stone * ratio);
								float denom = (wood + stone);
								var desRatio =
									Settings.donationsProportionateToWhatsNeeded && (i.wood > 0 || i.stone > 0)
										? (float) i.wood / (i.wood + i.stone).Max(1)
										: DonationTab.woodStoneRatio;
								if (desRatio >= 0)
								{
									while (wood > 1000 && stone < maxStone - 1000
									                   &&  (wood - 1000) / denom > desRatio)
									{
										wood -= 1000;
										stone += 1000;
									}

									while (stone > 1000 && wood < maxWood - 1000
									                    && (wood + 1000) / denom < desRatio)
									{
										wood += 1000;
										stone -= 1000;
									}
								}
							}

						//	sender.cartsHome -= (ushort) ((wood + stone + 999) / 1000);
						}

						//sender.wood -= wood;
						//sender.stone -= stone;
						BlessedCity.SendDonation(sender.cid, i.cid, wood, stone, useShips);
						i.wood -= wood;
						i.stone -= stone;
						DonationTab.ClearBlessedCity();
						//	BlessedCity.senderCity = null;
						//      var tempSource = DonationTab.instance.donationGrid.ItemsSource;
						//   DonationTab.instance.donationGrid.ItemsSource = tempSource;
						//     DonationTab.instance.donationGrid.ItemsSource = null;
						//     DonationTab.instance.donationGrid.ItemsSource = tempSource; // Force a refresh -  We set null in between, (might be needed)


					}
					}
					break;
				case nameof(i.xy):
					Spot.ProcessCoordClick(i.cid, false, AppS.keyModifiers);
					break;
				//            case nameof(Dungeon.plan):
				//                Raiding.SendRaids(i);
				//                break;

				//        }
			}
		}



	}

}
