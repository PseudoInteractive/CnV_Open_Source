
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV;

using System.Net.Http.Json;
using System.Text.Json;
using Windows.System;
using static CnV.Troops;
using DiscordCnV;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Services;
using Syncfusion.UI.Xaml.DataGrid;
using Syncfusion.UI.Xaml.Grids.ScrollAxis;
using static CnV.City;
using static CnV.CityUI;
using Microsoft.UI.Xaml.Media;

public static partial class CityUI
{
//	public static Action<City[]> cityListChanged;

	public static void SyncCityBox()
	{
		try
		{
			if( Sim.isWarmup || City.gridCitySource.Count==0 || CityStats.instance is null || City.GetBuild().IsInvalid())
			{
				return;// await Task.Delay(1000).ConfigureAwait(false);
			};
			AppS.QueueOnUIThread(() =>
								{
									try
									{
										var _build = City.GetBuild();
										if(!object.ReferenceEquals(_build,CityStats.instance.cityBox.SelectedItem))
										{
//											var id = City.gridCitySource.IndexOf(_build);
											CityStats.instance.cityBox.SelectedItem = _build;
										}
									}
									catch(Exception _ex)
									{
										LogEx(_ex);

									}
								});
		}
		catch(Exception ex)
		{
			LogEx(ex);
		}
	}
	public  static void Show(this Artifact.ArtifactType type, object sender)
		{
			//var level = Player.me.title.rank;
			var art = Artifact.GetForPlayerRank(type);
			if(art is not null)
				ArtifactDialogue.ShowInstance(art);
			else
				AppS.MessageBox("No artifact type for your rank");
		}
	public static void Init()
	{
		World.worldFirstLoadedActions += CityUI.LoadFromPriorSession;
	}

