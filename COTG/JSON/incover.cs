using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using J = System.Text.Json.Serialization.JsonPropertyNameAttribute;
using System.Text.Json.Serialization;
using static COTG.Debug;
using COTG.Services;
using COTG.Game;
using COTG.Helpers;
using COTG.Views;
using System.Collections.Concurrent;

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
        static string[] attackTypes = { "assault", "siege" };
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
            ConcurrentDictionary<int, Attack> attacks = new ConcurrentDictionary<int, Attack>();
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
                        var time = inc[5].GetString().ParseDateTime();
                        var source = TryDecodeCid(0, inc[7].GetString());
                        var recId = inc[11].GetAsString();
                        if (source > 0)
                        {
                            // Scout
                            // this is a scout
                            var a = new Attack()
                            {
                                sourceCid = source,
                                targetCid = target,
                                time = time,
                                spotted = time - TimeSpan.FromMinutes(target.CidToWorld().Distance(source.CidToWorld()) * averageScoutSpeed)
                            };
                            attacks.TryAdd(a.GetHashCode(), a);


                        }
                        else
                        {
                            // we have to look up the report
                            using (var jsdr = await Post.SendForJson("includes/gFrep2.php", "r=" + recId))
                            {
                                var reports = jsdr.RootElement;
                                foreach (var attackType in attackTypes)
                                {
                                    if (reports.TryGetProperty(attackType, out var reportsByType))
                                    {
                                        foreach (var report in reportsByType.GetProperty("reports").EnumerateArray())
                                        {
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
                                            }
                                        }
                                    }
                                }

                            }
                        }
                        if ((counter++ & 63) == 0)
                        {
                            Log("Attacks " + attacks.Count);
                        }
                    },7);
                }

             }
             Attack.attacks = attacks.Values.ToArray();
            Note.Show($"Complete: {Attack.attacks.Length} attacks");
        }
    }

}
/*
 * 
 */
