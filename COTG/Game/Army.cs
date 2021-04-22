using COTG.Helpers;
using COTG.JSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Grid;
using Windows.Foundation;
using Windows.UI.Xaml.Input;
using static COTG.Game.Enum;
using Windows.UI.Xaml.Media.Imaging;
using Windows.System;
using System.Text.Json.Serialization;
using System.Text.Json;
using COTG.Views;

namespace COTG.Game
{
    public sealed class Army
    {

        public static Army[] empty = Array.Empty<Army>(); 

        // todo
        public byte type; // reportType* in Enum.cs
        public byte claim { get; set; }
        public bool isAttack { get; set; }
        public string Type => reportStrings[type];
       
        public string aType => type switch
        {
            reportDefensePending=> "inc Def",
            reportDefenseStationed => "Def",
            reportPending =>  "Attack",
            reportSieging => "Siege",
            _ => "unk"
        };

        public TroopTypeCount[] troops { get; set; } = TroopTypeCount.empty;
        public TroopTypeCount[] sumDef { get; set; } = TroopTypeCount.empty;
        // todo
        public bool isDefense => !isAttack;
        public string sXY => sourceCid.CidToString();
        public string tXY => targetCid.CidToString();
        public int targetCid;
        public int sourceCid;

        public int sourceAlliance => Player.Get(sourceCid.CidToPid()).alliance;
        public string sourceAllianceName => Player.Get(sourceCid.CidToPid()).allianceName;

        public int targetAlliance => Player.Get(targetCid.CidToPid()).alliance;
        public string targetAllianceName => Player.Get(targetCid.CidToPid()).allianceName;

        public string reportId; // If not null, this is a history report with a report id
        public static int ReportHash(string reportId) => HashCode.Combine( reportId);
        public int ReportHash() => ReportHash(reportId);

        public string sourceCN => Spot.GetOrAdd(sourceCid).cityName;
        public string targetCN => Spot.GetOrAdd(targetCid).cityName;
        public DateTimeOffset time { get; set; }
        public DateTimeOffset spotted { get; set; }
        public float journeyTime => spotted == AUtil.dateTimeZero ? 2 * 60 * 60.0f : (float)(time - spotted).TotalSeconds;
        public float TimeToArrival(DateTimeOffset serverTime) => (float)(time - serverTime).TotalSeconds;
        public int Cont => sourceCid.CidToContinent();
        public int ts => troops.TS();
        public int sPid => sourceCid.CidToPid(); // The owner of the army, 
        public int tPid => targetCid.CidToPid(); // The owner of the army, 
        public string sPlayer => Player.IdToName(sPid);
        public string tPlayer => Player.IdToName(tPid);

        public int dTsKill { get; set; }
        public int aTsKill { get; set; }
        public int dTS => sumDef.TS();
        public int aTS => troops.TS();

        public int dTsLeft => dTS - dTsKill;
        public int aTsLeft => aTS - aTsKill;

        //    public bool isSiege => isAttack && !troops.IsNullOrEmpty();// this unforunately includes internal attack regardess of type

        public static (Army spot, string column, Vector2 point) HitTest(object sender, TappedRoutedEventArgs e)
        {
            var grid = sender as RadDataGrid;
            var physicalPoint = e.GetPosition(grid);
            var point = new Point { X = physicalPoint.X, Y = physicalPoint.Y };
            var cell = grid.HitTestService.CellInfoFromPoint(point);
            var army = (cell?.Item as Army);


            return (army, cell?.Column.Header?.ToString() ?? string.Empty, physicalPoint.ToVector2());
        }
        public void ProcessTap(string column)
        {
            switch (column)
            {
                case "city":
                case nameof(sXY): Spot.ProcessCoordClick(sourceCid,false, App.keyModifiers,false); break;
                case nameof(sPlayer):JSClient.ShowPlayer(sPlayer); break;
                case nameof(tPlayer): JSClient.ShowPlayer(tPlayer); break;
                case "Troops":
                case "Total Def":
                    {
                        var s = $"{targetCid.CidToCoords()}\t{sourceCid.CidToCoords()}{ (column=="Troops"?troops:sumDef).Format("", '\t',',')}";
                        Note.Show(s);
                        App.CopyTextToClipboard(s);
                    }
                    break;
                
            }
        }
        public static string cN(TroopTypeCount[] troops,int n) => troops.Length > n ? $" {troops[n].count:N0} " : null;
        public static BitmapImage iN(TroopTypeCount[] troops, int n) => troops.Length > n ? ImageHelper.FromImages($"Icons/troops{troops[n].type}.png") : null;

