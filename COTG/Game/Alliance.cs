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

namespace COTG.Game
{
    public enum Diplomacy : byte
    {
        none = 0,
        allied = 1,
        nap = 2,
        enemy = 3
    }

    public static class Alliance
    {
        public static JsonDocument aldt;

        public static int id;
        public static string name = string.Empty;

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
        public static void Ctor(JsonDocument _aldt)
        {
            Log(_aldt);
            aldt = _aldt;
            var element = _aldt.RootElement.GetProperty("aldt");
            id = element.GetAsInt("id");
            name = element.GetString("n");

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
        }
    }
}
