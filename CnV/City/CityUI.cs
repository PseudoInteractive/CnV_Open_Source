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
using static CnV.Spot;
using static CnV.CityUI;

public static partial class CityUI
{
	public static Action<City[]> cityListChanged;

	public static void SyncCityBox()
	{
		AppS.QueueOnUIThread(() =>
							{
								var _build = City.GetBuild();
								if(_build != ShellPage.instance.cityBox.SelectedItem)
								{
									ShellPage.instance.cityBox.SelectedItem = _build;
								}
							});
	}

	public static void Init()
	{
		World.worldFirstLoadedActions += CityUI.LoadFromPriorSession;
	}

	public static  void AddToFlyout(this City me, MenuFlyout flyout, bool useSelected = false)
	{
		var cid     = me.cid;
		var aMisc   = flyout.AddSubMenu("Misc..");
		var aExport = flyout.AddSubMenu("Import/Export..");
		var aSetup  = AApp.AddSubMenu(flyout, "Setup..");
		var aWar    = AApp.AddSubMenu(flyout, "War..");
		if (me.isCityOrCastle)
		{
			// Look - its my city!
			if (me.canVisit )
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
				var aRaid = AApp.AddSubMenu(flyout, "Raid..");
				aRaid.AddItem($"Raid ..", () => ScanDungeons.Post(cid, true, false));

				int count = 1;
				if (useSelected)
				{
					count = MainPage.GetContextCidCount(cid);
				}

				if (count > 1)
				{
					aRaid.AddItem($"End Raids x{count} selected", MainPage.ReturnSlowClick, cid);
					aRaid.AddItem($"Return At...x{count}",        ()=> ReturnAtBatch(cid) );

				}
				else
				{

					aRaid.AddItem("End Raids",    me.ReturnSlowClick);
					aRaid.AddItem("Return At...", me.ReturnAt);
				}


				aSetup.AddItem("Setup...",    (_, _) => CityUI.InfoClick(cid));
				aSetup.AddItem("Find Hub",    (_, _) => CityUI.SetClosestHub(cid));
				aSetup.AddItem("Set Recruit", (_, _) => CitySettings.SetRecruitFromTag(cid));
				aSetup.AddItem("Change...",   (_, _) => ShareString.Show(cid, default));
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
					CnVServer.FetchCity(cid);
				}

			}

