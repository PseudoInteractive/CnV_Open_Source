using COTG.Draw;
using COTG.Game;
using COTG.Helpers;
using COTG.JSON;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using static COTG.Draw.CityView;
using static COTG.Game.City;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.Menu;
using System.Windows.Input;
using static COTG.Debug;
using System.Threading.Tasks;
using static COTG.Views.CityBuild;
using Action = COTG.Views.CityBuild.Action;
using COTG.Services;
using Microsoft.Toolkit.HighPerformance;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

using System.Text.RegularExpressions;
using Windows.UI.Core;
using static COTG.Game.City;
using ContentDialog = Windows.UI.Xaml.Controls.ContentDialog;
using ContentDialogResult = Windows.UI.Xaml.Controls.ContentDialogResult;
using Windows.System;

namespace COTG.Views
{
	public sealed partial class CityBuild : Windows.UI.Xaml.Controls.UserControl
	{
		public static bool testFlag;
		public static int quickBuildId;
		public static int lastQuickBuildActionBSpot=-1;
		public static CityBuild instance;
		public static bool isPlanner;

		static Dictionary<int, ImageBrush> brushFromAtlasCache = new();
		static Dictionary<string, ImageBrush> brushFromImageCache = new();
		static Dictionary<BitmapImage, ImageBrush> brushFromImageCache2 = new();

		public static HashSet<ushort> outerTowerSpots =new HashSet<ushort>(new ushort[] {3, 7, 13, 17, 83, 167, 293, 377, 437, 433, 427, 423, 357, 273, 147, 63} );
		public static HashSet<ushort> innerTowerSpots = new HashSet<ushort>(new ushort[] { 113, 117, 173, 257, 323, 327, 183, 267 });
		public static HashSet<ushort> wallSpots = new HashSet<ushort>(new ushort[] { 1, 2, 4, 5, 6, 8, 9, 10, 11, 12, 14, 15, 16, 18, 19, 20, 21, 22, 23, 31, 39, 40, 41, 42, 43, 52, 61, 62, 73, 84, 94, 104, 105, 112, 114, 115, 116, 118, 125, 126, 132, 133, 139, 140, 146, 152, 153, 161, 162, 168, 188, 189, 194, 204, 209, 210, 211, 212, 213, 214, 215, 225, 226, 227, 228, 229, 230, 231, 236, 246, 251, 252, 272, 278, 279, 287, 288, 294, 300, 301, 307, 308, 314, 315, 322, 324, 325, 326, 328, 335, 336, 346, 356, 367, 378, 379, 388, 397, 398, 399, 400, 401, 409, 417, 418, 419, 420, 421, 422, 424, 425, 426, 428, 429, 430, 431, 432, 434, 435, 436, 438, 439, 440 });

		public static HashSet<ushort> shoreSpots = new HashSet<ushort>(new ushort[] {416,394,372,351,331,332,376,354});
		public static HashSet<ushort> waterSpots = new HashSet<ushort>(new ushort[] { 352, 353, 373, 374, 375, 395,396,397,
																			417,418});
		public static HashSet<ushort> emptySpotList = new HashSet<ushort>();

		public static HashSet<ushort> buildingSpots = new HashSet<ushort>( Enumerable.Range((ushort)1, (ushort)citySpotCount-1).Select (a=> (ushort)a). 
			Where(a=>!(wallSpots.Contains(a)|innerTowerSpots.Contains(a)|outerTowerSpots.Contains(a)|(a==City.bspotTownHall))) );
		
		public enum SpotType
		{
			invalid,
			wall,
			outerTower,
			innerTower,
			shore,
			water,
			building,
			townHall,
		}
		
		public static SpotType GetSpotType(int a )
		{
			return a switch {
				_ when IsInnerTowerSpot(a) => SpotType.innerTower,
				_ when IsOuterTowerSpot(a) => SpotType.outerTower,
				_ when IsShoreSpot(a) => SpotType.shore,
				_ when IsWaterSpot(a) => SpotType.water,
				_ when IsBuildingSpot(a) => SpotType.building,
				_ when (a==bspotTownHall) => SpotType.townHall,
				_ when IsWallSpot(a) => SpotType.wall,

				_ => SpotType.invalid };
		}

		public static HashSet<ushort> GetSpots( SpotType type)
		{
			switch (type)
			{
				case SpotType.innerTower:
					return innerTowerSpots;
				case SpotType.outerTower:
					return outerTowerSpots;
				case SpotType.shore:  return shoreSpots;
				case SpotType.building: return buildingSpots;
				default:
					return testFlag ? buildingSpots : emptySpotList; // how should this be properly handled?
			}
		}

		public static bool IsBuildingSpot(int spot) => buildingSpots.Contains((ushort)spot);
		public static bool IsBuildingSpotOrTownHall(int spot) => buildingSpots.Contains((ushort)spot) | (spot == City.bspotTownHall);
		public static bool IsBuildingSpot((int x, int y) cc) => IsBuildingSpot(XYToId(cc));
		public static bool IsTowerSpot(int spot) => outerTowerSpots.Contains((ushort)spot) | innerTowerSpots.Contains((ushort)spot);
		public static bool IsInnerTowerSpot(int spot) => innerTowerSpots.Contains((ushort)spot);
		public static bool IsOuterTowerSpot(int spot) => outerTowerSpots.Contains((ushort)spot);
		public static bool IsTowerSpot((int x, int y) cc) => IsTowerSpot(XYToId(cc));
		public static bool IsWallSpot(int spot) => wallSpots.Contains((ushort)spot);
		public static bool IsWallSpot((int x, int y) cc) => wallSpots.Contains((ushort)XYToId(cc));
		public static bool IsWaterSpot(int spot) => City.GetBuild().isOnWater && waterSpots.Contains((ushort)spot);
		public static bool IsShoreSpot(int spot) => City.GetBuild().isOnWater && shoreSpots.Contains((ushort)spot);

		public static Regex shortKeyRegEx = new Regex(@"Shortkey: (.)", RegexOptions.CultureInvariant | RegexOptions.Compiled);
		
		public enum Action
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
			abandon,
			flipLayoutV,
			flipLayoutH,
			togglePlanner,
			showShareString,
			invalid,
			count = invalid,

		};
		public static Action action;
		public static Action priorQuickAction; // set when you temporarily switch from quickbuild to select/move
		public static bool isSingleClickAction; // set on left click tool select

		public static void RevertToLastAction()
		{
			SetAction(priorQuickAction);
			priorQuickAction = Action.none;
			isSingleClickAction = false;
			ClearSelectedBuilding();

		}

		public static void ClearAction()
		{
			SetAction(Action.none);
			isSingleClickAction = false;
			priorQuickAction = Action.none;
			ClearSelectedBuilding();
		}
		public static void SetAction(Action _action)
		{
			Log($"{action}=>{_action}");
			action = _action;

			ShellPage.coreInputSource.Dispatcher.RunAsync(CoreDispatcherPriority.Low, ()=>
		   {
			   switch (action)
			   {
				   case Action.moveStart:
					   App.cursorMoveStart.Set();
					   break;
				   case Action.moveEnd:
					   App.cursorMoveEnd.Set();
					   break;
				   case Action.destroy:
					   App.cursorDestroy.Set();
					   break;
				   case Action.build:
					   App.cursorQuickBuild.Set();
					   break;
				   case Action.layout:
					   App.cursorLayout.Set();
					   break;
				   default:
					   App.cursorDefault.Set();
					   break;
			   }
		   });
			//	App.DispatchOnUIThreadSneaky( ()=> instance.quickBuild.SelectedIndex = (int)_action ); /// the first 3 are mapped. this triggers a selected changed event
		}
		public static void SetQuickBuild(int quickBuildItemBid)
		{

			SetAction(Action.build);

			
			lastQuickBuildActionBSpot = -1;
			quickBuildId = quickBuildItemBid;
			//	App.DispatchOnUIThreadSneaky( ()=> instance.quickBuild.SelectedIndex = (int)_action ); /// the first 3 are mapped. this triggers a selected changed event
		}

		//static List<QuickBuildItem> items;
		internal static bool menuOpen;

		// build menu cache
		public const int buildMenuRootItems = 8;
		public const int maxMRUSize = 7;
		public const int qbMRUSize = 4;
		public const int defaultMRUSize = 7;
		const int maxActionCount = 6;
		const int emptyActionCount = 6;
		const int resActionCount = 1;
		public enum MenuType
		{
			quickBuild,
			buliding,
			townhall,
			townhallPlanner,
			empty,
			res,
			tower,
			wall,
			shore,
			invalid
		}
		public static MenuType menuType = MenuType.invalid;
		public class BuildingMRU
		{
			public int bid;
			public float cacheScore;

			public BuildingMRU(int bid)
			{
				this.bid = bid;
			}
		}
		public readonly struct ActionInfo
		{
			public readonly string name;
			public readonly CityBuild.Action action;
			public readonly string icon;
			public readonly string toolTip;

			public ActionInfo(string name, Action action, string icon, string toolTip)
			{
				this.name = name;
				this.action = action;
				this.icon = icon;
				this.toolTip = toolTip;
			}
		}
		public static BuildingMRU[] buildingMru = new BuildingMRU[maxMRUSize];

