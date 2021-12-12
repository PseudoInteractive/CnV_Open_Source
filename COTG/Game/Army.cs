using System.Collections.Generic;
using System.Numerics;
using Windows.Foundation;
using Microsoft.UI.Xaml.Input;
using static CnV.Game.Troops;
using Microsoft.UI.Xaml.Media.Imaging;
using System.Text.Json.Serialization;
using System.Text.Json;
//COTG.DArray<COTG.Game.TroopTypeCount>;
using TroopTypeCountsRef = CnV.Game.TroopTypeCounts;
using System.Runtime.InteropServices;
using Cysharp.Text;

namespace CnV.Game
{
	using Helpers;
	using Syncfusion.UI.Xaml.DataGrid;

	public sealed class Army:IEquatable<Army>
	{

        public static Army[] empty = Array.Empty<Army>(); 

        // todo
        public byte type; // reportType* in Enum.cs
        public byte claim { get; set; }
        public bool isAttack { get; set; }
        public string Type => claim switch { >= 100 => "Cap!", >0=>"Capping",_=> reportStrings[type] };
		public bool isSiege => type == reportSieging;

		public string aType => type switch
        {
            reportDefensePending=> "inc Def",
            reportDefenseStationed => "Def",
            reportPending =>  "Attack",
            reportSieging when claim >= 100 => "Cap!",
			reportSieging when claim > 0 => "Capping",
			reportSiege  => "Siege",
			reportSieging => "Sieging",
			reportAssault => "Assult",
			reportPlunder => "Plunder",
			reportScout => "Scout",
			_ => "unk"
        };

		public TroopTypeCounts troops;
		public TroopTypeCounts sumDef;
		// todo
		public bool isDefense => !isAttack;
        public string sXY => sourceCid.CidToString();
        public string tXY => targetCid.CidToString();
        public int targetCid;
        public int sourceCid;
		// We can't get this from source and targetCid because there might have been a cap
		public int targetPid;
		public int sourcePid;
		public Player targetPlayer => Player.Get(targetPid);
		public Player sourcePlayer => Player.Get(sourcePid); 
		public City sourceCity => City.Get(sourceCid);
		public City targetCity => City.Get(targetCid);
		public int sourceAlliance => Player.Get(sourcePid).alliance;
        public string sourceAllianceName => Player.Get(sourcePid).allianceName;

        public int targetAlliance => Player.Get(targetPid).alliance;
        public string targetAllianceName => Player.Get(targetPid).allianceName;

        public string reportId; // If not null, this is a history report with a report id
        public static int ReportHash(string reportId) => HashCode.Combine( reportId);
        public int ReportHash() => ReportHash(reportId);

        public string sourceCN => Spot.GetOrAdd(sourceCid).cityName;
        public string targetCN => Spot.GetOrAdd(targetCid).cityName;
        public DateTimeOffset time { get; set; }
		public bool arrived => time <= CnVServer.ServerTime();
		public bool Arrived(DateTimeOffset serverTime) => time <= serverTime;
		public DateTimeOffset spotted { get; set; }
        public float journeyTime => spotted == AUtil.dateTimeZero ? 2 * 60 * 60.0f : (float)(time - spotted).TotalSeconds;
		public double journeyTimeMinutes => spotted == AUtil.dateTimeZero ? 2 * 60 * 60.0 : (time - spotted).TotalMinutes;
		public string journeyTimeString => spotted == AUtil.dateTimeZero ? ":(" : (time - spotted).Format();
		public float TimeToArrival(DateTimeOffset serverTime) => (float)(time - serverTime).TotalSeconds;
        public int Cont => sourceCid.CidToContinent();
        public int ts => troops.TS();
        public int sPid => sourcePid; // The owner of the army, 
        public int tPid => targetPid; // The owner of the army, 
        public string sPlayer => Player.IdToName(sourcePid);
        public string tPlayer => Player.IdToName(targetPid);

        public int dTsKill { get; set; }
        public int aTsKill { get; set; }
        public int dTS => sumDef.TS();
        public int aTS => troops.TS();
		public int approxKill => ApproximateKillsFromRefines(refines);
		public int refines { get; set; } 
		public int dTsLeft => dTS - dTsKill;
        public int aTsLeft => aTS - aTsKill;

