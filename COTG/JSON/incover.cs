using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using COTG.Views;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using static COTG.Debug;

namespace COTG.JSON
{
    public static class IncomingOverview 
    {
            public static Task ParallelForEachAsync<T>(this IEnumerable<T> source, Func<T, Task> funcBody, int maxDoP = 7)
            {
                async Task AwaitPartition(IEnumerator<T> partition)
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        { await funcBody(partition.Current); }
                    }
                }

                return Task.WhenAll(
                    Partitioner
                        .Create(source)
                        .GetPartitions(maxDoP)
                        .AsParallel()
                        .Select(p => AwaitPartition(p)));
            }

            public static Task ParallelForEachAsync<T1, T2>(this IEnumerable<T1> source, Func<T1, T2, Task> funcBody, T2 inputClass, int maxDoP = 7)
            {
                async Task AwaitPartition(IEnumerator<T1> partition)
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        { await funcBody(partition.Current, inputClass); }
                    }
                }

                return Task.WhenAll(
                    Partitioner
                        .Create(source)
                        .GetPartitions(maxDoP)
                        .AsParallel()
                        .Select(p => AwaitPartition(p)));
            }

            public static Task ParallelForEachAsync<T1, T2, T3>(this IEnumerable<T1> source, Func<T1, T2, T3, Task> funcBody, T2 inputClass, T3 secondInputClass, int maxDoP = 7)
            {
                async Task AwaitPartition(IEnumerator<T1> partition)
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        { await funcBody(partition.Current, inputClass, secondInputClass); }
                    }
                }

                return Task.WhenAll(
                    Partitioner
                        .Create(source)
                        .GetPartitions(maxDoP)
                        .AsParallel()
                        .Select(p => AwaitPartition(p)));
            }

            public static Task ParallelForEachAsync<T1, T2, T3, T4>(this IEnumerable<T1> source, Func<T1, T2, T3, T4, Task> funcBody, T2 inputClass, T3 secondInputClass, T4 thirdInputClass, int maxDoP = 7)
            {
                async Task AwaitPartition(IEnumerator<T1> partition)
                {
                    using (partition)
                    {
                        while (partition.MoveNext())
                        { await funcBody(partition.Current, inputClass, secondInputClass, thirdInputClass); }
                    }
                }

                return Task.WhenAll(
                    Partitioner
                        .Create(source)
                        .GetPartitions(maxDoP)
                        .AsParallel()
                        .Select(p => AwaitPartition(p)));
            }
       

        const float averageSpeed = 10f;
        const float averageScoutSpeed = 5f;
        static string[] attackTypes = { "assault", "siege","plunder" };
        //        public IncomingOverview() : base("overview/incover.php") { }
        public static int DecodeCid(int offset,string s)
        {
            var x = int.Parse(s.Substring(offset, 3));
            var y = int.Parse(s.Substring(offset+4, 3));
            return x + y * 65536;
        }
        public static int TryDecodeCid(int offset, string s)
        {
            var lg = s.Length;
            if (lg < offset + 7)
                return -1;
            if (!int.TryParse(s.Substring(offset, 3), out var x))
                return -1;
            if (!int.TryParse(s.Substring(offset+4, 3), out var y))
                return -1;

            return x + y * 65536;
        }

        public async static Task Process(bool fetchHistory)
        {
            for (; ; )
            {
                if (!Alliance.all.IsNullOrEmpty())
                    break;
                await Task.Delay(5000);
            }

            ConcurrentDictionary<int, Attack> attacks = new ConcurrentDictionary<int, Attack>();
            ConcurrentDictionary<int, Report> rs = new ConcurrentDictionary<int, Report>();
            using (var jsd = await Post.SendForJson("overview/incover.php", "a=0"))
            {
                var jse = jsd.RootElement.GetProperty("b");
                foreach (var b in jse.EnumerateArray())
                {
                    Attack attack = new Attack();
                    attack.sourceCid = DecodeCid(5,b.GetString("attacker_locatuin"));
                    attack.targetCid = DecodeCid(5,b.GetString("defender_location"));
                    var spotted = b.GetString("spotted");
                    var arrival = b.GetString("arrival");

                    attack.spotted = spotted.ParseDateTime();
                    attack.time = (arrival).ParseDateTime();
                    attacks.TryAdd(attack.GetHashCode(),attack);

                    Report rep = new Report();
                    rep.atkCid = attack.sourceCid;
                    rep.defCid = attack.targetCid;
                    rep.time = attack.time;
                    rep.spotted = attack.spotted;
                    rep.atkCN = b.GetAsString("attacker_city");
                    rep.defCN = b.GetAsString("defender_city");
                    rep.atkTS = b.GetAsInt("attack_ts");
                    rep.defTS = b.GetAsInt("defender_ts");
                    rep.type = b.GetString("attack_type") == "Sieging" ? Report.typeSiege : Report.typeUnknown;
                    rep.claim = b.GetAsFloat("baron cap %");
                    rs.TryAdd(rep.GetHashCode(),rep);
                }
            }
            if (fetchHistory)
            {
                // defense history
                using (var jsd = await Post.SendForJson("includes/ofdf.php", "a=2"))
                {
                    int counter = 0;
                    await jsd.RootElement.EnumerateArray().ParallelForEachAsync( async (inc) =>
                    {

                        /*
    [
		"Siege", //0
		"Falsestep", // 1
		"Phoenix", // 2
		"Pandemonium", // 3
		"220:224", // 4
		"02:15:08 03\/07", // 5
		"arve", // 6
		"-", // 7
		0, // 8
		3230, // 9
		"-", // 10
		"6dad1d830846567f",// 11
		61298, // 12
		0, // 13
		"-", // 14
		14680284, // 15
		{ // 16
			"14483687": 1
		}
	],
	[
		"Scout", // 0
		"Nayawen", // 1
		"Unidos-", // 2
		"C22 004", // 3
		"222:281", // 4
		"22:06:39 02\/07", // 5
		"3MP7Y", // 6
		"225:284", // 7
		44730, // 8  defending TS
		4000, // 9  attacking TS
		"-",// 10
		"489b050779fb3d90", // 11
		2195, // 12
		22, // 13
		"A22_005", // 14
		18415838, // 15 
		18612449 // 16
	],
                         */
                        var target = TryDecodeCid(0, inc[4].GetString());
                        if (target <= 0)
                            return;
                        var defP = Player.NameToId(inc[1].GetString());
                        var atkPNS = inc[6].GetString();
                        var defCN = inc[3].ToString();
                        var time = inc[5].GetString().ParseDateTime();
                        var source = TryDecodeCid(0, inc[7].GetString());
                        var recId = inc[11].GetAsString();
                        if (source > 0)
                        {
                            // Scout
                            // this is a scout
                            var a = new Attack()
                            {
                                // todo: attacker name?
                                sourceCid = source,
                                targetCid = target,
                                time = time,
                                spotted = time - TimeSpan.FromMinutes(target.CidToWorld().Distance(source.CidToWorld()) * averageScoutSpeed)
                            };
                            attacks.TryAdd(a.GetHashCode(), a);
                            var report = new Report()
                            {
                                atkCid = source,
                                defCid = target,
                                defCN=defCN,
                                atkCN = inc[14].GetString(),
                                defP=defP,
                                atkP=Player.NameToId(atkPNS),
                                time = time,
                                reportId = recId,
                                spotted = a.spotted,
                                type = Report.typeScout
                                // todo TS info

                            };
                            rs.TryAdd(report.GetHashCode(), report);


                        }
                        else
                        {
                            // we have to look up the report
                            using (var jsdr = await Post.SendForJson("includes/gFrep2.php", "r=" + recId))
                            {
                                var root = jsdr.RootElement;
                                int reportType = -1;
                                foreach (var attackType in attackTypes)
                                {
                                    ++reportType;
                                    if (root.TryGetProperty(attackType, out var reportsByType))
                                    {
                                        var defTS = reportsByType.GetAsInt("ts_sent");
                                        var defTSLeft = reportsByType.GetAsInt("ts_lost");
                                        var atkTSKilled = reportsByType.GetAsInt("ts_killed");

                                        foreach (var report in reportsByType.GetProperty("reports").EnumerateArray())
                                        {
                                            bool hasSen = false;
                                            bool hasSE = false;
                                            bool hasNavy = false;

                                            if (report.TryGetProperty("ats", out var ats))
                                            {
                                                foreach(var at in ats.EnumerateObject())
                                                {
                                                    //  public static string[] ttName = { 0:"guard", 1:"ballista", 2:"ranger", 3:"triari", 4:"priestess", 5:"vanquisher", 6:"sorcerers", 7:"scout", 8:"arbalist", 9:"praetor", 10:"horseman", 11:"druid", 12:"ram", 13:"scorpion", 14:"galley", 15:"stinger", 16:"warship", 17:"senator" };
                                                    switch (at.Name)
                                                    {
                                                        case "17": hasSen = true; break;
                                                        case "12":
                                                        case "13": hasSE = true;break;
                                                        case "16": hasSE = true;hasNavy = true;break;
                                                        case "14":
                                                        case "15": hasNavy = true; break;

                                                    }
                                                }
                                            }

                                                source = report.GetAsInt("acid");
                                            if (source > 0)
                                            {
                                                var a = new Attack()
                                                {
                                                    sourceCid = source,
                                                    targetCid = target,
                                                    time = time,
                                                    spotted = time - TimeSpan.FromMinutes(target.CidToWorld().Distance(source.CidToWorld()) * averageSpeed)
                                                };
                                                attacks.TryAdd(a.GetHashCode(), a);
                                                var atkTS = report.GetAsInt("ts_sent");
                                                var atkTSLeft = report.GetAsInt("ts_left");
                                                var atkPN = report.GetAsString("apn");
                                                var rep = new Report()
                                                {
                                                    reportId = recId,
                                                    defTS =defTS,
                                                    defTSLeft=defTSLeft,
                                                    atkTSKilled=atkTSKilled.Max(atkTS-atkTSLeft),
                                                    atkTS = atkTS,
                                                    atkTSLeft = atkTSLeft,
                                                    atkCid = source,
                                                    defCid = target,
                                                    hasSE=hasSE,
                                                    isNaval=hasNavy,
                                                    hasSen=hasSen,
                                                    claim = hasSen&& root.GetAsString("senatorapn")==atkPN ? root.GetAsFloat("senator") : -1,
                                                    defCN = defCN,
                                                    atkCN = report.GetAsString("acn"),
                                                    defP = defP,
                                                    atkP = Player.NameToId(atkPN),
                                                    time = time,
                                                    spotted = a.spotted,
                                                    type = (byte)reportType
                                                    // todo TS info

                                                };
                                                if(!rs.TryAdd(rep.GetHashCode(), rep) )
                                                {
                                                    Assert(false);
                                                }
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        if ((counter++ & 63) == 0)
                        {
                            Note.L("Attacks " + attacks.Count);
                        }
                    },7);
                }

             }

            var defPage = DefensePage.cache;
            if (defPage != null)
                defPage.history.Reset(rs.Values);

             Attack.attacks = attacks.Values.ToArray();
            Note.Show($"Complete: {Attack.attacks.Length} attacks");
        }
    }

}
/*
 * 
 */
