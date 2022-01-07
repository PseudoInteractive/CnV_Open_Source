using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CnV.CnVServer;
using static CnV.Troops;
using static CnV.City;
using static CnV.Spot;
using static CnV.ClientView;

namespace CnV
{
	using System.Text.Json;
	using System.Web;
	using Windows.System;
	using Services;
	using Microsoft.UI.Input;

	internal  static partial class CnVClient
	{
		static readonly float[] researchRamp = { 0, 1, 3, 6, 10, 15, 20, 25, 30, 35, 40, 45, 50 };
		private static void BonusesUpdated()
		{
			cartTravel = 10.0f / (1.0 + faith.merius * 0.5 / 100 + researchRamp[research[28]] / 100);
			shipTravel = 5.0f / (1.0 + faith.merius * 0.5 / 100 + researchRamp[research[27]] / 100);

			// these are all scaled by 100 to reduce rounding errors
			ttSpeedBonus[0] = 100; // no speed reserach for guard
			ttSpeedBonus[1] = 100 + faith.domdis * 0.5f  + researchRamp[research[12]];
			ttSpeedBonus[2] = 100 + faith.ibria * 0.5f  + researchRamp[research[8]];
			ttSpeedBonus[3] = 100 + faith.ibria * 0.5f  + researchRamp[research[8]];
			ttSpeedBonus[4] = 100 + faith.ibria * 0.5f  + researchRamp[research[8]];
			ttSpeedBonus[5] = 100 + faith.ibria * 0.5f  + researchRamp[research[8]];
			ttSpeedBonus[6] = 100 + faith.ibria * 0.5f  + researchRamp[research[8]];
			ttSpeedBonus[7] = 100 + faith.ibria * 0.5f  + researchRamp[research[11]];
			ttSpeedBonus[8] = 100 + faith.ibria * 0.5f + researchRamp[research[9]];
			ttSpeedBonus[9] = 100 + faith.ibria * 0.5f  + researchRamp[research[9]];
			ttSpeedBonus[10] = 100 + faith.ibria * 0.5f  + researchRamp[research[9]];
			ttSpeedBonus[11] = 100 + faith.ibria * 0.5f  + researchRamp[research[9]];
			ttSpeedBonus[12] = 100 + faith.domdis * 0.5f + researchRamp[research[12]];
			ttSpeedBonus[13] = 100 + faith.domdis * 0.5f + researchRamp[research[12]];
			ttSpeedBonus[14] = 100 + faith.domdis * 0.5f + researchRamp[research[13]];
			ttSpeedBonus[15] = 100 + faith.domdis * 0.5f + researchRamp[research[13]];
			ttSpeedBonus[16] = 100 + faith.domdis * 0.5f + researchRamp[research[13]];
			ttSpeedBonus[17] = 100 + faith.evara * 0.5f + researchRamp[research[14]];


			ttCombatBonus[0] = 1 + faith.naera * 0.5f / 100 + researchRamp[research[29]] / 100;
			ttCombatBonus[1] = 1 + faith.naera * 0.5f / 100 + researchRamp[research[42]] / 100;
			ttCombatBonus[2] = 1 + faith.naera * 0.5f / 100 + researchRamp[research[30]] / 100;
			ttCombatBonus[3] = 1 + faith.naera * 0.5f / 100 + researchRamp[research[31]] / 100;
			ttCombatBonus[4] = 1 + faith.naera * 0.5f / 100 + researchRamp[research[32]] / 100;
			ttCombatBonus[5] = 1 + faith.vexemis * 0.5f / 100 + researchRamp[research[33]] / 100;
			ttCombatBonus[6] = 1 + faith.vexemis * 0.5f / 100 + researchRamp[research[34]] / 100;
			ttCombatBonus[7] = 1 + faith.vexemis * 0.5f / 100 + researchRamp[research[46]] / 100;
			ttCombatBonus[8] = 1 + faith.naera * 0.5f / 100 + researchRamp[research[35]] / 100;
			ttCombatBonus[9] = 1 + faith.naera * 0.5f / 100 + researchRamp[research[36]] / 100;
			ttCombatBonus[10] = 1 + faith.vexemis * 0.5f / 100 + researchRamp[research[37]] / 100;
			ttCombatBonus[11] = 1 + faith.vexemis * 0.5f / 100 + researchRamp[research[38]] / 100;
			ttCombatBonus[14] = 1 + faith.ylanna * 0.5f / 100 + researchRamp[research[44]] / 100;
			ttCombatBonus[15] = 1 + faith.ylanna * 0.5f / 100 + researchRamp[research[43]] / 100;
			ttCombatBonus[16] = 1 + faith.cyndros * 0.5f / 100 + researchRamp[research[45]] / 100;
			ttCombatBonus[17] = 1; // no combat research for senator
		}

		
		static private int[] lastCln = null;
		static SortedList<int, int> GetIntArray(JsonElement cln)
		{
			var rv = new SortedList<int, int>();
			if(cln.ValueKind == JsonValueKind.Array)
				foreach(var cn in cln.EnumerateArray())
					rv.Add(rv.Count, cn.GetAsInt());
			else if(cln.ValueKind == JsonValueKind.Object)
				foreach(var cn in cln.EnumerateObject())
					rv.Add(int.Parse(cn.Name), cn.Value.GetAsInt());
			return rv;
		}

