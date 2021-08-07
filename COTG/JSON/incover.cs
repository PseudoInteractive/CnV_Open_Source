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
using Windows.UI.ViewManagement;
using Windows.Web.Http;
using System.Text.Json;
using TroopTypeCountsRef = COTG.Game.TroopTypeCounts;
using static COTG.Game.TroopTypeCountHelper;
//COTG.DArrayRef<COTG.Game.TroopTypeCount>;
using TroopTypeCounts = COTG.Game.TroopTypeCounts;
using DiscordCnV;
//COTG.DArray<COTG.Game.TroopTypeCount>;

namespace COTG.JSON
{
	public static class IncomingOverview
	{
		public static bool updateInProgress;
		static ConcurrentHashSet<int> supportReports = new ConcurrentHashSet<int>();
		public static ConcurrentDictionary<int, Army> reportCache = new();

		// uses Report.Hash(), can have several reports per reportId

		public static byte ClaimToByte(float claim)
		{
			if (claim <= 0)
				return 0;
			else
				return (byte)(((int)claim).Max(1)); // down down so that 99.6% doesn't show as 0, round up so that 0.1 shows as 1
		}

		//        public IncomingOverview() : base("overview/incover.php") { }

		// TODO
		//        static DateTime lastUpdate = new DateTime();
		static DateTime lastIncomingNotification = DateTime.UtcNow;
		static int lastPersonalIncomingCount = 0;
		static int lastWatchIncomingCount = 0;
		static bool hasRun;

		const string work = "fetch incoming";
		struct IncomingInfo
		{
			public Army firstArmy;
			public int count;
			public DateTimeOffset first;
			internal int targetCid => firstArmy.targetCid;
			internal int sourceCid => firstArmy.sourceCid;

			public string intel => $"{firstArmy.miscInfo}, {ttNameWithCaps[firstArmy.troops.GetPrimaryTroopType()]}";
			public string note => $"{Player.IdToName(targetCid.CidToPid())} attacker: {Player.IdToName(sourceCid.CidToPid())} to {targetCid.CidToContinent()} {first.FormatSkipDateIfToday()} first: {intel}) to {City.Get(targetCid).nameAndRemarks} at {first.FormatSkipDateIfToday()}{( count>1?$" and {count-1} others":"")}";
		}

		static Debounce IncomingUpdateDebounce = new(DoProcess) { throttled = true, debounceDelay = 1000, throttleDelay = 2000 };



		public static void Process(bool fetch)
		{
			fetchReportsRequested |= fetch;
			IncomingUpdateDebounce.Go();
		}
		public static void ProcessTask()
		{
			IncomingUpdateDebounce.Go();
		}