			{
				var sel = Spot.GetSelectedForContextMenu(cid, false);
				{
					var multiString = sel.Count > 1 ? $" _x {sel.Count} selected" : "";
					aWar.AddItem("Cancel Attacks..", me.CancelAttacks);
					var afly = aWar.AddSubMenu("Attack Planner");
					if (!Alliance.IsAllyOrNap(me.allianceId))
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
				if (!Alliance.IsAllyOrNap(me.allianceId))
				{
					aWar.AddItem("Add funky Attack String", async (_, _) =>
															{
																using var work =
																		new WorkScope("Add to attack string..");

																foreach (var id in sel)
																{
																	await CnVServer.AddToAttackSender(id);
																}
															}
								);
				}
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


			aWar.AddItem("Attack",       (_, _) => Spot.JSAttack(cid));
			aWar.AddItem("Near Defence", me.DefendMe);
			if (me.incoming.Any())
				aWar.AddItem("Incoming", me.ShowIncoming);

			//	if (Raid.test)
			aWar.AddItem("Recruit Sen",             (_, _) => Recruit.Send(cid, ttSenator, 1, true));
			aWar.AddItem("Send Defence",            (_, _) => Spot.JSDefend(cid));
			aWar.AddItem("Show Reinforcements",     (_, _) => ReinforcementsTab.ShowReinforcements(cid, null));
			aWar.AddItem("Show All Reinforcements", (_, _) => ReinforcementsTab.ShowReinforcements(0,   null));
			aExport.AddItem("Defense Sheet", me.ExportToDefenseSheet);
			AApp.AddItem(flyout, "Send Res", (_, _) => Spot.JSSendRes(cid));
			AApp.AddItem(flyout, "Near Res", me.ShowNearRes);
			if (me.canVisit)
			{
				AApp.AddItem(flyout, "Do the stuff",  (_, _) => me.DoTheStuff());
				AApp.AddItem(flyout, "Food Warnings", (_, _) => CitySettings.SetFoodWarnings(cid));
				flyout.AddItem("Ministers", me.ministersOn.IsTrueOrNull, (me as City).SetMinistersOn);
			}
		}
		else if (me.isDungeon || me.isBoss)
		{
			AApp.AddItem(flyout, "Raid", (_, _) => Spot.JSRaid(cid));

		}
		else if (me.isEmpty && DGame.isValidForIncomingNotes)
		{
			AApp.AddItem(flyout, "Claim", (_,_)=> CityUI.DiscordClaim(cid) );

		}

//		aMisc.AddItem("Notify on Decay", ()=>DecayQuery(cid));
		if (Raid.test)
		{
			aMisc.AddItem("Settle whenever water", (_, _) => Spot.TrySettle(City.build, cid, true));
			aMisc.AddItem("Settle whenever land",  (_, _) => Spot.TrySettle(City.build, cid, false));
		}

		aMisc.AddItem("Distance",       (_, _) => me.ShowDistanceTo());
		aMisc.AddItem("Select",         (_, _) => me.SelectMe(true, AppS.keyModifiers));
		aMisc.AddItem("Coords to Chat", () => CoordsToChat(cid));
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

		public static void DecayQuery(SpotId cid)
	{
		CnVServer.gStCB(cid, DecayQueryCB, AMath.random.Next());
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
		flyout.CopyXamlRootFrom(uie);

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
			foreach(var gridX in UserTab.dataGrids)
			{

				var grid = gridX.Key;

				if(!gridX.Value?.isFocused == true)
					continue;

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
						selected.SyncList(sel1, (cid, spot) => cid == ((Spot)spot).cid,
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
								grid.ScrollInView(new RowColumnIndex(rowIndex, columnIndex));
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
								AttackTab.instance.attackGrid.ScrollIntoView(focusSpot, null);
							}

							if(AttackTab.targets.Contains(focusSpot.cid)
								&& !AttackTab.instance.targetGrid.SelectedItems.Contains(focusSpot))
							{
								AttackTab.instance.targetGrid.SelectedItem = focusSpot as City;
								AttackTab.instance.targetGrid.ScrollIntoView(focusSpot, null);
							}
						}
						catch
						{
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
				if(spot.canVisit)
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

	
	public static async void InfoClick(int _intialCid)
	{
		var cids = MainPage.GetContextCids(_intialCid);
		foreach(var cid in cids)
		{
			var _cid = cid;
			await ShareString.Show(_cid);
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
	public static async void ShowIncoming(this City me)
	{
		// Todo:  use IsAlly?
		if(Alliance.IsAlly(me.allianceId) )
		{
			var tab = IncomingTab.instance;
			AppS.DispatchOnUIThread(() => tab.Show());
			for(; ; )
			{
				await Task.Delay(1000);
				if(tab.defenderGrid.ItemsSource != null)
					break;
			}
			AppS.DispatchOnUIThreadIdle(() =>
										{
											tab.defenderGrid.SelectedItem = (me);
											tab.defenderGrid.ScrollItemIntoView(me);
										});

		}
		else
		{
			var tab = OutgoingTab.instance;
			AppS.DispatchOnUIThread(() => tab.Show());
			for(; ; )
			{
				await Task.Delay(1000);
				if(tab.attackerGrid.ItemsSource != null)
					break;
			}
			AppS.DispatchOnUIThreadIdle(() =>
										{
											tab.attackerGrid.SelectedItem = (me);
											tab.attackerGrid.ScrollItemIntoView(me);
										});
		}
	}



	public static async void DiscordClaim(SpotId cid)
	{
		if(!DGame.isValidForIncomingNotes)
		{
			Log("Invalid");
			return;
		}
		try
		{
			Note.Show($"Registering claim on {cid.CidToCoords()}");
			var client = CnVServer.genericClient;


			var message = new DGame.Message() { username = "Cord Claim", content = $"{cid.CidToCoords()} claimed by {Player.myName}", avatar_url = "" };

			//var content =  JsonContent.Create(message);
			//, JSON.jsonSerializerOptions), Encoding.UTF8,
			//	   "application/json");

			var result = await client.PostAsJsonAsync(DGame.discordHook, message);
			result.EnsureSuccessStatusCode();
		}
		catch(Exception ex)
		{
			LogEx(ex);
		}


	}

}

public partial class Spot
{
	public void SetFocus(bool scrollIntoView, bool select = true, bool bringIntoWorldView = true, bool lazyMove = true)
	{
		SetFocus(cid, scrollIntoView, select, bringIntoWorldView, lazyMove);
	}

	public static async void ProcessCoordClick(int cid, bool lazyMove, VirtualKeyModifiers mod, bool scrollIntoUI = false)
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
		if(City.CanVisit(cid) && !mod.IsShiftOrControl())
		{
			if(City.IsBuild(cid))
			{
				View.SetViewMode(View.viewMode.GetNext());// toggle between city/region view
				if(scrollIntoUI)
				{
					Spot.SetFocus(cid, scrollIntoUI, true, true, lazyMove);
				}
				else
				{
					cid.BringCidIntoWorldView(lazyMove);
				}
			}
			else
			{
				await CnVServer.CitySwitch(cid, lazyMove, false, scrollIntoUI); // keep current view, switch to city
																			   //	View.SetViewMode(ShellPage.viewMode.GetNextUnowned());// toggle between city/region view
			}
			NavStack.Push(cid);

		}
		else
		{
			CityUI.ShowCity(cid, lazyMove, false, scrollIntoUI);
			NavStack.Push(cid);
		}
		//Spot.GetOrAdd(cid).SelectMe(false,mod);
		SpotTab.TouchSpot(cid, mod, true);


	}
}

public partial class City
{
	public async Task<bool> DoClick()
	{
		var cid = this.cid;
		if(City.CanVisit(cid))
		{
			var wasBuild = City.IsBuild(cid);

			if(!await CnVServer.CitySwitch(cid, false, true, false))
				return false;

			if(wasBuild)
			{
				View.SetViewMode(View.viewMode.GetNext());
			}
		}
		else
		{
			CityUI.ShowCity(cid, false, true, false);
		}

		return true;
	}

	public void CityRowClick(GridCellTappedEventArgs e)
	{
		var modifiers  = AppS.keyModifiers;
		var wantSelect = true;
		switch(e.Column.MappingName)
		{

			case nameof(cityName):
			case nameof(iconUri):
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
			case nameof(City.AutoWalls):
				AutoWalls  = !autoWalls;
				wantSelect = false;

				return;
			case nameof(City.AutoTowers):
				AutoTowers = !autoTowers;
				wantSelect = false;
				return;
			case nameof(City.raidCarry):
				if(City.CanVisit(cid))
				{
					Raiding.ReturnSlow(cid, true);
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
			case nameof(icon):
				DoClick();
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
					Raiding.ReturnFast(cid, true);
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

}