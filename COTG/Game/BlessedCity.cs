using COTG.Helpers;
using COTG.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace COTG.Game
{
    // these are transient, not cached, owned by their grid.
    public class BlessedCity
    {
        public static List<BlessedCity> all = new List<BlessedCity>();
        public static City senderCity;
        public Spot spot;
        public int cid => spot.cid;
        public int cont => spot.cont;
        public string city => spot.cityName;
        public string xy => spot.xy;
        public string player => spot.playerName;
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

		public int priToScore => pri == 4 ? 0 : pri+1;
		public float sortScore => (float)-(priToScore*1e20f - (JSClient.ServerTime() - blessedUntil).TotalHours - travelMinutes ); // todo: refine this, also take into acccount resources needed
        public static async void Refresh()
        {
            try
            {
                var cities = new List<BlessedCity>();
                var data = await Post.SendForJson("overview/bleover.php");
                foreach (var city in data.RootElement.GetProperty("a").EnumerateArray())
                {
                    var bc = new BlessedCity();
                    var cid = city[10].GetInt32();

                    bc.spot = Spot.GetOrAdd(cid);
                    bc.spot._cityName = city[0].GetString();
                    bc.spot.pid = Player.NameToId(city[2].GetString());
					bc.virtue = city[3].GetString();
					bc.level = city[4].GetAsInt();
					bc.blessedUntil = city[5].GetString().ParseDateTime(true);
                    bc.wood = city[6].GetInt32();
                    bc.stone = city[7].GetInt32();
                    bc.pri = city[8].GetInt32();
                    bc.notes = city[9].GetString();
                    cities.Add(bc);
                }
                all = cities;

            }
            catch (Exception e)
            {
                COTG.Debug.LogEx(e);
            }

        }

        public static List<BlessedCity> GetForCity(City city)
        {
            senderCity = city;
            if(city==null)
            {
                return all;
            }
            var rv = new List<BlessedCity>();
            var cid = city.cid;
            var worldC = cid.CidToWorld();
            var cont = cid.CidToContinent();
            foreach (var blc in all)
            {
                // only on same continent or if both are on water and we have ships
                if (blc.cont != cont && (!blc.spot.isOnWater || city.shipsHome == 0))
                    continue;
                blc.travelMinutes = (float)( worldC.DistanceToCidD(blc.spot.cid)*Troops.cartTravel);
                rv.Add(blc);
            }
            rv.SortSmall( b => b.sortScore);
            return rv;
        }
		public static async Task SendDonation(int senderCid,int targetCid, int woodToSend,int stoneToSend, bool useShips )
		{
			var sendType = useShips ? 2 : 1;
			App.UpdateKeyStates();
			var pid = World.CidToPlayerOrMe(senderCid);

			var secret = $"JJx452Tdd{pid}sRAssa";
            var reqF = $"{{\"a\":{woodToSend},\"b\":{stoneToSend},\"c\":0,\"d\":0,\"cid\":{senderCid},\"rcid\":{targetCid},\"t\":\"{sendType}\"}}"; // t==1 is land, t==2 is water
			Note.Show($"Sent {woodToSend:N0} wood and {stoneToSend:N0} stone in {((woodToSend + stoneToSend + 999) / (sendType == 1 ? 1000 : 10000)):N0} {(sendType == 1 ? "carts" : "ships")} from {City.Get(senderCid).nameMarkdown}");
			int count = App.IsKeyPressedShiftAndControl() ? 4 : 1;
			var _sender = senderCity.cid;
			for (int i = 0; i < count; ++i)
			{
				await Post.Get("includes/sndTtr.php", $"cid={_sender}&f=" + HttpUtility.UrlEncode(Aes.Encode(reqF, secret), Encoding.UTF8), pid);
				await Task.Delay(450);
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

    }
}
