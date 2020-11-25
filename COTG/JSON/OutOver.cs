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

        // uses Report.Hash(), can have several reports per reportId





        public async static Task Process(bool fetchReports)
        {
            if (true)
            {
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



            // ConcurrentDictionary<int, Attack> attacks = new ConcurrentDictionary<int, Attack>();
            var reportParts = new[] { new List<Army>(), new List<Army>(), new List<Army>(), new List<Army>() };
            var reportsOutgoing = new List<Army>();
            using (new ShellPage.WorkScope("fetch outgoing"))
            using (var defenders = new ConcurrentHashSet<Spot>())
            {
                var task0 = Task.Run(async () =>
                {
                    try
                    {
                        //ConcurrentDictionary<int, Report> rs = new ConcurrentDictionary<int, Report>();
                        using (var jsd = await Post.SendForJson("overview/outover.php", "a=0"))
                        {
                            foreach (var spot in Spot.defendersO)
                            {
                                // is this a safe time to do this?
                                spot.incoming = Army.empty; // Todo:  this wipes incoming as well, we may want to preserve it at some point
                                spot.claim = 0;
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

                                    var serverTime = JSClient.ServerTime();
                                    var spotted = time - TimeSpan.FromMinutes(atkCid.DistanceToCid(defCid) * TTTravel(ttVanquisher));
                                    if (spotted > serverTime)
                                        spotted = serverTime;
                                    var army = new Army()
                                    {
                                        isAttack = true,
                                        sourceCid = atkCid,
                                        targetCid = defCid,
                                        time = time,
                                        spotted = spotted
                                    };

                                    army.type = (byte)GetReportType(b[1].GetAsString());
                                    var claim = b[11].GetAsFloat().RoundToInt();
                                    spot.claim = (byte)claim.Max(spot.claim);
                                    var atkTS = b[9].GetAsInt();
                                    var defTS = b[10].GetAsInt();
                                    var attacker = Spot.GetOrAdd(atkCid);
                                    if (attacker.isClassified)
                                    {
                                        var type = attacker.GetPrimaryTroopType(false);

                                        army.troops = new[] { new TroopTypeCount(type, atkTS/ttTs[type]) };

                                    }
                                    else
                                    {
                                        attacker.ClassifyIfNeeded();
                                        army.troops = new[] { new TroopTypeCount(Game.Enum.ttVanquisher, atkTS) };
                                    }
                                    if (defTS > 0)
                                    {
                                        army.sumDef = new[] { new TroopTypeCount(Game.Enum.ttGuard, defTS) };
                                        if (!spot.isMine)
                                        {
                                            spot.tsHome = defTS;
                                        }
                                    }
                                    //                            army.sumDef = Array.Empty<TroopTypeCount>();
                                    spot.incoming = spot.incoming.ArrayAppend(army);
                                    defenders.Add(spot);
                                    //   defenders.Add(spot);
                                    if (fetchReports)
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
                if (fetchReports)
                {
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
                                    using (var jsdr = await Post.SendForJson("includes/gFrep2.php", "r=" + recId))
                                    {
                                        ++fetched;
                                        // scout
                                        if (source > 0)
                                        {
                                            // Scout
                                            // this is a scout
                                            var dts = inc[9].GetAsInt();
                                            var ats = inc[8].GetAsInt();
                                            //       Assert((ats & 1) == 0);
                                            var report = new Army()
                                            {
                                                troops = new[] { new TroopTypeCount(ttScout, ats / 2) },
                                                sourceCid = source,
                                                targetCid = target,
                                                isAttack = true,
                                                //  atkCN = inc[14].GetAsString(),
                                                // defP = defP,

                                                time = time,
                                                reportId = recId,
                                                spotted = time - TimeSpan.FromMinutes(target.CidToWorld().Distance(source.CidToWorld()) * TTTravel(ttScout)),// TODO!
                                                type = reportScout,
                                                // todo TS info

                                            };
                                            if (jsdr != null && jsdr.RootElement.TryGetProperty("tts", out var tts))
                                            {
                                                int counter = 0;
                                                var defTS = 0;
                                                var sumDef = TroopTypeCount.empty;
                                                foreach (var t in tts.EnumerateArray())
                                                {
                                                    var tc = t.GetInt32();
                                                    if (tc > 0)
                                                    {
                                                        sumDef = sumDef.ArrayAppend(new TroopTypeCount() { type = counter, count = tc });
                                                        defTS += tc * ttTs[counter];
                                                    }
                                                    ++counter;
                                                }
                                                //Assert(defTS == dts);
                                                report.sumDef = sumDef;
                                            }
                                            else
                                            {

                                                report.sumDef = new[] { new TroopTypeCount(ttGuard, dts) };
                                            }
                                            parts[part].Add(report);
                                            await Cosmos.AddBattleRecord(report);

                                        }
                                        else
                                        {

                                            // we have to look up the report
                                            // we have to look up the report
                                            if (jsdr != null)
                                            {
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
                                                                    if (tc > 0)
                                                                    {
                                                                        sumDef = sumDef.ArrayAppend(new TroopTypeCount() { type = counter, count = tc });
                                                                        defTS += tc * ttTs[counter];
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
                                                                    var sss = atkCN.Split('(', StringSplitOptions.RemoveEmptyEntries);
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
                                                                await Cosmos.AddBattleRecord(rep);

                                                            }
                                                            else
                                                            {
                                                                Log("Error!");
                                                            }

                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                Log("Bad!");
                                            }

                                        }
                                    }
                                }

                            });
                    }

                }

                await task0;
                Spot.defendersO = defenders.ToArray();
                App.DispatchOnUIThreadLow(() =>
                {

                    var killNote = "";
                    if (fetchReports)
                    {
                        var page = HitTab.instance;
                        for (int i = 0; i < reportParts.Length; ++i)
                            reportsOutgoing.AddRange(reportParts[i]);
                        // App.DispatchOnUIThread(() =>
                        // We should do this on the Render Thread
                        page.SetHistory((reportsOutgoing.OrderByDescending((atk) => atk.time.Ticks)).ToArray());
                        var defKilled = 0;
                        var atkKilled = 0;
                        var myDefKilled = 0;
                        var myAtkKilled = 0;
                        foreach (var i in reportsOutgoing)
                        {
                            if (i.aTsKill > 0 && i.dTsKill > 0)
                            {
                                defKilled += i.dTsKill;
                                atkKilled += i.aTsKill;
                                if (i.sPid == Player.myId || i.tPid == Player.myId)
                                {
                                    myDefKilled += i.dTsKill;
                                    myAtkKilled += i.aTsKill;
                                }
                            }
                        }
                        killNote= $", {atkKilled:N0}({myAtkKilled:N0})TS atk killed, {defKilled:N0}({myDefKilled:N0})TS def Killed";
                    }
                    App.CopyTextToClipboard(killNote);

                    OutgoingTab.NotifyOutgoingUpdated();

                    updateInProgress = false;
                    Note.Show($"Complete: {reportsOutgoing.Count} attacks {fetched} fetched records {Cosmos.battleRecordsUpserted}{killNote}");
                });
            }
        }

    }

}
/*
 * 
 */
