using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CnV;

using Microsoft.UI.Xaml.Controls;
using Services;

public static partial class CityUI
{
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
			if (me.isFriend)
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
			aWar.AddItem("Send Defence",            (_, _) => JSDefend(cid));
			aWar.AddItem("Show Reinforcements",     (_, _) => ReinforcementsTab.ShowReinforcements(cid, null));
			aWar.AddItem("Show All Reinforcements", (_, _) => ReinforcementsTab.ShowReinforcements(0,   null));
			aExport.AddItem("Defense Sheet", ExportToDefenseSheet);
			AApp.AddItem(flyout, "Send Res", (_, _) => Spot.JSSendRes(cid));
			AApp.AddItem(flyout, "Near Res", ShowNearRes);
			if (isFriend)
			{
				AApp.AddItem(flyout, "Do the stuff",  (_, _) => DoTheStuff());
				AApp.AddItem(flyout, "Food Warnings", (_, _) => CitySettings.SetFoodWarnings(cid));
				flyout.AddItem("Ministers", me.ministersOn.IsTrueOrNull, (me as City).SetMinistersOn);
			}
		}
		else if (this.isDungeon || this.isBoss)
		{
			AApp.AddItem(flyout, "Raid", (_, _) => Spot.JSRaid(cid));

		}
		else if (this.isEmpty && DGame.isValidForIncomingNotes)
		{
			AApp.AddItem(flyout, "Claim", this.DiscordClaim);

		}

		aMisc.AddItem("Notify on Decay", DecayQuery);
		if (Raid.test)
		{
			aMisc.AddItem("Settle whenever water", (_, _) => TrySettle(City.build, cid, true));
			aMisc.AddItem("Settle whenever land",  (_, _) => TrySettle(City.build, cid, false));
		}

		aMisc.AddItem("Distance",       (_, _) => ShowDistanceTo());
		aMisc.AddItem("Select",         (_, _) => SelectMe(true, App.keyModifiers));
		aMisc.AddItem("Coords to Chat", () => CoordsToChat(cid));
		flyout.RemoveEmpy();
	}

	public static void ShowContextMenu(this City me,UIElement uie, Windows.Foundation.Point position)
	{

		//   SelectMe(false) ;
		var me     = this as City;
		var flyout = new MenuFlyout();
		AddToFlyout(flyout, uie == MainPage.CityGrid || uie == BuildTab.CityGrid);
		flyout.CopyXamlRootFrom(uie);

		//   flyout.XamlRoot = uie.XamlRoot;
		flyout.ShowAt(uie, position);
	}
}