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
        public string xy => cid.CidToString();
        public int target { get; set; }
        public string targetXy => target.CidToString();
        public enum Type
        {
            senator,
            assault,
            se, 
        }
        // Todo: make enum
        public int type { get; set; }
        public bool fake { get; set; }
        public byte troopType { get; set; }
        [JsonIgnore]
        public string troopTypeString => Game.Enum.ttNameWithCaps[troopType];
        public int ts { get; set; }
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
        public byte classification { get; set; }
        public byte attackCluster { get; set; } // 0 is real

    }
}