		public static async void UpdatePPDT(JsonElement jse, int thisPid, bool pruneCities = false, bool updateBuildCity = false)
		{
			// Todo:  should we update our local PPDT to the server?

			var clChanged = 0;
			// City lists
			try
			{
				

				var bonusesUpdated = false;
				// research?
				if(jse.TryGetProperty("rs", out var rss))
				{
					foreach(var rs in rss.EnumerateObject())
					{
						var id = int.Parse(rs.Name);
						if(id < researchCount)
							research[id] = (byte)rs.Value.GetInt("l"); // this will wrap for senator level (research not supported here)
						else
						{
							Assert(false);
						}
					}
					bonusesUpdated = true;

				}
				//if(jse.TryGetProperty("tcps", out var tcps))
				//{
				//	TradeSettings.all = JsonSerializer.Deserialize<TradeSettings[]>(tcps.ToString(), JSON.jsonSerializerOptions);

				//	AppS.DispatchOnUIThreadIdle(() =>
				//	{
				//		ResSettings.tradeSettingsItemsSource = TradeSettings.all;
				//	});
				//}

				if(jse.TryGetProperty("wmo", out var wo))
				{
					WorldViewSettings.ownCities.isOn = wo.GetAsInt("0")==1;
					WorldViewSettings.ownCities.color = wo.GetColor("16");

					WorldViewSettings.ownAlliance.isOn = wo.GetAsInt("1") == 1;
					WorldViewSettings.ownAlliance.color = wo.GetColor("17");

					WorldViewSettings.alliedAlliance.isOn = wo.GetAsInt("2") == 1;
					WorldViewSettings.alliedAlliance.color = wo.GetColor("18");

					WorldViewSettings.napAlliance.isOn = wo.GetAsInt("3") == 1;
					WorldViewSettings.napAlliance.color = wo.GetColor("19");

					WorldViewSettings.enemyAlliance.isOn = wo.GetAsInt("4") == 1;
					WorldViewSettings.enemyAlliance.color = wo.GetColor("20");

					WorldViewSettings.otherPlayers.isOn = wo.GetAsInt("15") == 1;
					WorldViewSettings.otherPlayers.color = wo.GetColor("28");

					WorldViewSettings.lawless.isOn = wo.GetAsInt("5") == 1;
					WorldViewSettings.lawless.color = wo.GetColor("21");

					WorldViewSettings.friends.isOn = wo.GetAsInt("6") == 1;
					WorldViewSettings.friends.color = wo.GetColor("22");

					WorldViewSettings.citiesWithoutCastles = wo.GetAsInt("7") == 1;
					WorldViewSettings.citiesWithoutWater = wo.GetAsInt("8") == 1;
					WorldViewSettings.citiesWithoutTemples = wo.GetAsInt("9") == 1;

					WorldViewSettings.caverns.isOn = wo.GetAsInt("10") == 1;
					WorldViewSettings.caverns.color = wo.GetColor("23");

					WorldViewSettings.bosses.isOn = wo.GetAsInt("11") == 1;
					WorldViewSettings.bosses.color = wo.GetColor("24");

					WorldViewSettings.shrines.isOn = wo.GetAsInt("12") == 1;
					WorldViewSettings.shrines.color = wo.GetColor("25");

					WorldViewSettings.inactivePortals.isOn = wo.GetAsInt("13") == 1;
					WorldViewSettings.inactivePortals.color = wo.GetColor("26");

					WorldViewSettings.activePortals.isOn = wo.GetAsInt("14") == 1;
					WorldViewSettings.activePortals.color = wo.GetColor("27");

					WorldViewSettings.cavernMinLevel = wo.GetAsInt("29");
					WorldViewSettings.cavernMaxLevel = wo.GetAsInt("30");

					WorldViewSettings.bossMinLevel = wo.GetAsInt("31");
					WorldViewSettings.bossMaxLevel = wo.GetAsInt("32");


					WorldViewSettings.playerSettings.Clear();
					if(wo.TryGetProperty("p", out var p) && p.ValueKind == JsonValueKind.Object)
						foreach(var pset in p.EnumerateObject())
						{
							var ps = new WorldViewSettings.PlayerSetting();
							ps.pid = pset.Value.GetAsInt("a");
							ps.color = pset.Value.GetColor("c");
							ps.isOn = pset.Value.GetAsInt("d") == 1;

							WorldViewSettings.playerSettings.Add(ps.pid, ps);
						}
					WorldViewSettings.allianceSettings.Clear();
					if(wo.TryGetProperty("a", out var a))

						foreach(var pset in a.EnumerateObject())
						{
							var ps = new WorldViewSettings.AllianceSetting();
							ps.pid = pset.Value.GetAsInt("a");
							ps.color = pset.Value.GetColor("c");
							ps.isOn = pset.Value.GetAsInt("d") == 1;

							WorldViewSettings.allianceSettings.Add(ps.pid, ps);
						}

					//	if (World.completed)
					//		GetWorldInfo.Send();
				}

				if(jse.TryGetProperty("mvb", out var mvb))
				{
					Log("MVB: " + mvb.ToString());
					Player.moveSlots = mvb.ValueKind == JsonValueKind.Number ? mvb.GetAsInt() : mvb.GetAsInt("l");

				}

				if(jse.TryGetProperty("fa", out var fa))
				{
					faith.evara = fa.GetAsByte("1");
					faith.vexemis = fa.GetAsByte("2"); // 2
					faith.domdis = fa.GetAsByte("3");
					faith.cyndros = fa.GetAsByte("4");
					faith.merius = fa.GetAsByte("5");
					faith.ylanna = fa.GetAsByte("6");
					faith.ibria = fa.GetAsByte("7");
					faith.naera = fa.GetAsByte("8");

					bonusesUpdated = true;

				}
				if(bonusesUpdated)
					BonusesUpdated();

				var lists = new List<CityList>();
				if(jse.TryGetProperty("cl", out var cityListNames))
				{
					++clChanged;
					//  var clList = new List<string>();
					if(cityListNames.ValueKind == JsonValueKind.Object)
						foreach(var cn in cityListNames.EnumerateObject())
						{
							var l = new CityList() { name = cn.Value.GetString(), id = int.Parse(cn.Name) };
							lists.Add(l);
						}
					//  lists.Sort((a, b) => a.name.CompareTo(b.name));


					if(jse.TryGetProperty("cln", out var cln))
						//  ++clChanged;

						//  var clList = new List<string>();
						lastCln = GetIntArray(cln).Values.ToArray();
					if(lastCln != null)
					{

						var prior = lists;
						lists = new List<CityList>();
						foreach(var id in lastCln)
						{
							var ins = prior.Find((a) => a.id == id);
							if(ins != null)
								lists.Add(ins);

						}
					}
					//  lists.Sort((a, b) => a.name.CompareTo(b.name));

				}

				if(jse.TryGetProperty("r", out var r))
					Player.me.title = r.GetAsByte();

				if(jse.TryGetProperty("clc", out var cityListCities))
				{
					++clChanged;
					if(cityListCities.ValueKind == JsonValueKind.Object)
						foreach(var clc in cityListCities.EnumerateObject())
						{
							if(clc.Value.ValueKind == JsonValueKind.Null)
								continue;
							var id = int.Parse(clc.Name);
							var cityList = lists.Find((a) => a.id == id);
							foreach(var cityId in GetIntArray(clc.Value))
								cityList.cities.Add(cityId.Value);

						}
				}

				if(clChanged >= 2)
					AppS.DispatchOnUIThreadIdle(() =>
					{
						var priorIndex = ShellPage.CityListBox.SelectedIndex;
						CityList.selections = new CityList[lists.Count + 1];
						CityList.selections[0] = CityList.allCities;

						for(var i = 0; i < lists.Count; ++i)
							CityList.selections[i + 1] = lists[i];
						CityList.all = lists.ToArray();
						if(Settings.instance!=null)
						{
							Settings.instance.hubCityListBox.ItemsSource = null;
							Settings.instance.hubCityListBox.ItemsSource = CityList.all;
						}
						ShellPage.CityListBox.ItemsSource          = CityList.selections;
						ShellPage.CityListBox.SelectedIndex = priorIndex; // Hopefully this is close enough
																 //                       Settings.instance.
					});
			}

			catch(Exception E)
			{
				//	Log(E);
				Log("City lists invalid, maybe you have none");
			}


			// extract cities
			if(jse.TryGetProperty("c", out var cProp))
			{
				int citySwitch = 0;
				if(updateBuildCity)
				{
					if(jse.TryGetProperty("lcit", out var lcit))
					{
						citySwitch= lcit.GetAsInt();

					}

				}

				if(!World.initialized)
				{
					cProp = cProp.Clone();
					while(!World.initialized)
						await Task.Delay(1000);
				}

				var now = DateTimeOffset.UtcNow;
				foreach(var jsCity in cProp.EnumerateArray())
				{
					//                    Log(jsCity.ToString());
					var cid = jsCity.GetProperty("1").GetInt32();
					Assert(thisPid != 0);
					if(pruneCities)
						if(World.GetInfoFromCid(cid).player != thisPid)
						{

							Note.Show($"Invalid City, was it lost? {cid.CidToString()}");
							ChangeCityJS(cid);

							await Task.Delay(2000);
							continue;

						}


					var city = City.GetOrAddCity(cid);
					city.type = Spot.typeCity;
					if(thisPid != 0)
						city.pid = thisPid;
					var name = jsCity.GetProperty("2").GetString();
					var i = name.LastIndexOf('-');
					if(i != -1 && i+2 < name.Length)
					{
						city.remarks = name.Substring(i + 2);
						city._cityName = name.Substring(0, i - 1);
						city.UpdateTags();
					}
					else
						city._cityName = name;
					city.type = Spot.typeCity;
					city._tsTotal = jsCity.GetAsInt("8");
					//city._tsHome = jsCity.GetAsInt("17");
					//			city.troopsTotal = TroopTypeCount.empty;
					//				city.troopsHome = TroopTypeCount.empty;

					//			Trace($"TS Home {city._tsHome}");

					//   city.tsRaid = city.tsHome;
					city.isCastle = jsCity.GetAsInt("12") > 0;
					city.points = (ushort)jsCity.GetAsInt("4");

					city.isOnWater |= jsCity.GetAsInt("16") > 0;  // Use Or in case the data is imcomplete or missing, in which case we get it from world data, if that is not incomplete or missing ;)
					city.isTemple = jsCity.GetAsInt("15") > 0;
					//	city.pid = Player.myId;
					//  Log($"Temple:{jsCity.GetAsInt("15")}:{jsCity.ToString()}");


				}
				if(citySwitch != 0)
					CitySwitch(citySwitch, true);
				ShellPage.CityListNotifyChange(true);

				if(!ppdtInitialized)
				{

					ppdtInitialized = true;

					//Task.Delay(500).ContinueWith( _ => App.DispatchOnUIThreadSneakyLow( MainPage.instance.Refresh));
					ShellPage.RefreshTabs.Go();
				}

				//    Log(City.all.ToString());
				//   Log(City.all.Count());

			}
			MainPage.CheckTipRaiding();
			City.CitiesChanged();
			// Log($"PPDT: c:{cUpdated}, clc:{clChanged}");

			// Log(ppdt.ToString());
		}
		class WaitOnCityDataData
		{
			public int                        cid;
			public TaskCompletionSource<bool> t;

