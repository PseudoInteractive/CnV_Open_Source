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
        public const byte typeAssault = 0;
        public const byte typeSiege = 1; // siege in history
        public const byte typePlunder= 2;
        public const byte typeScout = 3;
        public const byte typeSieging = 4; // siege in progress
        public const byte typePending = 5;

        public string reportId;
        public int defP;
        public string dPlyr => Player.IdToName(defP);
        public int defCid;
        public string defCN { get; set; }
        public string defC => defCid.CidToString();
        public int atkP;
        public string aPlyr => Player.IdToName(atkP);
        public int atkCid;
        public string atkCN { get; set; }
        public string atkC => atkCid.CidToString();

        public int Cont => defCid.CidToContinent();

        public float claim { get; set; }
        public DateTimeOffset time { get; set; }
        // No longer used
        public string TT => time.ToString("dd'> 'HH':'mm':'ss");
        public DateTimeOffset spotted { get; set; }
        public byte type;
        public string Type => type switch
        {
            0 => "assault",
            1 => "siege",
            2 => "plunder",
            3 => "scout",
            4 => "sieging",
            _ => "incom"

        };
        public bool Sen { get; set; }
        public bool SE { get; set; }
        public bool Nvl { get; set; }
        public int dTS { get; set; }
        public int dTsKill => dTS - dTsLeft;
        public int dTsLeft { get; set; }
        public int aTsKill { get; set; }
        public int aTS { get; set; }
        public int aTsLeft { get; set; }

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
        public static int ReportHash(string reportId) => HashCode.Combine(reportId);
        public static int ReportHash(Report report) => ReportHash(report.reportId);
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
