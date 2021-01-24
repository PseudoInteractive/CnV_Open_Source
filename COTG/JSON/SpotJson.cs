using System.Text.Json;
using System.Text.Json.Serialization;
using System;
using System.Linq;

namespace COTG.DB
{
	
	// JSON version
	public sealed class PlayerPresenceDB
	{
		[JsonPropertyName("id")]
		public string id { get; set; } // playerId is actually an int
		public int cid { get; set; } // where they are located
		public int t { get; set; } // time last seen
								   // todo: last action
		public string tk { get; set; } // token
		public string ck { get; set; } // cookie

	}

	public sealed class SpotDB
    {
        public const byte typeEmpty = 0;
        public const byte typeCity = 1;
        public const byte typeFilled = 2;
        public const byte typeShrine = 3;
        [JsonPropertyName("id")]
        public string id { get; set; } // city id format is xxxyyy
        public (int x, int y) cid { get {
                var i = int.Parse(id);
                var x = i / 1000;
                var y = i - x * 1000;
                return (x, y);
            } }
 //       public static int XYToId(int x, int y)
//		{
//            return x + (y << idYShift);

	//	}
        public static string CoordsToString((int x, int y) c) => $"{c.x:000}{c.y:000}";
    //    const int idXMask = 65535;
   //     const int idYShift = 16;
      //  internal int x => id & idXMask;
     //   internal int y => (int)((uint)id >> idYShift);
        public int own { get; set; } // playerId
        public byte typ { get; set; } // playerId
        public long flg { get; set; } // playerId
        public RecordBattle[] recb { get; set; }
        public RecordNote[] recn { get; set; }
        public bool AddRecord(RecordBattle rb)
        {
            if (recb == null)
            {
                recb = new RecordBattle[1] { rb };
 //               recb[0] = rb;
                return true;
            }

            foreach(var r in recb)
            {
                if (r.rep == rb.rep)
                    return false;
            }
            recb = recb.ArrayAppend(rb);
            // this van be optimized
            Array.Sort(recb, (b, a) => a.t.CompareTo(b.t));
            return true;
        }
        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }


    public class Record
    {
        public int t { get; set; } // smalltime seconds
        public void  SetTime(DateTimeOffset _t) { t = SmallTime.ToSeconds(_t); }
        [JsonIgnore]
        public DateTimeOffset dateTime => SmallTime.ToDateTime(t);

    }



    public sealed class RecordBattle : Record
    {
        public string rep { get; set; } // 
        public COTG.Game.TroopTypeCount[] trp { get; set; } // todo: array?
        public byte typ { get; set; }
    }


    /// <summary>
    /// This can be a senator claim too.  Note should start with 'Claim'
    /// </summary>
    public sealed class RecordNote : Record
    {
        public int src { get; set; }
        public string n { get; set; }
    }
   
}
