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
    public static class IncomingOverview  
    {
        public static bool updateInProgress;
        public static bool hasFetchedReports; // sticky bit.  Once reports have been fetched at least once, all calls to update will process all report history
        static ConcurrentHashSet<int> supportReports = new ConcurrentHashSet<int>();

        // uses Report.Hash(), can have several reports per reportId



        //        public IncomingOverview() : base("overview/incover.php") { }


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
            foreach(var r in DefensePage.instance.history)
            {
                if (r.reportId.IsNullOrEmpty())
                    continue;
                var hash = Army.ReportHash(r.reportId);
                if(reportCache.TryGetValue(hash,out var reports))
                {
                    reportCache[hash] = reports.ArrayAppend(r);
                }
                else
                {
                    reportCache[hash] = new[] { r };
                }
            }

            var reportParts = new[] { new List<Army>(), new List<Army>(), new List<Army>(), new List<Army>() };
            var reportsIncoming = new List<Army>();
            int incCount = 0;

            var task0 = Task.Run(async () =>
              {
              //ConcurrentDictionary<int, Army> rs = new ConcurrentDictionary<int, Army>();
              using (var jsd = await Post.SendForJson("overview/incover.php", "a=0"))
              {
                  //{
                  //    var estimator = new Army();
                  //    var jse = jsd.RootElement.GetProperty("b");
                  //    foreach (var b in jse.EnumerateArray())
                  //    {
                  //        var atkCid = DecodeCid(5, b.GetString("attacker_locatuin"));
                  //        var defCid = DecodeCid(5, b.GetString("defender_location"));

                  //        var spotted = b.GetString("spotted").ParseDateTime();
                  //        var time = b.GetString("arrival").ParseDateTime();

                  //        Army rep = new Army();
                  //        rep.atkP = Player.NameToId(b.GetAsString("attacker_player"));
                  //        rep.defP = Player.NameToId(b.GetAsString("defender_player"));
                  //        rep.atkCid = atkCid;
                  //        rep.defCid = defCid;
                  //        rep.time = time;
                  //        rep.spotted = spotted;
                  //        Spot.GetOrAdd(atkCid, b.GetAsString("attacker_city"));
                  //        Spot.GetOrAdd(defCid, b.GetAsString("defender_city"));
                  //        rep.aTS = b.GetAsInt("attack_ts");
                  //        rep.dTS = b.GetAsInt("defender_ts");
                  //        rep.dTsLeft = rep.dTS;
                  //        rep.aTsLeft = rep.aTS;
                  //        rep.type = b.GetString("attack_type") == "Sieging" ? Report.typeSieging : Report.typePending;

                  //        rep.claim = (byte)b.GetAsFloat("baron cap %");
                          
                  //        rep.Sen = rep.claim > 0.0f;
                  //         if (rep.type == Report.typePending)
                  //            {
                  //                estimator.sourceCid = atkCid;
                  //                estimator.targetCid = defCid;
                  //                estimator.time = time;
                  //                estimator.spotted = spotted;
                  //                IncommingEstimate.Get(estimator);
                  //                bool wantComma = false;
                  //                string troops = string.Empty;
                  //                foreach(var tt in estimator.troops)
                  //                {
                  //                    if (wantComma)
                  //                        troops += ", ";
                  //                    else
                  //                        wantComma = true;
                  //                    troops += ttCategory[tt.type];
                  //                    if (tt.count == -1)
                  //                        troops += '?';
                  //                    else
                  //                        troops += $" {(tt.count)/ -10.0f}%";
                  //                }
                  //                estimator.troops = TroopTypeCount.empty;
                  //                rep.troopEstimate = troops;
                  //            }


                  //         reportsIncoming.Add(rep);
                          
                  //    }
                  //}
                  {
                      foreach (var spot in Spot.allSpots)
                      {
                          // is this a safe time to do this?
                          spot.Value.incoming = Army.empty;
                      }
                          if (jsd.RootElement.TryGetProperty("a", out var jse))
                          {
                              foreach (var prop in jse.EnumerateObject())
                              {
                                  ++incCount;
                                  var val = prop.Value;
                                  var cid = AUtil.DecodeCid(5, val.GetString("2"));
                                  if (cid >= 0)
                                  {

                                      var spot = Spot.GetOrAdd(cid, val.GetAsString("1"));
                                      // set info if needed
                                      spot.tsHome = val.GetAsInt("8");
                                      spot.pid = Player.NameToId(val.GetAsString("0"));
                                      spot.claim = (byte)val.GetAsFloat("4").RoundToInt();
                                      try
                                      {
                                          var scoutRange = val.GetAsString("6");
                                          var hrsMark = scoutRange.IndexOf('h');
                                          if (hrsMark >= 1)
                                              spot.scoutRange = float.Parse(scoutRange.Substring(0, hrsMark));
                                          else
                                          {
                                              var minMark = scoutRange.IndexOf('m');
                                              if (minMark >= 1)
                                                  spot.scoutRange = float.Parse(scoutRange.Substring(0, minMark)) / 60.0f;

                                          }
                                      }
                                      catch (Exception e)
                                      {
                                          Log(e);
                                      }
                                      var sumDef = new List<TroopTypeCount>();
                                      var processedTroopsHome = false; // for some reason, these repeat
                                      foreach (var armyV in val.GetProperty("9").EnumerateArray())
                                      {
                                          var army = new Army();
                                          army.isAttack = armyV.GetAsInt("5") != 3;
                                          var armyPid = armyV.GetAsString("1");
                                          //army.pid = armyPid switch
                                          //{
                                          //    "Troops home" => spot.pid,
                                          //    var name => Player.NameToId(name)
                                          //};
                                          var arrival = armyV.GetAsString("7");
                                          army.time = arrival switch
                                          {
                                              "home" => AUtil.dateTimeZero,
                                              "on support" => JSClient.ServerTime() - TimeSpan.FromHours(1),
                                              var t => t.ParseDateTime()
                                          };
                                          army.targetCid = cid;
                                          var home = (arrival == "home");
                                          if (home)
                                          {
                                              if (processedTroopsHome)
                                                  continue;
                                              processedTroopsHome = true;

                                              army.sourceCid = cid;
                                          }
                                          else
                                          {
                                              army.sourceCid = armyV.GetAsInt("11");
                                              //    var sourceSpot = Spot.GetOrAdd(army.sourceCid, army.sPid);

                                          }
                                          if (armyV.TryGetProperty("6", out var p6) && (p6.ValueKind == System.Text.Json.JsonValueKind.String))
                                          {
                                              army.spotted = p6.GetString().ParseDateTime();
                                          }
                                          else
                                          {
                                              army.spotted = army.time - TimeSpan.FromMinutes(cid.DistanceToCid(army.sourceCid) * TTTravel(ttScout));
                                          }
                                          var ttl = new List<TroopTypeCount>();
                                          if (armyV.TryGetProperty("3", out var ttp) && ttp.ValueKind == System.Text.Json.JsonValueKind.Array)
                                          {


                                              foreach (var tt in ttp.EnumerateArray())
                                              {
                                                  var str = tt.GetString();
                                                  int firstSpace = str.IndexOf(' ');

                                                  var type = Game.Enum.ttNameWithCapsAndBatteringRam.IndexOf(str.Substring(firstSpace + 1));
                                                  Assert(type != -1);
                                                  ttl.Add(new TroopTypeCount()
                                                  {
                                                      count = int.Parse(str.Substring(0, firstSpace), System.Globalization.NumberStyles.Number, NumberFormatInfo.InvariantInfo),
                                                      type = type >= 0 ? type : 0
                                                  });

                                              }
                                          }
                                          if (home && ttl.Count == 0)
                                              continue; // empty entries for troops at home when no def is present.
                                          if (army.isDefense)
                                          {
                                              army.type = reportDefensePending;
                                              foreach (var tti in ttl)
                                              {
                                                  var present = false;
                                                  for (int i = 0; i < sumDef.Count; ++i)
                                                  {
                                                      if (sumDef[i].type == tti.type)
                                                      {
                                                          var def = new TroopTypeCount(sumDef[i]);// copy on modify
                                                          def.count += tti.count;
                                                          sumDef[i] = def; // copy
                                                          present = true;
                                                          break;
                                                      }

                                                  }
                                                  if (!present)
                                                      sumDef.Add(tti); // reference in, this is safe as it is unmodified
                                              }
                                          }
                                          else
                                          {
                                              if (ttl.IsNullOrEmpty())
                                              {
                                                  army.type = reportPending;
                                                  COTG.Game.IncommingEstimate.Get(army);
                                              }
                                              else
                                              {
                                                  army.type = reportSieging;

                                              }
                                          }
                                          army.sumDef = sumDef.ToArray();
                                          Array.Sort(army.sumDef);

                                          if (!ttl.IsNullOrEmpty())
                                              army.troops = ttl.ToArray();
                                          Array.Sort(army.troops);
                                          spot.incoming = spot.incoming.ArrayAppend(army);
                                          if(!army.isDefense)
                                              reportsIncoming.Add(army);
                                      }
                                      spot.tsMax = sumDef.TS();
                                  }
                                  else
                                  { Assert(false); }
                              }
                          }
                      }

                  }
              });
            var fetched = 0;
            if (hasFetchedReports)
            {
                // defense history
                using (var jsd = await Post.SendForJson("includes/ofdf.php", "a=2"))
                {
                    //int counter = 0;
                    await jsd.RootElement.EnumerateArray().ToArray().ParallelForAsync4(reportParts,
                        async (_inc,_part,_index,_parts) =>
                        {
                            var inc = _inc;
                            var part = _part;
                            var index = _index;
                            var parts = _parts;


                            var target = AUtil.TryDecodeCid(0, inc[4].GetString());
                        if (target <= 0)
                            return;
                        var defP = Player.NameToId(inc[1].GetString());
                        var atkPNS = inc[6].GetString();
                        var defCN = inc[3].ToString();
                        Spot.GetOrAdd(target, defCN);
                        var time = inc[5].GetString().ParseDateTime(false);
                        var source = AUtil.TryDecodeCid(0, inc[7].GetString());
                        var recId = inc[11].GetAsString();
                        var hash = Army.ReportHash(recId);
                        if (reportCache.TryGetValue(hash, out var reports))
                        {
                            foreach (var r in reports)
                            {
                                parts[part].Add(r);
                            }
                        }
                        else if(supportReports.Contains(hash))
                            {
                                // Nothing needed
                            }
                        else
                        {
                            if (source > 0)
                            {
                                // Scout
                                // this is a scout

                                Spot.GetOrAdd(source, inc[14].GetString());
                                 var dts = inc[8].GetAsInt();
                                    var ats = inc[9].GetAsInt();
                                    var report = new Army()
                                {
                                    sourceCid = source,
                                    targetCid = target,
                                    troops = new[] { new TroopTypeCount(ttScout, ats / 2) },
                                    sumDef = new [] { new TroopTypeCount(ttGuard, dts) },
                                    time = time,
                                    reportId = recId,
                                    spotted = time - TimeSpan.FromMinutes(target.CidToWorld().Distance(source.CidToWorld()) * TTTravel(ttScout)),
                                    type = reportScout,
                                    

                                    // todo TS info

                                };
                                parts[part].Add(report);


                            }
                            else
                            {


                                // we have to look up the report
                                try
                                {
                                    using (var jsdr = await Post.SendForJson("includes/gFrep2.php", "r=" + recId))
                                    {
                                        var root = jsdr.RootElement;
                                        int reportType = -1;
                                        ++fetched;
                                        foreach (var attackType in Army.reportAttackTypes)
                                        {
                                            ++reportType;
                                            if (root.TryGetProperty(attackType, out var reportsByType))
                                            {
                                                var defTS = reportsByType.GetAsInt("ts_sent");
                                                //var defTSLeft = reportsByType.GetAsInt("ts_lost");
                                                var atkTSKilled = reportsByType.GetAsInt("ts_killed");

                                                foreach (var report in reportsByType.GetProperty("reports").EnumerateArray())
                                                {
                                                    if(report.GetAsInt("acid") <= 0)
                                                        {
                                                            supportReports.Add(Army.ReportHash(recId));
                                                            continue;
                                                        }
                                                    var atkTrops = TroopTypeCount.empty;
                                                    var defTrops = TroopTypeCount.empty;
                                                    var defTSLeft = 0;
                                                    var atkCN = report.GetAsString("acn");
                                                    source = AUtil.DecodeCid(atkCN.Length - 8, atkCN);
                                                   
                                                    {
                                                        var lg = atkCN.Length;
                                                        if (lg > 9)  // trim off '(000:000)'
                                                            atkCN = atkCN.Substring(0, lg - 9);

                                                    }


                                                    if (report.TryGetProperty("ats", out var ats))
                                                    {
                                                        foreach (var at in ats.EnumerateObject())
                                                        {
                                                            var tc = at.Value.GetAsInt();
                                                            if(tc > 0)
                                                            {

                                                                atkTrops = atkTrops.ArrayAppend( new TroopTypeCount() { count = tc, type =int.Parse(at.Name) });
                                                            }
                                                        }
                                                    }
                                                    {
                                                        if (report.TryGetProperty("tts", out var tts))
                                                        {
                                                            int tt = 0;
                                                            foreach (var tle in tts.EnumerateArray())
                                                            {
                                                                var tc = tle.GetInt32();
                                                                if (tc > 0)
                                                                {
                                                                    defTrops = defTrops.ArrayAppend(new TroopTypeCount() { count = tc, type = tt });
                                                                }
                                                                ++tt;
                                                            }
                                                        }
                                                    }
                                                    {
                                                        if (report.TryGetProperty("ttle", out var ttle))
                                                        {
                                                            int tt = 0;
                                                            foreach (var tle in ttle.EnumerateArray())
                                                            {
                                                                defTSLeft += tle.GetInt32() * ttTs[tt];
                                                                ++tt;
                                                            }
                                                        }
                                                    }
                                                    if (report.TryGetProperty("reinforcers", out var reinforcers))
                                                    {
                                                        if (reinforcers.ValueKind == System.Text.Json.JsonValueKind.Object)
                                                        {
                                                            foreach (var rein in reinforcers.EnumerateObject())
                                                            {
                                                                if (rein.Value.TryGetProperty("ttle", out var tle))
                                                                {
                                                                    if (tle.ValueKind == System.Text.Json.JsonValueKind.Object)
                                                                    {
                                                                        foreach (var ttc in tle.EnumerateObject())
                                                                        {
                                                                            var tt = int.Parse(ttc.Name);
                                                                            defTSLeft += ttc.Value.GetInt32() * ttTs[tt];
                                                                        }
                                                                    }
                                                                }
                                                                if (rein.Value.TryGetProperty("tts", out var tts))
                                                                {
                                                                    if (tle.ValueKind == System.Text.Json.JsonValueKind.Object)
                                                                    {
                                                                        foreach (var ttc in tle.EnumerateObject())
                                                                        {
                                                                            var tt = int.Parse(ttc.Name);
                                                                            defTrops = defTrops.ArrayAppend(new TroopTypeCount() { count = ttc.Value.GetInt32(), type = tt });
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                    
                                                    if (source > 0)
                                                    {
                                                        var atkTS = report.GetAsInt("ts_sent");
                                                        var atkTSLeft = report.GetAsInt("ts_left");
                                                        var atkPN = report.GetAsString("apn");
                                                        Spot.GetOrAdd(source, atkCN);
                                                        bool hasSen = atkTrops.HasTT(ttSenator);
                                                        var rep = new Army()
                                                        {
                                                            reportId = recId,
                                                            sumDef = defTrops,
                                                            dTsKill = defTS - defTSLeft,
                                                            aTsKill = atkTSKilled.Max(atkTS - atkTSLeft),
                                                            troops = atkTrops,

                                                            sourceCid = source,
                                                            targetCid = target,
                                                            claim = (byte)(  hasSen && root.GetAsString("senatorapn") == atkPN ? root.GetAsFloat("senator") : -1),
                                                            time = time,
                                                            spotted = time - TimeSpan.FromMinutes(target.CidToWorld().Distance(source.CidToWorld()) * TTTravel(ttVanquisher) ),
                                                            type = (byte)reportType
                                                            // todo TS info

                                                        };
                                                        parts[part].Add(rep);
                                                    }
                                                    else
                                                    {
                                                        Log("bad cid?");
                                                    }
                                                }
                                            }
                                        }

                                    }
                                }
                                catch (Exception e)
                                {
                                    Log(e);
                                }


                            }
                        }
                        
                    });
                }

            }

            await task0;

            //if(ShellPage.IsPageDefense())
            App.DispatchOnUIThreadLow(()=>{
                
                    {
                        var defPage = DefensePage.instance;
                        for (int i = 0; i < reportParts.Length; ++i)
                            reportsIncoming.AddRange(reportParts[i]);
                        // App.DispatchOnUIThread(() =>
                        // We should do this on the Render Thread
                        defPage.SetHistory((reportsIncoming.OrderByDescending((atk) => atk.time.Ticks)).ToArray());
                    }
                    {
                        var defenderPage = DefenderPage.instance;
                        if (defenderPage != null)
                            defenderPage.NotifyIncomingUpdated();
                    }

                    updateInProgress = false;
                    Note.Show($"Complete: {reportsIncoming.Count+ incCount} attacks, {fetched} fetched");
                });
        }
    }

}
/*
 * 
 */
