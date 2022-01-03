global using SpotId = System.Int32;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV;

using System.Text.Json;
using static CnV.Game.Troops;
using DiscordCnV;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Services;

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
					aRaid.AddItem($"Return At...x{count}",        me.ReturnAtBatch);

				}
				else
				{

					aRaid.AddItem("End Raids",    me.ReturnSlowClick);
					aRaid.AddItem("Return At...", me.ReturnAt);
				}


				aSetup.AddItem("Setup...",    (_, _) => Spot.InfoClick(cid));
				aSetup.AddItem("Find Hub",    (_, _) => CitySettings.SetClosestHub(cid));
				aSetup.AddItem("Set Recruit", (_, _) => CitySettings.SetRecruitFromTag(cid));
				aSetup.AddItem("Change...",   (_, _) => ShareString.Show(cid, default));
				aSetup.AddItem("Move Stuff",  (_, _) => me.MoveStuffLocked());
				//aSetup.AddItem("Remove Castle", (_, _) => 
				//{
				//	CityBuild.

				//}
				//   AApp.AddItem(flyout, "Clear Res", (_, _) => JSClient.ClearCenterRes(cid) );
				aSetup.AddItem("Clear Res", me.ClearRes);


				aExport.AddItem("Troops to Sheets", me.CopyForSheets);
			}
			else
			{
				if ( me.cityName == null)
				{
					JSClient.FetchCity(cid);
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
																	await JSClient.AddToAttackSender(id);
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
				aSetup.AddItem("Set target hub", (_, _) => CitySettings.SetTargetHub(City.build, cid));
				aSetup.AddItem("Set source hub", (_, _) => CitySettings.SetSourceHub(City.build, cid));
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
			if (me.CanVisit)
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
			AApp.AddItem(flyout, "Claim", me.DiscordClaim);

		}

		aMisc.AddItem("Notify on Decay", ()=>DecayQuery(cid);
		if (Raid.test)
		{
			aMisc.AddItem("Settle whenever water", (_, _) => Spot.TrySettle(City.build, cid, true));
			aMisc.AddItem("Settle whenever land",  (_, _) => Spot.TrySettle(City.build, cid, false));
		}

		aMisc.AddItem("Distance",       (_, _) => me.ShowDistanceTo());
		aMisc.AddItem("Select",         (_, _) => me.SelectMe(true, App.keyModifiers));
		aMisc.AddItem("Coords to Chat", () => Spot.CoordsToChat(cid));
		flyout.RemoveEmpy();
	}
	public static void DecayQuery(SpotId cid)
	{
		JSClient.gStCB(cid, DecayQueryCB, AMath.random.Next());
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
		var type = jso.GetAsInt("type");
		var _cid = jso.GetAsInt("cid");
		Assert(cid == _cid);
		if(type != 3 && type != -1) // 4 is empty, 3 is city or ruins, -1 means not open (for a continent)
		{
			AppS.DispatchOnUIThreadLow(() =>
										{
											var dialog = new ContentDialog()
											{
													Title             = "Spot has Changed",
													Content           = cid.CidToString(),
													PrimaryButtonText = "Okay"
											};
											//SettingsPage.BoostVolume();
											ElementSoundPlayer.Play(ElementSoundKind.Invoke);
											ToastNotificationsService.instance.SpotChanged($"{cid.CidToString()} has changed");
											dialog.ShowAsync2();
										});
			City.ShowCity(cid, false);
		}
		else
		{
			//	Note.Show($"Query {cid.CidToStringMD()},type:{type}");
			await Task.Delay(60 * 1000);
			DecayQuery();
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
				await JSClient.CitySwitch(cid, lazyMove, false, scrollIntoUI); // keep current view, switch to city
																			   //	View.SetViewMode(ShellPage.viewMode.GetNextUnowned());// toggle between city/region view
			}
			NavStack.Push(cid);

		}
		else
		{
			City.ShowCity(cid, lazyMove, false, scrollIntoUI);
			NavStack.Push(cid);
		}
		//Spot.GetOrAdd(cid).SelectMe(false,mod);
		SpotTab.TouchSpot(cid, mod, true);


	}
}