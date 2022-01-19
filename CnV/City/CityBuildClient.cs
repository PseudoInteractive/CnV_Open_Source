﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static CnV.City;
using static CnV.CityBuild;
using static CnV.CityView;
using static CnV.ToolTips;
using static CnV.CityViewS;
//using static CnV.CityBuild;

namespace CnV
{
	public sealed partial class CityBuild :Microsoft.UI.Xaml.Controls.UserControl
	{
		public static void PreviewBuildAction()
		{
			if(hovered.IsNotNan())
			{
				if(XYToId(hovered) == lastQuickBuildActionBSpot)
					return;
				PerformAction(action, hovered, quickBuildId, true);
			}
		}

		public static void RevertToLastAction()
		{
			SetAction(priorQuickAction);
			priorQuickAction    = CityBuildAction.none;
			isSingleClickAction = false;
			ClearSelectedBuilding();

		}

		public static void ClearAction()
		{
			SetAction(CityBuildAction.none);
			isSingleClickAction = false;
			priorQuickAction    = CityBuildAction.none;
			ClearSelectedBuilding();
		}
		public static void SetAction(CityBuildAction _action)
		{
			Log($"{action}=>{_action}");
			action = _action;



			//switch(action)
			// {
			//  case Action.moveStart:
			//	  App.cursorMoveStart.Set();
			//	  break;
			//  case Action.moveEnd:
			//	  App.cursorMoveEnd.Set();
			//	  break;
			//  case Action.destroy:
			//	  App.cursorDestroy.Set();
			//	  break;
			//  case Action.build:
			//	  App.cursorQuickBuild.Set();
			//	  break;
			//  case Action.layout:
			//	  App.cursorLayout.Set();
			//	  break;
			//  default:
			//	  App.cursorDefault.Set();
			//	  break;
			// }

			//	AppS.DispatchOnUIThreadLow( ()=> instance.quickBuild.SelectedIndex = (int)_action ); /// the first 3 are mapped. this triggers a selected changed event
		}
		public static void SetQuickBuild(int quickBuildItemBid)
		{

			SetAction(CityBuildAction.build);

			lastBuildToolTipSpot      = -1;
			lastQuickBuildActionBSpot = -1;
			quickBuildId              = quickBuildItemBid;
			//	AppS.DispatchOnUIThreadLow( ()=> instance.quickBuild.SelectedIndex = (int)_action ); /// the first 3 are mapped. this triggers a selected changed event
		}
		public static async Task PerformAction(CityBuildAction action, (int x, int y) cc, int _quickBuildId, bool dryRun)
		{


			int bspot = XYToId(cc);
			var build = GetBuild();
			var b = City.GetBuild().GetBuildingOrLayout(bspot);

			if(action == CityBuildAction.moveEnd)
			{
				// We lost our move source
				if(selected.IsNan())
				{
					if(isSingleClickAction)
					{
						RevertToLastAction();
					}
					else
					{
						SetAction(CityBuildAction.moveStart);
					}
				}
			}

			switch(action)
			{
				case CityBuildAction.layout:
					{

						if(CityBuild.isPlanner)
						{
							Status("You are in layout mode, exit to use the layout tool", dryRun);
						}
						else if(!build.isLayoutCustom)
						{

							Status("Please assign a layout", dryRun);
						}
						else
						{

							await City.GetBuild().SmartBuild(cc, build.GetLayoutBid(bspot), dryRun: dryRun, searchForSpare: true, wantDemoUI: null);

						}
						break;
					}
				case CityBuildAction.build:
					{
						if(!b.isEmpty && !isPlanner)
						{
							//	if(dryRun)
							//	{
							Status($"Select {b.name}", dryRun);
							//	}
							//	else
							//	{
							// redirect to normal click
							//		ShowContectMenu(cc, false);
							//	}
							//	result = false;
							break;
						}
						else
						{
							//if (buildQueueFull)
							//{
							//	Status("Build Queue full", dryRun);
							//	break;
							//}

							var sel = _quickBuildId;

							if(sel != 0)
							{
								await City.GetBuild().SmartBuild(cc, sel, searchForSpare: false, dryRun: dryRun, wantDemoUI: null);

								break;
							}
							Status("Please select a valid building", dryRun);
						}
						break;
					}
				case CityBuildAction.destroy:
					{
						//if (buildQueueFull)
						//{
						//	Status("Build Queue full", dryRun);
						//	break;
						//}
						await City.GetBuild().Demolish(cc, dryRun);


						break;
					}
				case CityBuildAction.moveStart:
				case CityBuildAction.moveEnd:
					{
						MoveHovered(isSingleClickAction, (action == CityBuildAction.moveStart), dryRun);
						break;
					}
				case CityBuildAction.downgrade:
					{
						await City.GetBuild().Downgrade(cc, dryRun);
						break;
					}
				case CityBuildAction.upgrade:
					{
						City.GetBuild().UpgradeToLevel(1, cc, dryRun);
						break;
					}
				case CityBuildAction.flipLayoutH:
					{
						if(!dryRun)
						{
							var city = GetBuild();
							city.FlipLayoutH(true);


						}

						break;
					}
				//case Action.showShareString:
				//	{
				//		if (!dryRun)
				//		{
				//			if (isSingleClickAction)
				//			await ShareString.Show(City.build);
				//		}
				//		break;
				//	}
				//case Action.doTheStuff:
				//	{
				//		if (!dryRun)
				//		{
				//			if (isSingleClickAction)
				//			await City.GetBuild().DoTheStuff();
				//		}
				//		break;
				//	}
				//case Action.togglePlanner:
				//	{
				//		if (!dryRun)
				//		{

				//			AppS.DispatchOnUIThreadLow(()=>TogglePlanner() );
				//		}

				//		break;
				//	}
				case CityBuildAction.flipLayoutV:
					{
						if(!dryRun)
							GetBuild().FlipLayoutV(true);
						break;
					}
				case CityBuildAction.none:
					{
						if(b.isEmpty)
						{
							if(IsBuildingSpotOrTownHall(XYToId(hovered)))
							{
								Status($"Left click to build something\nRight click to select a quick build tool", dryRun);

							}
							else if(IsTowerSpot(hovered))
							{
								Status($"Left click to build tower\nRight click to select a quick build tool", dryRun);

							}
							else if(IsWallSpot(hovered))
							{
								Status($"Left click to build wall\nRight click to select a quick build tool", dryRun);
							}
							else
							{
								Status($"Please don't left click here\nRight click to select a quick build tool", dryRun);
							}
						}
						else
						{
							Status($"Left click modify {b.def.Bn}, Right click to select a quick build tool", dryRun);
						}

						break;
					}
			}
		}
		public static int quickBuildId;
		public static int lastQuickBuildActionBSpot = -1;
		public static int lastBuildToolTipSpot = -1;
		public enum CityBuildAction
		{
			none,
			moveStart,
			moveEnd,
			destroy,
			build,
			layout,
			pending,
			upgrade,
			downgrade,
			flipLayoutV,
			flipLayoutH,
			invalid,
			count = invalid,

		};
		public static CityBuildAction action;
		public static CityBuildAction priorQuickAction;    // set when you temporarily switch from quickbuild to select/move
		public static bool isSingleClickAction; // set on left click tool select
		

