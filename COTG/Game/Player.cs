using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using static COTG.Debug;

namespace COTG.Game
{
    public class Player
    {
        public int id;
        public string name;
        public ushort alliance;
        public ushort pointsH;// divided by 100
        public ushort cities;
        public byte title;
        public static int myId;
        public static string myName;

        public string allianceName => Alliance.IdToName(alliance);

        public static Dictionary<int, Player> all = new Dictionary<int, Player>();
        public static Dictionary<string, int> nameToId = new Dictionary<string, int>();
        public static string IdToName(int id)
        {
            return Get(id).name;
         }
        public static int NameToId(string name)
        {
            return name!=null ? nameToId.GetValueOrDefault(name) : -1;
        }
        public static bool IsMe(int playerId)
        {
            return Player.myId == playerId;
        }
        public static Player _default = new Player() { name = "!Zut!" };
        public static Player Get(int id) => all.GetValueOrDefault(id, _default);
        public static void Ctor(JsonElement json)
        {
            var _nameToId = new Dictionary<string, int>(1024);
            var _all = new Dictionary<int, Player>(1024);
            foreach (var entry in json.EnumerateObject())
            {
                var id = int.Parse(entry.Name);
                var str = entry.Value.GetString();
                _all.Add(id, new Player() { id = id, name = str });
                _nameToId.Add(str, id);
            }
            // bonus!
            var bonus = new string[] { "Empty", "tlgger2","Vacant", "abandoned", "TTC","Gordy", "adi" };
            {
                int counter = -1;
                foreach (var i in bonus)
                {
                    _all.Add(counter, new Player() { id = counter, name = i });
                    _nameToId.Add(i, counter);
                    --counter;

                }
            }
            all = _all;
            nameToId = _nameToId;
      //      Note.Show("Got Players");


        }        
    }
}