        //    public bool isSiege => isAttack && !troops.IsNullOrEmpty();// this unforunately includes internal attack regardess of type

        //public static (Army spot, string column, Vector2 point) HitTest(object sender, TappedRoutedEventArgs e)
        //{
        //    var grid = sender as SfDataGrid;
        //    var physicalPoint = e.GetPosition(grid);
        //    var point = new Point { X = physicalPoint.X, Y = physicalPoint.Y };
        //    var cell = grid.HitTestService.CellInfoFromPoint(point);
        //    var army = (cell?.Item as Army);


        //    return (army, cell?.Column.Header?.ToString() ?? string.Empty, physicalPoint.ToVector2());
        //}
        public bool ProcessTap(string column)
        {
            switch (column)
            {
                case "city":
                case nameof(sXY): Spot.ProcessCoordClick(sourceCid,false, App.keyModifiers,false); return true;

				case nameof(sPlayer):JSClient.ShowPlayer(sPlayer); return true;

				case nameof(tPlayer): JSClient.ShowPlayer(tPlayer); return true;

				case "Troops":
                case "Total Def":
                    {
                        var s = $"{targetCid.CidToCoords()}\t{sourceCid.CidToCoords()}{ (column=="Troops"?troops:sumDef).Format("", '\t',',')}";
                        Note.Show(s);
                        App.CopyTextToClipboard(s);
                    }
	                return true;
                
            }

            return false;
        }
	
		public static string cN(in TroopTypeCounts troops,int n) => troops.Count > n ? $" {troops.GetIndexCount(n):N0} " : null;
		
		public static BitmapImage iN(in TroopTypeCounts troops, int n) => troops.Count > n ? ImageHelper.FromImages($"Icons/troops{troops.GetIndexType(n) }.png") : null;
		[JsonIgnore]
        public string c0        => cN(troops,0);
		[JsonIgnore]
		public BitmapImage i0   => iN(troops, 0);

		[JsonIgnore]
		public string c1 => cN(troops, 1);
		[JsonIgnore]
		public BitmapImage i1 => iN(troops, 1);


		[JsonIgnore]
		public string c2 => cN(troops, 2);
		[JsonIgnore]
		public BitmapImage i2 => iN(troops, 2);

		[JsonIgnore]
		public string c3 => cN(troops, 3);
		[JsonIgnore]
		public BitmapImage i3 => iN(troops, 3);


		[JsonIgnore]
		public string sc0 => cN(sumDef, 0);
		[JsonIgnore]
		public BitmapImage si0 => iN(sumDef, 0);

		[JsonIgnore]
		public string sc1 => cN(sumDef, 1);
		[JsonIgnore]
		public BitmapImage si1 => iN(sumDef, 1);


		[JsonIgnore]
		public string sc2 => cN(sumDef, 2);
		[JsonIgnore]
		public BitmapImage si2 => iN(sumDef, 2);

		[JsonIgnore]
		public string sc3 => cN(sumDef, 3);
		[JsonIgnore]
		public BitmapImage si3 => iN(sumDef, 3);

		[JsonIgnore]
		public string sc4 => cN(sumDef, 4);
		[JsonIgnore]
		public BitmapImage si4 => iN(sumDef, 4);
		[JsonIgnore]
		public string sc5 => cN(sumDef, 5);
		[JsonIgnore]
		public BitmapImage si5 => iN(sumDef, 5);
		[JsonIgnore]
		public string sc6 => cN(sumDef, 6);
		[JsonIgnore]
		public BitmapImage si6 => iN(sumDef, 6);
		[JsonIgnore]
		public string sc7 => cN(sumDef, 7);
		[JsonIgnore]
		public BitmapImage si7 => iN(sumDef, 7);

		public bool hasSenator => troops.Any((a) => a.isSenator);
        public bool hasNaval => troops.Any((a) => a.isNaval);
        public bool hasArt => troops.Any((a) => a.isArt);

