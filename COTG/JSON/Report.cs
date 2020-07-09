using COTG.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace COTG.JSON
{
	public class Report : IEquatable<Report>
	{
        public const byte typeAsault = 0;
        public const byte typeSiege = 1;
        public const byte typePlunder= 2;
        public const byte typeScout = 3;
        public const byte typeUnknown = 255;

        public string reportId;
        public int defP;
        public string defPlayer => Player.IdToName(defP);
        public int defCid;
        public string defCN { get; set; }
        public string defC => defCid.ToCoordinate();
        public int atkP;
        public string atkPlayer => Player.IdToName(atkP);
        public int atkCid;
        public string atkCN { get; set; }
        public string atkC => atkCid.ToCoordinate();

        public int Cont => defCid.CidToContinent();

        public float claim { get; set; }
        public DateTimeOffset time { get; set; }
        public string TT => time.ToString("dd HH':'mm':'ss");
        public DateTimeOffset spotted { get; set; }
        public byte type;
        public string atkType => type switch
        {
            0 => "assault",
            1 => "siege",
            2 => "plunder",
            3 => "scout",
            _ => "unknown"

        };
        public bool hasSen { get; set; }
        public bool hasSE { get; set; }
        public bool isNaval { get; set; }
        public int defTS { get; set; }
        public int defTSKilled => defTS - defTSLeft;
        public int defTSLeft { get; set; }
        public int atkTSKilled { get; set; }
        public int atkTS { get; set; }
        public int atkTSLeft { get; set; }

        public string atkAli => Player.Get(atkP).allianceName;
        public string defAli => Player.Get(defP).allianceName;

        // Todo: winer
        public static List<Report> all;

		public override bool Equals(object obj)
		{
			return Equals(obj as Report);
		}

		public bool Equals(Report other)
		{
			return other != null &&
				   defCid == other.defCid &&
				   atkCid == other.atkCid &&
				   time.Equals(other.time);
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(defCid, atkCid, time,reportId);
		}

		public static bool operator ==(Report left, Report right)
		{
			return EqualityComparer<Report>.Default.Equals(left, right);
		}

		public static bool operator !=(Report left, Report right)
		{
			return !(left == right);
		}
	}
}