		public async static Task DoProcess()
		{
			if (updateInProgress)
			{
				Assert(false);

				return;
			}
			var fetchReports = DefenseHistoryTab.IsVisible();
			fetchReportsRequested = false;

			try
			{

				updateInProgress = true;

				if (!World.initialized)
				{
					do
					{

						await Task.Delay(1000);
					} while (World.initialized);
				}




				using (var defenders = new ConcurrentHashSet<City>())
				{

					//				defenders.Add(Spot.pending);
					if (fetchReports)
						ShellPage.WorkStart(work);

					for (; ; )
					{
						if (!Alliance.all.IsNullOrEmpty())
							break;
						await Task.Delay(1000);
					}
					var _hasRun = hasRun;
					if (hasRun == false)
						hasRun = true;
					var watch = SettingsPage.incomingWatch;
					IncomingInfo personalIncoming = new IncomingInfo() { count = 0, first = new DateTimeOffset(2050, 1, 1, 1, 1, 1, TimeSpan.Zero) };
					IncomingInfo watchIncoming = new IncomingInfo() { count = 0, first = new DateTimeOffset(2050, 1, 1, 1, 1, 1, TimeSpan.Zero) };

					var reportParts = new[] { new List<Army>(), new List<Army>(), new List<Army>(), new List<Army>() };
					var reportsIncoming = new List<Army>();
					int incCount = 0;
					var reportCount = 0;
					var task0 = Task.Run(async () =>
					  {
						  try
						  {
							  //ConcurrentDictionary<int, Army> rs = new ConcurrentDictionary<int, Army>();
							  using (var jsd = await Post.SendForJson("overview/incover.php", "a=0", Player.myId))
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

								  var needToClear = new HashSet<Spot>(Spot.defendersI);
								  //var prior = new Dictionary<long, Army>();
								  //foreach (var spot in Spot.defendersI)
								  //{
								  // spot.claim = 0;
								  // // reuse these
								  // foreach (var i in spot.incoming)
								  // {
								  //  if (i.order != 0)
								  //	  prior.Add(i.order, i);
								  // }
								  // spot.incoming = Army.empty;
								  //}
								  if (jsd.RootElement.ValueKind == JsonValueKind.Object)
								  {
									  if (jsd.RootElement.TryGetProperty("a", out var jse))
									  {
										  foreach (var prop in jse.EnumerateObject())
										  {
											  ++incCount;
											  var val = prop.Value;
											  var cid = AUtil.DecodeCid(4, val.GetString("2"));
											  if (cid <= 0)
												  continue;


											  var spot = Spot.GetOrAdd(cid, val.GetAsString("1"));
											  var prior = spot.incoming;
											  var incoming = new List<Army>();
											  needToClear.Remove(spot);
											  defenders.Add(spot);
											  // set info if needed
											  spot._tsHome = val.GetAsInt("8");
											  //  spot.troopsHome = TroopTypeCount.empty;


											  var name = val.GetAsString("0");
											  spot.pid = Player.NameToId(name);

											  // round down
											  var claim = ClaimToByte(val.GetAsFloat("4"));

											  spot.claim = claim;
											  try
											  {
												  var scoutRange = val.GetAsString("6");
												  var hrsMark = scoutRange.IndexOf('h');
												  if (hrsMark >= 1)
													  spot.scoutRange = float.Parse(scoutRange.Substring(0, hrsMark - 1));
												  else
												  {
													  var minMark = scoutRange.IndexOf('m');
													  if (minMark >= 1)
														  spot.scoutRange = float.Parse(scoutRange.Substring(0, minMark)) / 60.0f;

												  }
											  }
											  catch (Exception e)
											  {
												  LogEx(e);
											  }
											  TroopTypeCountsRef sumDef = new();
											  var processedTroopsHome = false; // for some reason, these repeat
											  foreach (var armyV in val.GetProperty("9").EnumerateArray())
											  {
												  var order = armyV.GetAsInt64("0");
												  Army army = null;
												  const bool reused = false;
												  //if (order != 0)
												  //{
												  // foreach (var p in prior)
												  // {
												  //  if (p.order == order)
												  //  {
												  //	  army = p;
												  //	  break;
												  //  }
												  // }
												  //}

												  if (army == null)
												  {
													  army = new Army() { order = order };
													  // reused = false;
												  }
												  else
												  {
													  //  reused = true;
												  }
												  if (!reused)
												  {
													  var arrival = armyV.GetAsString("7");
													  var home = (arrival == "home");
													  bool onSupport = home || (arrival == "on support");
													  if (onSupport)
													  {
														  army.time = AUtil.dateTimeZero;
													  }
													  else
													  {
														  army.time = arrival.ParseDateTime();
													  }
													  army.claim = claim;
													  army.targetCid = cid;
													  var type = armyV.GetAsInt("5");
													  if (type != 3)
													  {

														  army.isAttack = true; // 0 is incoming 1 is sieging I think
																				// var armyPid = armyV.GetAsString("1");
																				//army.pid = armyPid switch
																				//{
																				//    "Troops home" => spot.pid,
																				//    var name => Player.NameToId(name)
																				//};
														  if (type == 0)
															  army.type = reportPending;
														  else
														  {
															  Assert(type == 1);
															  army.type = reportSieging;
														  }
													  }
													  else
													  {
														  // defense
														  army.type = home ? reportDefenseStationed : reportDefensePending;
													  }





													  if (home)
													  {
														  // for some reason these get duplicated
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
														  army.spotted = army.time - TimeSpan.FromSeconds(cid.DistanceToCidD(army.sourceCid) * TTTravel(ttScout));
													  }
												  }
												  var source = Spot.GetOrAdd(army.sourceCid);

												  TroopTypeCountsRef ttl = new();
												  if (!reused || army.isDefense)
												  {
													  if (armyV.TryGetProperty("3", out var ttp))
													  {

														  foreach (var tt in ttp.EnumerateArrayOrObject())
														  {
															  var str = tt.GetAsString();
															  int firstSpace = str.IndexOf(' ');

															  var ttype = Game.Enum.ttNameWithCapsAndBatteringRam.IndexOf(str.Substring(firstSpace + 1));
															  if (ttype != -1)
															  {
																  Add(ref ttl, new TroopTypeCount()
																  {
																	  count = int.Parse(str.Substring(0, firstSpace), System.Globalization.NumberStyles.Any, NumberFormatInfo.InvariantInfo),
																	  type = ttype >= 0 ? ttype : 0
																  });
															  }
															  else
															  {
																  Assert(false);
															  }

														  }
													  }
												  }


												  if (army.isDefense)
												  {

													  if (ttl.Count == 0)
														  continue; // skip empty entries for troops at home when no def is present.
													  Add(ref sumDef, ttl);
													  army.troops = (ttl);
													  Assert(army.troops.Any());
												  }
												  else // attack
												  {
													  // not sieging
													  if (army.type == reportPending)
													  {

														  //if (source.isClassified && army.troops.Length == 1 && army.troops[0].type == ttPending)
														  //{
														  //  army.miscInfo = COTG.Game.IncomingEstimate.Get(army);
														  //}
														  //else
														  {

															  source.QueueClassify(true);

															  COTG.Game.IncomingEstimate.Get(army);
															  // if (source.classification == Spot.Classification.pending)
															  // {
															  //	  army.miscInfo = "pending " + army.miscInfo;
															  // }

														  }
													  }
													  else
													  {
														  // if (!reused)
														  {
															  if (ttl.Any())
															  {
																  army.troops = (ttl);
																  Assert(army.troops.count >= 1);
															  }
															  else
															  {
																  // this will early out if its already queued
																  source.QueueClassify(true);
																  Set(ref army.troops, new TroopTypeCount(source.classificationTroopType, -1));
																  Assert(army.troops.Count == 1);
															  }

														  }
														  //else
														  //{
														  // // intel ready now
														  // if (source.isClassified && army.troops.Length == 1 && army.troops[0].type == ttPending)
														  // {
														  //  COTG.Game.IncomingEstimate.Get(army);

														  // }
														  //}

														  {
															  if (army.claim > 0)
																  army.miscInfo = $"Claim {claim}%";
															  else
																  army.miscInfo = "Sieging";
														  }
													  }





													  if (((army.type == reportPending) || (army.type == reportSieging && (army.claim > 0))) && !reused)
													  {

														  if (!Alliance.IsAlly(army.sourceAlliance))
														  {



															  if (DGame.isValidForIncomingNotes)
															  {


																  if (++reportCount < 64)
																  {
																	  var _army = army;
																	  if (await Tables.TryAddOrder(_army.time,_army.order))
																	  {
																		  //Log($"Enqueue {order}");
																		  App.EnqeueTask(async () =>
																		  {
																			  try
																			  {
																				  //Log($"Enqueue2 {order}");
																				  var name = _army.tPlayer;

																				  var target = Spot.GetOrAdd(_army.targetCid);
																				  var _source = Spot.GetOrAdd(_army.sourceCid);
																				  while (!_source.isClassified)
																				  {
																					  await Task.Delay(400);
																				  }
																				  if (!DGame.members.TryGetValue(name.ToLower(), out var id))
																					  id = name;
																					  
																				  var content = $"<@{id}> {_army.time.FormatSkipDateIfToday()}: {army.miscInfo}, {ttNameWithCaps[army.troops.GetPrimaryTroopType()]} to {target.cityName} ({target.xy}) from  {_source.player.name} {_source.cityName} ({_source.xy}";
																				  if (army.claim > 0)
																				  {
																					  content += $" claim {army.claim}%";
																				  }

																				  //      Note.Show(content);

																				  var client = JSClient.genericClient;


																				  var message = new DGame.Message() { username = "INCOMING", content = content, avatar_url = "" };
																				  for (int i = 0; i < 4; ++i) // retry up to 4 times;
																			  {

																					  var post = new HttpStringContent(
																									JsonSerializer.Serialize(message), Windows.Storage.Streams.UnicodeEncoding.Utf8,
																									 "application/json");

																					  var result = await client.PostAsync(DGame.discordIncomingHook, post);
																					  if (result.StatusCode == HttpStatusCode.TooManyRequests)
																					  {
																						  await Task.Delay(2000); // wait 2 seconds
																					  }
																					  else
																					  {
																						//  result.EnsureSuccessStatusCode();
																						  break;
																					  }
																				  }
																			  }
																			  catch (Exception ex)
																			  {
																				  Log(ex);
																			  }
																		  });
																	  }
																  }


															  }

														  }
														  if (watch.Contains(name))
														  {
															  watchIncoming.count++;
															  if (army.time < watchIncoming.first)
															  {
																  watchIncoming.first = army.time;
																  watchIncoming.firstArmy = army;
															  }
														  }
														  if (spot.pid == Player.myId)
														  {
															  if (army.time < personalIncoming.first)
															  {
																  personalIncoming.first = army.time;
																  personalIncoming.firstArmy = army;
															  }
															  ++personalIncoming.count;
														  }
													  }


												  } // if attack
												  army.sumDef = (sumDef);
												  //  army.sumDef.Sort();


												  //  army.troops.Sort();
												  incoming.Add(army);

												  if (!army.isDefense && fetchReports)
													  reportsIncoming.Add(army);


												  //  spot.tsMax = sumDef.TS();
											  }
											  spot.incoming = incoming.ToArray();
										  }
									  }
								  }

								  foreach (var spot in needToClear)
								  {
									  spot.claim = 0;
									  spot.incoming = Army.empty;
								  }

							  }
							  {
								  var defenderPage = IncomingTab.instance;
								  if (defenderPage != null)
								  {
									 App.DispatchOnUIThreadLow( defenderPage.NotifyIncomingUpdated );
								  }
							
							  }
							  if (lastPersonalIncomingCount != personalIncoming.count || watchIncoming.count != lastWatchIncomingCount)
							  {
								  string note = "Incoming";

								  if (personalIncoming.count != 0)
									  note += $" {personalIncoming.note}";
								  if (watchIncoming.count != 0)
									  note += $" (watched {watchIncoming.note})";

								  if (lastPersonalIncomingCount < personalIncoming.count || lastWatchIncomingCount < watchIncoming.count)
								  {
									  var now = DateTime.UtcNow;
									  if (now - lastIncomingNotification > TimeSpan.FromMinutes(3))
									  {
										  lastIncomingNotification = now;
										  Note.Show(note);
										  COTG.Services.ToastNotificationsService.instance.ShowNotification(note, "Incoming");

									  }
								  }
								  //   ShellPage.instance.incoming.Text = $"In {personalIncomingCount} at {firstIncomingStr} ({watchIncomingCount} at {firstWatchIncomingStr})";
								  if (watchIncoming.count + personalIncoming.count > 0)
									  ApplicationView.GetForCurrentView().Title = note;
								  else
									  ApplicationView.GetForCurrentView().Title = $"No incoming";

								  lastPersonalIncomingCount = personalIncoming.count;
								  lastWatchIncomingCount = watchIncoming.count;

							  }

						  }
						  catch (Exception ex)
						  {
							  LogEx(ex, eventName: "IncomingOverview", report: false);
						  }

					  });
					var fetched = 0;
					if (fetchReports)
					{
						//foreach (var r in DefenseHistoryTab.instance.history)
						//{
						//	if (r.reportId.IsNullOrEmpty())
						//		continue;
						//	var hash = Army.ReportHash(r.reportId);
						//	if (reportCache.TryGetValue(hash, out var reports))
						//	{
						//		reportCache[hash] = reports.ArrayAppend(r);
						//	}
						//	else
						//	{
						//		reportCache[hash] = new[] { r };
						//	}
						//}

						// defense history
						using (var jsd = await Post.SendForJson("includes/ofdf.php", "a=2", Player.myId))
						{
							//int counter = 0;
							await jsd.RootElement.EnumerateArray().ToArray().ParallelForAsync4(reportParts,
								async (_inc, _part, _index, _parts) =>
								{
									var inc = _inc;
									var part = _part;
									var index = _index;
									var parts = _parts;


									var target = AUtil.DecodeCid(0, inc[4].GetString());
									if (target <= 0)
										return;
									var defP = Player.NameToId(inc[1].GetAsString());
									//  var atkPNS = inc[6].GetAsString();
									var defCN = inc[3].ToString();
									Spot.GetOrAdd(target, defCN);
									var time = inc[5].GetString().ParseDateTime(false);
									var source = AUtil.DecodeCid(0, inc[7].GetString());
									var recId = inc[11].GetAsString();
									var hash = Army.ReportHash(recId);
									if (reportCache.TryGetValue(hash, out var reports))
									{
										if (reports != null)
											parts[part].Add(reports);

									}
									else if (supportReports.Contains(hash))
									{
										// Nothing needed
									}
									else
									{
										if (source > 0)
										{
											// Scout
											// this is a scout

											Spot.GetOrAdd(source, inc[14].GetAsString());
											var dts = inc[8].GetAsInt();
											var ats = inc[9].GetAsInt();
											var report = new Army()
											{
												isAttack = true,
												sourceCid = source,
												targetCid = target,
												time = time,
												reportId = recId,
												spotted = time - TimeSpan.FromSeconds(target.CidToWorld().DistanceD(source.CidToWorld()) * TTTravel(ttScout)),
												type = reportScout,

												// todo TS info

											};
											Set(ref report.troops, new TroopTypeCount(ttScout, ats / 2));
											Set(ref report.sumDef, new TroopTypeCount(ttGuard, dts));

											reportCache.TryAdd(hash, report);
											parts[part].Add(report);
											//			await Cosmos.AddBattleRecord(report);


										}
										else
										{


											// we have to look up the report
											try
											{
												using (var jsdr = await Post.SendForJson("includes/gFrep2.php", "r=" + recId, Player.myId))
												{
													if (jsdr != null)
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
																	if (report.GetAsInt("acid") <= 0)
																	{
																		supportReports.Add(Army.ReportHash(recId));
																		continue;
																	}
																	TroopTypeCountsRef atkTroops = new();
																	TroopTypeCountsRef defTroops = new();
																	var defTSLeft = 0;
																	var atkCN = report.GetAsString("acn");
																	source = AUtil.DecodeCid(atkCN.Length - 8, atkCN);

																	{
																		var lg = atkCN.Length;
																		if (lg > 9)  // trim off '(000:000)'
																			atkCN = atkCN.Substring(0, lg - 9);

																	}

																	int refines = 0;
																	if (report.TryGetProperty("rew", out var rew))
																	{
																		refines = rew.GetAsInt("w") * 4;// all four are always the same
																	}


																	if (report.TryGetProperty("ats", out var ats))
																	{
																		Set(ref atkTroops, ats);

																	}
																	{
																		if (report.TryGetProperty("tts", out var tts))
																		{
																			Set(ref defTroops, tts);
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
																					Set(ref defTroops, tts);
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
																		bool hasSen = atkTroops.HasTT(ttSenator);
																		var rep = new Army()
																		{
																			refines = refines,
																			isAttack = true,
																			reportId = recId,

																			dTsKill = defTS - defTSLeft,
																			aTsKill = atkTSKilled.Max(atkTS - atkTSLeft),


																			sourceCid = source,
																			targetCid = target,
																			claim = (byte)(hasSen && root.GetAsString("senatorapn") == atkPN ? IncomingOverview.ClaimToByte(root.GetAsFloat("senator")) : (byte)0),
																			time = time,
																			spotted = time - TimeSpan.FromSeconds(target.CidToWorld().DistanceD(source.CidToWorld()) * TTTravel(ttVanquisher)),
																			type = (byte)reportType
																			// todo TS info

																		};
																		rep.sumDef = defTroops;
																		rep.troops = (atkTroops);
																		reportCache.TryAdd(hash, rep);
																		parts[part].Add(rep);

																		//		await Cosmos.AddBattleRecord(rep);
																	}
																	else
																	{
																		Log("bad cid?");
																	}
																}
															}
														}

													}
													else
													{
														reportCache.TryAdd(hash, null);
													}
												}
											}
											catch (Exception e)
											{
												LogEx(e);
											}


										}
									}

								});
						}

					}