        public float dist => targetCid.DistanceToCid(sourceCid);
		public double distD => targetCid.DistanceToCidD(sourceCid);
		public static string[] reportAttackTypes = { "assault", "siege", "plunder" };
        public long order;
        public string miscInfo { get; set; } = string.Empty;

        public string troopInfo {
            get {
                
                    string rv = miscInfo;
                    foreach (var tt in troops.Enumerate())
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
                 //   return string.Empty;
                   return troops.Format($"{aType}\n{sPlayer} (from)\n{tPlayer} (to)\n{miscInfo}\n{time.Format()}",'\n');
        }

        internal string Format(string delimiter = " ")
        {

            var rv = type == reportPending ? miscInfo : string.Empty;
			troops.ForEach((tt) => rv += tt.Format(delimiter));

            return rv;
        }

		
		internal static int ApproximateKillsFromRefines(int refines)
		{
			// Ranger Equivalent kills based on refines, note troops are worth different amounts, i.e. sorcs 200 vs Vanqs 150.
			return refines * 1000 / 160;
		}

		public override int GetHashCode() => HashCode.Combine(reportId);
		public bool Equals(Army other) => reportId == other.reportId;

		public static bool operator ==(Army left,Army right) => EqualityComparer<Army>.Default.Equals(left,right);
		public static bool operator !=(Army left,Army right) => !(left==right);
	}

	[StructLayout(LayoutKind.Sequential, Pack=8,Size =8)]
	public struct TroopTypeCount : IComparable<TroopTypeCount>
    {
       // public static TroopTypeCounts empty = new(0);
        public int type;
		public int count; // 
		internal readonly float attack => count * ttAttack[type] * ttCombatBonus[type];

//		[JsonIgnore]
      //  public string Count => count.ToString(" N0 ");
        [JsonIgnore]
        public readonly BitmapImage Type => ImageHelper.FromImages($"Icons/troops{type}.png");
		
		public readonly double TravelTimeMinutes(double distance)
		{
			return Troops.TroopTravelMinutes(type, distance);
		}

		public TroopTypeCount(ref TroopTypeCount b)
        {
            type = b.type;
            count = b.count;
        }
        public TroopTypeCount(int _type,int _count)
        {
            type = _type;
            count = _count;
        }

        internal readonly string Format(string delimiter)
        {
            return count > 0?  $"{delimiter}{count:N0} {Troops.ttNameWithCaps[type]}" : (delimiter + Troops.ttNameWithCaps[type]);
        }
        [JsonIgnore]
        public readonly bool isSenator => type == Troops.ttSenator;
        [JsonIgnore]
        public readonly bool isArt  =>  Troops.ttArtillery[type];
        [JsonIgnore]
        public readonly bool isNaval => Troops.ttNavy[type];
        [JsonIgnore]
        public readonly bool isDef => Troops.ttIsDef[type];
		[JsonIgnore]
		public readonly bool isSE => Troops.ttSE[type];


		[JsonIgnore]
        public readonly int ts => Troops.ttTs[type] * count;
		///        public static void SortByTS(in TroopTypeCounts l) => Array.Sort(l.v);

		// Sort greatest TS to least TS
		readonly int IComparable<TroopTypeCount>.CompareTo(TroopTypeCount other)
        {
           return other.ts.CompareTo(ts);
        }
        /// <summary>
        ///  Json Serialization
        /// </summary>
        public byte t { readonly get => (byte)type; set => type = value; }

        public int c { readonly get => count; set => count = value; }
		//	public bool isWaterRaider => IsTTNaval(type);
		//	public bool isNaval => IsTTNaval(type);
		//public static TroopTypeCounts operator +(TroopTypeCounts me, TroopTypeCount tt)
		//{
		//	int counter = 0;
		//	if (tt.count == 0)
		//		return me;
		//	foreach (var i in me)
		//	{
		//		if (i.type == tt.type)
		//		{
		//			var sum = i.count + tt.count;
		//			if (sum == 0)
		//			{
		//				return me.ArrayRemove(counter); // 0 == remove
		//			}
		//			else
		//			{
		//				var rv = me.ArrayClone();
		//				rv[counter] = new TroopTypeCount(tt.type, sum);
		//				return rv;
		//			}
		//		}
		//		++counter;
		//	}