		static BuildMenuItem amSelect = new BuildMenuItem("Modify", Action.none, "City/decal_select_building.png", "Left click opens a menu");
		static BuildMenuItem amMove = new BuildMenuItem("Move", Action.moveStart, "City/decal_move_building.png", "In this mode you first click a building, then click empty space, then click the next buildin to move, etc.");
		static BuildMenuItem amDemo = new BuildMenuItem("Demo", Action.destroy, "City/decal_building_invalid.png", "Destroy anything you click");
		static BuildMenuItem amLayout = new BuildMenuItem("Layout", Action.layout, "City/decal_building_valid_multi.png", "Smart build based on city layouts");
		static BuildMenuItem amNone = new BuildMenuItem();
		static BuildMenuItem amUpgrade = new BuildMenuItem("Upgrade", Action.upgrade, "City/decal_building_valid.png", "Upgrade buildings");
		static BuildMenuItem amBuild = new BuildMenuItem("Build", Action.upgrade, "City/decal_building_valid.png", "Buidl this");
		static BuildMenuItem amDowngrade = new BuildMenuItem("Downgrade", Action.downgrade, "City/decal_building_invalid.png", "Downgrade buildings");
		static BuildMenuItem amAbandon = new BuildMenuItem("Abandon", Action.abandon, "City/decal_building_invalid.png", "Abandon this city");
		static BuildMenuItem amFlipLayoutH = new BuildMenuItem("Flip H", Action.flipLayoutH, "City/build_details_gloss_overlay.png", "Flip Layout Horizontally");
		static BuildMenuItem amFlipLayoutV = new BuildMenuItem("Flip V", Action.flipLayoutV, "City/build_details_gloss_overlay.png", "Flip Layout Vertically");
		static BuildMenuItem amSetPlanner = new BuildMenuItem("Planner", Action.togglePlanner, "City/build_details_gloss_overlay.png", "Set planner mode");
		static BuildMenuItem amSetBuild = new BuildMenuItem("Build", Action.togglePlanner, "City/build_details_gloss_overlay.png", "Set build mode");
		static BuildMenuItem amSelectShareString = new BuildMenuItem("ShareString", Action.showShareString, "City/build_details_gloss_overlay.png", "Sharestring selection and exporting");
		static BuildMenuItem amDoTheStuff = new BuildMenuItem("Planner", Action.togglePlanner, "City/build_details_gloss_overlay.png", "Set planner mode");



		static BuildMenuItem CreateBuildMenuItem(int bid)
		{
			var rv = new BuildMenuItem(bid); //  = 454;
			allBuildings.Add(rv);
			return rv;
		}
		public static List<BuildMenuItem> allBuildings = new();

		static BuildMenuItem bmForester = CreateBuildMenuItem(bidForester); //  = 448;
		static BuildMenuItem bmQuarry = CreateBuildMenuItem(bidQuarry); //  = 461;
		static BuildMenuItem bmMine = CreateBuildMenuItem(bidMine); //  = 465;
		static BuildMenuItem bmFarmhouse = CreateBuildMenuItem(bidFarmhouse); //  = 447;

		static BuildMenuItem bmSawmill = CreateBuildMenuItem(bidSawmill); //  = 460;
		static BuildMenuItem bmStonemason = CreateBuildMenuItem(bidStonemason); //  = 462;
		static BuildMenuItem bmSmelter = CreateBuildMenuItem(bidSmelter); //  = 477;
		static BuildMenuItem bmWindmill = CreateBuildMenuItem(bidWindmill); //  = 463;

		static BuildMenuItem bmBarracks = CreateBuildMenuItem(bidBarracks); //  = 445;
		static BuildMenuItem bmTrainingground = CreateBuildMenuItem(bidTrainingGround); //  = 483;
		static BuildMenuItem bmMage_tower = CreateBuildMenuItem(bidSorcTower); //  = 500;
		static BuildMenuItem bmStable = CreateBuildMenuItem(bidStable); //  = 466;

		static BuildMenuItem bmAcademy = CreateBuildMenuItem(bidAcademy); //  = 482;
		static BuildMenuItem bmBlacksmith = CreateBuildMenuItem(bidBlacksmith); //  = 502;
		static BuildMenuItem bmShipyard = CreateBuildMenuItem(bidShipyard); //  = 491;
		static BuildMenuItem bmCastle = CreateBuildMenuItem(bidCastle); //  = 467;


		static BuildMenuItem bmCottage = CreateBuildMenuItem(bidCottage); //  = 446;
		static BuildMenuItem bmStorehouse = CreateBuildMenuItem(bidStorehouse); //  = 464;
		static BuildMenuItem bmHideaway = CreateBuildMenuItem(bidHideaway); //  = 479;
		static BuildMenuItem bmCityguardhouse = CreateBuildMenuItem(bidCityguardhouse); //  = 504;
		static BuildMenuItem bmTownhouse = CreateBuildMenuItem(bidTownhouse); //  = 481;
		static BuildMenuItem bmTemple = CreateBuildMenuItem(bidTemple); //  = 890;
		static BuildMenuItem bmPort = CreateBuildMenuItem(bidPort); //  = 488;
		static BuildMenuItem bmMarketplace = CreateBuildMenuItem(bidMarketplace); //  = 449;



		static BuildMenuItem bmRangerPost = CreateBuildMenuItem(bidRangerPost); //  = 543;
		static BuildMenuItem bmTriariPost = CreateBuildMenuItem(bidTriariPost); //  = 539;
		static BuildMenuItem bmPriestessPost = CreateBuildMenuItem( bidPriestessPost); //  = 551;
		static BuildMenuItem bmBallistaPost = CreateBuildMenuItem( bidBallistaPost); //  = 555;

		static BuildMenuItem bmSnagBarricade = CreateBuildMenuItem(bidSnagBarricade); //  = 567;
		static BuildMenuItem bmEquineBarricade = CreateBuildMenuItem(bidEquineBarricade); //  = 559;
		static BuildMenuItem bmRuneBarricade = CreateBuildMenuItem( bidRuneBarricade); //  = 563;
		static BuildMenuItem bmVeiledBarricade = CreateBuildMenuItem( bidVeiledBarricade); //  = 571;

		static BuildMenuItem bmSentinelPost = CreateBuildMenuItem(bidSentinelPost); //  = 547;

		static BuildMenuItem bmTownHall = CreateBuildMenuItem( bidTownHall); //  = 455;
		static BuildMenuItem bmWall = CreateBuildMenuItem( bidWall); //  = 809;


		public static void UpdateBuildMenuType(MenuType _menuType,int bspot)
		{
			if (menuType == _menuType)
				return;
			var groups = new List<BuildMenuItemGroup>();
			var commands = new BuildMenuItemGroup() { title = "Action" };
			var items = new BuildMenuItemGroup() { title = _menuType==MenuType.quickBuild? "Quick Build" : "Build" };
			groups.Add(items);
			groups.Add(commands);
			//if ((menuType == MenuType.quickBuild) != (_menuType == MenuType.quickBuild))
			//{
			//	if (_menuType == MenuType.quickBuild)
			//	{
			//		buildMenu.ContentMenuBackgroundStyle = menuBackgroundQuick;
			//	}
			//	else
			//	{
			//		buildMenu.ContentMenuBackgroundStyle = menuBackground;

			//	}
			//}
			menuType = _menuType;

			var townHallLevel = postQueueBuildings[City.bspotTownHall].bl;


			switch (menuType)
			{
				case MenuType.quickBuild:
					commands.items.Add(amSelect);
					commands.items.Add(amDemo);
					commands.items.Add(amLayout);
					commands.items.Add(amMove);
					commands.items.Add(amUpgrade);
					commands.items.Add(amDowngrade);
					commands.items.Add(isPlanner ? amSetBuild: amSetPlanner);
					commands.items.Add(amSelectShareString);

					foreach (var i in allBuildings)
					{
						if (!App.IsKeyPressedShift())
						{
							if (i.bid == bidTownHall || i.bid== bidWall )
								continue;
						}
						items.items.Add(i);
					}

					break;
				case MenuType.buliding:

					commands.items.Add(amDemo);
					commands.items.Add(amDowngrade);
					commands.items.Add(amLayout);
					if(postQueueBuildings[bspot].bl == 0 )
						commands.items.Add(amBuild);
					else
						commands.items.Add(amUpgrade);
					commands.items.Add(amMove);
					commands.items.Add(isPlanner ? amSetBuild : amSetPlanner);


					break;
				case MenuType.townhall:
					commands.items.Add(amMove);
					commands.items.Add(amAbandon);
					commands.items.Add(amUpgrade);
					commands.items.Add(amDowngrade);
					commands.items.Add(amSelectShareString);
					commands.items.Add(isPlanner ? amSetBuild : amSetPlanner);

					break;
				case MenuType.townhallPlanner:
					commands.items.Add(amAbandon);
					commands.items.Add(amUpgrade);
					commands.items.Add(isPlanner ? amSetBuild : amSetPlanner);
					commands.items.Add(amSelectShareString);
					commands.items.Add(amFlipLayoutH);
					commands.items.Add(amFlipLayoutV);

					break;
				case MenuType.empty:

					commands.items.Add(amLayout);
					commands.items.Add(isPlanner ? amSetBuild : amSetPlanner);
					commands.items.Add(amSelectShareString);


					// restrict by level?
					foreach (var i in allBuildings)
					{
						var def = BuildingDef.all[i.bid];
						if(def.isTower || i.bid == bidPort || i.bid==bidShipyard)
						{
							continue;

						}
						if(!App.IsKeyPressedShift())
						{
							if (def.isTownHall || def.isWall)
								continue;
						}
						items.items.Add(i);
					}

					break;

				case MenuType.tower:
					items.items.Add(bmSentinelPost);
					items.items.Add(bmRangerPost);
					items.items.Add(bmTriariPost);
					items.items.Add(bmPriestessPost);
					items.items.Add(bmBallistaPost);
					items.items.Add(bmSnagBarricade);
					items.items.Add(bmEquineBarricade);
					items.items.Add(bmRuneBarricade);
					items.items.Add(bmVeiledBarricade);
					break;

				case MenuType.shore:
					items.items.Add(bmPort);
					items.items.Add(bmShipyard);
					break;

				case MenuType.res:
					commands.items.Add(amDemo);
					commands.items.Add(isPlanner ? amSetBuild : amSetPlanner);

					//	for (int i = 0; i < 7; ++i)
					//		Item(buildMenu, i + 1).SetBid(buildingMru[i].bid);


					break;

			}


			//foreach(var bi in items)
			//{
			//	if( bi.isBuilding)
			//	{
			//		var def = BuildingDef.all[bi.bid];
			//		var enabled = true;
			//		if(def.Thl > townHallLevel)
			//		{
			//			enabled = false;
			//		}
			//		else if(_menuType == MenuType.empty)
			//		{
			//			enabled = true; // todo

			//		}

			//	}


			//}
			instance.cvsGroups.Source = groups;
		}

		public static Windows.UI.Xaml.Controls.Flyout buildMenu;

