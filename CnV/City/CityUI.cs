﻿
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
									//	if(!object.ReferenceEquals(_build,CityStats.instance.cityBox.city))
										{
//											var id = City.gridCitySource.IndexOf(_build);
											CityStats.instance.cityBox.SetCity(_build,false);
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
	public  static void Show(this Artifact.ArtifactType type, object __)
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
		City.GetBuild().OnPropertyChanged();
		PlayerStats.Changed();
	}
	public static  void AddToFlyout(this City me, (Flyout flyout,NavigationView nav) flyout, bool useSelected = false)
	{
		var cid     = me.cid;
		var city = me;


//		if(CanVisit(cid) && cid != City.build) {
//			flyout.AddItem( "Zirc",icon:ImageHelper.GetIcon("zirconia.png") , isPrimary:true, command:async ()=>
//				{
//					if(Player.IsSubOrMe(cid.CidToPid()) || (await AppS.DoYesNoBox("Sub",$"Switch to sub {cid.AsCity().player.name}?")==1)) {
//						await CnVClient.CitySwitch(cid); // keep current view, switch to city
//					}
//			});
//		}
//		flyout.AddItem("",icon: ImageHelper.GetIcon("zirconia.png"),isPrimary: true);
//		flyout.AddItem("",icon: ImageHelper.GetIcon("Diamonds.png"),isPrimary: true);
//		flyout.AddItem("aaaaaaaaaa",icon: ImageHelper.GetIcon("mana.png"),isPrimary: true);
//		//if( Player.IsSubOrMe(cid.CidToPid() ) || (await AppS.DoYesNoBox("Sub", $"Switch to sub {cid.AsCity().player.name}?" )==1) ) {
//		//			await CnVClient.CitySwitch(cid, clickMods); // keep current view, switch to city
//		//																	   //	View.SetViewMode(ShellPage.viewMode.GetNextUnowned());// toggle between city/region view
//		//		}
	
//		var aSetup  = AApp.AddSubMenu(flyout, "Setup..");
//		var aMisc   = flyout.AddSubMenu("Misc..");
//		var aExport = (flyout.root,new CommandBarFlyout());// flyout.AddSubMenu("Import/Export..");
//		var aWar    = AApp.AddSubMenu(flyout, "War..");
//		if(me.isCityOrCastle) {
//			// Look - its my city!
//			if(me.isSubOrMine) {

//				//{
//				//	var tags = TagHelper.GetTags(this);
//				//	var tagFlyout = AApp.AddSubMenu(flyout, "Tags");
//				//	foreach(var t in TagHelper.tags)
//				//	{
//				//		var n = t.s;
//				//		var id = t.id;
//				//		AApp.AddItem(tagFlyout, n,tags.HasFlag(id),(on)=>SetTag(id,on) );


//				//	}
//				//}
//				// This one has multi select

//				int count = 1;
//				if(useSelected) {
//					count = MainPage.GetContextCidCount(cid);
//				}

//				var aRaid = AApp.AddSubMenu(flyout,"Raid..");
//				aRaid.AddItem($"Raid ..",() => DungeonView.ShowDungeonList(City.Get(cid),false));
//				aRaid.AddItem($"Return for scheduled",me.ReturnRaidsForScheduled,toolTip:"Brings home raids in time for scheuled attacks or defense");
//				//			return await DungeonView.ShowDungeonList(City.Get(_cid),  autoRaid);


//				if(count > 1) {
//					aRaid.AddItem($"End Raids x{count} selected",()=>MainPage.ReturnSlowClick(cid) )
//						.SetToolTip("Returns raids when they are done (stops them from repeating");
//					aRaid.AddItem($"Stop Raids x{count} selected",() => MainPage.ReturnFastClick(cid) )
//						.SetToolTip("Stops raids and returns them immediately");
//					//	aRaid.AddItem($"Return At...x{count}",        ()=> ReturnAtBatch(cid) );

//				}
//				else {

//					aRaid.AddItem("End Raids",me.ReturnSlowClick)
//						.SetToolTip("Returns raids when they are done (stops them from repeating");
//					aRaid.AddItem("Stop Raids",me.ReturnFastClick).SetToolTip("Stops raids and returns them immediately"); ;
//					//	aRaid.AddItem("Return At...", me.ReturnAt);
//				}


//				aSetup.AddItem("Setup...",() => CityUI.SetupClick(cid,SetupFlags.suggestAutobuild | SetupFlags.layout));
//				aSetup.AddItem("Settings...",() => CityUI.SetupClick(cid,SetupFlags.none));
//				aSetup.AddItem("Name...",() => CityUI.SetupClick(cid,SetupFlags.name));

//				aSetup.AddItem("Set Recruit",() => CitySettings.SetRecruitFromTag(cid));
//				//aSetup.AddItem("Change...",   (_, _) => ShareString.Show(cid, default));
//				aSetup.AddItem("Move Stuff",() => me.MoveStuffLocked(),toolTip:"Moves around buildings, matching existing buildings with those in its layout");
//				//aSetup.AddItem("Remove Castle", (_, _) => 
//				//{
//				//	CityBuild.

//				//}
//				//   AApp.AddItem(flyout, "Clear Res", (_, _) => CnVServer.ClearCenterRes(cid) );
//				aSetup.AddItem("Clear Res",me.ClearRes);


//				aExport.AddItem("Troops to Sheets",me.CopyForSheets);
//			}
//			else {
//				if(me.cityName == null) {
//					Sim.FetchCity(cid);
//				}

//			}

//			{
//				var sel = Spot.GetSelectedForContextMenu(cid,false);
//				{
//					aWar.AddItem("Assault..",() => SendTroops.ShowInstance(GetBuild(),City.Get(cid),isSettle: false,type: ArmyType.assault));
//					aWar.AddItem("Siege..",() => SendTroops.ShowInstance(GetBuild(),City.Get(cid),isSettle: false,type: ArmyType.siege));
//					aWar.AddItem("Scout..",() => SendTroops.ShowInstance(GetBuild(),City.Get(cid),isSettle: false,type: ArmyType.scout));
//					aWar.AddItem("Attack Targets",() => AttackSender.ShowInstance(GetBuild(),City.Get(cid)));
//					var multiString = sel.Count > 1 ? $" _x {sel.Count} selected" : "";
//					//	aWar.AddItem("Cancel Attacks..", me.CancelAttacks);
//					//var afly = aWar.AddSubMenu("Attack Planner");
//					//if(!me.player.isAllyOrNap) {
//					//	afly.AddItem("Ignore Player" + multiString,(_,_) => AttackTab.IgnorePlayer(cid.CidToPid()));
//					//}

//					//afly.AddItem("Add as Target" + multiString,(_,_) => AttackTab.AddTarget(sel));
//					//afly.AddItem("Add as Attacker" + multiString,(_,_) => {
//					//	using var work =
//					//			new WorkScope("Add as attackers..");

//					//	string s = string.Empty;
//					//	foreach(var id in sel) {
//					//		s = s + id.CidToString() + "\t";
//					//	}

//					//	AttackTab.AddAttacksFromString(s,false);
//					//	Note.Show($"Added attacker {s}");

//					//});

//				}
//				//else
//				//if (!Alliance.IsAllyOrNap(me))
//				//{
//				//	aWar.AddItem("Add funky Attack String", async (_, _) =>
//				//											{
//				//												using var work =
//				//														new WorkScope("Add to attack string..");

//				//												foreach (var id in sel)
//				//												{
//				//													await Sim.AddToAttackSender(id);
//				//												}
//				//											}
//				//				);
//				//}
//				//AApp.AddItem(flyout, "Add as Fake (2)", (_, _) => AttackTab.AddTarget(cid, 2));
//				//AApp.AddItem(flyout, "Add as Fake (3)", (_, _) => AttackTab.AddTarget(cid, 3));
//				//AApp.AddItem(flyout, "Add as Fake (4)", (_, _) => AttackTab.AddTarget(cid, 3));
//			}
//			//if (cid != City.build)
//			{
//				aSetup.AddItem("Find Closest Hub",() => CityUI.SetClosestHub(cid));
//				aSetup.AddItem($"Set as Hub for {City.buildCity.FormatName(false,false,false)}",() => SetTradeSettings(City.build,sourceHub: cid,targetHub: cid));
//				aSetup.AddItem($"Set as Target hub for {City.buildCity.FormatName(false,false,false)}",() => SetTradeSettings(City.build,sourceHub: null,targetHub: cid));
//				aSetup.AddItem($"Set as Source hub for  {City.buildCity.FormatName(false,false,false)}",() => SetTradeSettings(City.build,sourceHub: cid,targetHub: null));
//				//if(Player.myName == "Avatar")
//				//    AApp.AddItem(flyout, "Set target hub I", (_, _) => CitySettings.SetOtherHubSettings(City.build, cid));
//			}


//			//	aWar.AddItem("Attack",       (_, _) => Spot.JSAttack(cid));
//			aWar.AddItem("Near Defence",me.DefendMe);
//			if(me.incoming.Any() && me.isAlliedWithPlayer)
//				aWar.AddItem("Incoming",me.ShowIncoming);
//			if(me.outgoing.Any())
//				aWar.AddItem("Incoming",me.ShowOutgoing);
		
//			aWar.AddItem("Hits",me.ShowHitHistory);
		
//			//if(me.outgoing.Any())

//			//	if (Raid.test)
//			aWar.AddItem("Send Defence",() => City.GetBuild().SendDefence(WorldC.FromCid(cid)));
//			//	aWar.AddItem("Recruit Sen",             (_, _) => Recruit.Send(cid, ttSenator, 1, true));
//			aWar.AddItem("Show Reinforcements",() => ReinforcementsTab.ShowReinforcements(cid,null));
//		//	aWar.AddItem("Show All Reinforcements",(_,_) => ReinforcementsTab.ShowReinforcements(0,null));
//			aWar.AddItem("Return outgoing reinforcements",async () => {
//				foreach(var army in me.outgoing) {
//					if(army.isDefense && !army.isReturn) {
//						if(army.departed) {
//							await SendTroops.ShowInstance(prior: army);
//						}
//						else {
//							CnVEventReturnTroops.TryReturn(army);
//						}
//					}
//				}
//			});
//			aWar.AddItem("Return incoming reinforcements",async () => {
//				foreach(var army in me.incoming) {
//					Assert(!army.isReturn);
//					if(army.isDefense && !army.isReturn) {
						
//						if(army.departed) {
//							await SendTroops.ShowInstance(prior: army);
//						}
//						else {
//							CnVEventReturnTroops.TryReturn(army);
//						}
//					}
//				}
//			});


//			aExport.AddItem("Defense Sheet", me.ExportToDefenseSheet);
//			flyout.AddItem( "Send Res", () => SendResDialogue.ShowInstance(City.GetBuild(),me,null,null,palaceDonation:null));
//			AApp.AddItem(flyout, "Near Res", me.ShowNearRes);
//			if (me.isSubOrMine)
//			{
//				aWar.AddItem("Dismiss..",() => DismissDialog.ShowInstance(me));
//				flyout.AddItem( "Do the stuff",  () => me.DoTheStuff());
//			//	AApp.AddItem(flyout, "Food Warnings", (_, _) => CitySettings.SetFoodWarnings(cid));
//				flyout.AddToggle("Ministers", me.ministersOn, me.SetMinistersOn);
//			}
//		}
//		else if (me.isDungeonOrBoss)
//		{
//			flyout.AddItem( "Raid", () => City.GetBuild().Raid(WorldC.FromCid(cid) ));

//		}
//		if (me.isEmpty || (me.isCityOrCastle && !me.isCastle && me.isLawless) )
//		{
//			flyout.AddItem( "Settle", ()=> City.GetBuild().Settle(WorldC.FromCid(cid) ) );
//		//	AApp.AddItem(flyout, "Claim", (_,_)=> CityUI.DiscordClaim(WorldC.FromCid(cid) ) );

//		}
//		if(me.isBlessed && me.isAlliedWithPlayer) {
//			flyout.AddItem( "Palace Wood Artifact..", ()=> ArtifactDialogue.ShowInstance(Artifact.GetForPlayerRank(Artifact.ArtifactType.Arch),false,me.c) );;
//			flyout.AddItem( "Palace Stone Artifact..", ()=> ArtifactDialogue.ShowInstance(Artifact.GetForPlayerRank(Artifact.ArtifactType.Pillar),false,me.c) );;

//		}
		
////		aMisc.AddItem("Notify on Decay", ()=>DecayQuery(cid));
//		//if (Raid.test)
//		//{
//		//	aMisc.AddItem("Settle whenever water", (_, _) => Spot.TrySettle(City.build, cid, true));
//		//	aMisc.AddItem("Settle whenever land",  (_, _) => Spot.TrySettle(City.build, cid, false));
//		//}

//		aMisc.AddItem("Distance",       () => me.ShowDistanceTo());
//		aMisc.AddItem("Select",         () => me.ProcessClick(AppS.keyModifiers.ClickMods(false,false)|ClickModifiers.select));
//		aMisc.AddItem("Coords to Chat", () => CoordsToChat(cid));
//		if(AppS.isTest) {
//			aMisc.AddItem("Fixes",         () => FixDialog.ShowInstance(me));
			
//		}
////		flyout.RemoveEmpy();

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
									if(MainPage.IsVisible()) {
										MainPage.CityGrid.ScrollItemIntoView(City.GetOrAdd(cid),true);
									}
									if(BuildTab.IsVisible())
										BuildTab.CityGrid.ScrollItemIntoView(City.GetOrAdd(cid),true);
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
			AppS.QueueOnUIThread(() => {
				foreach(var m in Settings.pinned) {
					var c = m.AsCity();
					c.SetPinned(true);
					SpotTab.AddToGrid(c, false);
				}
			});
		}
	}
	public static void ShowContextMenu(this City me, Windows.Foundation.Point position)
	{
		if(!me.isValid) {
			Assert(false);
			return;
		}
		//   SelectMe(false) ;
		AppS.DispatchOnUIThread(() => {
			var flyout = ShellPage.instance.cityFlyoutFlyout;// new Flyout() {  AreOpenCloseAnimationsEnabled=false,ShouldConstrainToRootBounds=false,ShowMode=Microsoft.UI.Xaml.Controls.Primitives.FlyoutShowMode.Auto };
			var nav = ShellPage.instance.cityFlyout;
			//		nav.Reset(me);
			nav.SetCity(me);

			//foreach(var cat in City.ACatCity.children) {
			//	NavigationViewItem i = new();
			//	i.Icon = ImageHelper.GetIcon(cat.icon);
			//	i.Content = cat.label;
			//	if(cat.description is not null)
			//		i.SetToolTip(cat.description);
			//	nav.MenuItems.Add(i);

			//}

			//	flyout.SetXamlRoot(uie);
			//	AddToFlyout(me,(flyout,nav), uie is UserTab );
			//		nav.IsPaneOpen=false;
			//		nav.SelectedItem = nav.catCity;
			nav.UpdateCommands();

			//   flyout.XamlRoot = uie.XamlRoot;
			flyout.ShowAt(null,new() { Position= position });
		});
	}
	public static void ShowContextMenu(this City me) {
		ShowContextMenu(me,ShellPage.mousePosition.AsPoint());
	}


		//public static void SelectMe(this City me, ClickModifiers clickMods)
		//{
		//	if(!me.isValid) {
		//		Assert(false);
		//		return;
		//	}
		//	Assert(clickMods.HasFlag(ClickModifiers.select));
		//	var cid = me.cid;
		//	if(showClick || scrollIntoView)
		//		NavStack.Push(cid);
		//	SpotTab.AddToGrid(me, mod, scrollIntoView);

		//	CityUI.ProcessSelection(me, clickMods);

		//	if(showClick)
		//	{
		//		CityUI.ShowCity(cid, true,select:false);
		//	}
		//}
		//public static void SyncSelectionToUI(bool scrollIntoView, Spot focusSpot = null)
		//{
		//	++SpotTab.silenceSelectionChanges;
		//	try
		//	{
		//		SpotTab.SyncSelectionToUI();
		//		foreach(var tab in UserTab.userTabs)
		//		{
		//			if(!tab.isFocused)
		//				continue;
		//			foreach(var grid in tab.dataGrids)
		//			{

		//				//			var grid = gridX.Key;

		//				//		if(!gridX.Value?.isFocused == true)
		//				//		continue;

		//				if(grid.IsCityGrid())
		//				{
		//					var uiInSync = false;
		//					var sel1 = grid.SelectedItems;
		//					if(selected.Count == sel1.Count)
		//					{
		//						uiInSync = true;
		//						foreach(var i in sel1)
		//						{
		//							if(!selected.Contains((i as City).cid))
		//							{
		//								uiInSync = false;
		//								break;
		//							}
		//						}
		//					}

		//					if(!uiInSync)
		//					{
		//						selected.SyncList(sel1,(cid,spot) => cid == ((Spot)spot).cid,
		//							(cid) => City.Get(cid));
		//					}

		//					//if((scrollIntoView) && (sel1.Any() || focusSpot != null))
		//					//{
		//					//	var current = focusSpot ?? (City.GetBuild().isSelected ? City.GetBuild() : null);
		//					//	if(current != null)
		//					//	{
		//					//		grid.CurrentItem = current;
		//					//	}

		//					//	var any = current ?? sel1.First();
		//					//	{
		//					//		var rowIndex = grid.ResolveToRowIndex(any);
		//					//		var columnIndex = grid.ResolveToStartColumnIndex();
		//					//		if(rowIndex >= 0)
		//					//			grid.ScrollInView(new RowColumnIndex(rowIndex,columnIndex));
		//					//	}
		//					//}

		//					//if(AttackTab.IsVisible() && focusSpot != null)
		//					//{
		//					//	try
		//					//	{
		//					//		if(AttackTab.attacks.Contains(focusSpot.cid)
		//					//			 && !AttackTab.instance.attackGrid.SelectedItems.Contains(focusSpot))
		//					//		{
		//					//			AttackTab.instance.attackGrid.SelectedItem = focusSpot as City;
		//					//			AttackTab.instance.attackGrid.ScrollIntoView(focusSpot,null);
		//					//		}

		//					//		if(AttackTab.targets.Contains(focusSpot.cid)
		//					//			&& !AttackTab.instance.targetGrid.SelectedItems.Contains(focusSpot))
		//					//		{
		//					//			AttackTab.instance.targetGrid.SelectedItem = focusSpot as City;
		//					//			AttackTab.instance.targetGrid.ScrollIntoView(focusSpot,null);
		//					//		}
		//					//	}
		//					//	catch
		//					//	{
		//					//	}
		//					//}
		//				}
		//			}
		//		}
		//	}
		//	catch(Exception ex)
		//	{
		//		LogEx(ex);
		//	}
		//	finally
		//	{
		//		--SpotTab.silenceSelectionChanges;
		//	}

		//}

		public static bool OnKeyDown(object _spot, VirtualKey key)
	{
		var spot = _spot as Spot;
		switch(key)
		{
			case VirtualKey.Enter:
				spot.SetFocus(AppS.keyModifiers.ClickMods());
				return true;
				break;
			case VirtualKey.Space:
			{
				if(spot.isSubOrMine)
					spot.ShowDungeons();
				else
					spot.SetFocus(AppS.keyModifiers.ClickMods());
				return true;
			}

			default:
				break;
		}
		return false;
	}

	
	public static async void SetupClick(int _intialCid, SetupFlags flags)
	{
		//var cids =Spot.GetSelectedForContextMenu(_intialCid);
		//foreach(var cid in cids)
		//{
		//	var _cid = cid;
			await ShareString.Show(_intialCid,flags);
		//	{
		//		break;
		//	}
		//}
	}
	
	public static async void DefendMe(this Spot me)
	{
		//var cids = GetSelectedForContextMenu(me.cid, false);

		

		
		await UserTab.ShowOrAdd<NearDefenseTab>(true);
		NearDefenseTab.instance.SetDefendant(me );// cids.Select(a => City.Get(a)), true);
		NearDefenseTab.instance.refresh.Go();
	}


	public static async void ShowNearRes(this City me)
	{
		await UserTab.ShowOrAdd<NearRes>(true);

		var tab = NearRes.instance;
		tab.target = me;
		tab.refresh.Go();
	}
	public static async void ShowIncoming(this City me) {
		Assert(me.isAlliedWithPlayer);
		


			await IncomingTab.ShowOrAdd<IncomingTab>();

			AppS.DispatchOnUIThreadIdle(() => {
				IncomingTab.instance.defenderGrid.SetFocus(me);
			});

		}
	public static async void ShowOutgoing(this City me) {
			await OutgoingTab.ShowOrAdd<OutgoingTab>();
			AppS.DispatchOnUIThreadIdle(() =>
										{
											OutgoingTab.instance.attackerGrid.SetFocus(me);
										});
	}
	public static void ShowHitHistory(this City me) {
		
			AppS.DispatchOnUIThread(() => HitHistoryTab.SetFilter(me) );
	
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
	

	public void SetFocus(ClickModifiers clickMods)
	{
		SetFocus(cid, clickMods);
	}
	public void SetFocus()
	{
		SetFocus(cid);
	}

	public static async Task ProcessCoordClick(int cid, ClickModifiers clickMods)
	{
		ProcessClick(cid,clickMods);

		//var c = (WorldC)(cid);
		////mod.UpdateKeyModifiers();

		////if(mod.IsShiftAndControl() && AttackTab.IsVisible() && City.Get(cid).isCastle)
		////{
		////	var city = City.Get(cid);
		////	{
		////		using var __lock = await AttackTab.instance.TouchLists();
		////		var prior = AttackPlan.Get(cid);
		////		var isAttack = prior != null ? prior.isAttack : city.IsAllyOrNap();
		////		if (isAttack)
		////		{
		////			AttackPlan.AddOrUpdate( new(city, isAttack,city.attackType switch
		////			{ AttackType.assault => AttackType.senator, AttackType.senator => AttackType.se, AttackType.se => AttackType.none, _ => AttackType.assault }));
		////		}
		////		else
		////		{
		////			AttackPlan.AddOrUpdate( new(city, isAttack, city.attackType switch
		////			{
		////				AttackType.seFake => AttackType.se,
		////				AttackType.se => AttackType.senatorFake,
		////				AttackType.senatorFake => AttackType.senator,
		////				AttackType.senator => AttackType.none,
		////				_ => AttackType.seFake
		////			}));
		////		}
		////	}
		////	Note.Show($"{city.nameAndRemarks} set to {city.attackType}", Debug.Priority.high);
		////	AttackTab.WritebackAttacks();
		////	AttackTab.WaitAndSaveAttacks();
		////}
		////else
		////if(mod.IsShift())
		////	clickMods |= ClickModifiers.select;
		////clickMods |= mod.ClickMods(); // combine
		//if(clickMods.HasFlag(ClickModifiers.shiftPressed)|clickMods.HasFlag(ClickModifiers.controlPresed)) {
		//	CityUI.ProcessClick(cid,clickMods);
		//}
		//else {

		//	if(!c.IsOnScreen()) {
		//		CityUI.ProcessClick(cid,clickMods); // bring it on screen
		//	}
		//	else {

		//		cid.AsCity().ShowContextMenu();
		//	}

		//}


		////if( ((City.CanVisit(cid) && cid==focus) || Player.active.IsMyCity(cid) ) && !(clickMods.HasFlag(ClickModifiers.shiftPressed)|clickMods.HasFlag(ClickModifiers.controlPresed)) )
		////{
		////	if(City.IsBuild(cid))
		////	{
		////		if( cid==focus)
		////			View.SetViewMode(View.viewMode.GetNext());// toggle between city/region view
		////	//	if(scrollIntoUI)
		////		{
		////			CityUI.ProcessClick(cid, clickMods);
		////		}
		////	//	else
		////	//	{
		////	//		cid.BringCidIntoWorldView(lazyMove);
		////	//	}
		////	}
		////	else
		////	{
		////		if( Player.IsSubOrMe(cid.CidToPid() ) || (await AppS.DoYesNoBox("Sub", $"Switch to sub {cid.AsCity().player.name}?" )==1) ) {
		////			await CnVClient.CitySwitch(cid, clickMods); // keep current view, switch to city
		////																	   //	View.SetViewMode(ShellPage.viewMode.GetNextUnowned());// toggle between city/region view
		////		}
		////		else {
		////			CityUI.ProcessClick(cid,clickMods);
		////		}
		////	}
			

		////}
		////else
		////{
		////	CityUI.ProcessClick(cid,clickMods);
		////}
		////Spot.GetOrAdd(cid).SelectMe(false,mod);
		


	}

	internal void Settle(WorldC worldC)
	{
		SendTroops.ShowInstance(this,City.Get(worldC), isSettle:true,type:ArmyType.defense );
	}
	internal void Raid(WorldC worldC)
	{
		SendTroops.ShowInstance(this,City.Get(worldC),isSettle: false, type:ArmyType.raid) ;
	}
	internal void SendDefence(WorldC worldC)
	{
		SendTroops.ShowInstance(this,City.Get(worldC),isSettle: false, type:ArmyType.defense) ;
	}
}