		//	return me.ArrayAppend(new TroopTypeCount(tt));
		//}


		public static void Replace(ref TroopTypeCounts me, ref TroopTypeCounts value)
		{
			unsafe
			{
			for (int i = 0; i < ttCount; ++i)
			{
					me.counts[i] = value.counts[i];

			}
		}
		}

	}
	
	public static class TroopTypeCountHelper
    {

		// Combine opeeration
		//public static void Append(this TroopTypeCounts me,TroopTypeCount  t)
		//{
		//	if (t.count <= 0)
		//		return;
		//	for (int i = 0; i < me.count; ++i)
		//	{
		//		ref var v =ref me[i];
		//		if( v.type == t.type )
		//		{
		//			v.count += t.count;
		//			return;
		//		}
		//	}
			
		//	me.Add(in t);
		//}
		//public static void Append(this TroopTypeCounts me, TroopTypeCounts other, Func<TroopTypeCount,bool> pred )
		//{
		//	foreach (var t in other)
		//	{
		//		if (t.count <= 0 || !pred(t))
		//			continue;
		//		for (int i = 0; i < me.count; ++i)
		//		{
		//			ref var v = ref me[i];
		//			if (v.type == t.type)
		//			{
		//				v.count += t.count;
		//				goto __next;
		//			}
		//		}
		//		me.Add(in t);
		//		__next:;
		//	}
		//}
		//public static void Append(this TroopTypeCounts me, TroopTypeCounts other)
		//{
		//	foreach (var t in other)
		//	{
		//		if (t.count <= 0 )
		//			continue;
		//		for (int i = 0; i < me.count; ++i)
		//		{
		//			ref var v = ref me[i];
		//			if (v.type == t.type)
		//			{
		//				v.count += t.count;
		//				goto __next;
		//			}
		//		}
		//		me.Add(in t);
		//	__next:;
		//	}
		//}