		public static CityBuild Initialize()
		{
			instance = new CityBuild();
			buildMenu = new Windows.UI.Xaml.Controls.Flyout() { LightDismissOverlayMode= Windows.UI.Xaml.Controls.LightDismissOverlayMode.Auto,ShowMode=FlyoutShowMode.Standard,AreOpenCloseAnimationsEnabled=true,
				AllowFocusOnInteraction = true, Content = instance };

			buildMenu.Closed += BuildMenu_Closed;
			Style s = new Windows.UI.Xaml.Style { TargetType = typeof(Windows.UI.Xaml.Controls.FlyoutPresenter) };
			s.Setters.Add(new Setter(MinHeightProperty, "300"));
			s.Setters.Add(new Setter(MinWidthProperty, "300"));
			s.Setters.Add(new Setter(MaxWidthProperty, "600"));
			buildMenu.FlyoutPresenterStyle = s;
			//			{

			//	itemQB = new RadialMenuItem() { Header = "QuickBuild" };

			//	for (int i = 0; i < 8; ++i)
			//		buildMenu.Items.Add(new BuildMenuItem());
			//	buildMenu.Items.Add(itemQB);
			//	// Mru

			//	buildingMru[0] = new BuildingMRU(bidCottage);
			//	buildingMru[1] = new BuildingMRU(bidStorehouse);
			//	buildingMru[2] = new BuildingMRU(bidMarketplace);
			//	buildingMru[3] = new BuildingMRU(bidMage_tower);
			//	buildingMru[4] = new BuildingMRU(bidCastle);
			//	buildingMru[5] = new BuildingMRU(bidBarracks);
			//	buildingMru[6] = new BuildingMRU(bidTrainingground);
			//	// quick build
			//	itemQB.ChildItems.Add(new BuildMenuGroup("Misc", 446, 464, 449, 481, 467, 488, 479, bidTemple));
			//	itemQB.ChildItems.Add(new BuildMenuGroup("Military", 445, 500, 483, 466, 491, 482, 502, 504));
			//	itemQB.ChildItems.Add(new BuildMenuGroup("Posts", 547, 539, 543, 551, 555));
			//	itemQB.ChildItems.Add(new BuildMenuGroup("Barricade", 559, 563, 567, 571));
			//	itemQB.ChildItems.Add(new BuildMenuGroup("Res", 447, 448, 460, 461, 462, 463, 465, 477));


			//	buildMenu.isOpenChanged = async (open) =>
			//	{
			//		if (!open)
			//		{
			//			await Task.Delay(450);
			//			App.DispatchOnUIThreadSneaky( ()=>buildMenuCanvas.Visibility = Visibility.Collapsed );

			//			menuOpen = false;
			//		}
			//		else
			//		{
			//			buildMenuCanvas.Visibility = Visibility.Visible;
			//			menuOpen = true;
			//		}
			//	};
			//}
			//// individual menu
			//{

			//}
			//menuBackground = buildMenu.ContentMenuBackgroundStyle;
			//menuBackgroundQuick = buildMenu.ContentMenuBackgroundQuickStyle;
			return instance;

		}

		private static void BuildMenu_Closed(object sender, object e)
		{
			if (!contextMenuResultSelected)
			{
				RevertToLastAction();  // player aborted

			}
		}

		public CityBuild()
		{
			instance = this;
			this.InitializeComponent();
			//	items = new List<QuickBuildItem>();
			//	ushort[] buildingIds = {
			//	448, //Forester's Hut"),
			//	446, //Cabin"),
			//	464, //Storehouse"),
			//	461, //Stone Mine"),
			//	547, //Sentinel Post"),
			//	479, //Hideaway"),
			//	447, //Farm Estate"),
			//	504, //Guardhouse"),
			//	543, //Ranger Post"),
			//	445, //Barracks"),
			//	465, //Iron Mine"),
			//	483, //Training Arena"),
			//	449, //Forum"),
			//	481, //Villa"),
			//	567, //Snag Barricade"),
			//	460, //Sawmill"),
			//	466, //Stable"),
			//	539, //Triari Post"),
			//	462, //Mason's Hut"),
			//	500, //Sorcerer's Tower"),
			//	559, //Equine Barricade"),
			//	463,482,467,551,563,890,477,502,555,571,490,498
			//};
			//	// Add commands as "Pseudo" buildings for selection
			//	items.Add(new QuickBuildItem(Action.none, "Select", "City/decal_select_building.png"));
			//	items.Add(new QuickBuildItem(Action.move, "Move", "City/decal_move_building.png"));
			//	items.Add(new QuickBuildItem(Action.destroy, "Demo", "City/decal_building_invalid.png"));

			//	foreach (var i in buildingIds)
			//	{
			//		items.Add(new QuickBuildItem(i));
			//	}
			

			//	quickBuild.ItemsSource = items;
		}
		/*
		 * 			<AppBarToggleButton x:Name="planner" Checked="PlannerChecked" Unchecked="PlannerUnchecked"  x:FieldModifier="public" Icon="Orientation" Label="Planner"
                           ToolTipService.ToolTip="Toggles between normal city building and planner mode (i.e. like LOUOpt.com" />
		 */
		public static async Task _IsPlanner(bool value, bool showPlannerIfNeeded=false)
		{
				if (isPlanner == value)
					return;
				menuType = MenuType.invalid; //clear this
				isPlanner = value;
				if (isPlanner)
				{

					var build = GetBuild();
//					if (build.isLayoutValid)
					{
						build.ShareStringToBuildingsCache();
						
					}
					BuildingsOrQueueChanged();
					if(showPlannerIfNeeded && !PlannerTab.IsVisible())
						App.DispatchOnUIThreadSneakyLow(()=>PlannerTab.instance.Show());
				}
				else
				{
					var b = City.GetBuild();
					b.BuildingsCacheToShareString();
					await b.SaveLayout();
					await GetCity.Post(City.build);
					//App.DispatchOnUIThreadSneakyLow(() =>
				 //  {
					//   //if (PlannerTab.instance.isVisible)
					//   //{
					//	  // PlannerTab.instance.Close();
					//	  // BuildTab.instance.Show();
					//   //}
				 //  });

				}
			
		}


		internal static void ClearQueue()
		{
			BuildQueue.ClearQueue();
			JSClient.view.InvokeScriptAsync("cancelbuilds", Array.Empty<string>() );
		}

		public static Task Enqueue( int slvl, int elvl, int bid, int spot)
		{
			//if(slvl == 0 && elvl == 1)
			//{
			//	var tlvl = postQueueBuildings[bspotTownHall].bl;
			//	if(BuildingDef.all[bid].Thl > tlvl)
			//	{
			//		Note.Show($"Please upgrade town hall be level {BuildingDef.all[bid].Thl}");
			//		return;

			//	}	

			//}
			//var maxBuildings = postQueueBuildings[bspotTownHall].bl * 10;
			CityBuild.postQueueBuildings[spot].SetBid(elvl!=0 ? bid : 0, elvl);
			return BuildQueue.Enqueue(City.build, (byte)slvl, (byte)elvl, (ushort)bid, (ushort)spot);
		
		}
		public static async Task<int> EnqueueUpgrade( int elvl, int spot)
		{
			int rv = 0;
			var b = CityBuild.postQueueBuildings[spot];
			var bid = b.bid;
			Assert(bid != 0);
			for (var level = b.bl;level < elvl;++level)
			{
				++rv;
				await BuildQueue.Enqueue(City.build, (byte)level, (byte)(level +1), (ushort)bid, (ushort)spot);
			}
			return rv;

		}
		//private void Upgrade_Click(object sender, RoutedEventArgs e)
		//{

		//	var id = XYToId(selected);
		//	var sel = GetBuildingPostQueue(id);
		//	var lvl = sel.bl;

		//	if(lvl == 0)// special case
		//		Build(id,sel.def.bid,false);
		//	else
		//		Enqueue(lvl,(lvl + 1),sel.def.bid,id);

		//}
		public static async Task UpgradeToLevel(int level, (int x, int y) target, bool dryRun=false)
		{
			//var target = hovered;
			if (!target.IsValid())
			{
				Status("Please select a building", dryRun);
				return;
			}
			var id = XYToId(target);
			var sel = GetBuildingPostQueue(id);
			if (sel.isRes)
			{
				Note.Show("Cannot upgrade Res");
				return;
			}
			if (sel.isEmpty)
			{
				Note.Show("Nothing to upgrade");
				return;
			}
			var lvl = sel.bl;
			if (level == 1)
				level = lvl + 1;

			if (lvl == 0)
			{
				await Build(id, sel.def.bid, dryRun,false);
				lvl = 1;
			}
			if (lvl < level)
			{

				if(isPlanner)
				{
					buildingsCache[id].bl =(byte) level;
					PlannerTab.BuildingsChanged();

				}
				else if (!dryRun)
				{
					await EnqueueUpgrade( level,  id);
				}
			}
			else
			{
				Status($"{sel.name} is already level {sel.bl} (or upgrading there)", dryRun);

			}
		}

		public static Task Demolish((int x, int y) building, bool dryRun)
		{
			return Demolish(XYToId(building), dryRun);
		}

		public static Building GetBuildingPostQueue(int spot)
		{
			return postQueueBuildings[spot];

		}

		public static int postQueueBuildingCount;
		public static int postQueueTownHallLevel;
		public static Building[] GetBuildingCache()
		{
			if (!postQueueBuildingsDirty || isPlanner)
				return buildingsCache;
			buildingsCache = City.GetBuild().buildings;
			return buildingsCache;
		}
		public static void TouchBuildingCache()
		{
			GetBuildingCache();
		}