			public WaitOnCityDataData(int cid)
			{
				this.cid = cid;
				t        = new TaskCompletionSource<bool>();
			}

			public void Done()
			{
				try
				{
					cid = 0;
					var _t = t;
					t = null;
					if(_t != null)
						_t.SetResult(true);

				}
				catch(Exception ex)
				{

					LogEx(ex);
				}
			}
			public void Abort()
			{
				try
				{
					cid = 0;
					var _t = t;
					t = null;
					if(_t != null)
						_t.SetResult(false);

				}
				catch(Exception ex)
				{

					LogEx(ex);
				}
			}
			public bool isDone => t == null;
		}


		static WaitOnCityDataData[] waitingOnCityData = Array.Empty<WaitOnCityDataData>();

		static async Task<bool> ChangeCityJSWait(int cityId)
		{
			Log($"Wait {Spot.GetOrAdd(cityId).nameAndRemarks}");
			var i = new WaitOnCityDataData(cityId);
			waitingOnCityData =  waitingOnCityData.ArrayAppend(i);
			ChangeCityJS(cityId);
			var t = i.t.Task;
			var xx = await Task.WhenAny(t, Task.Delay(10000)) == t;
			Assert(xx);
			Log($"WaitComplete {xx} {Spot.GetOrAdd(cityId).nameAndRemarks}");
			if(!xx)
				i.Abort();
			return xx;
		}
		public static async Task<bool> CitySwitch(int cid, bool lazyMove = false, bool select = true, bool scrollIntoUI = true, bool isLocked = false, bool waitOnChange = false)
		{
			// Make sure we don't ignore the exception
			{
				// is it my city?
				if(CanVisit(cid))
				{
					//		Assert(cid != City.build);
					// Is it locked?
					if(!Spot.CanChangeCity(cid))
					{
						ClientView.EnsureNotCityView();
						Note.Show("Please wait for current operation to complete");
						return false;
					}
					var city = GetOrAddCity(cid);

					//if(city.pid != Player.myId)
						// no longer happens
					//	Assert(false);
					//else
					{
						var changed = cid != Spot.build;
						if(changed)
						{
							if(Spot.lockedBuild != 0 && cid != Spot.lockedBuild)
							{
								Note.Show("Please wait for current operation to complete");
								if(await AppS.DoYesNoBox("Busy", "Please wait for current operation to complete") != 1)
									throw new Exception("SetBuildOverlap");
							}
							var wantUnblock = false;
							// this blocks if we can't change the city
							if(!isLocked)
								await AppS.uiSema.WaitAsync();
							try
							{

								//	var wasPlanner = CityBuild.isPlanner;

								//if (CityBuild.isPlanner)
								//{
								//	//	var b = City.GetBuild();
								//	//	b.BuildingsCacheToShareString();
								//	//		await b.SaveLayout();
								//	//					CityBuild.isPlanner = false;
								//	await CityBuild._IsPlanner(false, true);
								//}

								//	Assert(pid == Player.myId);
								//Cosmos.PublishPlayerInfo(CnVServer.jsBase.pid, City.build, CnVServer.jsBase.token, CnVServer.jsBase.cookies); // broadcast change

								//foreach (var p in PlayerPresence.all)
								//{
								//	if (p.pid != Player.myId && p.cid == cid)
								//	{
								//		Note.Show($"You have joined {p.name } in {City.Get(p.cid).nameMarkdown}");
								//	}
								//}

								city.SetAsBuildCity();
								//if (wasPlanner)
								//{
								//	await GetCity.Post(cid);
								//	await CityBuild._IsPlanner(true, false);
								//}
								// async
								wantUnblock = true;
							}
							finally
							{
								if(!isLocked)
									AppS.uiSema.Release();
							}

							if(wantUnblock)
								ExtendedQueue.UnblockQueue(cid);

						}
						city.SetFocus(scrollIntoUI, select);
						CityUI.SyncCityBox();

						if(changed)
						{
							if(isLocked || waitOnChange)
							{
								if(await ChangeCityJSWait(cid) == false)
								{
									Note.Show("Somethings wrong, please try again");
									return false;
								}
							}
							else
							{
								ChangeCityJS(cid);
							}
						}

					}
					if(!lazyMove)
						cid.BringCidIntoWorldView(lazyMove);
				}
				else
					CityUI.ShowCity(cid, lazyMove, scrollIntoUI);

			}
			return true;

		}