		public static bool IsSuperSetOf(in this TroopTypeCounts me,TroopTypeCounts other)
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					var c = other.counts[i];
					if(c!=0)
					{
						var mv = me.counts[i];
						if (mv == 0 || mv < c)
							return false;
					}
				}
				return true;
			}
		}
        public static string Format(in this TroopTypeCounts l,string header,char firstSeparater,char furtherSeparator=(char)0)
        {
			using var sb = ZString.CreateUtf8StringBuilder();
			sb.Append( header );
			unsafe
			{
				for (int tt = 0; tt < ttCount; ++tt)
				{
					var c = l.counts[tt];
					if (c == 0)
						continue;

					sb.Append(firstSeparater);
					if (c > 0)
						sb.AppendFormat("{0:N0} {1}", c, Troops.ttNameWithCaps[tt]);
					else
					{
						sb.Append(Troops.ttNameWithCaps[tt]);
					}
					if (furtherSeparator != (char)0)
						firstSeparater = furtherSeparator;
				}
			}

			return sb.ToString(); ;
        }

		//public static string Format(this TroopTypeCounts l, string separator)
		//{
		//	string rv = string.Empty;
		//	string sep = string.Empty;
		//	foreach (var ttc in l)
		//	{
		//		rv += ttc.Format(sep);
		//		sep = separator;
		//	}
		//	return rv;
		//}
		//public static List<TroopTypeCount> GetTroopTypeCount(this JsonElement tt, Func<int, bool> filter = null)
		//      {
		//          var tc = new List<TroopTypeCount>();
		//          if (tt.ValueKind == JsonValueKind.Object)
		//          {
		//              foreach (var a in tt.EnumerateObject())
		//              {
		//                  if (int.TryParse(a.Name, out var tType))
		//                  {
		//                      if (filter == null || filter(tType))
		//                      {
		//                          var count = a.Value.GetAsInt();
		//                          if (count > 0)
		//                          {
		//                              tc.Add(new TroopTypeCount(tType, count));
		//                          }
		//                      }
		//                  }

		//              }
		//          }
		//          else if (tt.ValueKind == JsonValueKind.Array)
		//          {
		//              var tType = -1;
		//              foreach (var a in tt.EnumerateArray())
		//              {
		//                  ++tType;
		//                  if (filter == null || filter(tType))
		//                  {
		//                      var count = a.GetInt32();
		//                      if (count > 0)
		//                      {
		//                          tc.Add(new TroopTypeCount(tType, count));
		//                      }
		//                  }

		//              }
		//          }
		//          return tc;

		//      }
		//public static List<TroopTypeCount> GetTroopTypeCount2(this JsonElement tt, Func<int, bool> filter = null)
  //      {
  //          var tc = new List<TroopTypeCount>();
  //          if (tt.ValueKind == JsonValueKind.Array)
  //          {
  //              foreach (var a in tt.EnumerateArray())
  //              {
  //                  var tType = a.GetAsInt("tt");
  //                  var count = a.GetAsInt("tv");
  //                  tc.Add(new TroopTypeCount(tType, count));
                    
                    

  //              }
  //          }
  //          return tc;

  //      }

        // approximation using players speed bonus
        public static double TravelTimeMinutes(this byte tt,int cid0, int cid1)
        {
			return Troops.TroopTravelMinutes(tt, cid0.DistanceToCid(cid1) );
        }


		

		internal static TroopTypeCounts DividedBy(in this TroopTypeCounts me,int count)
		{
			var rv =new TroopTypeCounts();
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					var c = me.counts[i];
					if (c > 0)
						rv.counts[i] = c / count;
					else
						rv.counts[i] = c;

	//				rv.Add(new TroopTypeCount { type = i, count = c / count });
				}
			}
			return rv;
		}
		internal static void Add(ref TroopTypeCounts me, TroopTypeCount ttc)
		{
			unsafe
			{
				me.counts[ttc.type] += ttc.count;
			}
		}
		internal static void Add(ref TroopTypeCounts me,in TroopTypeCounts add)
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					me.counts[i] += add.counts[i];
				}
			}
		}
		public static void Add(ref TroopTypeCounts me, in TroopTypeCounts a, Func<TroopTypeCount, bool> pred)
		{
			unsafe
			{
				for (int i = 0; i < ttCount; ++i)
				{
					var c = a.counts[i];
					if (c == 0 || !pred(new TroopTypeCount(i, c)))
						continue;
					me.counts[i] += a.counts[i];
				}
			}
		}
		internal static void Set(ref TroopTypeCounts me, TroopTypeCount ttc)
		{
			unsafe
			{
				me.counts[ttc.type] = ttc.count;
			}
		}
		internal static void Clear(ref TroopTypeCounts me)
		{
			for (int i = 0; i < ttCount; ++i)
			{
				unsafe { me.counts[i] = 0; }
			}

		}
		internal static void Set(ref TroopTypeCounts me, int type, int count)
		{
			unsafe
			{
				me.counts[type] = count;
			}
		}
		public static int Set(ref TroopTypeCounts me, System.Text.Json.JsonElement tts)
		{
			int ts = 0;
			me.Clear();
			if (tts.ValueKind == JsonValueKind.Array)
			{
				int counter = 0;
				foreach (var t in tts.EnumerateArray())
				{
					var tc = t.GetInt32();
					if (tc > 0)
					{
						var tt = new TroopTypeCount(counter, tc);
						Set(ref me, tt);
						ts += tt.ts;
					}
					++counter;
				}
			}
			else if (tts.ValueKind == JsonValueKind.Object)
			{
				foreach (var at in tts.EnumerateObject())
				{
					var tt = new TroopTypeCount(int.Parse(at.Name), at.Value.GetAsInt());
					Set(ref me, tt);
					ts += tt.ts;
				}
			}
			else
			{
				Debug.Assert(false);
			}
			return ts;
		}

		public static void Set2(ref TroopTypeCounts me, System.Text.Json.JsonElement tts)
		{
			me.Clear();
			if (tts.ValueKind == JsonValueKind.Array)
			{
				foreach (var a in tts.EnumerateArray())
				{
					var tType = a.GetAsInt("tt");
					var count = a.GetAsInt("tv");
					Set(ref me, new TroopTypeCount(tType, count));
				}
			}
			else
			{
				Debug.Assert(false);
				Set(ref me,tts);
			}
		}

	}
}

