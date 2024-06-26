﻿//using COTG.Game;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace CnV.GameData
//{
//	public class Report : IEquatable<Report>
//	{
//        public const byte typeAssault = 0;
//        public const byte typeSiege = 1; // siege in history
//        public const byte typePlunder= 2;
//        public const byte typeScout = 3;
//        public const byte typeSieging = 4; // siege in progress
//        public const byte typePending = 5; // siege in progress
//        public const byte typeAttackCount = 6;
        
//        public const float averageSpeed = 10f;
//        public const float averageScoutSpeed = 5f;
//        public static string[] attackTypes = { "assault", "siege", "plunder" };

//        public static byte GetAttackType(string s) => (byte)typeStrings.IndexOf(s);

//        public string reportId;
//        public int defP;
//        public string dPlyr => Player.IdToName(defP);
//        public int defCid;
//        public string defCN => Spot.GetOrAdd(defCid).cityName;
//        public string defC => defCid.CidToString();
//        public int atkP;
//        public string aPlyr => Player.IdToName(atkP);
//        public int atkCid;
//        public string atkCN => Spot.GetOrAdd(atkCid).cityName;
//        public string atkC => atkCid.CidToString();

//        public int Cont => defCid.CidToContinent();
//        public float claim { get; set; }
//        public DateTimeOffset time { get; set; }
//        // No longer used
//        public string TT => time.ToString(AUtil.defaultDateFormat);
//        public DateTimeOffset spotted { get; set; }
//        public byte type;
//        public float journeyTime => spotted.iszero ? 2 * 60 * 60.0f : (float)(time - spotted).TotalSeconds;

//        public float TimeToArrival(DateTimeOffset serverTime) => (float)(time - serverTime).TotalSeconds;
//        public string Type => type == typePending ? troopEstimate : reportStrings[type];
//        public string troopEstimate = "Pending";
//        public bool Sen { get; set; }
//        public bool SE { get; set; }
//        public bool Nvl { get; set; }
//        public int dTS { get; set; }
//        public int dTsKill { get; set; }
//        public int dTsLeft { get; set; }
//        public int aTsKill { get; set; }
//        public int aTS { get; set; }
//        public int aTsLeft { get; set; }

//        public string atkAli => Player.Get(atkP).allianceName;
//        public string defAli => Player.Get(defP).allianceName;

//        // Todo: winer
//        public static List<Report> all;

//		public override bool Equals(object obj)
//		{
//			return Equals(obj as Report);
//		}

//        public bool Equals(Report other)
//        {
//            return other != null &&
//                   defCid == other.defCid &&
//                   atkCid == other.atkCid &&
//                   time.Equals(other.time);
//        }
//        public static int ReportHash(string reportId) => HashCode.Combine(reportId);
//        public static int ReportHash(Report report) => ReportHash(report.reportId);
//        public override int GetHashCode()
//		{
//			return HashCode.Combine(defCid, atkCid, time,reportId);
//		}

//		public static bool operator ==(Report left, Report right)
//		{
//			return EqualityComparer<Report>.Default.Equals(left, right);
//		}

//		public static bool operator !=(Report left, Report right)
//		{
//			return !(left == right);
//		}
//	}
//}
