using COTG.Game;
using COTG.Helpers;
using COTG.Services;
using COTG.Views;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using static COTG.Game.Enum;
using static COTG.Debug;

namespace COTG.JSON
{
    public static class OutgoingOverview
    {
        public static bool updateInProgress;
        public static bool hasFetchedReports; // sticky bit.  Once reports have been fetched at least once, all calls to update will process all report history

        // uses Report.Hash(), can have several reports per reportId





        public async static Task Process(bool fetchReports)
        {
            if (fetchReports && !hasFetchedReports)
            {
                hasFetchedReports = true;

                if (updateInProgress)
                {
                    // If there is one in progress that did not fetch history and this time we want history, we need to wait and the start a new fetch right after the prior one completes
                    for (; ; )
                    {
                        await Task.Delay(1000);
                        if (!updateInProgress)
                            break;
                    }

                }
            }

            if (updateInProgress)
                return;

            updateInProgress = true;


            for (; ; )
            {
                if (!Alliance.all.IsNullOrEmpty())
                    break;
                await Task.Delay(1000);
            }

            var reportCache = new Dictionary<int, Army[]>();
            foreach (var r in HitTab.instance.history)
            {
                if (r.reportId.IsNullOrEmpty())
                    continue;
                var hash = Army.ReportHash(r.reportId);
                if (reportCache.TryGetValue(hash, out var reports))
                {
                    reportCache[hash] = reports.ArrayAppend(r);
                }
                else
                {
                    reportCache[hash] = new[] { r };
                }
            }

            // ConcurrentDictionary<int, Attack> attacks = new ConcurrentDictionary<int, Attack>();
            var reportParts = new[] { new List<Army>(), new List<Army>(), new List<Army>(), new List<Army>() };
            var reportsOutgoing = new List<Army>();
            
              var task0 = Task.Run(async () =>
             {
                 try
                 {
                     //ConcurrentDictionary<int, Report> rs = new ConcurrentDictionary<int, Report>();
                     using (var jsd = await Post.SendForJson("overview/outover.php", "a=0"))
                     {
                         foreach (var spot in Spot.allSpots)
                         {
                             // is this a safe time to do this?
                             spot.Value.incoming = Army.empty; // Todo:  this wipes incoming as well, we may want to preserve it at some point
                             spot.Value.claim = 0;
                         }

                         {
                             var jse = jsd.RootElement.GetProperty("a");
                             foreach (var b in jse.EnumerateArray())
                             {
                                 var atkCid = b[12].GetAsInt(); // DecodeCid(5, b.GetString("attacker_locatuin"));
                                 var defCid = b[13].GetAsInt(); // DecodeCid(5, b.GetString("defender_location"));
                                 var atkP = Player.NameToId(b[7].GetAsString());
                                 var defP = Player.NameToId(b[2].GetAsString());

                                 var spot = Spot.GetOrAdd(defCid, b[4].GetAsString());
                                 //         var spotted = b[6].GetString().ParseDateTime();
                                 var time = b[6].GetAsString().ParseDateTime();// b.GetString("arrival").ParseDateTime();

                                 var army = new Army() { isAttack=true, sourceCid = atkCid, targetCid = defCid,  time = time, spotted = AUtil.dateTimeZero };

                                 army.type = (byte)GetReportType(b[1].GetAsString());
                                 var claim = b[11].GetAsFloat().RoundToInt();
                                 spot.claim = (byte)claim.Max(spot.claim);
                                 var atkTS = b[9].GetAsInt();
                                 var defTS = b[10].GetAsInt();

                                 army.troops = new[] { new TroopTypeCount(Game.Enum.ttVanquisher, atkTS) };
                                 if (defTS > 0)
                                 {
                                     army.sumDef = new[] { new TroopTypeCount(Game.Enum.ttRanger, defTS) };
                                     if (!spot.isMine)
                                     {
                                         spot.tsHome = defTS;
                                     }
                                 }
                                 //                            army.sumDef = Array.Empty<TroopTypeCount>();
                                 spot.incoming = spot.incoming.ArrayAppend(army);
                                 reportsOutgoing.Add(army);
                                 //var rep = new Army();
                                 //rep.atkCid = atkCid;
                                 //rep.defCid = defCid;
                                 //rep.spotted = AUtil.dateTimeZero;

                                 //rep.atkP = atkP;
                                 //rep.defP = defP;
                                 //rep.atkCid = atkCid;
                                 //rep.defCid = defCid;
                                 //rep.time = time;
                                 //rep.aTS = atkTS;
                                 //rep.dTS = defTS;
                                 //rep.dTsLeft = rep.dTS;
                                 //rep.aTsLeft = rep.aTS;
                                 //rep.type = army.type;
                                 //rep.claim = claim;
                                 //rep.Sen = rep.claim > 0.0f;

                                 //reportsOutgoing.Add(rep);
                             }



                         }
                     }
                 }
                 catch (Exception e)
                 {
                     Log(e);
                 }
             });


            int fetched = 0;
            if (hasFetchedReports)
            {
                // hits history
                using (var jsd = await Post.SendForJson("includes/ofdf.php", "a=1"))
                {
                    //int counter = 0;
                    await jsd.RootElement.EnumerateArray().ToArray().ParallelForAsync4(reportParts,
                        async (inc, part, index, parts) =>
                        {

                        var target = inc[15].GetAsInt();
                        if (target <= 0)
                            return;
                        var type = GetReportType(inc[0].GetAsString());
                        var defP = Player.NameToId(inc[1].GetAsString());
                        var atkPNS = inc[6].GetString();
                        var defCN = inc[3].ToString();
                        Spot.GetOrAdd(target, defCN);
                        var time = inc[5].GetString().ParseDateTime(false);
                        var source = inc[14].GetAsInt().Max(0);
                        var recId = inc[11].GetAsString();
                        var hash = Army.ReportHash(recId);
                        if (reportCache.TryGetValue(hash, out var reports))
                        {
                            foreach (var r in reports)
                            {
                                parts[part].Add(r);
                            }
                        }
                        else
                        {
                            if (source > 0)
                            {
                                // Scout
                                // this is a scout
                                var ts = inc[8].GetAsInt();
                                Assert((ts & 1) == 0);
                                    var report = new Army()
                                    {
                                        troops= new[] { new TroopTypeCount(ttScout, ts / 2) },
                                        sourceCid = source,
                                        targetCid = target,
                                        isAttack = true,
                                        //  atkCN = inc[14].GetAsString(),
                                        // defP = defP,
                                        
                                        time = time,
                                        reportId = recId,
                                        spotted = time - TimeSpan.FromMinutes(target.CidToWorld().Distance(source.CidToWorld()) * TTTravel(ttScout)),// TODO!
                                        type = (byte)type
                                        // todo TS info

                                    };
                                    parts[part].Add(report);


                                }
                                else
                                {

                                    // we have to look up the report
                                    // we have to look up the report
                                    using (var jsdr = await Post.SendForJson("includes/gFrep2.php", "r=" + recId))
                                    {
                                        ++fetched;
                                        var root = jsdr.RootElement;
                                        int reportType = -1;
                                        foreach (var attackType in Army.reportAttackTypes)
                                        {
                                            ++reportType;
                                            if (root.TryGetProperty(attackType, out var reportsByType))
                                            {
                                                //    var defTS = reportsByType.GetAsInt("ts_sent");
                                                //    var defTSLeft = reportsByType.GetAsInt("ts_lost");
                                                var defTSKilled = reportsByType.GetAsInt("ts_killed");

                                                foreach (var report in reportsByType.GetProperty("reports").EnumerateArray())
                                                {

                                                    var troops = TroopTypeCount.empty;
                                                    var sumDef = TroopTypeCount.empty; 
                                                    if (report.TryGetProperty("ats", out var ats))
                                                    {
                                                        foreach (var at in ats.EnumerateObject())
                                                        {

                                                            var tt = int.Parse(at.Name);
                                                            var tc = at.Value.GetAsInt();
                                                            troops = troops.ArrayAppend(new TroopTypeCount() { type = tt, count = tc });
                                                        }
                                                    }
                                                    int defTS = 0;
                                                    int defTSLeft = 0;
                                                    if (report.TryGetProperty("tts", out var tts))
                                                    {
                                                        int counter = 0;
                                                        foreach (var t in tts.EnumerateArray())
                                                        {
                                                            var tc = t.GetInt32();
                                                            if(tc > 0)
                                                            {
                                                                sumDef = sumDef.ArrayAppend(new TroopTypeCount() { type = counter, count = tc });
                                                                defTS += tc*ttTs[counter];
                                                            }
                                                            ++counter;
                                                        }
                                                    }
                                                    if (report.TryGetProperty("ttle", out var ttle))
                                                    {
                                                        int counter = 0;
                                                        foreach (var t in ttle.EnumerateArray())
                                                        {
                                                            defTSLeft += t.GetInt32() * Game.Enum.ttTs[counter];
                                                            ++counter;
                                                        }
                                                      //  Assert(defTS > 0);
                                                        {
                                                            defTSKilled = defTS - defTSLeft;  // overwrite with calculated value
                                                        }
                                                    }


                                                    source = report.GetAsInt("acid");
                                                    if (source > 0)
                                                    {
                                                        var atkTS = report.GetAsInt("ts_sent");
                                                        var atkTSLeft = report.GetAsInt("ts_left");
                                                        var atkPN = report.GetAsString("apn");
                                                        var atkCN = report.GetAsString("acn");
                                                        {
                                                            var sss = atkCN.Split('(',StringSplitOptions.RemoveEmptyEntries);
                                                            if (sss.Length > 0)
                                                                atkCN = sss[0];

                                                        }
                                                        Spot.GetOrAdd(source, atkCN);

                                                        var rep = new Army()
                                                        {
                                                            isAttack = true,
                                                            troops = troops,
                                                            sumDef = sumDef,
                                                            reportId = recId,

                                                            dTsKill = defTSKilled,
                                                            aTsKill = (atkTS - atkTSLeft),
                                                            sourceCid = source,
                                                            targetCid = target,
                                                            claim = (byte)report.GetAsFloat("senator").RoundToInt(),

                                                            time = time,
                                                            spotted = time - TimeSpan.FromMinutes(target.CidToWorld().Distance(source.CidToWorld()) * TTTravel(ttVanquisher)),
                                                            type = (byte)reportType
                                                            // todo TS info

                                                        };
                                                        parts[part].Add(rep);
                                                    }
                                                    else
                                                    {
                                                        Log("Error!");
                                                    }

                                                }
                                            }
                                        }

                                    }
                                }
                            }

                        });
                }

            }

            await task0;

            App.DispatchOnUIThreadLow(() => 
            {
                {
                    var page = HitTab.instance;
                    for (int i = 0; i < reportParts.Length; ++i)
                        reportsOutgoing.AddRange(reportParts[i]);
                    // App.DispatchOnUIThread(() =>
                    // We should do this on the Render Thread
                    page.SetHistory((reportsOutgoing.OrderByDescending((atk) => atk.time.Ticks)).ToArray());
                }

                OutgoingTab.NotifyOutgoingUpdated();

                updateInProgress = false;
                Note.Show($"Complete: {reportsOutgoing.Count} attacks {fetched} fetched");
            });
        }

    }    

}
/*
 * 
 */