					await task0;
					//var prior = new Dictionary<long, Army>();

					Spot.defendersI = defenders.ToArray();
					//if(ShellPage.IsPageDefense())
					await App.DispatchOnUIThreadTask(() =>
					{

						try
						{
							if (fetchReports)
								ShellPage.WorkEnd(work);

							string killNote = "";

							if (fetchReports)
							{
								var defPage = DefenseHistoryTab.instance;
								for (int i = 0; i < reportParts.Length; ++i)
									reportsIncoming.AddRange(reportParts[i]);
								var defKilled = 0;
								var atkKilled = 0;
								var myDefKilled = 0;
								var myAtkKilled = 0;
								foreach (var i in reportsIncoming)
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
								killNote = $", {atkKilled:N0}({myAtkKilled:N0})TS atk ts killed, {defKilled:N0}({myDefKilled:N0})TS def Killed";
								//	App.CopyTextToClipboard(killNote);
								// App.DispatchOnUIThread(() =>
								// We should do this on the Render Thread
								defPage.SetHistory((reportsIncoming.OrderByDescending((atk) => atk.time.Ticks)).ToArray());
							}
							if (fetchReports)
								Note.Show($"Complete: {reportsIncoming.Count + incCount} attacks, {fetched} fetched {killNote}");
						}
						catch (Exception _exception)
						{
							COTG.Debug.LogEx(_exception);
						}
						return Task.CompletedTask;

					});
				}
			}
			catch (Exception exception)
			{
				LogEx(exception);
				if (fetchReports)
					ShellPage.WorkEnd(work);
			}



			finally
			{
				updateInProgress = false;

			}

			return;

		}
	}

}
/*
 * 
 */