		private static void CoreWebView_WebMessageReceived(string eValue)
		{
			Task.Run(async () =>
			{
				try
				{
					var gotCreds = false;
					Log($"Notify: {eValue.Length}:{eValue.Truncate(128) }");
					using var jsDoc = JsonDocument.Parse(eValue);
					var jsd = jsDoc.RootElement;
					foreach(var jsp in jsd.EnumerateObject())
						switch(jsp.Name)
						{
							case "jsvars":
								{
									//	   AppS.DispatchOnUIThreadLow(() => ShellPage.instance.cookie.Visibility = Visibility.Collapsed);

									var jso = jsp.Value;

									//   var s = CookieDB.Serialize(cookieManager);// GetSecSessionId();
									var token = jso.GetString("token");
									var raidSecret = jso.GetString("raid");
									var agent = jso.GetString("agent");
									//cotgS = jso.GetString("s");
									//  var cookie = jso.GetString("cookie");
									//   Log(jsVars.cookie);
									Log(token);
									// Log(s);
									//  for (int i = 0; i < clientCount; ++i)
									//  {
									//   await clientPoolSema.WaitAsync();//.ConfigureAwait(false);
									////   httpFilter.CookieManager.SetCookie(new HttpCookie)

									//  }
									//HTTPCook
									// {

									//  var cooki
									// }

									for(; ; )
									{
										try
										{
											{
												//    var clients = clientPool.ToArray();
												//										   foreach (var httpClient in clientPool)
												//										   {
												//												  // httpClient.DefaultRequestHeaders.Cookie = "sec_session_id="+s;

												////											   		if (subId == 0)
												//											   		//  httpClient.DefaultRequestHeaders.TryAppendWithoutValidation("Cookie",cookies);//"sec_session_id=" + s);
												//										   }
											}
										}
										catch(Exception _ex)
										{
											LogEx(_ex);
											await Task.Delay(1000);//.ConfigureAwait(false);
											continue;

										}
										break;
									}

									//   clientPoolSema.Release(clientCount);

									var timeOffset = jso.GetAsInt64("timeoffset");
									var timeOffsetSecondsRounded = Math.Round(timeOffset / (1000.0 * 60*30)) * 60 * 30.0f; // round to nearest half hour
									gameTOffset = TimeSpan.FromSeconds(timeOffsetSecondsRounded);
									gameTOffsetSeconds = (int)timeOffsetSecondsRounded;
									//   gameTOffsetMs = (long)timeOffsetSecondsRounded*1000;
									var str = timeOffsetSecondsRounded >= 0 ? " +" : " ";
									str += $"{gameTOffset.Hours:D2}:{gameTOffset.Minutes:D2}";
									Helpers.JSON.timeZoneString = str;
									//   Log(JSONHelper.timeZoneString);

									Log($"TOffset {gameTOffset}");
									Log(ServerTime().ToString("u"));
								//	ppss = jso.GetAsInt("ppss");
								//	Player.myName = jso.GetString("player");
								//	if(Player.subOwner == null)
								//		Player.subOwner = Player.myName;
								//	Player.myId = Player.myId = jso.GetAsInt("pid"); ;
									Player.myIds.Add(Player.myId);
									var cid = jso.GetAsInt("cid");
									Spot.build = Spot.focus = cid;
									NavStack.Push(cid);
									AGame.CameraC = cid.CidToWorldV();
									//Note.L("cid=" + cid.CidToString());
									//gameMSAtStart = jso.GetAsInt64("time");
									//launchTime = DateTimeOffset.UtcNow;
									//    Log(jsVars.ToString());
									//  Settings.secSessionId = jso.GetAsString("s");
									//		AGame.clientTL.X = jso.GetAsFloat("left");
									//  AGame.clientTL.Y = jso.GetAsFloat("top");
									//   Log($"WebClient:{AGame.clientTL} {ShellPage.webclientSpan.y}");
									//     Note.Show($" {clientSpanX}:{clientSpanY} {ShellPage.clientTL} ");
									gotCreds = true;
									//			   spanX = jso.GetAsInt("spanX");
									//			   spanY = jso.GetAsInt("spanY");
									//			   Note.Show($"ClientSpan: {spanX}x{spanY}");
									//    Log($"Built heades {httpClient.DefaultRequestHeaders.ToString() }");

									//   UpdatePPDT(jso.GetProperty("ppdt"));
									var ppdt = jso.GetProperty("ppdt");
									// todo: utf
							//		AddPlayer(true, true, Player.myId, Player.myName, token, raidSecret, cookies);//, s, ppdt.ToString());


									UpdatePPDT(ppdt, Player.myId, pruneCities: true);
									//Alliance.alliancesFetchedTask.ContinueWith((_) =>
									//{
									//	if(Player.isSpecial)
									//		Raid.test = true;

									//});





									World.RunWhenLoaded(() => AppS.DispatchOnUIThreadIdle(CityUI.UpdateFocusText));


									BuildQueue.Initialize();
									AppS.DispatchOnUIThreadLow(() =>
									{
										ShellPage.instance.coords.Text = cid.CidToString();
										//		   ShellPage.instance.cookie.Visibility = Visibility.Collapsed;
									});

									break;
								}
							case "aexp":
								{
									var msg = jsp.Value.ToString();
									Note.Show($"Exported Order to clipboard: {msg}");
									AppS.CopyTextToClipboard(msg);
									break;

									;
								}
							case "buildFail":
								{
									var city = GetOrAddCity(jsp.Value.GetAsInt("cid"));
									var e = jsp.Value.GetAsInt("e");
									Log($"Build Command: {city.nameMarkdown} {e}, {jsp.Value.ToString()}");


									break;
								}
							case "error":
								{
									var msg = jsp.Value.GetString();
									Trace(msg);

									break;
								}
							case "sub":
								{
									var i = jsp.Value.GetAsInt();
									AppS.DispatchOnUIThread(() => Windows.System.Launcher.LaunchUriAsync(new Uri($"{App.appLink}:launch?w={world}&s={i}&n=1&p={HttpUtility.UrlEncode(Player.myName, Encoding.UTF8)}", UriKind.Absolute)));
									break;
								}
							case "shcit":
								{
									var jso = jsp.Value;
									var cid = jso.GetAsInt();
									ProcessCoordClick(cid, false, AppS.keyModifiers, true); // then normal click
																								//AppS.DispatchOnUIThreadLow(async () =>
																								//{
																								// try
																								// {
																								//  var t = await App.GetClipboardText();
																								//  if (t.StartsWith("{") && t.EndsWith("}"))
																								//  {
																								//   // is it json?
																								//   var p = JsonSerializer.Deserialize<AttackSenderScript>(t);
																								//   OpenAttackSender(t);
																								//  }
																								// }
																								// catch (Exception ex)
																								// {

									// }
									//});
									break;
								}
							case "keyDown":
								{
									Log("Key");
									//   Log($"Keydown: {jsp.Value.ToString()}");
									VirtualKey key = default;
									switch(jsp.Value.GetString("key"))
									{
										case "Control": key = VirtualKey.Control; break;
										case "Shift": key = VirtualKey.Shift; break;
										case "ScrollLock": key = VirtualKey.Scroll; break;
									}
									if(key != default)

										App.OnKeyDown(key);
									break;
								}
							case "keyUp":
								{
									Log("Key");
									VirtualKey key = default;
									switch(jsp.Value.GetString("key"))
									{
										case "Control": key = VirtualKey.Control; break;
										case "Shift": key = VirtualKey.Shift; break;
										case "ScrollLock": key = VirtualKey.Scroll; break;
									}
									if(key != default)
										//   Note.Show($"{key} Up");
										App.OnKeyUp(key);
									break;
								}
							//case "mouseDown":
							//	{
							//		Log($"mouseDown: {jsp.Value.ToString()}");
							//		var but = jsp.Value.GetInt("button");
							//		var x = jsp.Value.GetInt("x");
							//		var y = jsp.Value.GetInt("y");
							//		// 2 is context button
							//		//if(but==2)
							//		//    Spot.GetFocus().ShowContextMenu(this,App.Current.m.GetPointer)
							//		//else
							//		var kind = but switch
							//		{
							//			0 => PointerUpdateKind.LeftButtonPressed,
							//			1 => PointerUpdateKind.MiddleButtonPressed,
							//			2 => PointerUpdateKind.RightButtonPressed,
							//			3 => PointerUpdateKind.XButton1Pressed,
							//			4 => PointerUpdateKind.XButton2Pressed,
							//			_ => PointerUpdateKind.Other
							//		};

							//		App.OnPointerPressed(kind);
							//		{
							//			AppS.DispatchOnUIThread(() =>
							//			{
							//				var c = view.TransformToVisual(ShellPage.canvas).TransformPoint(new(x, y));
							//				ShellPage.Canvas_PointerPressed((c, 0, true, (ulong)Environment.TickCount64*1000, kind));
							//			});


							//		}

							//		break;
							//	}
							//case "cityinfo":
							//    {
							//        var jso = jsp.Value;
							//        var cid = jso.GetAsInt("cid");
							//        var pid = Player.NameToId(jso.GetAsString("player"));
							//        var city = Spot.GetOrAdd(cid);
							//        var name = jso.GetString("name");
							//        city.pid = pid; // todo: this shoule be an int playerId
							//                        //Assert(city.pid > 0);
							//        city.points = (ushort)jso.GetAsInt("score");
							//        //   city.allianceId = jso.GetString("alliance"); // todo:  this should be an into alliance id
							//        city.lastAccessed = DateTimeOffset.UtcNow;
							//        // city.isCastle = jso.GetAsInt("castle") == 1;
							//        city.isBlessed = city.pid > 0 ? jso.GetAsInt("bless") > 0 : false;
							//        city.isOnWater |= jso.GetAsInt("water") != 0;  // Use Or in case the data is imcomplete or missing, in which case we get it from world data, if that is not incomplete or missing ;)
							//        city.isTemple = jso.GetAsInt("plvl") != 0;


							//        break;
							//    }
							case "notify":
								{
									foreach(var note in jsp.Value.EnumerateArray())
									{
										var str = note.GetString();
										Log(str);
										var ss = str.Split(',', StringSplitOptions.RemoveEmptyEntries);
										if(int.TryParse(ss[0], out var id))
											if(id == 99)
											{
												// online notify
												var friend = ss[1];
												var online = ss[2] == "1";
												var msg = new ChatEntry(friend, online ? " has come online" : " has gone offline", ServerTime(), ChatEntry.typeAnnounce);
												AppS.DispatchOnUIThreadLow(() =>
												{
													ChatTab.alliance.Post(msg, true);
													ChatTab.world.Post(msg, true);
												}); // post on both
											}
											else if(id == 9)
											{
												var cid = int.Parse(ss[1]);
												// founded new city
												await Task.Delay(30000);//.ConfigureAwait(false);
												Note.Show($"You have founded a new city!  Would you like to run [Setup](/s/{cid.CidToString()})");

											}

									}
									break;
								}
							case "incoming":
								{
									AppS.QueueIdleTask(IncomingOverview.ProcessTask, 1000);
									break;
								}
							case "outgoing":
								{
									AppS.QueueIdleTask(OutgoingOverview.ProcessTask, 1000);
									break;
								}
							//case "gstcb":
							//	{
							//		Note.Show(jsp.ToString());
							//		var jso = jsp.Value;
							//		var tag = jso.GetAsInt("tag");
							//		if(gstCBs.TryGetValue(tag, out var cb))
							//			cb(jso);

							//		break;
							//	}
							case "rmp":
								{
									var str = jsp.ToString();
									AppS.CopyTextToClipboard(str);
									Note.Show(str);
									foreach(var o in jsp.Value.EnumerateObject())
										foreach(var st in o.Value.EnumerateArray())
											TileData.UpdateTile(st.GetAsString());
									break;
								}

							//case "gstempty":
							//	{
							//		var jso = jsp.Value;
							//		var water = jso.GetAsInt("water") == 1;
							//		var res = jso.GetAsString("res").Split('^', StringSplitOptions.RemoveEmptyEntries);
							//		var cid = jso.GetAsInt("cid");

							//		var food = float.Parse(res[3]);
							//		var wood = float.Parse(res[0]);
							//		var stone = float.Parse(res[1]);
							//		var iron = float.Parse(res[2]);
							//		var sum = wood + stone + iron + food;
							//		(var x, var y) = cid.CidToWorld();
							//		float woodCount = 10, stoneCount = 10, ironCount = 10, plainsCount = 2;
							//		TileData.instance.ResourceGain(x, y + 1, false, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
							//		TileData.instance.ResourceGain(x - 1, y, false, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
							//		TileData.instance.ResourceGain(x, y - 1, false, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
							//		TileData.instance.ResourceGain(x + 1, y, false, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
							//		TileData.instance.ResourceGain(x + 1, y + 1, true, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
							//		TileData.instance.ResourceGain(x - 1, y + 1, true, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
							//		TileData.instance.ResourceGain(x - 1, y - 1, true, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
							//		TileData.instance.ResourceGain(x + 1, y - 1, true, ref woodCount, ref stoneCount, ref ironCount, ref plainsCount);
							//		//if (!AppS.IsKeyPressedShift())
							//		//{
							//		//    woodCount = woodCount.Min(30);
							//		//    stoneCount = stoneCount.Min(30);
							//		//    ironCount = ironCount.Min(30);
							//		//}

							//		var totalRes = woodCount + stoneCount + ironCount + plainsCount;
							//		var iWood = ((int)(woodCount * 80.0 / totalRes)).Min(30);
							//		var iStone = ((int)(stoneCount * 80.0 / totalRes)).Min(30);
							//		var iIron = ((int)(ironCount * 80.0 / totalRes)).Min(30);
							//		var maxDelta = (iWood - wood).Abs().Max((iStone - stone).Abs()).Max((iIron - iIron).Abs()).Max((food - plainsCount).Abs());
							//		var predicted = iWood + iStone + iIron + plainsCount;
							//		if(iWood - wood >= 1.0f)
							//		{
							//			Note.Show($"{predicted} predicted, {sum} actual, {iWood}:{wood} {iStone}:{stone} {iIron}:{iron} {plainsCount}:{food}");
							//			SpotTab.TouchSpot(cid, VirtualKeyModifiers.None, false, true);
							//		}
							//		//  var nodes = (nodeCount+30)/(nodeCount+30+2+ plainsCount)*80 + 2+ plainsCount;
							//		break;
							//	}
							case "cityclick":
								{
									var jso = jsp.Value;
									var cid = jso.GetAsInt("cid");
									{
										var pid = Player.NameToId(jso.GetAsString("player"));
										var city = Spot.GetOrAdd(cid);
										var name = jso.GetString("name");
										city.pid = pid; // todo: this shoule be an int playerId
										city.type = jso.GetAsByte("type");
										city.remarks = jso.GetAsString("notes");                //Assert(city.pid > 0);
										city.UpdateTags();
										city.points = (ushort)jso.GetAsInt("score");
										//   city.allianceId = jso.GetString("alliance"); // todo:  this should be an into alliance id
										//       city.lastAccessed = DateTimeOffset.UtcNow;
										// city.isCastle = jso.GetAsInt("castle") == 1;
										var blessed = city.pid > 0 ? jso.GetAsInt("bless") > 0 : false;
										if(blessed != city.isBlessed)
										{
											city.isBlessed = blessed;
											city.OnPropertyChanged(nameof(City.icon));
										}
										city.isOnWater |= jso.GetAsInt("water") != 0;  // Use Or in case the data is imcomplete or missing, in which case we get it from world data, if that is not incomplete or missing ;)
										city.isTemple = jso.GetAsInt("plvl") != 0;

										//if(City.focus != cid)
										// cid.BringCidIntoWorldView(true,false);
										if(city._cityName != name)
										{
											city._cityName = name;
											if(cid == Spot.focus)
												AppS.DispatchOnUIThreadLow(() => ShellPage.instance.focus.Content = city.nameAndRemarks);
										}
										if(Spot.focus != cid)
											city.SetFocus(true);
										//
									}

									break;

								}


							case "ext":
								{
									Assert(false);
									break;
								}


							case "citydata":
								{
									try
									{
										var jse = jsp.Value;
										// var priorCid = cid;
										var cid = jse.GetInt("cid");
										//if (!ShellPage.IsWorldView())
										// AGame.cameraC = cid.CidToWorldV();
										var isFromTs = jse.TryGetProperty("ts", out _);
										//Note.L("citydata=" + cid.CidToString());
										var city = GetOrAddCity(cid);
										city.LoadCityData(jse);

										// If it does not include TS it is from a call to chcity
										// Otherwise is is from a change in TS

										if(!isFromTs)
										{
											//		if (cid != City.build)
											//		   city.SetBuild(false);
										}
										if(isFromTs && cid == DungeonView.openCity && DungeonView.IsVisible())
											//   if (jse.TryGetProperty("ts", out _))
											//  {
											ScanDungeons.Post(cid, city.commandSlots == 0, false);  // if command slots is 0, something was not send correctly
										NavStack.Push(cid);
										if(waitingOnCityData.Length > 0)
										{
											var allDone = true;
											foreach(var i in waitingOnCityData)
											{
												if(i.cid == cid)
													i.Done();
												allDone &= i.isDone;
											}
											if(allDone)
												waitingOnCityData = Array.Empty<WaitOnCityDataData>();
										}
									}
									catch(Exception ex)
									{
										LogEx(ex);
									}
									finally
									{

									}


									break;

								}
							case "OGA":
								{
									Log("OGA" + eValue.ToString());
									break;
								}
							case "OGR":
								{
									//  Log(e.Value);
									break;
								}
							case "snd":
								{
									UpdateSenatorInfo();
									break;
								}
							case "OGT":
								{
									// Log(e.Value);
									break;
								}
							case "aldt":
								{
									Log("Aldt");
									Alliance.Ctor(jsDoc);

									// now we can update player info
									//Cosmos.PublishPlayerInfo(jsBase.pid, City.build, jsBase.token, jsBase.cookies);


									break;
								}
							//case "gPlA":
							//	{
							//		Player.Ctor(jsp.Value);
							//		//await 
							//		//while(!ppdtInitialized || !Alliance.diplomacyFetched)
							//		// await Task.Delay(500).ConfigureAwait(false);
							//		////if (Player.isAvatarOrTest)
							//		////{
							//		//// AppS.DispatchOnUIThreadLow(() =>
							//		//// {
							//		//// // create a timer for precense updates
							//		//// presenceTimer = new DispatcherTimer();
							//		////  presenceTimer.Interval = TimeSpan.FromSeconds(16);
							//		////  presenceTimer.Tick += PresenceTimer_Tick; ;
							//		////  presenceTimer.Start();
							//		//// // Seed it off

							//		////});
							//		//// PresenceTimer_Tick(null, null); // seed it off, but only after our token has time to have been set
							//		////}
							//		break;
							//	}
							// city lists
							case "ppdt":
								{
									var jse = jsp.Value;
									UpdatePPDT(jse, jse.TryGetProperty("pid", out var _pid) ? _pid.GetAsInt() : Player.myId);
									break;
								}
							case "chat":
								{
									var jsv = jsp.Value.Clone();
									AppS.DispatchOnUIThreadLow(() => ChatTab.ProcessIncomingChat(jsv));

									break;
								}
							case "chatin":
								var s = jsp.Value.GetString();
								AppS.DispatchOnUIThreadLow(() => ChatTab.PasteToChatInput(s));
								break;
							case "copyclip":
								{
									AppS.CopyTextToClipboard(jsp.Value.GetAsString());
									break;
								}
							case "cmd":
								{
									var str = jsp.Value.GetAsString();
									OpenAttackSender(str);
									break;
								}
							case "setglobals":
								{
									Assert(false);
									//var jso = jsp.Value;
									//var raidSecret = jso.GetString("secret");
									//var pid = jso.GetInt("pid");

									//pendingCookies = null;
									//var pn = jso.GetString("pn");
									//var ppdt = jso.GetProperty("ppdt");
									//var token = jso.GetString("token");
									//var s = jso.GetString("s");
									//var cid = jso.GetAsInt("cid");
									//AddPlayer(false, true, pid, pn, token, raidSecret, s, ppdt.ToString());

									//var city = City.GetOrAdd(cid);
									//// If they are visiting somene elses city we don't want to be directed there
									//// so we go to the default city
									//UpdatePPDT(ppdt, pid,updateBuildCity:(city.pid != pid) ); 

									//if (city.pid == pid) // we want ot visit a specific city
									//{
									//CitySwitch(cid,true);
									//}

									break;
								}
							case "restoreglobals":
								{
									Assert(false);
									//Note.Show("Cookies failed, maybe they need to log in again to refresh cookies?");
									//// only need to restore cookies
									//CookieDB.Apply(jsVars.cookies);
									//pendingCookies = null;

									//AppS.DispatchOnUIThreadLow(() => ShellPage.instance.friendListBox.SelectedItem = Player.activePlayerName);

									break;
								}
							case "c":
								{

									var jso = jsp.Value;
									var popupCount = jso.GetAsInt("p");
									//     Note.L("cid=" + cid.CidToString());
									if(ppdtInitialized && jso.TryGetProperty("v", out var v))
									{
										var vm = (ViewMode)v.GetAsInt();
										switch(vm)
										{
											case ViewMode.city:
												View.cameraZoom = View.cityZoomDefault;
												break;
											case ViewMode.region:
												View.cameraZoom = View.cameraZoomRegionDefault;
												break;
											case ViewMode.world:
												View.cameraZoom = View.cameraZoomWorldDefault;

												break;
										}
										Spot.build.BringCidIntoWorldView(false);
										ClientView.AutoSwitchViewMode();
									}

									//   ShellPage.SetViewMode((ViewMode)jso.GetInt("v"));
									if(jso.TryGetProperty("pop", out var pop))
									{
										var str = pop.ToString();

										var popup = JsonSerializer.Deserialize<Models.JSPopupNode[]>(str, JSON.jsonSerializerOptions);
										Log(popup.Length.ToString());
										// AppS.DispatchOnUIThreadLow(() => Models.JSPopupNode.Show(popup));
										Models.JSPopupNode.Show(popup);

									}
									//  ShellPage.NotifyCotgPopup(popupCount);
									//                                ShellPage.SetCanvasVisibility(noPopup);
									if(ppdtInitialized && jso.TryGetProperty("c", out var _cid))
									{
										// this should be rare, sometimes the JS city is out of sync with the registered city
										// Assert(false);
										var cid = _cid.GetAsInt();
										if(cid != Spot.build)
											CitySwitch(cid, true, false, false, false, false);
									}
									break;
								}

								//case "stable":
								//    {
								//        var jse = jsp.Value;
								//        int counter = 0;
								//        StringBuilder sb = new StringBuilder();
								//        foreach (var i in jse.EnumerateArray())
								//        {
								//            sb.Append('"');

								//            sb.Append(HttpUtility.JavaScriptStringEncode(i.GetString()));
								//            sb.Append("\" /* " + counter++ + " */,"); 

								//        }
								//        var s = sb.ToString();
								//        Log(s);
								//        break;

								//    }
								//    break;
						}

					if(gotCreds)
						CnVClient.InitializeForWorld();
				}
				//}


				// }
				//var cookie = httpClient.DefaultRequestHeaders.Cookie;
				//cookie.Clear();
				//foreach (var c in jsVars.cookie.Split(";"))
				//{
				//    cookie.ParseAdd(c);
				//}






				catch(Exception ex)
				{

					LogEx(ex);
				}
			});
		}