	internal static void Refresh()
	{
		Player.active.OnPropertyChanged();
		CityStats.Invalidate();
		PlayerStats.Changed();
	}
	public static  void AddToFlyout(this City me, MenuFlyout flyout, bool useSelected = false)
	{
		var cid     = me.cid;
		var aSetup  = AApp.AddSubMenu(flyout, "Setup..");
		var aMisc   = flyout.AddSubMenu("Misc..");
		var aExport = new MenuFlyoutSubItem();// flyout.AddSubMenu("Import/Export..");
		var aWar    = AApp.AddSubMenu(flyout, "War..");
		if (me.isCityOrCastle)
		{
			// Look - its my city!
			if (me.isSubOrMine )
			{

				//{
				//	var tags = TagHelper.GetTags(this);
				//	var tagFlyout = AApp.AddSubMenu(flyout, "Tags");
				//	foreach(var t in TagHelper.tags)
				//	{
				//		var n = t.s;
				//		var id = t.id;
				//		AApp.AddItem(tagFlyout, n,tags.HasFlag(id),(on)=>SetTag(id,on) );


				//	}
				//}
				// This one has multi select
				
				int count = 1;
				if (useSelected)
				{
					count = MainPage.GetContextCidCount(cid);
				}

				var aRaid = AApp.AddSubMenu(flyout, "Raid..");
				aRaid.AddItem($"Raid ..", () => DungeonView.ShowDungeonList(City.Get(cid),false));
	//			return await DungeonView.ShowDungeonList(City.Get(_cid),  autoRaid);


				if (count > 1)
				{
					aRaid.AddItem($"End Raids x{count} selected", MainPage.ReturnSlowClick, cid)
						.SetToolTip("Returns raids when they are done (stops them from repeating");
					aRaid.AddItem($"Stop Raids x{count} selected", MainPage.ReturnFastClick, cid)
						.SetToolTip("Stops raids and returns them immediately");
				//	aRaid.AddItem($"Return At...x{count}",        ()=> ReturnAtBatch(cid) );

				}
				else
				{

					aRaid.AddItem("End Raids",    me.ReturnSlowClick)
						.SetToolTip("Returns raids when they are done (stops them from repeating");
					aRaid.AddItem("Stop Raids",    me.ReturnFastClick).SetToolTip("Stops raids and returns them immediately");;
				//	aRaid.AddItem("Return At...", me.ReturnAt);
				}


				aSetup.AddItem("Setup...",    (_, _) => CityUI.SetupClick(cid,SetupFlags.suggestAutobuild | SetupFlags.layout));
				aSetup.AddItem("Settings...",    (_, _) => CityUI.SetupClick(cid,SetupFlags.none));
				aSetup.AddItem("Name...",    (_, _) => CityUI.SetupClick(cid,SetupFlags.name));
				aSetup.AddItem("Find Hub",    (_, _) => CityUI.SetClosestHub(cid));
				aSetup.AddItem("Set Recruit", (_, _) => CitySettings.SetRecruitFromTag(cid));
				//aSetup.AddItem("Change...",   (_, _) => ShareString.Show(cid, default));
				aSetup.AddItem("Move Stuff",  (_, _) => me.MoveStuffLocked());
				//aSetup.AddItem("Remove Castle", (_, _) => 
				//{
				//	CityBuild.

				//}
				//   AApp.AddItem(flyout, "Clear Res", (_, _) => CnVServer.ClearCenterRes(cid) );
				aSetup.AddItem("Clear Res", me.ClearRes);


				aExport.AddItem("Troops to Sheets", me.CopyForSheets);
			}
			else
			{
				if ( me.cityName == null)
				{
					Sim.FetchCity(cid);
				}

			}

			{
				var sel = Spot.GetSelectedForContextMenu(cid, false);
				{
					aWar.AddItem("Attack",(_,_) => SendTroops.ShowInstance(GetBuild(),City.Get(cid),isSettle: false,viaWater: false,type: ArmyType.assault));
					var multiString = sel.Count > 1 ? $" _x {sel.Count} selected" : "";
				//	aWar.AddItem("Cancel Attacks..", me.CancelAttacks);
					var afly = aWar.AddSubMenu("Attack Planner");
					if (!me.player.isAllyOrNap)
					{
						afly.AddItem("Ignore Player" + multiString, (_, _) => AttackTab.IgnorePlayer(cid.CidToPid()));
					}

					afly.AddItem("Add as Target" + multiString, (_, _) => AttackTab.AddTarget(sel));
					afly.AddItem("Add as Attacker" + multiString, (_, _) =>
																{
																	using var work =
																			new WorkScope("Add as attackers..");

																	string s = string.Empty;
																	foreach (var id in sel)
																	{
																		s = s + id.CidToString() + "\t";
																	}

																	AttackTab.AddAttacksFromString(s, false);
																	Note.Show($"Added attacker {s}");

																});

				}
				//else
				//if (!Alliance.IsAllyOrNap(me))
				//{
				//	aWar.AddItem("Add funky Attack String", async (_, _) =>
				//											{
				//												using var work =
				//														new WorkScope("Add to attack string..");

				//												foreach (var id in sel)
				//												{
				//													await Sim.AddToAttackSender(id);
				//												}
				//											}
				//				);
				//}
				//AApp.AddItem(flyout, "Add as Fake (2)", (_, _) => AttackTab.AddTarget(cid, 2));
				//AApp.AddItem(flyout, "Add as Fake (3)", (_, _) => AttackTab.AddTarget(cid, 3));
				//AApp.AddItem(flyout, "Add as Fake (4)", (_, _) => AttackTab.AddTarget(cid, 3));
			}
			//if (cid != City.build)
			{
				aSetup.AddItem("Set target hub", (_, _) => CityUI.SetTargetHub(City.build, cid));
				aSetup.AddItem("Set source hub", (_, _) => CityUI.SetSourceHub(City.build, cid));
				//if(Player.myName == "Avatar")
				//    AApp.AddItem(flyout, "Set target hub I", (_, _) => CitySettings.SetOtherHubSettings(City.build, cid));
			}


		//	aWar.AddItem("Attack",       (_, _) => Spot.JSAttack(cid));
			aWar.AddItem("Near Defence", me.DefendMe);
			if (me.incoming.Any())
				aWar.AddItem("Incoming", me.ShowIncoming);
			aWar.AddItem("Hits", me.ShowHitHistory );
			//if(me.outgoing.Any())
			
			//	if (Raid.test)
			aWar.AddItem("Send Defence",            (_, _) => City.GetBuild().SendDefence(WorldC.FromCid(cid) ));
		//	aWar.AddItem("Recruit Sen",             (_, _) => Recruit.Send(cid, ttSenator, 1, true));
			aWar.AddItem("Show Reinforcements",     (_, _) => ReinforcementsTab.ShowReinforcements(cid, null));
			aWar.AddItem("Show All Reinforcements", (_, _) => ReinforcementsTab.ShowReinforcements(0,   null));
			aExport.AddItem("Defense Sheet", me.ExportToDefenseSheet);
			AApp.AddItem(flyout, "Send Res", (_, _) => SendResDialogue.ShowInstance(City.GetBuild(),me,null,null,palaceDonation:null));
			AApp.AddItem(flyout, "Near Res", me.ShowNearRes);
			if (me.isSubOrMine)
			{
				aWar.AddItem("Dismiss..",(_,_) => DismissDialog.ShowInstance(me));
				AApp.AddItem(flyout, "Do the stuff",  (_, _) => me.DoTheStuff());
			//	AApp.AddItem(flyout, "Food Warnings", (_, _) => CitySettings.SetFoodWarnings(cid));
				flyout.AddItem("Ministers", me.ministersOn, me.SetMinistersOn);
			}
		}
		else if (me.isDungeonOrBoss)
		{
			AApp.AddItem(flyout, "Raid", (_, _) => City.GetBuild().Raid(WorldC.FromCid(cid) ));

		}
		if (me.isEmpty || (me.isCityOrCastle && !me.isCastle && me.isLawless) )
		{
			AApp.AddItem(flyout, "Settle", (_,_)=> City.GetBuild().Settle(WorldC.FromCid(cid) ) );
		//	AApp.AddItem(flyout, "Claim", (_,_)=> CityUI.DiscordClaim(WorldC.FromCid(cid) ) );

		}
		if(me.isBlessed && me.isAlliedWithPlayer) {
			AApp.AddItem(flyout, "Palace Wood Artifact..", (_,_)=> ArtifactDialogue.ShowInstance(Artifact.GetForPlayerRank(Artifact.ArtifactType.Arch),false,me.c) );;
			AApp.AddItem(flyout, "Palace Stone Artifact..", (_,_)=> ArtifactDialogue.ShowInstance(Artifact.GetForPlayerRank(Artifact.ArtifactType.Pillar),false,me.c) );;

		}
		
//		aMisc.AddItem("Notify on Decay", ()=>DecayQuery(cid));
		//if (Raid.test)
		//{
		//	aMisc.AddItem("Settle whenever water", (_, _) => Spot.TrySettle(City.build, cid, true));
		//	aMisc.AddItem("Settle whenever land",  (_, _) => Spot.TrySettle(City.build, cid, false));
		//}

		aMisc.AddItem("Distance",       (_, _) => me.ShowDistanceTo());
		aMisc.AddItem("Select",         (_, _) => me.SelectMe(true, AppS.keyModifiers));
		aMisc.AddItem("Coords to Chat", () => CoordsToChat(cid));
		if(AppS.isTest) {
			aMisc.AddItem("Fixes",         (_, _) => FixDialog.ShowInstance(me));
			
		}
		flyout.RemoveEmpy();
	}

