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
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
	public sealed partial class CityBuild : UserControl
	{
		public static int quickBuildId;
		public static CityBuild instance;
		public enum Action
		{
			none,
			move,
			destroy,
			build,
			layout,
			pending,
			count,

		};
		public static Action action;
		public static Action singleClickAction; // set on left click tool select
		public static void ClearAction() 
		{
			SetAction( Action.none);
			singleClickAction = Action.none;
		}
		public static void SetAction(Action _action )
		{
			action = _action;
		//	App.DispatchOnUIThreadSneaky( ()=> instance.quickBuild.SelectedIndex = (int)_action ); /// the first 3 are mapped. this triggers a selected changed event
		}
		public static void SetQuickBuild( int quickBuildItemBid )
		{
			action = Action.build;
			quickBuildId = quickBuildItemBid;
			//	App.DispatchOnUIThreadSneaky( ()=> instance.quickBuild.SelectedIndex = (int)_action ); /// the first 3 are mapped. this triggers a selected changed event
		}

		//static List<QuickBuildItem> items;
		internal static bool menuOpen;
	

		public static CityBuild Initialize(RadRadialMenu buildMenu,Canvas buildMenuCanvas)
		{
			instance = new CityBuild();

			{

				var qb = new RadialMenuItem() { Header = "QuickBuild" };
				buildMenu.Items.Add(new BuildMenuAction("Select", Action.none, "City/decal_select_building.png"));
				buildMenu.Items.Add(new BuildMenuAction("Move", Action.move, "City/decal_move_building.png"));
				buildMenu.Items.Add(new BuildMenuAction("Demo", Action.destroy, "City/decal_building_invalid.png"));
				buildMenu.Items.Add(new BuildMenuAction("Layout", Action.layout, "City/decal_building_valid_multi.png"));
				// Mru
				buildMenu.Items.Add(new BuildMenuItem(446));
				buildMenu.Items.Add(new BuildMenuItem(464));
				buildMenu.Items.Add(new BuildMenuItem(449));
				buildMenu.Items.Add(new BuildMenuItem(500));
				buildMenu.Items.Add(qb);
				// quick build
				qb.ChildItems.Add(new BuildMenuGroup("Misc", 446, 464, 449, 481, 467, 488, 479));
				qb.ChildItems.Add(new BuildMenuGroup("Military", 445, 500, 483, 466, 491, 482, 502, 504));
				qb.ChildItems.Add(new BuildMenuGroup("Posts", 547, 539, 543, 551, 555));
				qb.ChildItems.Add(new BuildMenuGroup("Barricade", 559, 563, 567, 571));
				qb.ChildItems.Add(new BuildMenuGroup("Res", 447, 448, 460, 461, 462, 463, 465, 477));

				buildMenu.isOpenChanged = async (open) =>
				{
					if (!open)
					{
						await Task.Delay(350).ConfigureAwait(true);
						buildMenuCanvas.Visibility = Visibility.Collapsed;
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

		private void Upgrade_Click(object sender, RoutedEventArgs e)
		{

			var id = XYToId(selected);
			var sel = GetBuilding(id);
			var lvl = sel.bl;
			var def = sel.def;
			// check for queued upgrades
			for(var it = buildQueue.iterate;it.Next();)
			{
				if (it.r.bspot == id)
					lvl = it.r.elvl;
			}
			JSClient.view.InvokeScriptAsync("upgradeBuilding", new[] { (selected.x - span0).ToString(), (selected.y - span0).ToString(), (lvl + 1).ToString() });
			buildQueue.Add(new JSON.BuildQueueItem() { bspot = (ushort)id, slvl = (byte)(lvl), elvl = (byte)(lvl + 1) });
		}
		public static void UpgradeToLevel(int level)
		{
			if(!selected.IsValid())
			{
				Note.Show("Please select a building");
				return;
			}
			var id = XYToId(selected);
			var sel = GetBuilding(id);
			var lvl = sel.bl;
			if (level == 1)
				level = lvl + 1;
			for (var it = buildQueue.iterate; it.Next();)
			{
				if (it.r.bspot == id)
					lvl = it.r.elvl;
			}
			JSClient.view.InvokeScriptAsync("upgradeBuilding", new[] { (selected.x - span0).ToString(), (selected.y - span0).ToString(), (level).ToString() });
			for (; lvl < level; ++lvl)
				buildQueue.Add(new JSON.BuildQueueItem() { bspot = id, slvl = (byte)(lvl), elvl = (byte)(lvl + 1) });
		}

		public static void Demolish((int x, int y) building)
		{
			var id = XYToId(building);
			var sel = GetBuilding(id);
			JSClient.view.InvokeScriptAsync("buildop", new[] { sel.def.bid.ToString(), id.ToString(), "3" }); // op 3 is destroy
		}
		public static void Downgrade((int x, int y) building)
		{
			var id = XYToId(building);
			var sel = GetBuilding(id);
			JSClient.view.InvokeScriptAsync("buildop", new[] { sel.def.bid.ToString(), id.ToString(), "2" }); // op 2 is downgrade
		}

		private void Downgrade_Click(object sender, RoutedEventArgs e)
		{
			Downgrade(selected);
		}

		private void Destroy_Click(object sender, RoutedEventArgs e)
		{
			Demolish(selected);
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
			var v0 = (int)(iconId / atlasColumns) * dvDt;
			var uri = SettingsPage.IsThemeWinter() ? "City/Winter/building_set5.png" :
			"City/building_set5.png";
			return BrushFromAtlas(uri, u0, v0, scale);
		}
		public static void MoveBuilding(int idFrom, int idTo)
		{
			Services.Post.Send("includes/mBu.php", $"a={idFrom}&b={idTo}&c={Spot.build}");


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
		public static void PerformAction(Action action,(int x, int y) cc , int _quickBuildId)
		{
			int bspot = XYToId(cc);
			var build = GetBuild();
			var b = build.buildings[bspot];

			switch (action)
			{
				case Action.build:
					{
						if ((b.id != 0) || buildQueue.Any(a => a.bspot == bspot))
						{
							Note.Show("There is already someting here");
							break;
						}
						if (buildQueueFull)
						{
							Note.Show("Build Queue full");
							break;
						}

						var sel = _quickBuildId;

						if (sel != 0)
						{

							JSClient.view.InvokeScriptAsync("buildop", new[] { sel.ToString(), bspot.ToString(), "0" }); // 0 is build
							buildQueue.Add(new BuildQueueItem() { bspot = bspot, brep = sel, slvl = 0, elvl = 1 });
							break;
						}
						Note.Show("Please select a valid building");
						break;
					}
				case Action.destroy:
					{
						if (buildQueueFull)
						{
							Note.Show("Build Queue full");
							break;
						}
						Demolish(cc);

						buildQueue.Add(new BuildQueueItem() { bspot = bspot, brep = b.def.bid, slvl = b.bl, elvl = 0 });
						break;
					}
				case Action.move:
					{
						if (CanvasHelpers.IsValid(selected))
						{
							if (b.id == 0)
							{

								MoveBuilding(XYToId(selected), bspot);
								ref var b1 = ref build.buildings[bspot];
								ref var b0 = ref build.buildings[XYToId(selected)];
								// I hope that these operations are what I expect with references
								var temp = b0;
								b0 = b1;
								b1 = temp;
								selected = CanvasHelpers.invalidXY;
							}
							else
							{
								Note.Show("Please select an empty spot");

							}
						}
						else
						{
							if (b.isBuilding)
								selected = cc;
							else
								Note.Show("Please select a building to move");
						}
						break;
					}
			}
		}
		public static void Click((int x, int y) cc, bool isRight)
		{
			if (ShellPage.instance.buildMenu.IsOpen)
			{
				ShellPage.instance.buildMenu.IsOpen = false;
				//				ShellPage.instance.buildMenuCanvas.Visibility = Visibility.Collapsed;
				return;
			}

			int bspot = XYToId(cc);
			var build = GetBuild();
			var b = build.buildings[bspot];
			
			if (!isRight && action != Action.none)
			{
				PerformAction(action, cc,quickBuildId);
				if (singleClickAction == Action.pending)
				{
					action = Action.none;
					singleClickAction = Action.none;
				}
				Assert(singleClickAction == Action.none);
			}
			else
			{
				singleClickAction = Action.none; // default
				// toggle visibility
				var d = b.def;
				var i = instance;
				i.building.Text = d.Bn;
				i.description.Text = d.Ds;
				i.upgrade.IsEnabled = d.Bc.Count() > b.bl && b.isBuilding;
				i.downgrade.IsEnabled = b.bl > 1;
				i.rect.Fill = BuildingBrush(d.bid, 1.0f);
				if (!isRight)
					singleClickAction = Action.pending;
				{
					selected = cc;

					//				ShellPage.instance.buildMenu.IsOpen = true;
					var sc = ShellPage.CanvasToScreen(ShellPage.mousePosition);
					var bm = ShellPage.instance.buildMenu;
					Canvas.SetLeft(bm, sc.X - buildToolSpan / 2 - 1);
					Canvas.SetTop(bm, sc.Y - buildToolSpan / 2 + 41);
					//		ShellPage.instance.buildMenuCanvas.Visibility = Visibility.Visible;
					bm.IsOpen = true;

				}
			}
		}
		public const int buildToolSpan = 448;
	
		public const int buildMenuMruCount = 4;

		private void Abandon_Click(object sender, RoutedEventArgs e)
		{
			JSClient.view.InvokeScriptAsync("misccommand", new[] { "abandoncity" });
		}
	}

	public class BuildMenuItem : RadialMenuItem
	{
		public int bid;
		public float cacheScore;
		public BuildMenuItem(int _bid)
		{
//			GroupName = "actions";
			Command = BuildMenuItemCommand.instance;
			SetBid(_bid);
		}
		public void SetBid(int _bid)
		{
			bid = _bid;
			var def = BuildingDef.all[_bid];
			Header = def.Bn;
			ToolTipContent = def.Ds;
			IconContent = new BuildingRect() { Width = 64, Height = 64, bid = _bid };
		}
	}
	public class BuildMenuGroup : RadialMenuItem
	{
		public BuildMenuGroup(string name, params int[] ids )
		{
			Header = name;
			//		GroupName = "actions";
			IconContent = new BuildingRect() { Width = 64, Height = 64, bid = ids[0] };
			foreach (var i in ids)
				ChildItems.Add(new BuildMenuItem(i));
		}
	}
	public class BuildMenuAction : RadialMenuItem
	{
		public CityBuild.Action action;
		public BuildMenuAction(string name, CityBuild.Action action, string icon)
		{
			Header = name;
			GroupName = "actions";
			this.action = action;
			IconContent = new Image() { Width = 64,Height=64, Source = ImageHelper.FromImages(icon) };
			Command = BuildMenuItemCommand.instance;
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
				Background = BuildingBrush(bid,(float)Width/128.0f);
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
			const int i0 = (8 - buildMenuMruCount);
			const int i1 = 8;
			if ( context.MenuItem is BuildMenuItem bi)
			{
				if (singleClickAction == CityBuild.Action.pending)
				{
					PerformAction(CityBuild.Action.build,selected, bi.bid);
				}
				else
				{
					SetQuickBuild(bi.bid);

					BuildMenuItem found = null;
					float bestScore = float.MaxValue;
					BuildMenuItem best = null;
					for (int i = 0; i < i0; ++i)
					{
						var item = ShellPage.instance.buildMenu.Items[i];
						item.IsSelected = false;
					}
					for (int i = i0; i < i1; ++i)
					{
						var item = ShellPage.instance.buildMenu.Items[i] as BuildMenuItem;
						item.cacheScore *= 0.75f;
						if (item.bid == bi.bid)
							found = item;
						else
							item.IsSelected = false;
						if (item.cacheScore < bestScore)
						{
							bestScore = item.cacheScore;
							best = item;
						}
					}
					if (found == null)
					{
						found = best;
						best.SetBid(bi.bid);
					}
					found.cacheScore += 1.0f;
					found.IsSelected = true;
				}


			}
			else if( context.MenuItem is BuildMenuAction a)
			{
				for (int i = 0; i < i1; ++i)
				{
					var item = ShellPage.instance.buildMenu.Items[i];
					item.IsSelected = (item == context.MenuItem);
				}
				if (singleClickAction == CityBuild.Action.pending)
				{
					if (a.action == CityBuild.Action.move)
					{

						SetAction(CityBuild.Action.move); 
						// leave action pending
					}
					else
					{
						PerformAction(a.action, selected, 0);
						singleClickAction = CityBuild.Action.none;

					}
				}
				else
				{
					SetAction(a.action);
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
