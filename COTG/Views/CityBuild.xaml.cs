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
using Windows.UI.Xaml.Controls;
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
using Microsoft.Toolkit.HighPerformance.Extensions;
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236
using static COTG.Draw.CityView;

namespace COTG.Views
{
	public sealed partial class CityBuild : UserControl
	{
		public static int quickBuildId;
		public static CityBuild instance;
		public static bool isLayout;
	
		
		public static HashSet<ushort> outerTowerSpots =new HashSet<ushort>(new ushort[] {3, 7, 13, 17, 83, 167, 293, 377, 437, 433, 427, 423, 357, 273, 147, 63} );
		public static HashSet<ushort> innerTowerSpots = new HashSet<ushort>(new ushort[] { 113, 117, 173, 257, 323, 327, 183, 267 });
		public static HashSet<ushort> wallSpots = new HashSet<ushort>(new ushort[] { 1, 2, 4, 5, 6, 8, 9, 10, 11, 12, 14, 15, 16, 18, 19, 20, 21, 22, 23, 31, 39, 40, 41, 42, 43, 52, 61, 62, 73, 84, 94, 104, 105, 112, 114, 115, 116, 118, 125, 126, 132, 133, 139, 140, 146, 152, 153, 161, 162, 168, 188, 189, 194, 204, 209, 210, 211, 212, 213, 214, 215, 225, 226, 227, 228, 229, 230, 231, 236, 246, 251, 252, 272, 278, 279, 287, 288, 294, 300, 301, 307, 308, 314, 315, 322, 324, 325, 326, 328, 335, 336, 346, 356, 367, 378, 379, 388, 397, 398, 399, 400, 401, 409, 417, 418, 419, 420, 421, 422, 424, 425, 426, 428, 429, 430, 431, 432, 434, 435, 436, 438, 439, 440 });

		public static HashSet<ushort> shoreSpots = new HashSet<ushort>(new ushort[] {416,394,372,351,331,332,376,354});
		public static HashSet<ushort> waterSpots = new HashSet<ushort>(new ushort[] { 352, 353, 373, 374, 375, 395,396,397,
																			417,418});

		public static bool IsBuildingSpot(int spot) => !(outerTowerSpots.Contains((ushort)spot) | innerTowerSpots.Contains((ushort)spot) | wallSpots.Contains((ushort)spot));
		public static bool IsBuildingSpot((int x, int y) cc) => IsBuildingSpot(XYToId(cc));
		public static bool IsTowerSpot(int spot) => outerTowerSpots.Contains((ushort)spot) | innerTowerSpots.Contains((ushort)spot);
		public static bool IsTowerSpot((int x, int y) cc) => IsTowerSpot(XYToId(cc));
		public static bool IsWallSpot(int spot) => wallSpots.Contains((ushort)spot);
		public static bool IsWallSpot((int x, int y) cc) => wallSpots.Contains((ushort)XYToId(cc));
		public static bool IsWaterSpot(int spot) => City.GetBuild().isOnWater && waterSpots.Contains((ushort)spot);

		public static bool IsShoreSpot(int spot) => City.GetBuild().isOnWater && shoreSpots.Contains((ushort)spot);