	public static void CoordsToChat(int _cid)
	{
		var           targets = Spot.GetSelectedForContextMenu(_cid, false, onlyMine: false, onlyCities: false);
		StringBuilder sb      = new();
		var           first   = true;
		foreach(var cid in targets)
		{
			if(first)
				first = false;
			else
				sb.Append('\t');
			sb.Append(cid.CidToCoords());
		}
		var str = sb.ToString();
		AppS.CopyTextToClipboard(str);
		ChatTab.PasteToChatInput(str);
	}

		public static void DecayQuery(WorldC cid)
	{
		Sim.gStCB(cid.cid, DecayQueryCB, AMath.random.Next());
	}

	public static void ScrollMeIntoView(this City city) => ScrollIntoView(city.cid);

	public static void ScrollIntoView(int cid)
	{
		//         await Task.Delay(2000);
		//          instance.Dispatcher.RunAsync(DispatcherQueuePriority.Low, () =>
		//           {
		//   await Task.Delay(200);
		AppS.QueueOnUIThread(() =>
							{

								{
									/// MainPage.CityGrid.SelectedItem = this;
									//                      MainPage.CityGrid.SetCurrentItem(this);

									//     MainPage.CityGrid.SetCurrentItem(this,false);
									if(MainPage.IsVisible())
										MainPage.CityGrid.ScrollItemIntoView(City.GetOrAdd(cid));
									if(BuildTab.IsVisible())
										BuildTab.CityGrid.CurrentItem = (City.GetOrAdd(cid));
									// await Task.Delay(200);
									//MainPage.CityGrid.SelectItem(this);
									//var id = gridCitySource.IndexOf(this);
									//if (id != -1)
									//{
									//    MainPage.CityGrid.ScrollIndexIntoView(id);

									//}
								}
								// todo: donations page and boss hunting


								// ShellPage.instance.coords.Text = cid.CidToString();
								//            });
							});

	}
	async static void DecayQueryCB(JsonElement jso)
	{
		//var type = jso.GetAsInt("type");
		//var _cid = jso.GetAsInt("cid");
		//Assert(cid == _cid);
		//if(type != 3 && type != -1) // 4 is empty, 3 is city or ruins, -1 means not open (for a continent)
		//{
		//	AppS.DispatchOnUIThreadLow(() =>
		//								{
		//									var dialog = new ContentDialog()
		//									{
		//											Title             = "Spot has Changed",
		//											Content           = cid.CidToString(),
		//											PrimaryButtonText = "Okay"
		//									};
		//									//Settings.BoostVolume();
		//									ElementSoundPlayer.Play(ElementSoundKind.Invoke);
		//									ToastNotificationsService.instance.SpotChanged($"{cid.CidToString()} has changed");
		//									dialog.ShowAsync2();
		//								});
		//	CityUI.ShowCity(cid, false);
		//}
		//else
		//{
		//	//	Note.Show($"Query {cid.CidToStringMD()},type:{type}");
		//	await Task.Delay(60 * 1000);
		//	DecayQuery();
		//}
	}