		public static Building[] postQueueBuildings
		{
			get
			{

				if (!postQueueBuildingsDirty)
					return postQueuebuildingsCache;
				postQueueBuildingsDirty = false;
				if (!CityBuild.isPlanner)
				{
					buildingsCache = City.GetBuild().buildings;
				}
				//
				// copy current buildings
				//
				for (var i = 0; i < citySpotCount; ++i)
				{
					postQueuebuildingsCache.DangerousGetReferenceAt(i) = buildingsCache.DangerousGetReferenceAt(i);
				}
				if (!CityBuild.isPlanner)
				{
					//
					// Apply queue
					//
					{
						foreach (var q in buildQueue)
						{
							ref var b = ref postQueuebuildingsCache.DangerousGetReferenceAt(q.bspot);
							b.bl = q.elvl;
							if (q.elvl == 0)
								b.id = 0;
							else
								b.id = BuildingDef.BidToId(q.bid);
						}

						if (CityBuildQueue.all.TryGetValue(City.build, out var bq))
						{
							var count = bq.queue.count;
							var data = bq.queue.v;

							for (int i = 0; i < count; ++i)
							{
								ref var q = ref data[i];
								ref var b = ref postQueuebuildingsCache.DangerousGetReferenceAt(q.bspot);
								b.bl = q.elvl;
								if (q.elvl == 0)
									b.id = 0;
								else
									b.id = BuildingDef.BidToId(q.bid);
							}
						}



					}
				}
				// calculate counts
				postQueueBuildingCount = 0;
				postQueueTownHallLevel = 10;
				foreach (var bi in postQueuebuildingsCache)
				{
					if (bi.id == 0 || bi.bl == 0)
						continue;
					var bd = bi.def;
					if (bd.isTower || bd.isWall)
					{
						continue;
					}
					if (bd.isTownHall)
					{
						postQueueTownHallLevel = bi.bl;
						continue;
					}
					++postQueueBuildingCount;
				}
				return postQueuebuildingsCache;
			}


		}
		

		public static async Task Demolish(int id, bool dryRun)
		{
			var sel = GetBuildingPostQueue(id);
			if (sel.isEmpty)
			{
				Status("Already destoryed", dryRun);
			}
			else
			{
				//if (buildQueueFull)
				//{
				//	Status("Build Queue full", dryRun);
				//	return;
				//}

				Status($"Destroy {sel.def.Bn}", dryRun);
				if (!dryRun)
				{
					if (isPlanner)
					{
						buildingsCache[id].SetBid(0, 0);
						PlannerTab.BuildingsChanged();
					}
					else
					{
						await Enqueue(sel.bl, 0, sel.def.bid, id);
					}
				}
				else
				{
					DrawSprite(IdToXY(id), decalBuildingInvalid, 0.312f);
				}
			}
		}
		public static async Task Downgrade((int x, int y) building, bool dryRun)
		{
			var id = XYToId(building);
			var sel = GetBuildingPostQueue(id);

			if (sel.bl == 1)
			{
				await Demolish(id, dryRun);
			}
			else if(sel.bl == 0)
			{
				Status("Already destoryed", dryRun);
				return;

			}
			else
			{
				Status($"Downgrade {sel.def.Bn}", dryRun);
				if (!dryRun)
				{
					if (isPlanner)
					{
						--buildingsCache[id].bl;
						PlannerTab.BuildingsChanged();
					}
					else
					{
						await Enqueue(sel.bl, sel.bl - 1, sel.def.bid, id);
					}
				}
			}
			//if(!dryRun)
			//	buildQueue.Add(new BuildQueueItem() { bspot = id, bid = sel.def.bid, slvl = sel.bl, elvl = (byte)(sel.bl-1) });
		}
		static async Task<bool> BuildWallDialogue()
		{
			var result = await App.DispatchOnUIThreadTask(async () =>
			{

				var dialog = new ContentDialog()
				{
					Title = "Want a wall?",
					Content = "Would you like to build a wall first?",
					PrimaryButtonText = "Please",
					SecondaryButtonText = "No"
				};
				return await dialog.ShowAsync2();
			});
			if (result == ContentDialogResult.Primary)
			{
				await Enqueue(0, 1, bidWall, bspotWall);
				await Task.Delay(400);

				return true;
			}
			else
			{
				return false;
			}
		}
		static async Task<bool> UpgradeTownHallDialogue(int toLevel)
		{
			toLevel = toLevel.Min(10);
			var currentLevel = postQueueBuildings[bspotTownHall].bl;
			if (currentLevel >= toLevel)
				return true;
			var a = await App.DispatchOnUIThreadTask(async () =>
			{
				var dialog = new ContentDialog()
			{
				Title = "Upgrade TownHall?",
				Content = $"Would you like to upgrade your Town Hall to level {toLevel} first?",
				PrimaryButtonText = "Please",
				SecondaryButtonText = "No"
			};
			return await dialog.ShowAsync2();
		});
			if (a == ContentDialogResult.Primary)
			{
				
				await EnqueueUpgrade( toLevel, bspotTownHall);
				await Task.Delay(400);
				return true;
			}
			else if( a == ContentDialogResult.Secondary)
			{
				return true;
			}
			// canceled
			return false;
		}

		public static async Task Build(int id, int bid, bool dryRun, bool wantUI)
		{
			var sel = GetBuildingPostQueue(id);
			if (bid != bidWall && !sel.isEmpty && !SettingsPage.extendedBuild) // special case, wall upgrade from level is allowed as a synonym for build
			{
				Status("Spot is occupied",dryRun);
			}
			else
			{
				var buildDef = BuildingDef.all[bid];
				if( IsWallSpot(id) && !testFlag)
				{
					Status("Walls go here", dryRun);
					return;
				}
				if( IsWaterSpot(id) && !testFlag)
				{
					Status("There is water here :(", dryRun);
					return;
				}
				if ( IsTowerSpot(id))
				{
					if(!buildDef.isTower)
					{
						Status("This looks like a nice place for a tower.", dryRun);
						return;
					}
					else
					{
						if (postQueueBuildings[bspotWall].bl == 0)
						{
							Status("Please build a wall first", dryRun);
							if (!dryRun)
							{

								var good = await BuildWallDialogue();
								if (!good)
									return;
							}
							else
							{
								return;
							}
						}

					}
				}
				else
				{
					if (buildDef.isTower)
					{
						Status("This does not looks like a nice place for a tower.", dryRun);
						return;
					}
					if(IsShoreSpot(id) )
					{
						if(!buildDef.isShoreBuilding && !CityBuild.testFlag)
						{
							Status("Ports and Shipyards go here", dryRun);
							return;
						}
					}
					else
					{
						if(buildDef.isShoreBuilding && !CityBuild.testFlag)
						{
							Status("Please put this on the shore", dryRun);
							return;
						}
					}

				}

				if (dryRun)
				{
					DrawBuilding(IdToXY(id), cityDrawAlpha, bid, AGame.animationT * 0.3247f);
				}
				else
				{
					if (isPlanner)
					{
						var build = GetBuild();
						buildingsCache[id].SetBid(bid, 10);
						PlannerTab.BuildingsChanged();
					}
					else
					{
						var counts = GetBuildingCountAndTownHallLevel();
						var usesSpot = !buildDef.isTower && bid != bidWall;
						if ( ( counts.count == counts.max && counts.townHallLevel < 10 && usesSpot ) || buildDef.Thl > counts.townHallLevel)
						{
							if (!await UpgradeTownHallDialogue( ((counts.count)/10+1 ).Max(buildDef.Thl)))
								return;

						}
						else if (counts.townHallLevel == 10 && counts.count >= 100 && usesSpot)
						{
							if (!dryRun && bid != bidCottage && wantUI)
							{
								int toRemove = await FindBuildingToRemoveUI();
								if (toRemove != -1)
									await Demolish(toRemove, dryRun);
							}
						}
						await Enqueue(0, 1, bid, id);
						
					}
				}
			}
		}

		private static async Task<int> FindBuildingToRemoveUI()
		{
			var toRemove = -1;
			if (SettingsPage.demoBuildingOnBuildIfFull != false)
			{
				var bd = QueueTab.FindExtraBuilding();
				if (bd != -1)
				{
					if (SettingsPage.demoBuildingOnBuildIfFull == null)
					{
						var xy = await App.DoYesNoBoxSticky($"Demo {CityBuild.postQueueBuildings[bd].name} to make room?");
						SettingsPage.demoBuildingOnBuildIfFull = xy.sticky;
						if (!xy.rv)
							bd = -1;
					}
					if (bd != -1)
					{
						toRemove = bd;
					}
				}
			}
			if (toRemove == -1)
			{
				if (SettingsPage.demoCottageOnBuildIfFull != false)
				{
					var bd = FindCabinToDemo();
					if (bd != -1)
					{
						if (SettingsPage.demoCottageOnBuildIfFull == null)
						{
							var xy = await App.DoYesNoBoxSticky($"Demo cabin to make room?");
							SettingsPage.demoCottageOnBuildIfFull = xy.sticky;
							if (!xy.rv)
								bd = -1;
						}
						if (bd != -1)
						{
							toRemove = bd;
						}
					}
				}
			}

			return toRemove;
		}

		public  static Task Build((int x, int y) cc, int bid, bool dryRun, bool wantRemoveUI)
		{
			if (bid != 0)
				return Build(XYToId(cc), bid, dryRun, wantRemoveUI);
			else
				return Task.CompletedTask;
		}


		private Task Downgrade_Click(object sender, RoutedEventArgs e)
		{
			return Downgrade(selected,false);
		}

		private Task Destroy_Click(object sender, RoutedEventArgs e)
		{
			return Demolish(selected,false);
		}

		static int GetHash(string name, int x, int y, float scale)
		{
			return HashCode.Combine(name, x, y, scale);
		}

		public static ImageBrush BrushFromAtlas(string name, int x, int y, float scale)
		{
			var hash = GetHash(name, x, y, scale);
			if (brushFromAtlasCache.TryGetValue(hash, out var rv))
				return rv;

			var bitmap = ImageHelper.FromImages(name);
			var brush = new ImageBrush()
			{
				ImageSource = bitmap,
				Stretch = Stretch.None,
				AlignmentX = AlignmentX.Left,
				AlignmentY = AlignmentY.Top,
				Transform = new MatrixTransform() { Matrix = new Matrix(scale, 0f, 0.0f, scale, -x * scale, -y * scale) }
			};
			//	rect.Stretch = Stretch.None;
			//			rect.Width = width;
			//			rect.Height = height;
			brushFromAtlasCache.Add(hash, brush);
			return brush;
		}
		public static ImageBrush BrushFromImage(string name)
		{
			if (brushFromImageCache.TryGetValue(name, out var rv))
				return rv;

			var bitmap = ImageHelper.FromImages(name);
			var brush = new ImageBrush()
			{
				ImageSource = bitmap,
				Stretch = Stretch.Fill,
			};
			//	rect.Stretch = Stretch.None;
			//			rect.Width = width;
			//			rect.Height = height;
			brushFromImageCache.Add(name, brush);
			return brush;
		}
		public static ImageBrush BrushFromImage(BitmapImage bitmap)
		{
			if (brushFromImageCache2.TryGetValue(bitmap, out var rv))
				return rv;

			var brush = new ImageBrush()
			{
				ImageSource = bitmap,
				Stretch = Stretch.Fill,
			};
			//	rect.Stretch = Stretch.None;
			//			rect.Width = width;
			//			rect.Height = height;
			brushFromImageCache2.Add(bitmap, brush);
			return brush;
		}