		public static async void MoveHovered(bool _isSingleAction, bool isStart, bool dryRun)
		{
			var build = GetBuild();

			Status($"Move slots left: {Player.moveSlots}", dryRun);
			if(hovered.IsNan())
			{
				Status("Please select something in the city", dryRun);
				return;
			}
			int targetSpot = XYToId(hovered);
			var targetB = isPlanner ? build.GetLayoutBuilding(targetSpot) : build.buildings[targetSpot];

			if(isStart)
			{
				if(targetB.isBuilding)
				{

					Status($"Move {targetB.def.Bn} at {hovered.bspotToString()} to ... ", dryRun);
					if(!dryRun)
					{
						CityView.SetSelectedBuilding(hovered, _isSingleAction);
						if(_isSingleAction)
						{
							CityBuild.PushSingleAction(CityBuildAction.moveEnd);
						}
						else
						{
							SetAction(CityBuildAction.moveEnd);
						}

					}
				}
				else
				{
					Status("Please select a building to move", dryRun);
				}

			}
			else
			{
				Assert(selected.IsNotNan());

				if(targetB.isRes)
				{
					Status("Please select an empty spot", dryRun);
				}
				else
				{
					var source = XYToId(selected);
					var sourceBid = build.postQueueBuildings[source].bid;
					// Is this a valid transition
					//var bs1 = GetSpotType(targetSpot);
					var sourceSpotType = build.GetSpotTypeFromBid(sourceBid);
					var validSpots = build.GetSpots(sourceSpotType);

					if(!validSpots.Contains((ushort)targetSpot))
					{
						Status("Doesn't go here", dryRun);
						return;
					}


					if(dryRun)
					{
						DrawSprite(hovered, decalMoveBuilding, 0.343f);
						DrawSprite(selected, decalMoveBuilding, 0.323f);
					}

					{

						if(!targetB.isEmpty)
						{
							if(IsTowerSpot(selected))
								Status("Cannot swap towers, please move them one at a time", dryRun);
							else
								await City.GetBuild().SwapBuilding(source, targetSpot, dryRun);
						}
						else
						{
							await City.GetBuild().MoveBuilding(source, targetSpot, dryRun);
						}
						if(!dryRun)
						{
							ClearSelectedBuilding();
							if(_isSingleAction)
							{
								RevertToLastAction();
							}
							else
							{
								SetAction(CityBuildAction.moveStart);
							}
						}
					}
				}
			}
		}

		public static void PushSingleAction(CityBuildAction _action)
		{
			priorQuickAction = action;
			SetAction(_action);
			isSingleClickAction = true;
		}
	}
}