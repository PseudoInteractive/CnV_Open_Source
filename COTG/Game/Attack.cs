using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace COTG.Game
{
   
    public class Attack
    {
        public string player { get; set; } = string.Empty;
        public int cid { get; set; }
        [JsonIgnore]
        public string xy => cid.CidToString();
        public int[] targets { get; set; } = Array.Empty<int>();
        [JsonIgnore]
        public string targetString {
            get {
                var sb = new StringBuilder();
                var wantSpace = false;
                foreach(var t in targets)
                {
                    if (wantSpace)
                        sb.Append(' ');
                    else
                        wantSpace= true;
                    sb.Append(t.CidToStringMD());
                }
                return sb.ToString();
            }
        }
        public enum Type
        {
            senator,
            assault,
            se, 
        }
        // Todo: make enum
        public int type { get; set; }
        [JsonIgnore]
        public Type typeT { get => (Type)type; set=>type=(int)value; }
        public bool fake { get; set; } // not used currently
        public byte troopType { get; set; }
        [JsonIgnore]
        public string troopTypeString => Game.Enum.ttNameWithCaps[troopType];
        public int ts { get; set; } // not used currently
    }
    public class AttackType
    {
        public AttackType(int _type, string _name) { type = _type;name = _name; }
        public int type { get; set; }
        public string name { get; set; }
        public static AttackType[] types = { new AttackType(0, "Senator"), new AttackType(1, "Assault"), new AttackType(2, "se") };
    }
   
    public struct TargetPersist // structure use to persist targets
    {
        public int cid { get; set; }
        public bool fake { get; set; }
        public byte attackCluster { get; set; } // for SE clusters, 

    }
    public struct AttackSenderScript
    {
        public List<int> x { get; set; }
        public List<int> y { get; set; }
        public List<int> type { get; set; }
        public string[] time { get; set; }
/*	"x": [
		"282",
		"282"
	],
	"y": [
		"231",
		"230"
	],
	"type": [
		"0",
		"0"
	],
	"time": [
		"10",
		"00",
		"00",
		"00/00/0000"
	]
    }*/

    }
}