		public static ImageBrush BuildingBrush(int id, float scale)
		{
			var iconId = id - 443;
			const int atlasColumns = 4;
			const int duDt = 128;
			const int dvDt = 128;
			var u0 = (iconId % atlasColumns) * duDt;
			var v0 = ((int)(iconId / atlasColumns)%33) * dvDt;
			var uri = SettingsPage.IsThemeWinter() ? "City/Winter/building_set5.png" :
			"City/building_set5.png";
			return BrushFromAtlas(uri, u0, v0, scale);
		}

		public static bool HasBuildOps(int bspot)
		{
			return (IterateQueue().Any(a => a.bspot == bspot));

		}

		public static async Task<bool> MoveBuilding(int a, int b, bool dryRun)
		{
			// Todo:  Cannot be moved if queued
			// Todo: Error checking
			var build = City.GetBuild();
			var bds = isPlanner ? buildingsCache : build.buildings;
			if (HasBuildOps(a) && !isPlanner)
			{
				Status($"Cannot move a building that is being rennovated, please wait or cancel build ops on {bds[a].name} at {IdToXY(a).bspotToString()} or wait", dryRun);
				return false;
			}
			if (HasBuildOps(b) && !isPlanner)
			{
				Status($"Cannot move a building to a spot that is being rennovated, please wait or cancel build ops on {bds[b].name} at {IdToXY(b).bspotToString()} or wait for best results", dryRun);
				return false;
			}
			if (Player.moveSlots <= 0 && !isPlanner)
			{
				Status($"No move spots",dryRun);
				return false;
			}
			else
			{
				Status($"Move {bds[a].name} {IdToXY(a).bspotToString()} to {IdToXY(b).bspotToString()} ", dryRun);

				var rv = true;
				if (!dryRun)
				{
					AUtil.Swap(ref bds[b], ref bds[a]);
					if (!isPlanner)
					{
						rv = await Move(a, b);

					//	await Task.Delay(200);
						
						--Player.moveSlots;
						BuildingsOrQueueChanged();
						await Task.Delay(200);
					}
					else
					{
						AUtil.Swap(ref postQueuebuildingsCache[b], ref postQueuebuildingsCache[a]);
						PlannerTab.BuildingsChanged();
					}
					// I hope that these operations are what I expect with references

				}
				return true;
			}
		}
		async static Task<bool> Move(int from, int to)
		{
			var cid = City.build;
			var s = await Services.Post.SendForText("includes/mBu.php", $"a={from}&b={to}&c={cid}", World.CidToPlayerOrMe(cid));
			if( ( int.TryParse(s.Trim(),System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.CurrentInfo,out var i) &&
			
				( i >= City.bidMin && i <= City.bidMax)) )
			{
				return true;
			}
			else
			{
				Trace($"Invalid Move {i}" );
				return false;
			}

		}

		public static async Task<bool> SwapBuilding(int a, int b, bool dryRun)
		{
			var build = City.GetBuild();
			var bds = isPlanner ? buildingsCache : build.buildings;
			if ( (HasBuildOps(a) || HasBuildOps(b)) && !isPlanner)
			{
				{
					Status($"Cannot move a building that is being rennovated, please wait or cancel build ops on {bds[a].name} at {IdToXY(a).bspotToString()} for best results", dryRun);
				}
				if (HasBuildOps(b))
				{
					Status($"Cannot move a building that is being rennovated, please wait or cancel build ops on {bds[b].name} at {IdToXY(b).bspotToString()} for best results", dryRun);
				}

				return false;
			}
			if (Player.moveSlots >= 3)
			{
				// I hope that these operations are what I expect with references
				Status($"Swap {bds[b].name} and {bds[a].name} ({Player.moveSlots} moves left) ", dryRun);
				if (!dryRun)
				{

					AUtil.Swap(ref bds[a], ref bds[b]);

					if (!isPlanner)
					{
						var scratch = FindAnyFreeSpot(a);
						--Player.moveSlots;
						if (!await Move(a, scratch))
							return false;
						--Player.moveSlots;
						if (!await Move(b, a))
							return false;
						--Player.moveSlots;
						if (!await Move(scratch, b))
						{
							Note.Show("Failed to move for some reason?  Was the city building?");
							return false;
						}
						BuildingsOrQueueChanged();
						await Task.Delay(200);

					}
					else
					{
						AUtil.Swap(ref postQueuebuildingsCache[a], ref postQueuebuildingsCache[b]);
						PlannerTab.BuildingsChanged();
					}
				}
				return true;
			}
			else
			{
				Status("Note enough move spots (press shift F5 if this is not true)", dryRun);
				return false;
			}
			
		}

