using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using static COTG.Debug;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using COTG.Helpers;
using System.Text.Json;
using static COTG.Game.Enum;
using COTG.Services;
using System.Web;

namespace COTG.Game
{
    public enum Diplomacy : byte
    {
        none = 0,
        allied = 1,
        nap = 2,
        enemy = 3
    }

    public class Alliance
    {
        public int id;
        public string name;


        public static JsonDocument aldt;

        public static Alliance my = new Alliance();
        public static Dictionary<int, Alliance> all = new Dictionary<int, Alliance>();
        public static Dictionary<string, int> nameToId = new Dictionary<string, int>();

        public static string IdToName(int id)
        {
            if (all.TryGetValue(id, out var a))
                return a.name;
            return string.Empty;
        }

        public static Diplomacy GetDiplomacy(int allianceId)
        {
            if (diplomacy.TryGetValue((byte)allianceId, out var result) == false)
                return Diplomacy.none;
            return result switch
            {
                1 => Diplomacy.allied,
                2 => Diplomacy.nap,
                3 => Diplomacy.enemy,
                _ => Diplomacy.none,
            };
        }
        public static SortedList<byte, byte> diplomacy = new SortedList<byte, byte>(); // small Dictionary 
        public static async void Ctor(JsonDocument _aldt)
        {
           // Log(_aldt);
            aldt = _aldt;
            var element = _aldt.RootElement.GetProperty("aldt");
            my.id = element.GetAsInt("id");
            my.name = element.GetString("n");
            diplomacy = new SortedList<byte, byte>();
            // all.Add(my.id, my);
            //  nameToId.Add(my.name, my.id);

            if (element.TryGetProperty("d", out var dRoot))
            {
                foreach (var prop in dRoot.EnumerateObject())
                {
                    byte relationship = (byte)int.Parse(prop.Name);
                    foreach (var a in prop.Value.EnumerateArray())
                    {
                        byte allianceId = (byte)a.GetAsInt("id");
                        var good = diplomacy.TryAdd(allianceId, relationship);
                        Assert(good == true);
                    }

                    //           { "1":[{ "id":"7","n":"España"}],"2":[{ "id":"80","n":"Blood & Thunder"}],"3":[{ "id":"1","n":"Horizon"},{ "id":"2","n":"The Lunatic Asylum"},{ "id":"49","n":"Unidos-"},{ "id":"62","n":"OvernightObservation"}]}

                }
            }
            for (; ; )
            {
                await Task.Delay(5000);
                if (Player.all.Count != 0)
                    break;
            }
            var alliances = new List<Alliance>();
            var _all = new Dictionary<int, Alliance>();
            var _nameToId = new Dictionary<string, int>();

            using (var jso = await Post.SendForJson("includes/gR.php", "a=1"))
            {
                var r = jso.RootElement;
                var prop2 = r.GetProperty("1");
                foreach (var alliance in prop2.EnumerateArray())
                {
                    var alName = alliance.GetAsString("1");
                    var al = alName == my.name ? my : new Alliance() { name = alName };
                    // Log(alName);
                    alliances.Add(al);
                }
            }
            foreach (var _al in alliances)
            {
                var alName = _al.name;
                var al = _al;
                using (var jsa = await Post.SendForJson("includes/gAd.php", "a=" + HttpUtility.UrlEncode(alName)))
                {
                    var id = jsa.RootElement.GetAsInt("id");
                    _all.Add(id, al);
                    _nameToId.Add(alName, id);
                    int counter = 0;
                    foreach (var me in jsa.RootElement.GetProperty("me").EnumerateArray())
                    {
                        var meName = me.GetString("n");
                        if (meName == null)
                        {
                            Log("Missing name? " + counter);
                            foreach (var member in me.EnumerateObject())
                            {
                                Log($"{member.Name}:{member.Value.ToString()}");
                            }
                        }
                        else if (Player.nameToId.TryGetValue(meName, out var pId))
                        {
                            ++counter;
                            var p = Player.all[pId];
                            p.alliance = (ushort)id;
                            p.cities = (byte)me.GetInt("c");
                            p.pointsH = (ushort)(me.GetInt("s") / 100);

                        }
                        else
                        {
                            Log("Error: " + meName);
                        }

                    }
                }
            }
        
            nameToId = _nameToId;
            all = _all;
        }
    }
}