		//private static async void PresenceTimer_Tick(object sender, object e)
		//{
		//	/*
		//	var players = await Cosmos.GetPlayersInfo();
		//	var changed = false;
		//	int put = 0;
		//	int validCount = 0;
		//	foreach (var _p in players)
		//	{
		//		var pid = int.Parse(_p.id);
		//		if (pid == Player.myId || Friend.all.Any(a =>a.pid==pid) || Player.isAvatarOrTest )
		//			++validCount;
		//	}
		//	var presence = new PlayerPresence[validCount];
		//	foreach (var _p in players)
		//	{
		//		var p = new PlayerPresence(_p);
		//		int priorCid;
		//		var pid = p.pid;
		//		if (!(pid == Player.myId || Friend.all.Any(a => a.pid == pid)||Player.isAvatarOrTest))
		//			continue;

		//		var priorIndex = PlayerPresence.all.IndexOf( ( a) => a.pid == pid );
		//		if (priorIndex == -1)
		//		{
		//			changed = true;
		//			priorCid = 0;
		//		}
		//		else
		//		{
		//			if (PlayerPresence.all[priorIndex].token != p.token)
		//				changed = true; // need to refresh token
		//			priorCid = PlayerPresence.all[priorIndex].cid;
		//		}

		//	//	Player.myIds.Add(pid);
		//	// TODO:  restore this functionality when it works again
		//		if (pid != Player.myId)
		//		{
		//			if (p.cid != priorCid)
		//			{
		//				if (p.cid == City.build && priorCid != City.build)
		//					Note.Show($"{p.name } has joined you in {p.cid.CidToStringMD()}");
		//				if (p.cid != City.build && priorCid == City.build)
		//					Note.Show($"{p.name } has left {p.cid.CidToStringMD()}");

		//			}
		//		}
		//		presence[put++] = p;

		//	}
		//	PlayerPresence.all = presence;

		//	if(changed)
		//	{
		//		App.(() =>
		//		{
		//			// Update menu
		//			ShellPage.instance.friendListBox.SelectedIndex = -1;
		//			ShellPage.instance.friendListBox.Items.Clear();
		//			int counter = 0;
		//			int sel = -1;
		//			foreach (var p in PlayerPresence.all)
		//			{
		//				ShellPage.instance.friendListBox.Items.Add(p.name);
		//				if (p.pid == Player.myId)
		//					sel = counter;
		//				++counter;
		//				// reset menu, TOTO:  Keep track of active selection
		//			}

		//			ShellPage.instance.friendListBox.SelectedIndex = sel;
		//			ShellPage.instance.friendListBox.Visibility = PlayerPresence.all.Length > 1 ? Visibility.Visible : Visibility.Collapsed;
		//		});
		//	}
		//	*/
		//}

	}
}