		internal static async void MoveHovered(bool _isSingleAction,bool isStart, bool dryRun)
		{
			var build = GetBuild();

			Status($"Move slots left: {Player.moveSlots}", dryRun);
			if (!CanvasHelpers.IsValid(hovered) )
			{
				Status("Please select something in the city", dryRun);
				return;
			}
			int bspot = XYToId(hovered);
			var b = isPlanner ? buildingsCache[bspot] : build.buildings[bspot];

			if (isStart)
			{
				if (b.isBuilding)
				{

					Status($"Move {b.def.Bn} at {hovered.bspotToString()} to ... ", dryRun);
					if (!dryRun)
					{
						CityView.SetSelectedBuilding( hovered, _isSingleAction );
						if (_isSingleAction)
						{
							PushSingleAction(Action.moveEnd);
						}
						else
						{
							SetAction(Action.moveEnd);
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
				Assert(CanvasHelpers.IsValid(selected));

				if (b.isRes )
				{
					Status("Please select an empty spot", dryRun);
				}
				else
				{
					var source = XYToId(selected);

					// Is this a valid transition
					var bs1 = GetSpotType(bspot);
					
					var bs0 = GetSpotType(XYToId(selected));
					if(testFlag)
					{
						if (bs1 == SpotType.wall || bs1 == SpotType.invalid || bs1 == SpotType.water)
							bs1 = SpotType.building;
						if (bs0 == SpotType.wall || bs0 == SpotType.invalid|| bs0 == SpotType.water)
							bs0 = SpotType.building;
					}

					if (bs0 != bs1)
					{
						Status("Doesn't fit there", dryRun);
						return;
					}
					if((HasBuildOps(bspot) || HasBuildOps(source))&&!isPlanner )
					{
						Status($"Cannot move a building that is being rennovated", dryRun);
						return;
					}


					if (dryRun)
					{
						DrawSprite(hovered, decalMoveBuilding, 0.343f);
						DrawSprite(selected, decalMoveBuilding, 0.323f);

					}

					{

						if (!b.isEmpty )
						{
							if(IsTowerSpot(selected))
								Status("Cannot swap towers, please move them one at a time", dryRun);
							else
								await SwapBuilding(source, bspot, dryRun);
						}
						else
						{
							await MoveBuilding(source, bspot, dryRun);
						}
						if (!dryRun)
						{
							ClearSelectedBuilding();
							if (_isSingleAction)
							{
								RevertToLastAction();
							}
							else
							{
								SetAction(Action.moveStart);
							}
						}
					}
				}
			}
		}

		public static void PushSingleAction(Action _action)
		{
			priorQuickAction = action;
			SetAction(_action);
			isSingleClickAction = true;
		}

		private void quickBuild_SelectionChanged(object sender, Windows.UI.Xaml.Controls.SelectionChangedEventArgs e)
		{
			//var sel = quickBuild.SelectedItem  as QuickBuildItem;
			//if (sel != null )
			//{
			//	var bid = sel.bid;
			//	quickBuildId = bid;
			//	// clear selection on tool change
			//	if (bid <  (int)Action.count)
			//	{
			//		action = (Action)bid;
			//	}
			//	else
			//	{
			//		action = Action.build;
			//	}
			//	if(action != Action.move)
			//		selected = CanvasHelpers.invalidXY;

			//}
			//else
			//{
			//	// this is an invalid setting
			//	action = Action.none; // is this an appropriate action to take?
			//	quickBuildId = 0;
			//}
		}
	
		public static async Task PerformAction(Action action, (int x, int y) cc, int _quickBuildId, bool dryRun)
		{
		

			int bspot = XYToId(cc);
			var build = GetBuild();
			var b = GetBuildingPostQueue(bspot);

			if(action == Action.moveEnd)
			{
				// We lost our move source
				if(!CanvasHelpers.IsValid(selected))
				{
					if(isSingleClickAction)
					{
						RevertToLastAction();
					}
					else
					{
						SetAction(Action.moveStart);
					}
				}
			}

			switch (action)
			{
				case Action.layout:
					{
					
						if(CityBuild.isPlanner)
						{
							Status("You are in layout mode, exit to use the layout tool", dryRun);
						}
						else if ( !build.isLayoutValid)
						{

							Status("Please assign a layout",dryRun);
						}
						else
						{

							await SmartBuild( build,cc, build.BidFromOverlay(bspot),true,dryRun, wantDemoUI:true);

						}
						break;
					}
				case Action.build:
					{
						if (!b.isEmpty && !isPlanner)
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

							if (sel != 0)
							{
								if(isPlanner)
									await Build(cc, sel,  dryRun,false);
								else
									await SmartBuild(City.GetBuild(),cc, sel, false, dryRun, wantDemoUI: true);

								break;
							}
							Status("Please select a valid building", dryRun);
						}
						break;
					}
				case Action.destroy:
					{
						//if (buildQueueFull)
						//{
						//	Status("Build Queue full", dryRun);
						//	break;
						//}
						await Demolish(cc, dryRun);

						
						break;
					}
				case Action.moveStart:
				case Action.moveEnd:
					{
						MoveHovered(isSingleClickAction, (action==Action.moveStart), dryRun);
						break;
					}
				case Action.downgrade:
					{
						await Downgrade(cc,dryRun);
						break;
					}
				case Action.upgrade:
					{
						UpgradeToLevel(1,cc,dryRun);
						break;
					}
				case Action.abandon:
					{
						if(!dryRun)
							instance.Abandon_Click(null, null);
						break;
					}
				case Action.flipLayoutH:
					{
						if (!dryRun)
						{
							var city = GetBuild();
							city.FlipLayoutH();
							

						}

						break;
					}
				case Action.showShareString:
					{
						if(isSingleClickAction)
							await ShareString.Show(City.build);
						break;
					}
				case Action.togglePlanner:
					{
						if (!dryRun)
						{

							App.DispatchOnUIThreadSneaky(()=>TogglePlanner() );
						}

						break;
					}
				case Action.flipLayoutV:
					{
						if (!dryRun)
							GetBuild().FlipLayoutV();
						break;
					}
				case Action.none:
				{
					if (b.isEmpty)
					{
							if (IsBuildingSpotOrTownHall(XYToId(hovered)))
							{
								Status($"Left click to build something\nRight click to select a quick build tool",dryRun);

							}
							else if (IsTowerSpot(hovered))
							{
								Status($"Left click to build tower\nRight click to select a quick build tool",dryRun);

							}
							else if (IsWallSpot(hovered))
							{
								Status($"Left click to build wall\nRight click to select a quick build tool",dryRun);
							}
							else
							{
								Status($"Please don't left click here\nRight click to select a quick build tool",dryRun);
							}
					}
					else 
					{
						Status($"Left click modify {b.def.Bn}, Right click to select a quick build tool",dryRun);
					}
					
					break;
				}
			}
		}
		public static int FindSpare(int bid, bool dryRun)
		{
			var rv = 0;
			var build = City.GetBuild();
			for (int xy = 0; xy < City.citySpotCount; ++xy)
			{
				var overlayBid = build.BidFromOverlay(xy);
				var xyBuilding = postQueueBuildings[xy].def.bid;

				if (overlayBid == xyBuilding || xyBuilding != bid)
					continue;
				if (HasBuildOps(xy) && !isPlanner)
				{
					Status($"{postQueueBuildings[xy].name} at {IdToXY(xy).bspotToString()} is desired, but it is being rennovated, please wait or cancel build ops for best resuts", dryRun);
					rv = -1;
					continue;
				}
				return xy;
			}
			return rv;
		
	}

		public static async Task<int> SmartBuild(City build, (int x, int y) cc, int desBid, bool searchForSpare, bool dryRun = false, bool demoExtraBuildings=false,bool wantDemoUI=false)
		{
			int rv = 0;
			var bspot = XYToId(cc);
			var b = GetBuildingPostQueue(bspot);
			var desB = BuildingDef.all[desBid];
			var desName = desB.Bn;
			if (BuildingDef.IsRes(desBid))
			{
				desBid = 0; // if it is a resource, ignore it
			}
			if (!IsBuildingSpot(bspot))
			{
				await Build(cc, desBid, dryRun,false); ;
				return 1;
			}
			var usesSpot = IsBuildingSpot(bspot) && !desB.isTower && !desB.isTownHall && (bspot != bspotWall);
			var curBid = b.def.bid;
			var takeFrom = -1;
			var takeScore = 0;
			var putTo = -1;
			var putScore = 0;

			if (searchForSpare && !isPlanner && build.isLayoutValid && usesSpot )
			{
				// See if there are spare buildings that we can take
				for (int xy = 1; xy < City.citySpotCount-1; ++xy)
				{
					var overlayBid = build.BidFromOverlay(xy);
					var xyBuilding = postQueueBuildings[xy].def.bid;

					if (overlayBid != xyBuilding)
					{

						// do they have what we need?
						if (xyBuilding == desBid && desBid !=0)
						{
							// two points for ourbuilding is also needed there
							var score = ((overlayBid == curBid) ? 2 : 1);
							if (!HasBuildOps(xy))
								score += 8;
							if (score > takeScore)
							{
								takeScore = score;
								takeFrom = xy;
							}
						}

						//do they want what we have?
						if (overlayBid == curBid && curBid != 0)
						{
							var score = (xyBuilding == desBid) ? 4 : (xyBuilding == 0) ? 3 : postQueueBuildings[xy].isBuilding ? 2 : 1;
							if (!HasBuildOps(xy))
								score += 8;
							if (score > putScore)
							{
								putScore = score;
								putTo = xy;

							}

						}
					}
				}
			}
			var counts = GetBuildingCountAndTownHallLevel();

			// case 1:  nothing here, or res. if res, demo first, then Add building
			if (b.id == 0 || b.isRes)
			{

				// Do we want a building here?
				if (desBid != 0)
				{
					if (b.isRes)
					{

						Status($"Destorying {b.def.Bn} to make way for {desName}", dryRun);


						await Demolish(cc, dryRun);
						++rv;
						if (!dryRun)
							await Task.Delay(400).ConfigureAwait(true);
						if (takeScore > 0 && searchForSpare)
						{
							Status($"Please wait on demo, then spare {desName} can be moved into place", dryRun);
							// wait for demo
							return rv;
						}
					}
					else if (takeScore > 0 && searchForSpare)
					{
						Status($"Found an unneeded {desName}, will move it to the right spot for you", dryRun);

						if (!await MoveBuilding(takeFrom, bspot, dryRun))
							return -1;
						return rv;
					}
					if (counts.townHallLevel < desB.Thl || (counts.count == counts.max && counts.townHallLevel < 10))
					{
						var level = (counts.count / 10 + 1).Max(b.def.Thl).Min(10);
						if (dryRun)
						{
							Status($"Upgrade town hall to level {level}", dryRun);

						}
						else
						{
							if (!await UpgradeTownHallDialogue(level))
								return rv;
							++rv; // not exact
 						}
					}
					else if (counts.count >= 100 && !desB.isTower && !desB.isWall )
					{
						if (!desB.isCabin && desBid != 0)
						{
							int bestSpot = -1;
							if (wantDemoUI && !dryRun)
							{
								bestSpot = await FindBuildingToRemoveUI();

							}
							else
							{
								// Is there a cabin to remove?
								bestSpot = demoExtraBuildings ? QueueTab.FindExtraBuilding() : -1;
								if (bestSpot == -1)
									bestSpot = FindCabinToDemo();
							}
							if (bestSpot != -1)
							{
								Status("Will Demolish something to make room", dryRun);

								await Demolish(bestSpot, dryRun);
								//break;
								--counts.count;
								++rv;

							}
						}
						else
						{
							Status("Don't want to demo a cabin to build a cabin", dryRun);
						}


					}
					//if (counts.count < 100)
					await Build(cc, desBid == 0 ? bidCottage : desBid, dryRun,false);

				}
				else
				{
					Assert(!isPlanner);
					// Nothing wanted here
					if (b.isRes)
					{
						Status($"What a lovely {b.def.Bn}.", dryRun);

					}
					else
					{
						// nothing here
						if (counts.count < 100 ) // can we put a cabin here?
						{
							Status($"No building is wanted here, how about a cottage instead?", dryRun);
							await Build(cc, bidCottage, dryRun, false) ;
							++rv;
						}
						else
						{
							Status("Don't want to demo a cabin to build a cabin", dryRun);
						}
					}
				}
			}
			else
			{
				// building is here
				// a building
				// Try to move it to some place where one is needed

				if (desBid != curBid)
				{
					if (putScore > 0)
					{
						var name = b.def.Bn;
						if (putScore < 8 )
						{
							Status($"{b.def.Bn} is wanted elsewhere but someone is doing something - please clear the queue or wait", dryRun);
						}
						else
						{
							switch (putScore)
							{
								case 4:
								case 4 + 8:
									{
										Status($"Swaping {b.def.Bn} and {desName} as they are mixed up ({Player.moveSlots} moves left)", dryRun);
										if (!await SwapBuilding(bspot, putTo, dryRun))
											return -1;
										// two way swap 
										break;
									}
								case 3:
								case 3 + 8:
									Status($"Move {name} to where it is wanted", dryRun);
									if(!await MoveBuilding(bspot, putTo, dryRun))
										return -1;
									break;
								case 2:
								case 2 + 8:
									Status($"{name} is wanted elsewhere but there is a building in the way", dryRun);
									break;
								case 1:
								case 1 + 8:
									Status($"{name} is wanted elsewhere but there are reources in the way", dryRun);
									break;


							}
						}
						


					}
					else
					{

						if (takeScore == 0)
						{
							if (desBid == 0)
							{
								Status($"{b.def.Bn} is not wanted, destroying it", dryRun);

								++rv;
								await Demolish(cc, dryRun);

							}
							else
							{
								if (counts.count >= 100)
								{

									if (demoExtraBuildings)
									{
										int bestSpot = QueueTab.FindExtraBuilding();
										if (bestSpot != -1)
										{
											await Demolish(bestSpot, dryRun);
											--counts.count;
											++rv;
										}

									}

									if (counts.count >= 100 && desBid != bidCottage)
									{
										var bestSpot = FindCabinToDemo();
										if (bestSpot != -1)
										{
											Status("Will Demolish a Cottage to make room", dryRun);

											await Demolish(bestSpot, dryRun);
											//break;
											--counts.count;

											++rv;

										}

									}
								}


								var cb = FindAnyFreeSpot(bspot);
								if (!await MoveBuilding(bspot, cb, dryRun))
									return -1;


								// build the correct building
								if (dryRun)
									DrawSprite(hovered, decalBuildingInvalid, .31f);

								await Build(cc, desBid, dryRun,wantDemoUI);
								++rv;
							}
						}
						else
						{
							if (takeScore < 8)
							{
								Status($"There an extra {desName}, but it cannot be moved because someone is doing something, please clear queue or wait", dryRun);

							}
							else
							{
								if (!await MoveBuilding(bspot, FindAnyFreeSpot(bspot), dryRun))
									return - 1;

								Status($"Found an unneeded {desName}, will move it to the right spot for you", dryRun);

								if (!await MoveBuilding(takeFrom, bspot, dryRun))
									return -1;
							}
						}
					}		
					
				}
				else
				{
					Status($"{desName} is in the right spot, no changed needed", dryRun);

				}
			}
			return rv;
		}

		private static int FindCabinToDemo()
		{
			var bestSpot = -1;
			int bestLevel = int.MaxValue;
			for (int spot = 0; spot < citySpotCount; ++spot)
			{
				var bld = postQueueBuildings[spot];
				if (bld.isCabin)
				{
					if (bld.bl < bestLevel)
					{
						// is it not being modified?
						if (HasBuildOps(spot))
							continue;

						bestLevel = bld.bl;
						bestSpot = spot;
					}
				}
			}

			return bestSpot;
		}

	public static int FindAnyFreeSpot(int referenceBid)
	{
		return FindFreeSpot(CityBuild.GetSpotType(referenceBid));
	}
		public static ushort findSpotOffset;
		public static int FindFreeSpot(SpotType type = SpotType.building)
		{
			var build = City.GetBuild();
			var spots = CityBuild.GetSpots(type);
			for(int iter=0;iter<=City.citySpotCount; ++iter)
			{
				// update and wrap
				++findSpotOffset;
				if (findSpotOffset >= City.citySpotCount - 1)
					findSpotOffset = 1;
				
				if (!spots.Contains(findSpotOffset))
					continue;
				var bld = postQueueBuildings[findSpotOffset];
				if (!bld.isEmpty)
					continue;
				if (build.BidFromOverlay(findSpotOffset) != 0)
					continue;
				if (HasBuildOps(findSpotOffset))
					continue;
				return findSpotOffset;
			}
			Assert(false);
			return 0; // error
		}

		private static async Task TogglePlanner()
		{
			if (CityBuild.isPlanner)
			{
				await CityBuild._IsPlanner(false,false);
			}
			else
			{
				if (!GetBuild().isLayoutValid)
					await ShareString.Show(City.build);
				await CityBuild._IsPlanner(true, true);
			
			}
		}
		public static (int x, int y) int00 = (0, 0);

		private static async Task RemoveCastleFromLayout(City city)
		{
			if(CityBuild.isPlanner)
				Note.Show("Might not work properly in planner mode, good luck!");
			
			var id = city.FindOverlayBuildingOfType(bidCastle);
			if(id == int00 )
			{
				Assert(false);
				return;
			}
		}

		internal static void ShortBuild(short bid)
		{
			PerformAction(CityBuild.Action.build, hovered, bid,false);
			lastQuickBuildActionBSpot = XYToId(hovered);

		}

		public static void PointerDown((int x, int y) cc)
		{
			//  called before pointer release, pointer release is a click or drag/swaipe
			hovered = cc;

		}

		public static void PreviewBuildAction()
		{
			if (hovered.IsValid())
			{
				if (XYToId(hovered) == lastQuickBuildActionBSpot)
					return;
				PerformAction(action, hovered, quickBuildId, true);
			}
		}

		public static async void Click((int x, int y) cc, bool isRight)
		{
			//if (CityBuild.menuOpen)
			//{
			//	if (ShellPage.instance.buildMenu.IsOpen)
			//	{
			//		ShellPage.instance.buildMenu.IsOpen = false;
			//		//				ShellPage.instance.buildMenuCanvas.Visibility = Visibility.Collapsed;
			//		//	Assert(false);
			//	//	return;
			//	}
			//}
			//while( CityBuild.menuOpen )
			//{
			//	await Task.Delay(200).ConfigureAwait(true);
			//}

			int bspot = XYToId(cc);

			// tempoararily switch to Select from quickbuild
			if(!isRight && action == Action.build && !postQueueBuildings[bspot].isEmpty && !isPlanner)
			{
				priorQuickAction = action;
				action = Action.none;
			}

			if (!isRight && (action != Action.none) )
			{
				ElementSoundPlayer.Play(action == Action.destroy ? ElementSoundKind.GoBack: ElementSoundKind.Focus );
				await PerformAction(action, cc, quickBuildId, false);
				if(action != Action.moveStart && action != Action.moveEnd)
					lastQuickBuildActionBSpot = bspot;

			//	Assert(!isSingleClickAction);
//				{
//					Assert(action == Action.moveEnd);
//					RevertToLastAction(); // this is only for move
//				}
				//Assert(singleClickAction == Action.none);
			}
			else
			{
				//	Log($"{action} => None");
				////	action = Action.none;

				//if (isRight)
				//{
				//	Tips.Show(instance.tipBuildRight, nameof(instance.tipBuildRight));
				//}
				//else
				//{
				//	Tips.Show(instance.tipBuildLeft,nameof(instance.tipBuildLeft));
		
				//}

				//	var i = instance;
				//i.building.Text = d.Bn;
				//i.description.Text = d.Ds;
				//i.upgrade.IsEnabled = d.Bc.Count() > b.bl && b.isBuilding;
				//i.downgrade.IsEnabled = b.bl > 1;
				//i.rect.Fill = BuildingBrush(d.bid, 1.0f);
				{
					ElementSoundPlayer.Play( ElementSoundKind.Show);
					ShowContextMenu(cc,isRight);

				}
			}
		}
		public const int buildToolSpan = 448;


		static bool contextMenuResultSelected = false;
		public static void ShowContextMenu((int x, int y) cc, bool isRight)
		{
			isSingleClickAction = false; // default
											   // toggle visibility

			contextMenuResultSelected = false;
			int bspot = XYToId(cc);
			var b = postQueueBuildings[bspot];

			if (!isRight)
			{
				if (IsWaterSpot(bspot) && !testFlag)
				{
					Note.Show("There is water here. :(");
					return;
				}
				isSingleClickAction = true;
				if (CityBuild.IsWallSpot(bspot))
				{
					bspot = 0;
					cc = (span0, span0);
					b = postQueueBuildings[bspot];

				}
			}
			var d = b.def;
			if (d.bid != 0)
				JSClient.view.InvokeScriptAsync("exBuildingInfo", new[] { d.bid.ToString(), b.bl.ToString(), bspot.ToString() });

			CityView.SetSelectedBuilding( cc, isSingleClickAction );

			var type = isRight ? MenuType.quickBuild :
				 
				
				bspot == 0 ? MenuType.buliding :
				b.id == 0 ? (CityBuild.IsTowerSpot(bspot) ? MenuType.tower : CityBuild.IsShoreSpot(bspot) ? MenuType.shore : MenuType.empty ) :
				b.bl == 0 ? MenuType.res :
				d.bid == bidTownHall ? isPlanner ? MenuType.townhallPlanner: MenuType.townhall :
				MenuType.buliding;
			UpdateBuildMenuType(type, bspot);

			//				ShellPage.instance.buildMenu.IsOpen = true;
			var sc = ShellPage.CanvasToScreen(ShellPage.mousePosition);
			//var bm = ShellPage.instance.buildMenu;
			//Canvas.SetLeft(bm, sc.X - buildToolSpan / 2 - 1);
			//Canvas.SetTop(bm, sc.Y - buildToolSpan / 2 + 41);
			//		ShellPage.instance.buildMenuCanvas.Visibility = Visibility.Visible;
			//bm.ContentMenuBackgroundStyle = new Style( typeof(Rectangle) ) {  (Style)Application.Current.Resources[isRight? "ContentMenuStyle" : "ContentMenu2Style"];

			buildMenu.ShowAt(ShellPage.instance.grid, new FlyoutShowOptions() { Position = new Windows.Foundation.Point(sc.X, sc.Y), Placement = FlyoutPlacementMode.Top });

		}

		private async void Abandon_Click(object sender, RoutedEventArgs e)
		{
			var cid = City.build;
			await App.DispatchOnUIThreadExclusive(cid,async() =>
			{
				var dialog = new ContentDialog()
			{
				Title = "Are you Sure?",
				Content = "Abandon " + City.GetOrAdd(cid).nameAndRemarks,
				PrimaryButtonText = "Yes",
				SecondaryButtonText = "Cancel"
			};
				var rv= await dialog.ShowAsync2();
				if (rv == ContentDialogResult.Primary)
				{
					await JSClient.view.InvokeScriptAsync("misccommand", new[] { "abandoncity", cid.ToString() });
					
					await Task.Delay(500);
					City.GetOrAdd(cid).pid = 0; //
					CitiesChanged();
					NavStack.Back(true);
				}

			});
		}
		public async void ItemClick(object sender, Windows.UI.Xaml.Controls.ItemClickEventArgs e)
		{
			contextMenuResultSelected = true;
			var bi = e.ClickedItem as BuildMenuItem;
			lastQuickBuildActionBSpot = -1; // reset
			if (bi != null)
			{
				if (bi.isBuilding)
				{
					if (isSingleClickAction )
					{
					
						await PerformAction(CityBuild.Action.build, selected, bi.bid, false);
						RevertToLastAction();

					}
					else
					{
						SetQuickBuild(bi.bid);
						ClearSelectedBuilding();
					}
					//Array.Sort(buildingMru, (a, b) => a.cacheScore.CompareTo(b.cacheScore) );
					//if (menuType == MenuType.quickBuild)


				}
				else if (bi.isAction)
				{
		//			var items = ShellPage.instance.buildMenu.Items;
					if (bi.action == Action.layout && !City.GetBuild().isLayoutValid )
					{
						Note.Show("Please assign a layout");
						//		JSClient.JSInvoke("showLayout", null);
						await ShareString.Show(City.build);
						SetAction(bi.action);
						ClearSelectedBuilding();
//						App.DispatchOnUIThreadLow( ()=> PlannerTeachingTip.Show(nameof(PlannerTeachingTip)));


					}
					else
					{
						if (isSingleClickAction)
						{
							if (bi.action == CityBuild.Action.moveStart)
							{

								SetAction(CityBuild.Action.moveEnd);
								// leave action pending
							}
							else
							{
								await PerformAction(bi.action, selected, 0, false);
								lastQuickBuildActionBSpot = XYToId(selected);
								
								RevertToLastAction();

							}
						}
						else if(bi.action == Action.togglePlanner)
						{
							await TogglePlanner();
							

						}
						else if (bi.action == Action.showShareString)
						{
							await ShareString.Show(City.build); 
						}
						else
						{
							SetAction(bi.action);
							ClearSelectedBuilding();
						}
					}
				}
				else
				{
					Assert(false);
					ClearSelectedBuilding();
				}
			}
			else
			{
				Assert(false);
			}
			buildMenu.Hide();

		}
		private void StackPanel_AccessKeyInvoked(UIElement sender, AccessKeyInvokedEventArgs args)
		{

		}

		public static void ProcessKey(VirtualKey key)
		{
			switch (key)
			{
				case VirtualKey.Space: CityBuild.Click(CityView.hovered, true); return;
				case VirtualKey.Enter: CityBuild.Click(CityView.hovered, false); return;
				case Windows.System.VirtualKey.Left:
					if (CityView.hovered.IsValid())
						CityView.hovered.x = (CityView.hovered.x - 1).Max(City.span0);
					else
						CityView.hovered = (0, 0);

					break;

				case Windows.System.VirtualKey.Up:
					if (CityView.hovered.IsValid())
						CityView.hovered.y = (CityView.hovered.y - 1).Max(City.span0);
					else
						CityView.hovered = (0, 0);
					break;

				case Windows.System.VirtualKey.Right:
					if (CityView.hovered.IsValid())
						CityView.hovered.x = (CityView.hovered.x + 1).Min(City.span1);
					else
						CityView.hovered = (0, 0);

					break;

				case Windows.System.VirtualKey.Down:
					if (CityView.hovered.IsValid())
						CityView.hovered.y = (CityView.hovered.y + 1).Min(City.span1);
					else
						CityView.hovered = (0, 0);
					break;

				case Windows.System.VirtualKey.Number1: UpgradeOrTower(1); break;
				case Windows.System.VirtualKey.Number2: UpgradeOrTower(2); break;
				case Windows.System.VirtualKey.Number3: UpgradeOrTower(3); break;
				case Windows.System.VirtualKey.Number4: UpgradeOrTower(4); break;
				case Windows.System.VirtualKey.Number5: UpgradeOrTower(5); break;
				case Windows.System.VirtualKey.Number6: UpgradeOrTower(6); break;
				case Windows.System.VirtualKey.Number7: UpgradeOrTower(7); break;
				case Windows.System.VirtualKey.Number8: UpgradeOrTower(8); break;
				case Windows.System.VirtualKey.Number9: UpgradeOrTower(9); break;
				case Windows.System.VirtualKey.Number0: CityBuild.UpgradeToLevel(10, CityView.hovered); break;
				case Windows.System.VirtualKey.U: CityBuild.UpgradeToLevel(1, CityView.hovered, false); break;
				// case Windows.System.VirtualKey.Q: CityBuild.ClearQueue(); break;
				case Windows.System.VirtualKey.D: CityBuild.Demolish(CityView.hovered, false); break;
				case Windows.System.VirtualKey.Escape: CityBuild.ClearAction(); break;
				case (VirtualKey)192:
					{
						if (action == CityBuild.Action.moveEnd)
							CityBuild.MoveHovered(true, false, false);
						else
						{
							CityView.ClearSelectedBuilding();
							CityBuild.MoveHovered(true, true, false);
						}
						break; //  (City.XYToId(CityView.selected), City.XYToId(CityView.hovered)); break;
					}
				// short keys
				case Windows.System.VirtualKey.F: CityBuild.ShortBuild(City.bidForester); return; //  448;
				case Windows.System.VirtualKey.C: CityBuild.ShortBuild(City.bidCottage); return; //  446;
				case Windows.System.VirtualKey.R: CityBuild.ShortBuild(City.bidStorehouse); return; //  464;
				case Windows.System.VirtualKey.S: CityBuild.ShortBuild(City.bidQuarry); return; //  461;
																								// case
																								// Windows.System.VirtualKey.Q
																								// :
																								// CityBuild.ShortBuild(City.bidHideaway
																								// );
																								// return;
																								// // 479;
				case Windows.System.VirtualKey.A: CityBuild.ShortBuild(City.bidFarmhouse); return; //  447;
																								   // case
																								   // Windows.System.VirtualKey.U
																								   // :
																								   // CityBuild.ShortBuild(City.bidCityguardhouse
																								   // );
																								   // return;
																								   // // 504;
				case Windows.System.VirtualKey.B: CityBuild.ShortBuild(City.bidBarracks); return; //  445;
				case Windows.System.VirtualKey.I: CityBuild.ShortBuild(City.bidMine); return; //  465;
				case Windows.System.VirtualKey.T: CityBuild.ShortBuild(City.bidTrainingGround); return; //  483;
				case Windows.System.VirtualKey.M: CityBuild.ShortBuild(City.bidMarketplace); return; //  449;
				case Windows.System.VirtualKey.V: CityBuild.ShortBuild(City.bidTownhouse); return; //  481;
				case Windows.System.VirtualKey.L: CityBuild.ShortBuild(City.bidSawmill); return; //  460;
				case Windows.System.VirtualKey.E: CityBuild.ShortBuild(City.bidStable); return; //  466;
				case Windows.System.VirtualKey.H: CityBuild.ShortBuild(City.bidStonemason); return; //  462;
				case Windows.System.VirtualKey.W: CityBuild.ShortBuild(City.bidSorcTower); return; //  500;
				case Windows.System.VirtualKey.G: CityBuild.ShortBuild(City.bidWindmill); return; //  463;
				case Windows.System.VirtualKey.Y: CityBuild.ShortBuild(City.bidAcademy); return; //  482;
				case Windows.System.VirtualKey.Z: CityBuild.ShortBuild(City.bidSmelter); return; //  477;
				case Windows.System.VirtualKey.K: CityBuild.ShortBuild(City.bidBlacksmith); return; //  502;
				case Windows.System.VirtualKey.X: CityBuild.ShortBuild(City.bidCastle); return; //  467;
				case Windows.System.VirtualKey.O: CityBuild.ShortBuild(City.bidPort); return; //  488;
				case Windows.System.VirtualKey.P: CityBuild.ShortBuild(City.bidShipyard); return; //  491;
				case Windows.System.VirtualKey.Q: SmartBuild(City.GetBuild(), hovered,City.GetBuild().BidFromOverlay(hovered), true, false, wantDemoUI: true); return; 

				default:
					break;
			}
		}

		private static void UpgradeOrTower(int number)
		{
			var xy = CityView.hovered;
			var spot = City.XYToId(xy);
			if (CityBuild.IsTowerSpot(spot) && CityBuild.postQueueBuildings[spot].bl == 0)
			{
				var bid = number switch
				{
					1 => City.bidSentinelPost,
					2 => bidRangerPost,
					3 => bidTriariPost,
					4 => bidPriestessPost,
					5 => bidBallistaPost,
					6 => bidSnagBarricade,
					7 => bidEquineBarricade,
					8 => bidRuneBarricade,
					_ => bidVeiledBarricade
				};

				ShortBuild(bid);
			}
			else
			{
				CityBuild.UpgradeToLevel(number, CityView.hovered);
			}
		}
		//public static BuildPhase GetBuildPhase()
		//{
		//	var buildings = postQueueBuildings;

		//	foreach (var b in buildings )
		//	{
		//		// any mil buildings full?
		//		if(b.isMilitary)
		//		{

		//		}
		//	}
		//}
	}
	public enum BuildPhase
	{
		cabins,
		buildings,
		teardown,
	}

	public class BuildMenuItemGroup
	{
		public string title { get; set; }
		public List<BuildMenuItem> items { get; set; } = new();
	}
	
	public class BuildMenuItem 
	{
		public int bid;
		public bool isAction => action != Action.invalid;
		public bool isBuilding => bid != 0;
		public ImageBrush brush;
		public string toolTip;
		public string header;
		public Windows.UI.Color textColor;
		public CityBuild.Action action = Action.invalid;
		public string accessKey { get; set; }
		public const int width = 64;
		public const int height = 64;

		public BuildMenuItem()
		{
			action = Action.invalid;
			bid = -1;
			textColor = Windows.UI.Colors.Black;

		}
		public BuildMenuItem(int _bid)
		{
			action = Action.invalid;
			{
				bid = _bid;
				var def = BuildingDef.all[_bid];
				header = def.Bn;
				toolTip = def.Ds;
				brush= BuildingBrush(bid, (float)width / 128.0f);
			//	Command = BuildMenuItemCommand.instance;
				var match = shortKeyRegEx.Match(toolTip);
				if(match.Success && match.Groups.Count == 2)
				{
					accessKey = match.Groups[1].Value;
				}
			}
		}
		public BuildMenuItem(string name, Action action, string icon, string toolTip)
		{
			header = name;
			this.action = action;
			brush = CityBuild.BrushFromImage(icon);
			this.toolTip = toolTip;
		}
	}

	//// nested types not supported
	//public class QuickBuildItem
	//{
	//	public int bid;
	//	public string name { get; set; }
	//	public ImageBrush brush { get; set; }
	//	public QuickBuildItem(int _id)
	//	{
	//		bid = _id;
	//		name = BuildingDef.all[_id].Bn;
	//		brush = CityBuild.BuildingBrush(_id, 0.5f);
	//	}
	//	public QuickBuildItem(CityBuild.Action _id, string _name, string image)
	//	{
	//		bid = (int)_id;
	//		name = _name;
	//		brush = CityBuild.BrushFromImage(image);

	//	}
	//}
	public class BuildingButton : Windows.UI.Xaml.Controls.Button
	{
		/// <summary>
		/// Identifies the <see cref="Image id"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty bidProperty =
			DependencyProperty.Register("bid", typeof(int), typeof(BuildingButton), new PropertyMetadata(null));

		public int bid
		{
			get
			{
				return (int)this.GetValue(bidProperty);
			}
			set
			{
				this.SetValue(bidProperty, value);
				Background = BuildingBrush(bid, (float)Width / 128.0f);
			}
		}

		public BuildingButton()
		{
			Width = 64;
			Height = 64;
		}
	}
	public class BuildingRect : Windows.UI.Xaml.Controls.Canvas
	{
		/// <summary>
		/// Identifies the <see cref="Image bid"/> dependency property.
		/// </summary>
		public static readonly DependencyProperty bidProperty =
			DependencyProperty.Register("bid", typeof(int), typeof(BuildingButton), new PropertyMetadata(null));

		public int bid
		{
			get
			{
				return (int)this.GetValue(bidProperty);
			}
			set
			{
				this.SetValue(bidProperty, value);
				Background = BuildingBrush(bid, (float)Width / 128.0f);
			}
		}
		public string image
		{
			set
			{
				Background = BrushFromImage(value);
			}
		}
		public BuildingRect()
		{
			Width = 64;
			Height = 64;
		}
	}

}
