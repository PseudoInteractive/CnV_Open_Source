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

using static COTG.Debug;

namespace COTG.JSON
{
    public static class IncomingOverview 
    {
        public static bool updateInProgress;
        public static bool hasFetchedReports; // sticky bit.  Once reports have been fetched at least once, all calls to update will process all report history

        // uses Report.Hash(), can have several reports per reportId



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
                await Task.Delay(2000);
            }

            var reportCache = new Dictionary<int, Report[]>();
            foreach(var r in DefensePage.instance.history)
            {
                if (r.reportId.IsNullOrEmpty())
                    continue;
                var hash = Report.ReportHash(r);
                if(reportCache.TryGetValue(hash,out var reports))
                {
                    reportCache[hash] = reports.ArrayAppend(r);
                }
                else
                {
                    reportCache[hash] = new[] { r };
                }
            }

            // ConcurrentDictionary<int, Attack> attacks = new ConcurrentDictionary<int, Attack>();
            var reportParts = new[] { new List<Report>(), new List<Report>(), new List<Report>(), new List<Report>() };
            var reportsIncoming = new List<Report>();

            var task0 = Task.Run(async () =>
              {
              //ConcurrentDictionary<int, Report> rs = new ConcurrentDictionary<int, Report>();
              using (var jsd = await Post.SendForJson("overview/incover.php", "a=0"))
              {
                  {
                      var jse = jsd.RootElement.GetProperty("b");
                      foreach (var b in jse.EnumerateArray())
                      {
                          var atkCid = DecodeCid(5, b.GetString("attacker_locatuin"));
                          var defCid = DecodeCid(5, b.GetString("defender_location"));

                          var spotted = b.GetString("spotted").ParseDateTime();
                          var time = b.GetString("arrival").ParseDateTime();

                          Report rep = new Report();
                          rep.atkP = Player.NameToId(b.GetAsString("attacker_player"));
                          rep.defP = Player.NameToId(b.GetAsString("defender_player"));
                          rep.atkCid = atkCid;
                          rep.defCid = defCid;
                          rep.time = time;
                          rep.spotted = spotted;
                          rep.atkCN = b.GetAsString("attacker_city");
                          rep.defCN = b.GetAsString("defender_city");
                          rep.aTS = b.GetAsInt("attack_ts");
                          rep.dTS = b.GetAsInt("defender_ts");
                          rep.dTsLeft = rep.dTS;
                          rep.aTsLeft = rep.aTS;
                          rep.type = b.GetString("attack_type") == "Sieging" ? Report.typeSieging : Report.typePending;
                          rep.claim = b.GetAsFloat("baron cap %");
                          rep.Sen = rep.claim > 0.0f;
                          reportsIncoming.Add(rep);
                      }
                  }
                  {
                      foreach (var spot in Spot.allSpots)
                      {
                          // is this a safe time to do this?
                          spot.Value.incoming.Clear();
                      }
                      var jse = jsd.RootElement.GetProperty("a");
                      foreach (var prop in jse.EnumerateObject())
                      {
                          var val = prop.Value;
                          var cid = DecodeCid(5, val.GetString("2"));
                          if (cid >= 0)
                          {

                              var spot = Spot.GetOrAdd(cid);
                              // set info if needed
                              spot.cityName = val.GetAsString("1");
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
                                  catch(Exception e)
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
                                  army.pid = armyPid switch
                                  {
                                      "Troops home" => spot.pid,
                                      var name => Player.NameToId(name)
                                  };
                                      var arrival = armyV.GetAsString("7");
                                  army.arrival = arrival switch
                                  {
                                      "home" => AUtil.dateTimeZero,
                                      "on support"=> AUtil.dateTimeZero,
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
                                          var sourceSpot = Spot.GetOrAdd(army.sourceCid);
                                          sourceSpot.pid = army.pid;
                                          
                                      }
                                  var p6 = armyV.GetProperty("6");
                                  if (p6.ValueKind == System.Text.Json.JsonValueKind.String)
                                  {
                                      army.spotted = p6.GetString().ParseDateTime();
                                  }
                                  else
                                  {
                                      army.spotted = AUtil.dateTimeZero;
                                  }
                                      var ttp = armyV.GetProperty("3");
                                      var ttl = new List<TroopTypeCount>();
                                      if (ttp.ValueKind==System.Text.Json.JsonValueKind.Array)
                                      {


                                          foreach (var tt in ttp.EnumerateArray())
                                          {
                                              var str = tt.GetString();
                                              int firstSpace = str.IndexOf(' ');
                                              
                                              var type = Game.Enum.ttNameWithCaps.IndexOf(str.Substring(firstSpace+1));
                                              Assert(type != -1);
                                              ttl.Add(new TroopTypeCount()
                                              {
                                                  count = int.Parse(str.Substring(0,firstSpace), System.Globalization.NumberStyles.Number, NumberFormatInfo.InvariantInfo),
                                                  type = type >= 0 ? type : 0
                                              });

                                          }
                                      }
                                      if (home && ttl.Count == 0)
                                          continue; // empty entries for troops at home when no def is present.
                                      if(army.isDefense)
                                      {
                                          foreach(var tti in ttl)
                                          {
                                              var present = false;
                                              for (int i = 0; i < sumDef.Count;++i)
                                              {
                                                  if(sumDef[i].type == tti.type)
                                                  {
                                                      var def = new TroopTypeCount(sumDef[i]);// copy on modify
                                                      def.count += tti.count;
                                                      sumDef[i] = def; // copy
                                                      present = true;
                                                      break;
                                                  }

                                              }
                                              if (!present)
                                                  sumDef.Add( tti ); // reference in, this is safe as it is unmodified
                                          }
                                      }
                                      army.sumDef = sumDef.ToArray();
                                      Array.Sort(army.sumDef);
                                      army.troops = ttl.ToArray();
                                      Array.Sort(army.troops);
                                      spot.incoming.Enqueue(army);
                                  }
                                  spot.tsMax = TroopTypeCount.TS(sumDef);
                              }
                          else
                              { Assert(false); }
                          }
                      }

                  }
              });
            if (hasFetchedReports)
            {
                // defense history
                using (var jsd = await Post.SendForJson("includes/ofdf.php", "a=2"))
                {
                    //int counter = 0;
                    await jsd.RootElement.EnumerateArray().ToArray().ParallelForAsync4(reportParts,
                        async (inc,part,index,parts) =>
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
                        var time = inc[5].GetString().ParseDateTime(false);
                        var source = TryDecodeCid(0, inc[7].GetString());
                        var recId = inc[11].GetAsString();
                        var hash = Report.ReportHash(recId);
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

                                var report = new Report()
                                {
                                    atkCid = source,
                                    defCid = target,
                                    defCN = defCN,
                                    atkCN = inc[14].GetString(),
                                    defP = defP,
                                    atkP = Player.NameToId(atkPNS),
                                    time = time,
                                    reportId = recId,
                                    spotted = time - TimeSpan.FromMinutes(target.CidToWorld().Distance(source.CidToWorld()) * averageScoutSpeed),
                                    type = Report.typeScout
                                    // todo TS info

                                };
                                parts[part].Add(report);


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
                                                    foreach (var at in ats.EnumerateObject())
                                                    {
                                                        //  public static string[] ttName = { 0:"guard", 1:"ballista", 2:"ranger", 3:"triari", 4:"priestess", 5:"vanquisher", 6:"sorcerers", 7:"scout", 8:"arbalist", 9:"praetor", 10:"horseman", 11:"druid", 12:"ram", 13:"scorpion", 14:"galley", 15:"stinger", 16:"warship", 17:"senator" };
                                                        switch (at.Name)
                                                        {
                                                            case "17": hasSen = true; break;
                                                            case "12":
                                                            case "13": hasSE = true; break;
                                                            case "16": hasSE = true; hasNavy = true; break;
                                                            case "14":
                                                            case "15": hasNavy = true; break;

                                                        }
                                                    }
                                                }

                                                source = report.GetAsInt("acid");
                                                if (source > 0)
                                                {
                                                    var atkTS = report.GetAsInt("ts_sent");
                                                    var atkTSLeft = report.GetAsInt("ts_left");
                                                    var atkPN = report.GetAsString("apn");
                                                    var rep = new Report()
                                                    {
                                                        reportId = recId,
                                                        dTS = defTS,
                                                        dTsLeft = defTSLeft,
                                                        aTsKill = atkTSKilled.Max(atkTS - atkTSLeft),
                                                        aTS = atkTS,
                                                        aTsLeft = atkTSLeft,
                                                        atkCid = source,
                                                        defCid = target,
                                                        SE = hasSE,
                                                        Nvl = hasNavy,
                                                        Sen = hasSen,
                                                        claim = hasSen && root.GetAsString("senatorapn") == atkPN ? root.GetAsFloat("senator") : -1,
                                                        defCN = defCN,
                                                        atkCN = report.GetAsString("acn"),
                                                        defP = defP,
                                                        atkP = Player.NameToId(atkPN),
                                                        time = time,
                                                        spotted = time - TimeSpan.FromMinutes(target.CidToWorld().Distance(source.CidToWorld()) * averageSpeed),
                                                        type = (byte)reportType
                                                        // todo TS info

                                                    };
                                                    {
                                                        var lg = rep.atkCN.Length;
                                                        if (lg > 9)  // trim off '(000:000)'
                                                            rep.atkCN = rep.atkCN.Substring(0, lg - 9);

                                                    }
                                                    parts[part].Add(rep);
                                                }
                                            }
                                        }
                                    }

                                }
                            }
                        }
                        
                    } );
                }

            }

            await task0;

            //if(ShellPage.IsPageDefense())
            {
                var defPage = DefensePage.instance;
                    for (int i = 0; i < reportParts.Length; ++i)
                        reportsIncoming.AddRange(reportParts[i]);
                    App.DispatchOnUIThread(() =>
                   // We should do this on the Render Thread
                   defPage.history.Reset(reportsIncoming.OrderByDescending((atk) => atk.time.Ticks)));
            }
            {
                var defenderPage = DefenderPage.instance;
                if (defenderPage != null)
                    defenderPage.NotifyIncomingUpdated();
            }

            updateInProgress = false;
            Note.Show($"Complete: {reportsIncoming.Count} attacks");
        }
    }

}
/*
 * 
 */
