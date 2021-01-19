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
			count,

		};
		public static Action action;
		public static void ClearAction() 
		{
			SetAction( Action.none);
		}
		public static void SetAction(Action _action)
		{
			action = _action;
			App.DispatchOnUIThreadSneaky( ()=> instance.quickBuild.SelectedIndex = (int)_action ); /// the first 3 are mapped. this triggers a selected changed event
		}

		
		static List<QuickBuildItem> items;

		public CityBuild()
		{
			instance = this;
			this.InitializeComponent();
			items = new List<QuickBuildItem>();
			ushort[] buildingIds = {
			448, //Forester's Hut"),
			446, //Cabin"),
			464, //Storehouse"),
			461, //Stone Mine"),
			547, //Sentinel Post"),
			479, //Hideaway"),
			447, //Farm Estate"),
			504, //Guardhouse"),
			543, //Ranger Post"),
			445, //Barracks"),
			465, //Iron Mine"),
			483, //Training Arena"),
			449, //Forum"),
			481, //Villa"),
			567, //Snag Barricade"),
			460, //Sawmill"),
			466, //Stable"),
			539, //Triari Post"),
			462, //Mason's Hut"),
			500, //Sorcerer's Tower"),
			559, //Equine Barricade"),
			463,482,467,551,563,890,477,502,555,571,490,498
		};
			// Add commands as "Pseudo" buildings for selection
			items.Add(new QuickBuildItem(Action.none, "Select", "City/decal_select_building.png"));
			items.Add(new QuickBuildItem(Action.move, "Move", "City/decal_move_building.png"));
			items.Add(new QuickBuildItem(Action.destroy, "Demo", "City/decal_building_invalid.png"));

			foreach (var i in buildingIds)
			{
				items.Add(new QuickBuildItem(i));
			}


			quickBuild.ItemsSource = items;
		}

		private void Upgrade_Click(object sender, RoutedEventArgs e)
		{

			var id = City.XYToId(selected);
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
			var id = City.XYToId(selected);
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
			var id = City.XYToId(building);
			var sel = GetBuilding(id);
			JSClient.view.InvokeScriptAsync("buildop", new[] { sel.def.bid.ToString(), id.ToString(), "3" }); // op 3 is destroy
		}
		public static void Downgrade((int x, int y) building)
		{
			var id = City.XYToId(building);
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
			Services.Post.Send("includes/mBu.php", $"a={idFrom}&b={idTo}&c={City.build}");


		}


		private void quickBuild_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			var sel = quickBuild.SelectedItem  as QuickBuildItem;
			if (sel != null )
			{
				var bid = sel.bid;
				quickBuildId = bid;
				// clear selection on tool change
				if (bid <  (int)Action.count)
				{
					action = (Action)bid;
				}
				else
				{
					action = Action.build;
				}
				if(action != Action.move)
					selected = CanvasHelpers.invalidXY;

			}
			else
			{
				// this is an invalid setting
				action = Action.none; // is this an appropriate action to take?
				quickBuildId = 0;
			}
		}
		public static void Click((int x, int y) cc)
		{
			int bspot = XYToId(cc);
			var build = City.GetBuild();
			var b = build.buildings[bspot];

			switch (action)
			{
				case Action.build:
					{
						if ( (b.id != 0 ) || buildQueue.Any(a => a.bspot == bspot))
						{
							Note.Show("There is already someting here");
							break;
						}
						if (City.buildQueueFull)
						{
							Note.Show("Build Queue full");
							break;
						}

						var sel = CityBuild.quickBuildId;

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
						if (City.buildQueueFull)
						{
							Note.Show("Build Queue full");
							break;
						}
						Demolish(cc);

						City.buildQueue.Add(new BuildQueueItem() { bspot = bspot, brep = b.def.bid, slvl = b.bl, elvl = 0 });
						break;
					}
				case Action.move:
					{
						if (CanvasHelpers.IsValid(selected))
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
							selected = cc;
						}
						break;
					}
				default:
					{
						selected = cc;
						var d = b.def;
						var i = CityBuild.instance;
						i.building.Text = d.Bn;
						i.description.Text = d.Ds;
						i.upgrade.IsEnabled = d.Bc.Count() > b.bl && b.bl != 0;
						i.downgrade.IsEnabled = b.bl > 1;
						i.rect.Fill = CityBuild.BuildingBrush(d.bid, 1.0f);
						
						break;
					}


			}
		}
		public const int buildToolSpan = 380;
		public static void RightClick((int x, int y) cc)
		{
			// toggle visibility
			if (ShellPage.instance.buildMenuCanvas.Visibility == Visibility.Visible)
			{
				ShellPage.instance.buildMenuCanvas.Visibility = Visibility.Collapsed;
			}
			else
			{
				var sc = ShellPage.CanvasToScreen(ShellPage.mousePosition);
				var b = ShellPage.instance.buildMenu;
				Canvas.SetLeft(b, sc.X - buildToolSpan / 2);
				Canvas.SetTop(b, sc.Y - buildToolSpan / 2);
				ShellPage.instance.buildMenuCanvas.Visibility = Visibility.Visible;
			}			
		}
	}

	public class BuildMenuItem : RadialMenuItem
	{
		public int bid;
		public BuildMenuItem(int _bid)
		{
			bid = _bid;
			var def = BuildingDef.all[_bid];
			Header = def.Bn;
			ToolTipContent = def.Ds;
			IconContent = new BuildingRect() { Width = 64, Height = 64, bid = _bid };
			GroupName = "actions";
		}
	}
	// nested types not supported
	public class QuickBuildItem
	{
		public int bid;
		public string name { get; set; }
		public ImageBrush brush { get; set; }
		public QuickBuildItem(int _id)
		{
			bid = _id;
			name = BuildingDef.all[_id].Bn;
			brush = CityBuild.BuildingBrush(_id, 0.5f);
		}
		public QuickBuildItem(CityBuild.Action _id, string _name, string image)
		{
			bid = (int)_id;
			name = _name;
			brush = CityBuild.BrushFromImage(image);

		}
	}
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
				Background = CityBuild.BuildingBrush(bid,(float)Width/128.0f);
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
				Background = CityBuild.BuildingBrush(bid, (float)Width / 128.0f);
			}
		}

		public BuildingRect()
		{
			Width = 64;
			Height = 64;
		}
	}
}