		public enum Action
		{
			none,
			move,
			destroy,
			build,
			layout,
			pending,
			upgrade,
			downgrade,
			abandon,
			flipLayoutV,
			flipLayoutH,
			invalid,
			count = invalid,

		};
		public static Action action;
		public static Action singleClickAction; // set on left click tool select
		public static void ClearAction()
		{
			SetAction(Action.none);
			singleClickAction = Action.none;
		}
		public static void SetAction(Action _action)
		{
			action = _action;
			//	App.DispatchOnUIThreadSneaky( ()=> instance.quickBuild.SelectedIndex = (int)_action ); /// the first 3 are mapped. this triggers a selected changed event
		}
		public static void SetQuickBuild(int quickBuildItemBid)
		{
			action = Action.build;
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

		static ActionInfo actionSelect = new ActionInfo("None", Action.none, "City/decal_select_building.png", "Left click opens a menu");
		static ActionInfo actionMove = new ActionInfo("Move", Action.move, "City/decal_move_building.png", "In this mode you first click a building, then click empty space, then click the next buildin to move, etc.");
		static ActionInfo actionDemo = new ActionInfo("Demo", Action.destroy, "City/decal_building_invalid.png", "Destroy anything you click");
		static ActionInfo actionLayout = new ActionInfo("Layout", Action.layout, "City/decal_building_valid_multi.png", "Smart build based on city layouts");
		static ActionInfo actionUpgrade = new ActionInfo("Upgrade", Action.upgrade, "City/decal_building_valid.png", "Upgrade buildings");
		static ActionInfo actionBuild = new ActionInfo("Build", Action.upgrade, "City/decal_building_valid.png", "Buidl this");
		static ActionInfo actionDowngrade = new ActionInfo("Downgrade", Action.downgrade, "City/decal_building_invalid.png", "Downgrade buildings");
		static ActionInfo actionAbandon = new ActionInfo("Abandon", Action.abandon, "City/decal_building_invalid.png", "Abandon this city");
		static ActionInfo actionFlipLayoutH = new ActionInfo("Flip H", Action.flipLayoutH, "City/decal_select_building.png", "Flip Layout Horizontally");
		static ActionInfo actionFlipLayoutV = new ActionInfo("Flip V", Action.flipLayoutV, "City/decal_select_building.png", "Flip Layout Vertically");
		static RadialMenuItem itemQB;

		static Style menuBackground;
		static Style menuBackgroundQuick;


		public static BuildMenuItem Item(RadRadialMenu buildMenu, int id) => (BuildMenuItem)buildMenu.Items[id];
		public static void UpdateBuildMenuType(MenuType _menuType,int bspot)
		{
			//	if (menuType == _menuType)
			//		return;
			var buildMenu = ShellPage.instance.buildMenu;
			if ((menuType == MenuType.quickBuild) != (_menuType == MenuType.quickBuild))
			{
				if (_menuType == MenuType.quickBuild)
				{
					buildMenu.ContentMenuBackgroundStyle = menuBackgroundQuick;
				}
				else
				{
					buildMenu.ContentMenuBackgroundStyle = menuBackground;

				}
			}
			menuType = _menuType;
			


			switch (menuType)
			{
				case MenuType.quickBuild:
					Item(buildMenu, 0).SetAction(actionSelect);
					Item(buildMenu, 1).SetAction(actionMove);
					Item(buildMenu, 2).SetAction(actionDemo);
					Item(buildMenu, 3).SetAction(actionLayout);
					for (int i = 0; i < qbMRUSize; ++i)
					{
						Item(buildMenu, 4 + i).SetBid(buildingMru[i].bid);
					}



					break;
				case MenuType.buliding:
					Item(buildMenu, 1).SetAction(actionMove);
					Item(buildMenu, 2).SetAction(actionDemo);
					Item(buildMenu, 3).SetAction(actionLayout);
					if( GetBuild().buildings[bspot].bl == 0 )
						Item(buildMenu, 4).SetAction(actionBuild);
					else
						Item(buildMenu, 4).SetAction(actionUpgrade);
					Item(buildMenu, 5).SetAction(actionDowngrade);
					Item(buildMenu, 6).Clear();
					Item(buildMenu, 7).Clear();
					Item(buildMenu, 0).Clear();

					break;
				case MenuType.townhall:
					Item(buildMenu, 1).SetAction(actionMove);
					Item(buildMenu, 2).SetAction(actionAbandon);
					Item(buildMenu, 3).SetAction(actionLayout);
					Item(buildMenu, 4).SetAction(actionUpgrade);
					Item(buildMenu, 5).SetAction(actionDowngrade);
					Item(buildMenu, 6).SetAction(actionFlipLayoutH);
					Item(buildMenu, 7).SetAction(actionFlipLayoutV);
					Item(buildMenu, 0).Clear();
					break;
				case MenuType.empty:
					Item(buildMenu, 0).SetAction(actionLayout);
					for (int i = 0; i < 7; ++i)
						Item(buildMenu, i + 1).SetBid(buildingMru[i].bid);



					break;
				case MenuType.tower:
					Item(buildMenu, 0).SetBid(bidSentinelPost);
					Item(buildMenu, 1).SetBid(bidRangerPost);
					Item(buildMenu, 2).SetBid(bidTriariPost);
					Item(buildMenu, 3).SetBid(bidPriestessPost);
					Item(buildMenu, 4).SetBid(bidBallistaPost);
					Item(buildMenu, 5).SetBid(bidEquineBarricade);
					Item(buildMenu, 6).SetBid(bidRuneBarricade);
					Item(buildMenu, 7).SetBid(bidVeiledBarricade);
					break;
				case MenuType.shore:
					Item(buildMenu, 0).SetBid(bidPort);
					Item(buildMenu, 1).SetBid(bidShipyard);
					Item(buildMenu, 2).Clear();
					Item(buildMenu, 3).Clear();
					Item(buildMenu, 4).Clear();
					Item(buildMenu, 5).Clear();
					Item(buildMenu, 6).Clear();
					Item(buildMenu, 7).Clear();
					break;

				case MenuType.res:
					Item(buildMenu, 0).SetAction(actionDemo);
					for (int i = 0; i < 7; ++i)
						Item(buildMenu, i + 1).SetBid(buildingMru[i].bid);


					break;

			}
			var townHallLevel = GetBuild().buildings[City.bspotTownHall].bl;
			for(int i=0;i<8;++i)
			{
				var item = buildMenu.Items[i];
				if(item is BuildMenuItem bi)
				{
					if(bi.isAction)
					{
						bi.SetEnabled(true);
					}
					else if( bi.isBuilding)
					{
						var def = BuildingDef.all[bi.bid];
						var enabled = true;
						if(def.Thl > townHallLevel)
						{
							enabled = false;
						}
						else if(_menuType == MenuType.empty)
						{
							enabled = true; // todo

						}
						bi.SetEnabled(enabled);
					}
					else
					{
						bi.SetEnabled(false);
					}

				}
				
			}
		}


		public static CityBuild Initialize(RadRadialMenu buildMenu, Canvas buildMenuCanvas)
		{
			instance = new CityBuild();

			{

				itemQB = new RadialMenuItem() { Header = "QuickBuild" };

				for (int i = 0; i < 8; ++i)
					buildMenu.Items.Add(new BuildMenuItem());
				buildMenu.Items.Add(itemQB);
				// Mru

				buildingMru[0] = new BuildingMRU(bidCottage);
				buildingMru[1] = new BuildingMRU(bidStorehouse);
				buildingMru[2] = new BuildingMRU(bidMarketplace);
				buildingMru[3] = new BuildingMRU(bidMage_tower);
				buildingMru[4] = new BuildingMRU(bidCastle);
				buildingMru[5] = new BuildingMRU(bidBarracks);
				buildingMru[6] = new BuildingMRU(bidTrainingground);
				// quick build
				itemQB.ChildItems.Add(new BuildMenuGroup("Misc", 446, 464, 449, 481, 467, 488, 479, bidTemple));
				itemQB.ChildItems.Add(new BuildMenuGroup("Military", 445, 500, 483, 466, 491, 482, 502, 504));
				itemQB.ChildItems.Add(new BuildMenuGroup("Posts", 547, 539, 543, 551, 555));
				itemQB.ChildItems.Add(new BuildMenuGroup("Barricade", 559, 563, 567, 571));
				itemQB.ChildItems.Add(new BuildMenuGroup("Res", 447, 448, 460, 461, 462, 463, 465, 477));


				buildMenu.isOpenChanged = async (open) =>
				{
					if (!open)
					{
						await Task.Delay(450);
						App.DispatchOnUIThreadSneaky( ()=>buildMenuCanvas.Visibility = Visibility.Collapsed );
					
						menuOpen = false;
					}
					else
					{
						buildMenuCanvas.Visibility = Visibility.Visible;
						menuOpen = true;
					}
				};
			}
			// individual menu
			{

			}
			menuBackground = buildMenu.ContentMenuBackgroundStyle;
			menuBackgroundQuick = buildMenu.ContentMenuBackgroundQuickStyle;
			return instance;

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

		public static bool _isLayout
		{
			get => isLayout;
			 set
			{
				if (isLayout == value)
					return;
				isLayout = value;
				if (isLayout)
				{

					var build = GetBuild();
					if (build.layout != null)
					{
						build.LayoutToBuildings();
						
					}

				}
				else
				{
					City.GetBuild().SaveBuildingsToLayout();
					GetCity.Post(City.build);

				}
			}
		}


		internal static void ClearQueue()
		{
			BuildQueueHelper.ClearQueue();
			JSClient.view.InvokeScriptAsync("cancelbuilds", Array.Empty<string>() );
		}

		public static void Enqueue( int slvl, int elvl, int bid, int spot)
		{
			BuildQueueHelper.Enqueue(City.build, (byte)slvl, (byte)elvl, (ushort)bid, (ushort)spot);
		}
		
		private void Upgrade_Click(object sender, RoutedEventArgs e)
		{

			var id = XYToId(selected);
			var sel = GetBuildingPostQueue(id);
			var lvl = sel.bl;
			
			if(lvl == 0)// special case
				Build(id,sel.def.bid,false);
			else
				Enqueue(lvl,(lvl + 1),sel.def.bid,id);
		
		}
		public static void UpgradeToLevel(int level, (int x, int y) target, bool dryRun=false)
		{
			//var target = hovered;
			if (!target.IsValid())
			{
				Status("Please select a building", dryRun);
				return;
			}
			var id = XYToId(target);
			var sel = GetBuildingPostQueue(id);
			var lvl = sel.bl;
			if (level == 1)
				level = lvl + 1;

			if (lvl == 0)
			{
				Build(id, sel.def.bid, dryRun);
				lvl = 1;
			}
			if (lvl < level)
			{

				if(isLayout)
				{
					GetBuild().buildings[id].bl =(byte) level;

				}
				else if (!dryRun)
				{
					Enqueue(lvl, level, sel.def.bid, id);
				}
			}			
		}

		public static void Demolish((int x, int y) building, bool dryRun)
		{
			Demolish(XYToId(building), dryRun);
		}

		public static Building GetBuildingPostQueue(int spot)
		{
			var b = GetBuilding(spot);
			var wasBuilding = b.isBuilding;
			// its a struct so we can mutate it
			IterateQueue((q) =>
		   {
			   if (q.bspot == spot)
			   {
				   b.bl = q.elvl;
				   b.id = BuildingDef.BidToId(q.bid);
				   if (b.bl == 0 && wasBuilding)
					   b.id = 0;
			   }
		   });
			return b;

		}

		public static Building[] GetPostQueueBuildings()
		{
			if (!postQueueBuildingsDirty)
				return postQueuebuildingsCache;
			postQueueBuildingsDirty = false;
			for(var i =0;i<citySpotCount;++i)
			{
				postQueuebuildingsCache.DangerousGetReferenceAt(i) = buildingsCache.DangerousGetReferenceAt(i);
			}
			IterateQueue((q) =>
			{
				ref var b = ref postQueuebuildingsCache.DangerousGetReferenceAt(q.bspot);
				b.bl = q.elvl;
				b.id = BuildingDef.BidToId(q.bid);
			});


			return postQueuebuildingsCache;
		}

		public static void Demolish(int id, bool dryRun)
		{
			var sel = GetBuildingPostQueue(id);
			if (sel.isEmpty)
			{
				Status("Already destoryed", dryRun);
			}
			else
			{
				Status($"Destory {sel.def.Bn}", dryRun);
				if (!dryRun)
				{
					if (isLayout)
					{
						var build = GetBuild();
						build.buildings[id].SetBid(0, 0);
					}
					else
					{
						Enqueue(sel.bl, 0, sel.def.bid, id);
					}
				}
				else
				{
					DrawSprite(IdToXY(id), decalBuildingInvalid, 0.312f);
				}
			}
		}
		public static void Downgrade((int x, int y) building, bool dryRun)
		{
			var id = XYToId(building);
			var sel = GetBuildingPostQueue(id);

			if (sel.bl == 1)
			{
				Demolish(id, dryRun);
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
					if (isLayout)
					{
						--GetBuild().buildings[id].bl;
					}
					else
					{
						Enqueue(sel.bl, sel.bl - 1, sel.def.bid, id);
					}
				}
			}
			//if(!dryRun)
			//	buildQueue.Add(new BuildQueueItem() { bspot = id, bid = sel.def.bid, slvl = sel.bl, elvl = (byte)(sel.bl-1) });
		}
		public static void Build(int id, int bid, bool dryRun)
		{
			var sel = GetBuildingPostQueue(id);
			if (bid != bidWall && !sel.isEmpty) // special case, wall upgrade from level is allowed as a synanom for build
			{
				Status("Spot is occupied",dryRun);
			}
			else
			{
				var buildDef = BuildingDef.all[bid];
				if( IsWallSpot(id))
				{
					Status("Walls go here", dryRun);
					return;
				}
				if( IsWaterSpot(id))
				{
					Status("There is water here :(", dryRun);
					return;
				}
				if ( IsTowerSpot(id))
				{
					if(City.GetBuild().buildings[bspotWall].bl == 0)
					{
						Status("Please build a wall first", dryRun);
						return;
					}
					if(!buildDef.isTower)
					{
						Status("This looks like a nice place for a tower.", dryRun);
						return;
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
						if(!buildDef.isShoreBuilding)
						{
							Status("Ports and Shipyards go here", dryRun);
							return;
						}
					}
					else
					{
						if(buildDef.isShoreBuilding)
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
					if (isLayout)
					{
						var build = GetBuild();
						build.buildings[id].SetBid(bid, 10);
				
					}
					else
					{

						Enqueue(0, 1, bid, id);
					}
				}
			}
		}
		public static void Build((int x, int y) cc, int bid, bool dryRun)
		{
			Build(XYToId(cc), bid, dryRun);
		}


		private void Downgrade_Click(object sender, RoutedEventArgs e)
		{
			Downgrade(selected,false);
		}

		private void Destroy_Click(object sender, RoutedEventArgs e)
		{
			Demolish(selected,false);
		}
		public static ImageBrush BrushFromAtlas(string name, int x, int y, float scale)
		{

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
			return brush;
		}
		public static ImageBrush BrushFromImage(string name)
		{

			var bitmap = ImageHelper.FromImages(name);
			var brush = new ImageBrush()
			{
				ImageSource = bitmap,
				Stretch = Stretch.Fill,
			};
			//	rect.Stretch = Stretch.None;
			//			rect.Width = width;
			//			rect.Height = height;
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

		public static void MoveBuilding(int a, int b)
		{
			// Todo: Error checking
			var build = City.GetBuild();
			if (!isLayout)
			{
				Services.Post.Send("includes/mBu.php", $"a={a}&b={b}&c={Spot.build}", World.CidToPlayer(City.build));

			}
			ref var b1 = ref build.buildings[a];
			ref var b0 = ref build.buildings[b];
			// I hope that these operations are what I expect with references
			var temp = b0;
			b0 = b1;
			b1 = temp;
		}
		public static async void SwapBuilding(int a, int b)
		{
			// Todo: Error checking
			var build = City.GetBuild();
			// I hope that these operations are what I expect with references
			var temp = build.buildings[b];
			build.buildings[b] = build.buildings[a];
			build.buildings[a] = temp;
			if (!isLayout)
			{
				await Services.Post.Send("includes/mBu.php", $"a={a}&b={cityScratchSpot}&c={Spot.build}", World.CidToPlayer(City.build));
				await Services.Post.Send("includes/mBu.php", $"a={b}&b={a}&c={Spot.build}", World.CidToPlayer(City.build));
				await Services.Post.Send("includes/mBu.php", $"a={cityScratchSpot}&b={b}&c={Spot.build}", World.CidToPlayer(City.build));
			}
		}

		internal static void MoveHovered(bool isSingleAction, bool dryRun)
		{
			int bspot = XYToId(hovered);
			var build = GetBuild();
			var b = build.buildings[bspot];
			if (CanvasHelpers.IsValid(selected) )
			{
				if (b.id != 0 && b.bl ==0)
				{
					Status("Please select an empty spot", dryRun);
				}
				else
				{
					var source = XYToId(selected);


					// Is this a valid transition
					var bs1 = IsTowerSpot(bspot) ? 1 : IsWallSpot(bspot) ? 0 : 2;
					if(bs1 ==0  && !Player.isAvatarOrTest )
					{
						Status("Please don't put buildings on walls.", dryRun);
						return;
					}
					var bs0 = IsTowerSpot(selected) ? 1 : 2;
					if (bs0 != bs1)
					{
						Status("Cannot move between building and wall spots", dryRun);
						return;
					}

					if (isSingleAction && !dryRun)
					{
						singleClickAction = Action.none;
						action = Action.none;
					}

					if (dryRun)
					{
						DrawSprite(hovered, decalMoveBuilding, 0.343f);
						DrawSprite(selected, decalMoveBuilding, 0.323f);

					}
					else
					{

						if (b.bl > 0)
						{
							SwapBuilding(source, bspot);
						}
						else
						{
							MoveBuilding(source, bspot);
						}
						ClearSelectedBuilding();
					}
				}
			}
			else
			{
				if (b.bl <= 0)
				{
					Status("Please select a building", dryRun);
				}
				else
				{
					Status($"Select building {b.def.Bn} at {hovered.bspotToString()}" );
					if (!dryRun)
					{
						selected = hovered;
						if (isSingleAction)
						{
							singleClickAction = Action.pending;
							action = Action.move;
						}
					}
				}
			}
		}




		private void quickBuild_SelectionChanged(object sender, SelectionChangedEventArgs e)
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

		public static void PerformAction(Action action, (int x, int y) cc, int _quickBuildId, bool dryRun)
		{
		

			int bspot = XYToId(cc);
			var build = GetBuild();
			var b = build.buildings[bspot];


			switch (action)
			{
				case Action.layout:
					{
						var layout = build.layout;
						if (build.layout == null)
						{
							Status("Please assign a layout",dryRun);
						}
						else
						{
							var desBid = build.BidFromOverlay(bspot);
							var desName = BuildingDef.all[desBid].Bn;
							var curBid = b.def.bid;
							var takeFrom = -1;
							var takeScore = 0;
							var putTo = -1;
							var putScore = 0;
							// See if there are spare buildings that we can take
							for (int xy = 0; xy < City.citySpotCount; ++xy)
							{
								var overlayBid = build.BidFromOverlay(xy);
								var xyBuilding = build.buildings[xy].def.bid;

								if (overlayBid != xyBuilding)
								{
									// do they have what we need?
									if (xyBuilding == desBid)
									{
										// two points for ourbuilding is also needed there
										var score = (overlayBid == curBid) ? 2 : 1;
										if (score > takeScore)
										{
											takeScore = score;
											takeFrom = xy;
										}
									}
									//do they want what we have?
									if (overlayBid == curBid)
									{
										var score = (xyBuilding == desBid) ? 4 : (xyBuilding == 0) ? 3 : build.buildings[xy].isBuilding ? 2 : 1;
										if (score > putScore)
										{
											putScore = score;
											putTo = xy;

										}

									}
								}
							}


							// case 1:  nothing here, add building
							if (b.id == 0)
							{
								var counts = CountBuildings();
								// see if we can re-use one
								if (desBid != 0)
								{
									if (takeScore > 0)
									{
										Status($"Found an unneeded {desName}, will move it to the right spot for you", dryRun);
										if (!dryRun)
										{
											MoveBuilding(takeFrom, bspot);
										}
										else
										{
											DrawSprite(cc, decalMoveBuilding, 0.343f);
											DrawSprite(selected, decalMoveBuilding, 0.323f);
										}
										break;
									}
									if (counts.count >= counts.max)
									{
										if (buildQueue.count <= buildQMax - 2)
										{
											// Is there a cabin to remove?
											var buildings = build.buildings;
											var bestSpot = -1;
											int bestLevel = int.MaxValue;
											for (int spot = 0; spot < citySpotCount; ++spot)
											{
												var bld = buildings[spot];
												if (bld.def.bid == bidCottage)
												{
													if (bld.bl < bestLevel)
													{
														// is it not being modified?
														if (IterateQueue().Any(a => a.bspot == spot))
															continue;

														bestLevel = bld.bl;
														bestSpot = spot;
													}
												}
											}
											if(bestSpot != -1)
											{
												Status("Will Demolish a Cottage to make room", dryRun);
												
												Demolish(bestSpot,dryRun);
												break;

											}
										}


									}

								}
								else
								{
									if(counts.count >= counts.max)
									{
										Status("City is full",dryRun);
										break;
									}
								}
								{
									//if (buildQueueFull)
									//{
									//	Status("Build Queue full", dryRun);
									//	break;
									//}

									if (desBid == 0)
										Status($"No building is wanted here, how about a cottage instead?", dryRun);
									else
										Status($"Build a {desName}", dryRun);

									
									
									Build(cc, desBid == 0 ? bidCottage : desBid,dryRun);
									
								}
							}
							else if (b.isRes)
							{
								if (desBid != 0)
								{
									//if (buildQueueFull)
									//{
									//	Status("Build Queue full",dryRun);
									//	break;
									//}
									//else
									{
										Status($"Destorying {b.def.Bn} to make way for {desName}",dryRun);
									}
									Demolish(cc,dryRun);
									
									// Test!
									//JSClient.view.InvokeScriptAsync("buildop", new[] { (desBid == 0 ? bidCottage.ToString() : desBid.ToString()), bspot.ToString(), "0" });
								}
							}
							else
							{
								// building is here
								// a building
								// Try to move it to some place where one is needed
								var buildingWanted = false;
								if (desBid != curBid)
								{
									if (putScore > 0)
									{
										var name = b.def.Bn;
										buildingWanted = true;
										switch (putScore)
										{
											case 4:
												{
													Status($"Swaping {b.def.Bn} and {desName} as they are mixed up", dryRun);
													if (dryRun)
													{
														DrawSprite(hovered, decalMoveBuilding, 0.343f);
														DrawSprite(IdToXY(putTo), decalMoveBuilding, 0.323f);

													}
													else
													{
														SwapBuilding(bspot, putTo);
													}
													// two way swap 
													break;
												}
											case 3:
												Status($"Move {name} to where it is wanted", dryRun);
												if (dryRun)
												{
													DrawSprite(hovered, decalMoveBuilding, 0.343f);
													DrawSprite(IdToXY(putTo), decalMoveBuilding, 0.323f);
												}
												else
													MoveBuilding(bspot, putTo);
												break;
											case 2:
												Status($"{name} is wanted elsewhere but there is a building in the way", dryRun);
												break;
											case 1:
												Status($"{name} is wanted elsewhere but there are reources in the way", dryRun);
												break;


										}


									}

									if (!buildingWanted)
									{
										Status($"{b.def.Bn} is not wanted, destroying it", dryRun);
										if(dryRun)
											DrawSprite(hovered, decalBuildingInvalid, .31f);
										else
											Demolish(cc, dryRun);
									}
								}
								else
								{
									Status($"{desName} is in the right spot, no changed needed", dryRun);

								}
							}

						}
						break;
					}
				case Action.build:
					{
						if(b.isRes && !IterateQueue().Any(a => a.bspot == bspot))
						{
							Status("Spot is not empty", dryRun);
							break;
						}
						//if (buildQueueFull)
						//{
						//	Status("Build Queue full", dryRun);
						//	break;
						//}

						var sel = _quickBuildId;

						if (sel != 0)
						{
							Build(bspot,sel,dryRun);
							
							break;
						}
						Status("Please select a valid building");
						break;
					}
				case Action.destroy:
					{
						//if (buildQueueFull)
						//{
						//	Status("Build Queue full", dryRun);
						//	break;
						//}
						Demolish(cc, dryRun);

						
						break;
					}
				case Action.move:
					{
						MoveHovered(singleClickAction == Action.pending, dryRun);
						break;
					}
				case Action.downgrade:
					{
						Downgrade(cc,dryRun);
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
							GetBuild().FlipLayoutH();
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
							if (IsBuildingSpot(hovered))
							{
								ShellPage.contToolTip = $"Left click to build something\nRight click to select a quick build tool";

							}
							else if (IsTowerSpot(hovered))
							{
								ShellPage.contToolTip = $"Left click to build tower\nRight click to select a quick build tool";

							}
							else if (IsWallSpot(hovered))
							{
								ShellPage.contToolTip = $"Left click to build wall\nRight click to select a quick build tool";
							}
							else
							{
								ShellPage.contToolTip = $"Please don't left click here\nRight click to select a quick build tool";
							}
					}
					else 
					{
						ShellPage.contToolTip = $"Left click modify {b.def.Bn}, Right click to select a quick build tool";
					}
					
					break;
				}
			}
		}

		internal static void ShortBuild(short bid)
		{
			PerformAction(CityBuild.Action.build, hovered, bid,false);

		}

		public static void PointerDown((int x, int y) cc)
		{
			//  called before pointer release, pointer release is a click or drag/swaipe
			hovered = cc;

		}

		public static void PreviewBuildAction()
		{
			if(hovered.IsValid())
				PerformAction(action, hovered, quickBuildId, true);

		}

		public static async void Click((int x, int y) cc, bool isRight)
		{
			if (CityBuild.menuOpen)
			{
				if (ShellPage.instance.buildMenu.IsOpen)
				{
					ShellPage.instance.buildMenu.IsOpen = false;
					//				ShellPage.instance.buildMenuCanvas.Visibility = Visibility.Collapsed;
					//	Assert(false);
				//	return;
				}
			}
			while( CityBuild.menuOpen )
			{
				await Task.Delay(200).ConfigureAwait(true);
			}

			int bspot = XYToId(cc);
			var build = GetBuild();
			var b = build.buildings[bspot];

			if (!isRight && action != Action.none)
			{
				PerformAction(action, cc, quickBuildId, false);
				if (singleClickAction == Action.pending)
				{
					action = Action.none;
					singleClickAction = Action.none;
				}
				Assert(singleClickAction == Action.none);
			}
			else
			{
				action = Action.none;

				singleClickAction = Action.none; // default
												 // toggle visibility

				//	var i = instance;
				//i.building.Text = d.Bn;
				//i.description.Text = d.Ds;
				//i.upgrade.IsEnabled = d.Bc.Count() > b.bl && b.isBuilding;
				//i.downgrade.IsEnabled = b.bl > 1;
				//i.rect.Fill = BuildingBrush(d.bid, 1.0f);
				if (!isRight)
				{
					if (IsWallSpot(bspot))
					{
						Note.Show("There is water here. :(");
						return;
					} 
					singleClickAction = Action.pending;
					if(CityBuild.IsWallSpot(bspot))
					{
						bspot = 0;
						cc = (span0, span0);
						b = build.buildings[bspot];
						
					}
				}
				var d = b.def;
				if (d.bid!=0)
					JSClient.view.InvokeScriptAsync("exBuildingInfo", new[] { d.bid.ToString(), b.bl.ToString(), bspot.ToString() });
				{
					selected = cc;
					var type = isRight ? MenuType.quickBuild :
						CityBuild.IsTowerSpot(bspot) ? MenuType.tower :
						CityBuild.IsShoreSpot(bspot) ? MenuType.shore :
						bspot == 0 ? MenuType.buliding :
						b.id == 0 ? MenuType.empty :
						b.bl == 0 ? MenuType.res :
						d.bid == bidTownHall ? MenuType.townhall :
						MenuType.buliding;
					UpdateBuildMenuType(type, bspot);

					//				ShellPage.instance.buildMenu.IsOpen = true;
					var sc = ShellPage.CanvasToScreen(ShellPage.mousePosition);
					var bm = ShellPage.instance.buildMenu;
					Canvas.SetLeft(bm, sc.X - buildToolSpan / 2 - 1);
					Canvas.SetTop(bm, sc.Y - buildToolSpan / 2 + 41);
					//		ShellPage.instance.buildMenuCanvas.Visibility = Visibility.Visible;
					//bm.ContentMenuBackgroundStyle = new Style( typeof(Rectangle) ) {  (Style)Application.Current.Resources[isRight? "ContentMenuStyle" : "ContentMenu2Style"];
					
					bm.IsOpen = true;

				}
			}
		}
		public const int buildToolSpan = 448;



		private async void Abandon_Click(object sender, RoutedEventArgs e)
		{
			var dialog = new ContentDialog()
			{
				Title = "Are you Sure?",
				Content = "Abandon " + City.GetBuild().nameAndRemarks,
				PrimaryButtonText = "Yes",
				SecondaryButtonText = "Cancel"
			};
			if (await dialog.ShowAsync2().ConfigureAwait(true) == ContentDialogResult.Primary)
			{
				  await JSClient.view.InvokeScriptAsync("misccommand", new[] { "abandoncity", City.build.ToString() });

				await Task.Delay(500);
				NavStack.Back(true);
			}
		}
	}

	public class BuildMenuItem : RadialMenuItem
	{
		public int bid;
		public bool isAction => action != Action.invalid;
		public bool isBuilding => bid != 0;
		public BuildMenuItem() {
			IconContent = new BuildingRect() { Width = 64, Height = 64, image = ("sphere64.png") }; ;
			Command = BuildMenuItemCommand.instance;
			Clear();
		} 
		public BuildMenuItem SetBid(int _bid)
		{
			if (bid != _bid)
			{
				bid = _bid;
				var def = BuildingDef.all[_bid];
				Header = def.Bn;
				ToolTipContent = def.Ds;
				((BuildingRect)IconContent).bid = _bid;
				action = Action.invalid;
				IsSelected = false;
			//	Command = BuildMenuItemCommand.instance;
			}
			return this;
		}
		public CityBuild.Action action = Action.invalid;
		public void SetAction(ActionInfo action)
		{

			if (!(Header is string s && s == action.name))
			{
				bid = 0;
				ToolTipContent = action.toolTip;
				Header = action.name;
				this.action = action.action;
				((BuildingRect)IconContent).image=(action.icon);
				IsSelected = false;
				//	Command = BuildMenuItemCommand.instance;
			}
		}
		public void Clear()
		{
			if (action != Action.invalid || bid != 0)
			{
				action = Action.invalid;
				bid = 0;
				Header = "~";
				ToolTipContent = "This space is intentionally blank.";
				((BuildingRect)IconContent).image = "sphere64.png";
				IsSelected = false;
			}


		}

		internal void SetEnabled(bool v)
		{
			if(v != IsEnabled)
			{
				IsEnabled = v;
			}
		}
	}
	public class BuildMenuGroup : RadialMenuItem
	{
		public BuildMenuGroup(string name, params int[] ids)
		{
			Header = name;
			//		GroupName = "actions";
			IconContent = new BuildingRect() { Width = 64, Height = 64, bid = ids[0] };
			foreach (var i in ids)
				ChildItems.Add(new BuildMenuItem().SetBid(i));
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
	public class BuildingButton : Button
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
	public class BuildingRect : Canvas
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
	public class BuildMenuItemCommand : ICommand
	{
		public static BuildMenuItemCommand instance = new BuildMenuItemCommand();
		public bool CanExecute(object parameter)
		{
			var item = parameter as RadialMenuItemContext;

			// perform custom logic here

			return true;
		}

		public void Execute(object parameter)
		{
			var context = parameter as RadialMenuItemContext;
			var target = context.TargetElement;
			var bi = context.MenuItem as BuildMenuItem;
			if (bi != null)
			{
				if (bi.isBuilding)
				{
					if (singleClickAction == CityBuild.Action.pending)
					{
						PerformAction(CityBuild.Action.build, selected, bi.bid,false);
					}
					else
					{
						SetQuickBuild(bi.bid);
					}
					var items = ShellPage.instance.buildMenu.Items;
					BuildingMRU found = null;
					BuildingMRU lowest = null;
					float lowestScore = float.MaxValue;
					for (int i = 0; i < buildMenuRootItems; ++i)
					{
						var item = items[i];
						item.IsSelected = false;
					}
					for (int i = 0; i < buildingMru.Length; ++i)
					{
						var item = buildingMru[i];
						item.cacheScore *= 0.75f;
						if (item.bid == bi.bid)
						{
							found = item;
						}
						else
						{
							if (item.cacheScore < lowestScore)
							{
								lowestScore = item.cacheScore;
								lowest = item;
							}
						}
					}
					if (found == null)
					{
						found = lowest;
						found.bid = (bi.bid);

					}
					found.cacheScore += 1.0f;
					//Array.Sort(buildingMru, (a, b) => a.cacheScore.CompareTo(b.cacheScore) );
					//if (menuType == MenuType.quickBuild)
					{

						int lowestId = -1;
						lowestScore = float.MaxValue;
						for (int put = 0; put < buildMenuRootItems; ++put)
						{
							var i = items[put] as BuildMenuItem;
							i.IsSelected = false;
							if (i == null || !i.isBuilding)
								continue;
							if (i.bid == found.bid)
							{
								i.IsSelected = true;
								lowestId = -1;
								lowestScore = -float.MaxValue;
							}
							var entry = (from a in buildingMru where a.bid == i.bid select a.cacheScore).FirstOrDefault();
							if (entry < lowestScore)
							{
								lowestScore = entry;
								lowestId = put;

							}
						}
						if (lowestId != -1)
						{
							(items[lowestId] as BuildMenuItem).SetBid(bi.bid);
						}
						ClearSelectedBuilding();


					}
				}
				else if (bi.isAction)
				{
					var items = ShellPage.instance.buildMenu.Items;
					if (bi.action == Action.layout && City.GetBuild().layout == null)
					{
						Note.Show("Please assign a layout");
					}
					else
					{
						for (int i = 0; i < buildMenuRootItems; ++i)
						{
							var item = items[i];
							item.IsSelected = (item == context.MenuItem);
						}
						if (singleClickAction == CityBuild.Action.pending)
						{
							if (bi.action == CityBuild.Action.move)
							{

								SetAction(CityBuild.Action.move);
								// leave action pending
							}
							else
							{
								PerformAction(bi.action, selected, 0,false);
								singleClickAction = CityBuild.Action.none;
								ClearSelectedBuilding();
							}
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
					ClearSelectedBuilding();
				}
			}
			else
			{
				Assert(false);
			}
			//		var def = context.CommandParameter as BuildingDef;


			// perform custom logic here
			context.MenuItem.Owner.Owner.IsOpen = false;

		}

		public event EventHandler CanExecuteChanged;
	}
}
