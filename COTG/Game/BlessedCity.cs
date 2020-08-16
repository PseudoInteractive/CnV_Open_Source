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
        public static BlessedCity[] all = Array.Empty<BlessedCity>();
        public static City senderCity;
        public Spot spot;
        public int cid => spot.cid;
        public int cont => spot.cont;
        public string city => spot.cityName;
        public string xy => spot.xy;
        public string player => spot.player;
        public string virtue { get; set; }
        public DateTimeOffset blessedUntil { get; set; }
        public int wood { get; set; }
        public int stone { get; set; }
        public int pri { get; set; }
        public string notes { get; set; }
        public float dist { get; set; } // distance to sending city
        public float sortScore => dist - pri * 8.0f; // todo: refine this, also take into acccount resources needed
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
                    bc.blessedUntil = city[5].GetString().ParseDateTime(true);
                    bc.wood = city[6].GetInt32();
                    bc.stone = city[7].GetInt32();
                    bc.pri = city[8].GetInt32();
                    bc.notes = city[9].GetString();
                    cities.Add(bc);
                }
                all = cities.ToArray();

            }
            catch (Exception e)
            {
                COTG.Debug.Log(e);
            }

        }

        public static IList<BlessedCity> GetForCity(City city)
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
                blc.dist = worldC.DistanceToCid(blc.spot.cid)*Enum.cartTravel/60.0f;
                rv.Add(blc);
            }
            rv.Sort((a, b) => a.sortScore.CompareTo(b.sortScore));
            return rv;
        }
        public void SendDonation(int woodToSend,int stoneToSend)
        {
            var secret = $"JJx452Tdd{Player.myId}sRAssa";
            var reqF = $"{{\"a\":{woodToSend},\"b\":{stoneToSend},\"c\":0,\"d\":0,\"cid\":{senderCity.cid},\"rcid\":{cid},\"t\":\"1\"}}"; // t==1 is land, t==2 is water

            Post.Send("includes/sndTtr.php", $"cid={senderCity.cid}&f="+HttpUtility.UrlEncode(Aes.Encode(reqF, secret), Encoding.UTF8)) ;
            Note.Show($"Sent {woodToSend:N0} wood and {stoneToSend:N0} stone in {((woodToSend + stoneToSend + 999) / 1000):N0} carts");
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