	static bool loaded;

	public static void LoadFromPriorSession()
	{
		if(!loaded)
		{

			loaded              = true;
			Settings.pinned = Settings.pinned.ArrayRemoveDuplicates();

			foreach(var m in Settings.pinned)
			{
				var spot = SpotTab.TouchSpot(m, VirtualKeyModifiers.None, false, true);
			}
		}
	}
	public static void ShowContextMenu(this City me,UIElement uie, Windows.Foundation.Point position)
	{

		//   SelectMe(false) ;
	
		var flyout = new MenuFlyout();
		AddToFlyout(me,flyout, uie == MainPage.CityGrid || uie == BuildTab.CityGrid);
		flyout.SetXamlRoot(uie);

		//   flyout.XamlRoot = uie.XamlRoot;
		flyout.ShowAt(uie, position);
	}
	

	public static void SelectMe(this City me, bool showClick = false, VirtualKeyModifiers mod = VirtualKeyModifiers.Shift, bool scrollIntoView = true)
	{
		var cid = me.cid;
		if(showClick || scrollIntoView)
			NavStack.Push(cid);
		SpotTab.AddToGrid(me, mod, true, scrollIntoView);
		if(showClick)
		{
			CityUI.ShowCity(cid, true);
		}
	}
	public static void SyncSelectionToUI(bool scrollIntoView, Spot focusSpot = null)
	{
		++SpotTab.silenceSelectionChanges;
		try
		{
			foreach(var tab in UserTab.userTabs)
			{
				if(!tab.isFocused)
					continue;
				foreach(var grid in tab.dataGrids)
				{

					//			var grid = gridX.Key;

					//		if(!gridX.Value?.isFocused == true)
					//		continue;

					if(grid.IsCityGrid())
					{
						var uiInSync = false;
						var sel1 = grid.SelectedItems;
						if(selected.Count == sel1.Count)
						{
							uiInSync = true;
							foreach(var i in sel1)
							{
								if(!selected.Contains((i as City).cid))
								{
									uiInSync = false;
									break;
								}
							}
						}

						if(!uiInSync)
						{
							selected.SyncList(sel1,(cid,spot) => cid == ((Spot)spot).cid,
								(cid) => City.Get(cid));
						}

						if((scrollIntoView) && (sel1.Any() || focusSpot != null))
						{
							var current = focusSpot ?? (City.GetBuild().isSelected ? City.GetBuild() : null);
							if(current != null)
							{
								grid.CurrentItem = current;
							}

							var any = current ?? sel1.First();
							{
								var rowIndex = grid.ResolveToRowIndex(any);
								var columnIndex = grid.ResolveToStartColumnIndex();
								if(rowIndex >= 0)
									grid.ScrollInView(new RowColumnIndex(rowIndex,columnIndex));
							}
						}

						if(AttackTab.IsVisible() && focusSpot != null)
						{
							try
							{
								if(AttackTab.attacks.Contains(focusSpot.cid)
									 && !AttackTab.instance.attackGrid.SelectedItems.Contains(focusSpot))
								{
									AttackTab.instance.attackGrid.SelectedItem = focusSpot as City;
									AttackTab.instance.attackGrid.ScrollIntoView(focusSpot,null);
								}

								if(AttackTab.targets.Contains(focusSpot.cid)
									&& !AttackTab.instance.targetGrid.SelectedItems.Contains(focusSpot))
								{
									AttackTab.instance.targetGrid.SelectedItem = focusSpot as City;
									AttackTab.instance.targetGrid.ScrollIntoView(focusSpot,null);
								}
							}
							catch
							{
							}
						}
					}
				}
			}
		}
		catch(Exception ex)
		{
			LogEx(ex);
		}
		finally
		{
			--SpotTab.silenceSelectionChanges;
		}

	}
	
