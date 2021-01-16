using COTG.Draw;
using COTG.Game;
using COTG.Helpers;

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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace COTG.Views
{
	public sealed partial class CityBuild : UserControl
	{
		public static CityBuild instance = new CityBuild();

		class QuickBuildItem
		{
			public int id;
			public string name { get; set; }
			public ImageBrush brush { get; set; }
			public QuickBuildItem(int _id, string _name)
			{
				name = _name;
				id = _id;
				brush = BuildingBrush(_id);
			}
		}
		static QuickBuildItem[] items = new[] {
			new QuickBuildItem(448, "Forester's Hut"),
			new QuickBuildItem(446, "Cabin"),
			new QuickBuildItem(464, "Storehouse"),
			new QuickBuildItem(461, "Stone Mine"),
			new QuickBuildItem(547, "Sentinel Post"),
			/*
											<input id = 'hideaway' class='builds tttwo' type="radio" name="builds" value="479" />
														<label class="quickbuild-b hideaway ttwo" for="hideaway">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Hideaway</div>
														</label>
													</td>
												</tr>
												<tr id = 'b3' >

													< td >

														< input id='farmhouse' class='builds ttthree' type="radio" name="builds" value="447" />
														<label class="quickbuild-b farmhouse tthree" for="farmhouse">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Farm Estate</div>
														</label>
													</td>
													<td>
														<input id = 'guardhouse' class='builds ttthree' type="radio" name="builds" value="504" />
														<label class="quickbuild-b guardhouse tthree" for="guardhouse">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Guardhouse</div>
														</label>
													</td>
													<td>
														<input id = 't_ran' class='builds ttthree' type="radio" name="builds" value="543" />
														<label class="quickbuild-b t_ran tthree" for="t_ran">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Ranger Post</div>
														</label>
													</td>
												</tr>
												<tr id = 'b4' >

													< td >

														< input id= 'barracks' class='builds ttfour' type="radio" name="builds" value="445" />
														<label class="quickbuild-b barracks tfour" for="barracks">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Barracks</div>
														</label>
													</td>
													<td>
														<input id = 'mine' class='builds ttfour' type="radio" name="builds" value="465" />
														<label class="quickbuild-b mine tfour" for="mine">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Iron Mine</div>
														</label>
													</td>
													<td>
														<input id = 'trainingground' class='builds ttfour' type="radio" name="builds" value="483" />
														<label class="quickbuild-b trainingground tfour" for="trainingground">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Training Arena</div>
														</label>
													</td>
												</tr>
												<tr id = 'b5' >

													< td >

														< input id= 'marketplace' class='builds ttfive' type="radio" name="builds" value="449" />
														<label class="quickbuild-b marketplace tfive" for="marketplace">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Forum</div>
														</label>
													</td>
													<td>
														<input id = 'townhouse' class='builds ttfive' type="radio" name="builds" value="481" />
														<label class="quickbuild-b townhouse tfive" for="townhouse">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Villa</div>
														</label>
													</td>
													<td>
														<input id = 't_snag' class='builds ttfive' type="radio" name="builds" value="567" disabled/>
														<label class="quickbuild-b t_snag tfive" for="t_snag">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Snag Barricade</div>
														</label>
													</td>
												</tr>
												<tr id = 'b6' >

													< td >

														< input id= 'sawmill' class='builds ttsix' type="radio" name="builds" value="460" />
														<label class="quickbuild-b sawmill tsix" for="sawmill">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Sawmill</div>
														</label>
													</td>
													<td>
														<input id = 'stable' class='builds ttsix' type="radio" name="builds" value="466" />
														<label class="quickbuild-b stable tsix" for="stable">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Stable</div>
														</label>
													</td>
													<td>
														<input id = 't_spe' class='builds ttsix' type="radio" name="builds" value="539" disabled/>
														<label class="quickbuild-b t_spe tsix" for="t_spe">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Triari Post</div>
														</label>
													</td>
												</tr>
												<tr id = 'b7' >

													< td >

														< input id= 'stonemason' class='builds ttseven' type="radio" name="builds" value="462" />
														<label class="quickbuild-b stonemason tseven" for="stonemason">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Mason's Hut</div>
														</label>
													</td>
													<td>
														<input id = 'magetower' class='builds ttseven' type="radio" name="builds" value="500" />
														<label class="quickbuild-b magetower tseven" for="magetower">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Sorcerer's Tower</div>
														</label>
													</td>
													<td>
														<input id = 't_equi' class='builds ttseven' type="radio" name="builds" value="559" disabled/>
														<label class="quickbuild-b t_equi tseven" for="t_equi">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Equine Barricade</div>
														</label>
													</td>
												</tr>
												<tr id = 'b8' >

													< td >

														< input id= 'windmill' class='builds tteight' type="radio" name="builds" value="463" />
														<label class="quickbuild-b windmill teight" for="windmill">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Grain Mill</div>
														</label>
													</td>
													<td>
														<input id = 'temple' class='builds tteight' type="radio" name="builds" value="482" />
														<label class="quickbuild-b temple teight" for="temple">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Academy</div>
														</label>
													</td>
													<td>
														<input id = 'castle' class='builds tteight' type="radio" name="builds" value="467" />
														<label class="quickbuild-b castle teight" for="castle">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Castle</div>
														</label>
													</td>
												</tr>
												<tr id = 'b85' >

													< td >

														< input id='l_post' class='builds tteight' type="radio" name="builds" value="551" disabled/>
														<label class="quickbuild-b l_post teight" for="l_post">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Priestess Post</div>
														</label>
													</td>
													<td>
														<input id = 't_rune' class='builds tteight' type="radio" name="builds" value="563" disabled/>
														<label class="quickbuild-b t_rune teight" for="t_rune">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Rune Barricade</div>
														</label>
													</td>
													<td>
														<input id = 'palace' class='builds' type="radio" name="builds" value="0" disabled/>
														<label class="quickbuild-b palace " for="palace">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Temple</div>
														</label>
													</td>
												</tr>
												<tr id = 'b9' >

													< td >

														< input id='smelter' class='builds ttnine' type="radio" name="builds" value="477" />
														<label class="quickbuild-b smelter tnine" for="smelter">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Smelter</div>
														</label>
													</td>
													<td>
														<input id = 'blacksmith' class='builds ttnine' type="radio" name="builds" value="502" />
														<label class="quickbuild-b blacksmith tnine" for="blacksmith">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Blacksmith</div>
														</label>
													</td>
													<td>
														<input id = 'b_post' class='builds ttnine' type="radio" name="builds" value="555" disabled/>
														<label class="quickbuild-b b_post tnine" for="b_post">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Ballista Post</div>
														</label>
													</td>
												</tr>
												<tr id = 'b10' >

													< td >

														< input id= 't_veil' class='builds ttten' type="radio" name="builds" value="571" disabled/>
														<label class="quickbuild-b t_veil tten" for="t_veil">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Veiled Barricade</div>
														</label>
													</td>
													<td>
														<input id = 'prt' class='builds ttten' type="radio" name="builds" value="490" />
														<label class="quickbuild-b prt tten" for="prt">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Port</div>
														</label>
													</td>
													<td>
														<input id = 'shipyrd' class='builds ttten' type="radio" name="builds" value="498" />
														<label class="quickbuild-b shipyrd tten" for="shipyrd">
															<div class="bldgmnudv"></div>
															<br/>
															<div class="bldgnmedv">Shipyard</div>
														</label>
													</td>
			*/
		};
		public CityBuild()
		{
			this.InitializeComponent();
			quickBuild.ItemsSource = items;
		}

		private void Upgrade_Click(object sender, RoutedEventArgs e)
		{
			var id = City.XYToId(selected);
			var sel = GetBuilding(id);
			var lvl = sel.bl;
			var def = sel.def;
			// check for queued upgrades
			foreach (var q in City.buildQueue)
			{
				if (q.bspot == id)
					lvl = q.elvl;
			}
			JSClient.view.InvokeScriptAsync("upgradeBuilding", new[] { (selected.x - span0).ToString(), (selected.y - span0).ToString(), (lvl + 1).ToString() });
			buildQueue.Add(new JSON.BuildQueueItem() { bspot = (ushort)id, slvl = (byte)(lvl), elvl = (byte)(lvl + 1) });
		}

		private void Downgrade_Click(object sender, RoutedEventArgs e)
		{

		}

		private void Destroy_Click(object sender, RoutedEventArgs e)
		{

		}
		public static ImageBrush BrushFromAtlas(string name, int x, int y)
		{

			var bitmap = ImageHelper.FromImages(name);
			var brush = new ImageBrush()
			{
				ImageSource = bitmap,
				Stretch = Stretch.None,
				AlignmentX = AlignmentX.Left,
				AlignmentY = AlignmentY.Top,
				Transform = new TranslateTransform() { X = -x, Y = -y },
			};
			//	rect.Stretch = Stretch.None;
			//			rect.Width = width;
			//			rect.Height = height;
			return brush;
		}
		public static ImageBrush BuildingBrush(int id)
		{
			var iconId = id - 443;
			const int atlasColumns = 4;
			const int duDt = 128;
			const int dvDt = 128;
			var u0 = (iconId % atlasColumns) * duDt;
			var v0 = (int)(iconId / atlasColumns) * dvDt;
			var uri = SettingsPage.IsThemeWinter() ? "City/Winter/building_set5.png" :
			"City/building_set5.png";
			return BrushFromAtlas(uri, u0, v0);
		}
	}
}