        public string c0        => cN(troops,0);
        public BitmapImage i0   => iN(troops, 0);

        public string c1 => cN(troops, 1);
        public BitmapImage i1 => iN(troops, 1);


        public string c2 => cN(troops, 2);
        public BitmapImage i2 => iN(troops, 2);

        public string c3 => cN(troops, 3);
        public BitmapImage i3 => iN(troops, 3);


        public string sc0 => cN(sumDef, 0);
        public BitmapImage si0 => iN(sumDef, 0);

        public string sc1 => cN(sumDef, 1);
        public BitmapImage si1 => iN(sumDef, 1);


        public string sc2 => cN(sumDef, 2);
        public BitmapImage si2 => iN(sumDef, 2);

        public string sc3 => cN(sumDef, 3);
        public BitmapImage si3 => iN(sumDef, 3);

        public string sc4 => cN(sumDef, 4);
        public BitmapImage si4 => iN(sumDef, 4);
        public string sc5 => cN(sumDef, 5);
        public BitmapImage si5 => iN(sumDef, 5);
        public string sc6 => cN(sumDef, 6);
        public BitmapImage si6 => iN(sumDef, 6);
        public string sc7 => cN(sumDef, 7);
        public BitmapImage si7 => iN(sumDef, 7);


        public bool hasSenator => troops.Any((a) => a.isSenator);
        public bool hasNaval => troops.Any((a) => a.isNaval);
        public bool hasArt => troops.Any((a) => a.isArt);

        public float dist => targetCid.DistanceToCid(sourceCid);
        public static string[] reportAttackTypes = { "assault", "siege", "plunder" };
        public long order;
        public string miscInfo { get; set; } = string.Empty;

        public string troopInfo {
            get {
                
                    string rv = miscInfo;
                    foreach (var tt in troops)
                    {
                        if (tt.count>0)
                            rv += $", {tt.count:N0} {ttNameWithCaps[tt.type]}";
                        else
                            rv += $", {ttNameWithCaps[tt.type]}";

                    }
                    return rv;

                }
            }

            internal string GetToopTip(DateTimeOffset serverNow)
        {
            if (isDefense)
            {
                return  troops.Format(time <= serverNow ? "Stationed:": "Incoming:",'\n'); ;
            }
            else
            {
                if (troops.IsNullOrEmpty())
                    return string.Empty;
                   return troops.Format($"{sPlayer} (from)\n{tPlayer} (to)\n{miscInfo}",'\n');
                    

               
            }
        }

        internal string Format(string delimiter = " ")
        {

            var rv = type == reportPending ? miscInfo : string.Empty;
                foreach (var tt in troops)
                {
					rv += tt.Format(delimiter);

                }
            rv += ";";
            return rv;
        }
    }
    public sealed class TroopTypeCount : IComparable<TroopTypeCount>
    {
        public static TroopTypeCount[] empty = Array.Empty<TroopTypeCount>();
        public int type;
        public int count;
		internal float attack => count * ttAttack[type] * ttCombatBonus[type];

		[JsonIgnore]
        public string Count => count.ToString(" N0 ");
        [JsonIgnore]
        public BitmapImage Type => ImageHelper.FromImages($"Icons/troops{type}.png");

        public TroopTypeCount() { }
        public TroopTypeCount(TroopTypeCount b)
        {
            type = b.type;
            count = b.count;
        }
        public TroopTypeCount(int _type,int _count)
        {
            type = _type;
            count = _count;
        }