	public static bool OnKeyDown(object _spot, VirtualKey key)
	{
		var spot = _spot as Spot;
		switch(key)
		{
			case VirtualKey.Enter:
				spot.SetFocus(false);
				return true;
				break;
			case VirtualKey.Space:
			{
				if(spot.isSubOrMine)
					spot.ShowDungeons();
				else
					spot.SetFocus(false);
				return true;
			}

			default:
				break;
		}
		return false;
	}

	
	public static async void SetupClick(int _intialCid, SetupFlags flags)
	{
		var cids =Spot.GetSelectedForContextMenu(_intialCid);
		foreach(var cid in cids)
		{
			var _cid = cid;
			await ShareString.Show(_cid,flags);
			{
				break;
			}
		}
	}
	
	public static void DefendMe(this Spot me)
	{
		var cids = GetSelectedForContextMenu(me.cid, false);

		NearDefenseTab.defendants.Set(cids.Select(a => City.Get(a)), true);

		var tab = NearDefenseTab.instance;
		tab.ShowOrAdd(true);
		tab.refresh.Go();
	}


	public static void ShowNearRes(this City me)
	{
		var tab = NearRes.instance;
		tab.target = me;
		if(!tab.isOpen)
		{
			tab.ShowOrAdd(true);
		}
		else
		{
			if(!tab.isFocused)
				TabPage.Show(tab);
			else
				tab.refresh.Go();
		}
	}
	public static void ShowIncoming(this City me) {
		if(me.isAlliedWithPlayer) {


			var tab = IncomingTab.instance;
			AppS.DispatchOnUIThread(() => tab.Show());

			AppS.DispatchOnUIThreadIdle(() => {
				tab.defenderGrid.SetFocus(me);
			});

		}
		else {
			var tab = OutgoingTab.instance;
			AppS.DispatchOnUIThread(() => tab.Show());
			AppS.DispatchOnUIThreadIdle(() =>
										{
											tab.attackerGrid.SetFocus(me);
										});
		}
	}
	public static void ShowHitHistory(this City me) {
			var tab = HitHistoryTab.instance;
			AppS.DispatchOnUIThread(() => tab.SetFilter(me) );
	
	}


//	public static async void DiscordClaim(WorldC cid)
//	{
//		if(!DGame.isValidForIncomingNotes)
//		{
//			Log("Invalid");
//			return;
//		}
//		try
//		{
//			Note.Show($"Registering claim on {cid.ToStringCoords()}");
//			//var client = Sim.genericClient;


//			var message = new DGame.Message() { username = "Cord Claim", content = $"{cid.ToStringCoords()} claimed by {Player.myName}", avatar_url = "" };

//			//var content =  JsonContent.Create(message);
//			//, JSON.jsonSerializerOptions), Encoding.UTF8,
//			//	   "application/json");

//			var result = await client.PostAsJsonAsync(DGame.discordHook, message);
//			result.EnsureSuccessStatusCode();
//		}
//		catch(Exception ex)
//		{
//			LogEx(ex);
//		}


//	}

}