public partial class City
{

	public int constructionSpeed => stats.cs;
	public void CityRowClick(GridCellTappedEventArgs e)
	{
		var modifiers  = AppS.keyModifiers;
		//var wantSelect = true;
		switch(e.Column.MappingName)
		{

		
		
			case nameof(bStage):
				DoTheStuff();
				break;
		
			case nameof(City.autobuild):
				autobuild  = !autobuild;

				return;
			case nameof(City.AutoWalls):
				AutoWalls  = !autoWalls;

				return;
			case nameof(City.AutoTowers):
				AutoTowers = !autoTowers;
				return;
			case nameof(City.ministersOn):
				SetMinistersOn(!ministersOn);
				return;
			case nameof(City.raidCarry):
				if(City.CanVisit(cid))
				{
					Raiding.Return(cid, true,fast:false);
				}
				break;
			case nameof(cityName):
			case nameof(icon):
			case nameof(nameAndRemarks):
				// first click selects
				// second acts as coord click
//				if(IsSelected(cid))
//				{
					ProcessCoordClick(cid, modifiers.ClickMods());
			//	} else {	}


				break;
			case nameof(xy):
				ProcessCoordClick(cid, modifiers.ClickMods());
			
				break;
			
			case nameof(City.dungeonsToggle):
			{
				ShowDungeons();
				break;
			}
			
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
			}
				return;

		}
		//if(wantSelect)
		//	SetFocus(false, true, true, false);
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


	public bool IsNearFocus() => c.IsNearFocus();
	public bool IsOnScreen() => c.IsOnScreen();	


	}