        internal string Format(string delimiter)
        {
            return count > 0?  $"{delimiter}{count:N0} {Enum.ttNameWithCaps[type]}" : (delimiter + Enum.ttNameWithCaps[type]);
        }
        [JsonIgnore]
        public bool isSenator => type == Enum.ttSenator;
        [JsonIgnore]
        public bool isArt  =>  Enum.ttArtillery[type];
        [JsonIgnore]
        public bool isNaval => Enum.ttNavy[type];
        [JsonIgnore]
        public bool isDef=> Enum.ttIsDef[type];

        [JsonIgnore]
        public int ts => Enum.ttTs[type] * count;
        public static void SortByTS(TroopTypeCount[] l) => Array.Sort(l);

        // Sort greatest TS to least TS
        int IComparable<TroopTypeCount>.CompareTo(TroopTypeCount other)
        {
           return other.ts.CompareTo(ts);
        }
        /// <summary>
        ///  Json Serialization
        /// </summary>
        public byte t { get => (byte)type; set => type = value; }

        public int c { get => count; set => count = value; }
		public bool isWaterRaider => IsWaterRaider(type);

		public static TroopTypeCount[] operator +(TroopTypeCount[] me, TroopTypeCount tt)
		{
			int counter = 0;
			if (tt.count == 0)
				return me;
			foreach (var i in me)
			{
				if (i.type == tt.type)
				{
					var sum = i.count + tt.count;
					if (sum == 0)
					{
						return me.ArrayRemove(counter); // 0 == remove
					}
					else
					{
						var rv = me.ArrayClone();
						rv[counter] = new TroopTypeCount(tt.type, sum);
						return rv;
					}
				}
				++counter;
			}

			return me.ArrayAppend(new TroopTypeCount(tt));
		}
		
	}
    public static class TroopTypeCountHelper
    {
		public static bool IsSuperSetOf(this TroopTypeCount[] me,TroopTypeCount[] other)
		{
			foreach(var t in other)
			{
				if(me.Count(t.type) < t.count)
				{
					return false;
				}
			}
			return true;
		}
        public static int Count(this TroopTypeCount[] me, int type)
        {
            foreach (var i in me)
            {
                if (i.type == type)
                    return i.count;
            }
            return 0;
        }
        public static bool  HasTT(this TroopTypeCount[] me, int type)
        {
            foreach (var i in me)
            {
                if (i.type == type)
                    return true;
            }
            return false;
        }
        public static bool SetCount(this TroopTypeCount[] me, int type, int count)
        {
            foreach (var i in me)
            {
                if (i.type == type)
                {
                    i.count=count;
                    return true;
                }
            }
            return false;
        }
        public static TroopTypeCount[]  SetOrAdd(this TroopTypeCount[] me, int type, int count)
        {
            int counter = 0;
            foreach (var i in me)
            {
                if (i.type == type)
                {
                    if(count == 0)
                    {
                        return me.ArrayRemove(counter); // 0 == remove
                    }
                    i.count = count;
                    return me;
                }
                ++counter;
            }
            if (count == 0)
                return me;
            return me.ArrayAppend(new TroopTypeCount(type, count));
        }

		
        public static int TS(this TroopTypeCount[] me, int type)
        {
            foreach (var i in me)
            {
                if (i.type == type)
                    return i.count * Game.Enum.ttTs[type];
            }
            return 0;
        }
        // combined TS
        public static int TS(this IEnumerable<TroopTypeCount> l)
        {
            if (l.IsNullOrEmpty())
                return 0;
            var rv = 0;
            foreach (var ttc in l)
            {
                if(ttc.count > 0)
                    rv += ttc.ts;
            }
            return rv;
        }
        public static int TSRaid(this IEnumerable<TroopTypeCount> l)
        {
            if (l.IsNullOrEmpty())
                return 0;
            var rv = 0;
            foreach (var ttc in l)
            {
                if( IsRaider(ttc.type) && SettingsPage.includeRaiders[ttc.type])
                    rv += ttc.ts;
            }
            return rv;
        }
        public static int TSDef(this IEnumerable<TroopTypeCount> l)
        {
            if (l.IsNullOrEmpty())
                return 0;
            var rv = 0;
            foreach (var ttc in l)
            {
                if (ttIsDef[ttc.type])
                    rv += ttc.ts;
            }
            return rv;
        }
        public static int TSOff(this IEnumerable<TroopTypeCount> l)
        {
            if (l.IsNullOrEmpty())
                return 0;
            var rv = 0;
            foreach (var ttc in l)
            {
                if (!ttIsDef[ttc.type])
                    rv += ttc.ts;
            }
            return rv;
        }
        public static byte GetPrimaryTroopType(this IEnumerable<TroopTypeCount> l)
        {
            byte best = 0; // if no troops we return guards 
            var bestTS = 0;
            foreach (var ttc in l)
            {
                var type = ttc.type;
                var ts = ttc.ts;
                if (ts > bestTS)
                {
                    bestTS = ts;
                    best = (byte)type;
                }

            }
            return best;
        }
        public static string Format(this IEnumerable<TroopTypeCount> l,string header,char firstSeparater,char furtherSeparator=(char)0)
        {
            string rv = header;
            foreach (var ttc in l)
            {
                if(ttc.count>0)
                    rv += $"{firstSeparater}{ttc.count:N0} {Enum.ttNameWithCaps[ttc.type]}";
                else
                    rv += $"{firstSeparater}{Enum.ttNameWithCaps[ttc.type]}";
                if (furtherSeparator != (char)0 )
                    firstSeparater = furtherSeparator;
            }
            return rv;
        }