public partial class City
{
	

	public void SetFocus(bool scrollIntoView, bool select = true, bool bringIntoWorldView = true, bool lazyMove = true)
	{
		SetFocus(cid, scrollIntoView, select, bringIntoWorldView, lazyMove);
	}

	public static async 
	Task ProcessCoordClick(int cid, bool lazyMove, VirtualKeyModifiers mod, bool scrollIntoUI = false)
	{
		mod.UpdateKeyModifiers();

		//if(mod.IsShiftAndControl() && AttackTab.IsVisible() && City.Get(cid).isCastle)
		//{
		//	var city = City.Get(cid);
		//	{
		//		using var __lock = await AttackTab.instance.TouchLists();
		//		var prior = AttackPlan.Get(cid);
		//		var isAttack = prior != null ? prior.isAttack : city.IsAllyOrNap();
		//		if (isAttack)
		//		{
		//			AttackPlan.AddOrUpdate( new(city, isAttack,city.attackType switch
		//			{ AttackType.assault => AttackType.senator, AttackType.senator => AttackType.se, AttackType.se => AttackType.none, _ => AttackType.assault }));
		//		}
		//		else
		//		{
		//			AttackPlan.AddOrUpdate( new(city, isAttack, city.attackType switch
		//			{
		//				AttackType.seFake => AttackType.se,
		//				AttackType.se => AttackType.senatorFake,
		//				AttackType.senatorFake => AttackType.senator,
		//				AttackType.senator => AttackType.none,
		//				_ => AttackType.seFake
		//			}));
		//		}
		//	}
		//	Note.Show($"{city.nameAndRemarks} set to {city.attackType}", Debug.Priority.high);
		//	AttackTab.WritebackAttacks();
		//	AttackTab.WaitAndSaveAttacks();
		//}
		//else
		if( ((City.CanVisit(cid) && cid==focus) || Player.active.IsMyCity(cid) ) && !mod.IsShiftOrControl())
		{
			if(City.IsBuild(cid))
			{
				if( cid==focus)
					View.SetViewMode(View.viewMode.GetNext());// toggle between city/region view
			//	if(scrollIntoUI)
				{
					CityUI.ShowCity(cid, lazyMove, true, scrollIntoUI);
				}
			//	else
			//	{
			//		cid.BringCidIntoWorldView(lazyMove);
			//	}
			}
			else
			{
				if( Player.IsSubOrMe(cid.CidToPid() ) || (await AppS.DoYesNoBox("Sub", $"Switch to sub {cid.AsCity().player.name}?" )==1) ) {
					await CnVClient.CitySwitch(cid, false, false, scrollIntoUI); // keep current view, switch to city
																			   //	View.SetViewMode(ShellPage.viewMode.GetNextUnowned());// toggle between city/region view
				}
				else {
					CityUI.ShowCity(cid, lazyMove, true, scrollIntoUI);
				}
			}
			NavStack.Push(cid);

		}
		else
		{
			CityUI.ShowCity(cid, lazyMove, true, scrollIntoUI);
			NavStack.Push(cid);
		}
		//Spot.GetOrAdd(cid).SelectMe(false,mod);
		SpotTab.TouchSpot(cid, mod, false);


	}

