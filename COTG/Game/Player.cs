using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace COTG.Game
{
    public class Player
    {
        public int id;
        public string name;
        public ushort alliance;
        public ushort pointsH;// divided by 100
        public byte cities;
        public byte title;
        public static Dictionary<int, Player> all = new Dictionary<int, Player>();
        public static Dictionary<string, int> nameToId = new Dictionary<string, int>();
        internal static Player _default = new Player() { name = "Error" };

        public static void Ctor(JsonElement json)
        {
            var _nameToId = new Dictionary<string, int>(1024);
            var _all = new Dictionary<int, Player>(1024);
            foreach (var entry in json.EnumerateObject())
            {
                var id = int.Parse(entry.Name);
                var str = entry.Value.GetString();
                _all.Add(id, new Player() {id=id,name=str } );
                _nameToId.Add(str, id);
            }
            all = _all;
            nameToId = _nameToId;
        }
    }
}