		public static string Format(this IEnumerable<TroopTypeCount> l, string separator)
		{
			string rv = string.Empty;
			string sep = string.Empty;
			foreach (var ttc in l)
			{
				rv += ttc.Format(sep);
				sep = separator;
			}
			return rv;
		}
		public static List<TroopTypeCount> GetTroopTypeCount(this JsonElement tt, Func<int, bool> filter = null)
        {
            var tc = new List<TroopTypeCount>();
            if (tt.ValueKind == JsonValueKind.Object)
            {
                foreach (var a in tt.EnumerateObject())
                {
                    if (int.TryParse(a.Name, out var tType))
                    {
                        if (filter == null || filter(tType))
                        {
                            var count = a.Value.GetAsInt();
                            if (count > 0)
                            {
                                tc.Add(new TroopTypeCount(tType, count));
                            }
                        }
                    }

                }
            }
            else if (tt.ValueKind == JsonValueKind.Array)
            {
                var tType = -1;
                foreach (var a in tt.EnumerateArray())
                {
                    ++tType;
                    if (filter == null || filter(tType))
                    {
                        var count = a.GetInt32();
                        if (count > 0)
                        {
                            tc.Add(new TroopTypeCount(tType, count));
                        }
                    }

                }
            }
            return tc;

        }
        public static List<TroopTypeCount> GetTroopTypeCount2(this JsonElement tt, Func<int, bool> filter = null)
        {
            var tc = new List<TroopTypeCount>();
            if (tt.ValueKind == JsonValueKind.Array)
            {
                foreach (var a in tt.EnumerateArray())
                {
                    var tType = a.GetAsInt("tt");
                    var count = a.GetAsInt("tv");
                    tc.Add(new TroopTypeCount(tType, count));
                    
                    

                }
            }
            return tc;

        }

        // approximation using players speed bonus
        public static float ApproxTravelTime(this byte tt,int cid0, int cid1)
        {
            var dist = cid0.DistanceToCid(cid1);
            var hours = dist*ttTravel[tt] / (60f * ttSpeedBonus[tt]);
            if (ttNavy[tt])
                hours += 1.0f;
            return hours;
        }

		public static TroopTypeCount[] Sum(this TroopTypeCount[] a, TroopTypeCount[] b)
		{
			var rv = a;
			foreach (var tt in b)
			{
				rv = rv + tt;
			}
			return rv;

		}

	}
}