	internal void Settle(WorldC worldC)
	{
		SendTroops.ShowInstance(this,City.Get(worldC), isSettle:true,viaWater:cont != worldC.continentDigits,type:ArmyType.defense );
	}
	internal void Raid(WorldC worldC)
	{
		SendTroops.ShowInstance(this,City.Get(worldC),isSettle: false,viaWater:cont != worldC.continentDigits, type:ArmyType.raid) ;
	}
	internal void SendDefence(WorldC worldC)
	{
		SendTroops.ShowInstance(this,City.Get(worldC),isSettle: false,viaWater:cont != worldC.continentDigits, type:ArmyType.defense) ;
	}
}

public partial class City
{

	public int constructionSpeed => stats.cs;
	public void CityRowClick(GridCellTappedEventArgs e)
	{
		var modifiers  = AppS.keyModifiers;
		var wantSelect = true;
		switch(e.Column.MappingName)
		{

			case nameof(cityName):
			case nameof(icon):
			case nameof(remarks):
				wantSelect = false;
				DoClick();
				break;
			case nameof(bStage):
				DoTheStuff();
				break;
			case nameof(tsHome):
			case nameof(tsRaid):
				if(City.CanVisit(cid))
				{
					Raiding.UpdateTS(true, true);
				}
				break;
			case nameof(City.autobuild):
				autobuild  = !autobuild;
				wantSelect = false;

				return;
			case nameof(City.AutoWalls):
				AutoWalls  = !autoWalls;
				wantSelect = false;

				return;
			case nameof(City.AutoTowers):
				AutoTowers = !autoTowers;
				wantSelect = false;
				return;
			case nameof(City.ministersOn):
				SetMinistersOn(!ministersOn);
				wantSelect = false;
				return;
			case nameof(City.raidCarry):
				if(City.CanVisit(cid))
				{
					Raiding.Return(cid, true,fast:false);
				}
				break;
			case nameof(nameAndRemarks):
				// first click selects
				// second acts as coord click
				if(IsSelected(cid))
				{
					ProcessCoordClick(cid, false, modifiers, false);
					wantSelect = false;
				}


				break;
			case nameof(xy):
				ProcessCoordClick(cid, false, modifiers, false);
				wantSelect = false;
				break;
			
			case nameof(City.dungeonsToggle):
			{
				ShowDungeons();
				wantSelect = false;
				break;
			}
			case nameof(City.tsTotal):
				if(City.CanVisit(cid))
				{
					Raiding.UpdateTS(true, true);
				}

				break;
			case nameof(City.raidReturn):
				if(City.CanVisit(cid))
				{
					Raiding.Return(cid,updateUI: true,fast: true);
				}
				break;
			case nameof(pinned):
			{
				var newSetting = !pinned;

				SetPinned(newSetting);
				wantSelect = false;
			}
				return;

		}
		if(wantSelect)
			SetFocus(false, true, true, false);
		NavStack.Push(cid);

	}
	public Task DoTheStuff()
	{


		return AppS.DispatchOnUIThreadExclusive(cid, async () =>
														{
															await CnV.DoTheStuff.Go(this, true, true);
														});

	}
	public void ClearRes()
	{
		AppS.DispatchOnUIThreadExclusive(cid,async () =>
		{
			await ClearResUI();
		});
	}
	


}