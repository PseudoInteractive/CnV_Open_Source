using COTG.Draw;
using COTG.Game;
using COTG.JSON;

using System;
using System.Linq;
using System.Threading.Tasks;

using Windows.UI.Xaml;

using static COTG.Debug;
using static COTG.Game.City;
using static COTG.Game.Enum;
using static COTG.BuildingDef;
using System.Buffers;
using static System.Buffers.ArrayPool<COTG.Building>;

namespace COTG.Views
{
	public sealed partial class PlannerTab : UserTab
	{
		public static PlannerTab instance;
		public static bool IsVisible() => instance.isVisible;

		public  override async Task VisibilityChanged(bool visible, bool longTerm)
		{
			if (visible)
			{
			//	await CityBuild._IsPlanner(true,false);
				statsDirty = true;
			//	BuildingsChanged(City.GetBuild(),false);
			}
			else
			{
				//await CityBuild._IsPlanner( false,false );

			}
			await base.VisibilityChanged(visible, longTerm: longTerm);
		}

		public PlannerTab()
		{
			Assert(instance == null);
			instance = this;

			this.InitializeComponent();
		}
		static bool SetParentVisible( bool visible, FrameworkElement e)
		{
			var p = e.Parent as FrameworkElement;
			p = p.Parent as FrameworkElement; // two levels up
			p.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
			return visible;
		}
		//public static bool TryFindBuildingInLayout(City city,int bid,out int _xy)
		//{
		//	if( !isPlanner )
		//	{
		//		return TryFindBuildingInLayout
		//		return city.
		//	}
		//	var bc = City.buildingsCache;
		//	for (int x = span0; x <= span1; ++x)
		//	{
		//		for (int y = span0; y <= span1; ++y)
		//		{
		//			var id  = XYToId( (x, y) );
		//			var bd1 = bc[id];
		//			if(bd1 =bid )
		//			{
		//				_xy = id;
		//				return true;
		//			}	
		//		}
		//	}
		//	_xy = 0;
		//	return false;
		//}
		//static bool TryFindBuildingInLayout(City bid, out int _xy)
		//{
		//	if (!isPlanner)
		//	{
		//		return
		//	}
		//	var bc = City.buildingsCache;
		//	for (int x = span0; x <= span1; ++x)
		//	{
		//		for (int y = span0; y <= span1; ++y)
		//		{
		//			var id = XYToId((x, y));
		//			var bd1 = bc[id];
		//			if (bd1 = bid)
		//			{
		//				_xy = id;
		//				return true;
		//			}
		//		}
		//	}
		//	_xy = 0;
		//	return false;
		//}
		//static bool TryRemoveBuildingInLayout(int bid)
		//{
		//	if(TryFindBuildingInLayout(bid, out var id))
		//	{
		//		CityBuild.remove
		//		return true;
		//	}
		//	return false;

		//}

		static int GetScore(City city, Building[]cityB ,(int x,int y) xy, int bid, int militaryBid )
		{
			int id = XYToId(xy);
			var rv = 0;
			if (cityB[id].bid == bid)
				rv+=2;
			else if (cityB[id].isEmpty)
				rv += 1;
			else if (cityB[id].isRes)
				rv -= 4;

			for(int x=-1;x<=1;++x)
			{
				for (int y = -1; y <= 1; ++y)
				{
					if (x == 0 && y == 0)
						continue;
					var cc1 = (xy.x + x, xy.y + y);
					if (!IsValidCityCoord(cc1))
						continue;

					var bid1 = city.GetLayoutBid(XYToId(cc1));
					if (bid1==0)
						continue;
					if (IsResHelper(bid))
					{
						if (bid1 == bidStorehouse)
							rv += 16;
					}
					else if (bid == militaryBid || bid == bidAcademy)
					{
						if (bid1 == bidBarracks)
							rv += 16;
					}
					else if( bid == bidStorehouse)
					{
						if (IsResHelper(bid1))
							rv += 16;

					}
					else if (bid == bidBarracks)
					{
						if (bid1 == militaryBid)
							rv += 16;

					}
				}

			}

			return rv;
		}
		/// <summary>
		///  0,1,-1,2,-2,3,-3
		/// </summary>
		// 8 offsets surrounding a square
		// ordered from closest to farthest, top down, lef to right
		public static (int dx, int dy)[] surroundingSquare = new[]  {       (0,-1),
																	(-1, 0),       (1, 0),
																	        (0, 1),

																	(-1,-1),       (1,-1),
										 						     
															 	    (-1, 1),       (1, 1) };
		public static Task UpdateStats()
		{
			//SetShareStrongFromLayout();
			if (!IsVisible())
			  return Task.CompletedTask;
			statsDirty = false;
			// recruit speeds
			var city = City.GetBuild();
			var bds = city.GetLayoutBuildings();
			var tsMultipler = 1;
			var stTownHall = BuildingDef.all[bidTownHall].St[10];
			int carts = 0,ships=0,ts=0, cs=100,rsInf = 0, rsBlessed=0,rsMagic=0,rsArt=0,rsNavy=0,rsCav=0, stWood = stTownHall, stIron = stTownHall, stStone = stTownHall, stFood= stTownHall;
			for (int x = span0; x <= span1; ++x)
			{
				for (int y = span0; y <= span1; ++y)
				{
					var cc = (x, y);
					var spot = XYToId(cc);
					var bd = bds[spot];
					var bid = bd.bid;
					var bdef = BuildingDef.all[bid];
					switch (bid)
					{
						case bidCastle:
							tsMultipler = 4;
							break;
						case bidTrainingGround:
							rsInf += AddMilBuilding(bdef, bd, bds, x, y);
							break;
						case bidAcademy:
							rsBlessed += AddMilBuilding(bdef, bd, bds, x, y);
							break;
						case bidStable:
							rsCav += AddMilBuilding(bdef, bd, bds, x, y);
							break;
						case bidSorcTower:
							rsMagic += AddMilBuilding(bdef, bd, bds, x, y);
							break;
						case bidShipyard:
							rsNavy += AddMilBuilding(bdef, bd, bds, x, y);
							break;
						case bidBlacksmith:
							rsArt += AddMilBuilding(bdef, bd, bds, x, y);
							break;
						case bidMarketplace:
							carts += bdef.Trn[bd.bl];
							 break;
						case bidPort:
							ships += bdef.Trn[bd.bl];
							break;
						case bidCottage:
							cs += bdef.Cs[bd.bl];
							break;
						case bidBarracks:
							ts += bdef.Tc[bd.bl];
							break;
						case bidStorehouse: {
								var str = bdef.St[bd.bl];
								stWood += str;
								stStone += str;
								stFood += str;
								stIron += str;
								for (int dx = -1; dx <= 1; ++dx)
								{
									for (int dy = -1; dy <= 1; ++dy)
									{
										var cc1 = (x + dx, y + dy);
										if (!IsValidCityCoord(cc1))
											continue;

										var bd1 = bds[XYToId(cc1)];
										switch (bd1.bid)
										{
											case bidSmelter:
												stIron += bd1.def.St[bd1.bl] * str / 100;
												break;
											case bidSawmill:
												stWood += bd1.def.St[bd1.bl] * str / 100;
												break;
											case bidStonemason:
												stStone += bd1.def.St[bd1.bl] * str / 100;
												break;
											case bidWindmill:
												stFood += bd1.def.St[bd1.bl] * str / 100;
												break;
										}
									}
								}

							}
							break;
						default:
							break;
					}
				}
			}

			instance.ships.Text = $"{ships:N0}";
			instance.carts.Text = $"{carts:N0}";
			
			instance.maxTS.Text = $"{ts * tsMultipler:N0}";

			instance.stWood.Text = $"{stWood:N0}";
			instance.stStone.Text = $"{stStone:N0}";
			instance.stIron.Text = $"{stIron:N0}";
			instance.stFood.Text = $"{stFood:N0}";
			instance.cs.Text = $"{cs:N0}%";

			if (SetParentVisible(rsInf != 0, instance.rsInf) )
			{
				rsInf += 100;
				var gain = 100.0 / rsInf.Max(1);
				instance.rsInf.Text = $"{rsInf:N0}%";
				instance.rtVanq.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttVanquisher].ps * gain);
				instance.rtRanger.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttRanger].ps * gain);
				instance.rtTriari.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttTriari].ps * gain);
			}
			if (SetParentVisible(rsMagic != 0, instance.rsMagic))
			{
				rsMagic += 100;
				var gain = 100.0 / rsMagic.Max(1);
				instance.rsMagic.Text = $"{rsMagic:N0}%";
				instance.rtSorc.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttSorcerer].ps * gain);
				instance.rtDruid.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttDruid].ps * gain);
			}
			if (SetParentVisible(rsCav != 0, instance.rsCav))
			{
					rsCav += 100;
				var gain = 100.0 / rsCav.Max(1);
				instance.rsCav.Text = $"{rsCav:N0}%";
				instance.rtScout.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttScout].ps * gain);
				instance.rtArb.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttArbalist].ps * gain);
				instance.rtHorse.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttHorseman].ps * gain);
			}
			if (SetParentVisible(rsArt != 0, instance.rsArt))
			{
				rsArt += 100;
				var gain = 100.0 / rsArt.Max(1);
				instance.rsArt.Text = $"{rsArt:N0}%";
				instance.rtRam.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttRam].ps * gain);
				instance.rtBal.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttBallista].ps * gain);
				instance.rtCat.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttScorpion].ps * gain);
			}
			if (SetParentVisible(rsNavy != 0, instance.rsNavy))
			{
				rsNavy += 100;
				var gain = 100.0 / rsNavy.Max(1);
				instance.rsNavy.Text = $"{rsNavy:N0}%";
				instance.rtStinger.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttStinger].ps * gain);
				instance.rtGalley.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttGalley].ps * gain);
				instance.rtWarship.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttWarship].ps * gain);
			}
			if (SetParentVisible(rsBlessed != 0, instance.rsBlessed))
			{
				rsBlessed += 100;
				var gain = 100.0 / rsBlessed.Max(1);
				instance.rsBlessed.Text = $"{rsBlessed:N0}%";
				instance.rtPriestess.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttPriestess].ps * gain);
				instance.rtPrae.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttPraetor].ps * gain);
				instance.rtSen.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttSenator ].ps * gain);
			}
			static int AddMilBuilding(in BuildingDef bdef,in Building bd, Building[] bds, int x, int y)
			{
				var rv = bdef.Ts[bd.bl];
				for (int dx = -1; dx <= 1; ++dx)
				{
					for (int dy = -1; dy <= 1; ++dy)
					{
						var cc1 = (x + dx, y + dy);
						if (!IsValidCityCoord(cc1))
							continue;
						var bd1 = bds[XYToId(cc1)];
						if (bd1.bid == bidBarracks)
						{
							
							rv += bd1.def.Ts[bd1.bl];
						}

					}
				}

				return rv;
			}

		//	City.buildingCache.Return(bds);
			return Task.CompletedTask;

		}
		static bool statsDirty;
		public static bool IsValidCityCoord((int x, int y) cc)
		{
			return (cc.x >= span0) && (cc.y>=span0) && (cc.x <= span1) && (cc.y <= span1);
		}
		public static Debounce PleaseRefresh = new (PlannerTab.UpdateStats) { runOnUiThead = true };

		internal static void BuildingsChanged(City city, bool wasWritten=true)
		{
			Assert(City.emptyLayout.GetHashCode() == City.emptyLayoutHashCode);

			if(wasWritten)
				city.SaveShareStringFromLayout();

			if(city.cid == City.build)
			{
				CityView.BuildingsOrQueueChanged();
				statsDirty = true;
				PleaseRefresh.Go();
			}
		}

		private void ShareStringClick(object sender, RoutedEventArgs e)
		{
			ShareString.Show(City.build);

		}
		public async Task Rotate(City city,bool center, bool outer)
		{
			Assert(CityBuild.isPlanner);
			await CityBuild._IsPlanner(true);
			//	if (layout == null)
			//		return;
			city.TouchLayoutForWrite();
			var bc = city.layout.ToArray();
			
			Assert(CityBuild.isPlanner);
			for (int y = span0; y <= span1; ++y)
			{
				for (int x = span0; x <= span1; ++x)
				{
					var c = (x, y);
					if (c.IsXYInCenter() ? center : outer)
					{
						city.layout[XYToId((x, y))] = bc[XYToId((y, -x))];
					}

				}
			}
			PlannerTab.BuildingsChanged(city,true);
			// SaveLayout();
		}


		private async void FlipHClick(object sender, RoutedEventArgs e)
		{
			Assert(CityBuild.isPlanner);
			await CityBuild._IsPlanner(true);
			var city = GetBuild();
			city.FlipLayoutH(true,App.IsKeyPressedControl());
		//	PlannerTab.BuildingsChanged(city,true);
		}
		private async void FlipVClick(object sender, RoutedEventArgs e)
		{
			Assert(CityBuild.isPlanner);
			await CityBuild._IsPlanner(true);
			var city = GetBuild();
			city.FlipLayoutV(true,App.IsKeyPressedControl());
			//PlannerTab.BuildingsChanged(city,true);
		}

		struct AllowedToMove
		{
			public bool storage;
			public bool sorc;

		}


		static int CountResOverlaps(City city, Building[] bds,  ref AllowedToMove allowed)
		{
//			var city = City.GetBuild();
			int rv = 0;
			//var bdc = city.layout;
		//	var bds = city.postQueueBuildings;
			for(int i=0;i<citySpotCount;++i)
			{
				var des = city.GetLayoutBuilding(i);
				var bdBid = des.bid;
				// these ones can be arranged
			
				if (!des.isBuilding)
					continue;
				if (bds[i].bid == bdBid)
				{
					--rv;
				}
				else
				{
					if (bdBid == bidCastle || (bdBid == bidSorcTower && allowed.sorc) || (bdBid == bidMarketplace) || (bdBid == bidStorehouse && allowed.storage))
						continue;

					if (bds[i].isBuilding)
					{
						rv += 2;
					}
					else if (bds[i].isEmpty)
					{
						rv += 1;
					}
					else if (bds[i].isRes)
					{
						rv += 8;
					}
				}
			}
			return rv;
		}

		async void SmartRearrange(object _, RoutedEventArgs __)
		{
		//	Assert(CityBuild.isPlanner);
		//	await CityBuild._IsPlanner(true);

			await SmartRearrange(City.GetBuild(),false);
		}
		public static async Task SmartRearrange(City city,bool revertIsPlanner)
		{
			//var wasPlanner = CityBuild.isPlanner;
			if(city.isLayoutEmpty)
			{
				return;
			}
			//if (!wasPlanner)
			//	await CityBuild._IsPlanner(true);
		
			Assert(city.isLayoutCustom);
		//	var layoutB = city.GetLayoutBuildings();
			var bds = city.postQueueBuildings;

			var bc = city.GetLayoutBuildingCounts();

			var hasInvalid = false;
			int resHelpers = 0;
			{
				for(int id = 0;id < City.citySpotCount;++id)
				{
					var bid = city.GetLayoutBid(id);
					if(bid != 0 && bid != bidShipyard && bid != bidPort)
					{
						if(CityBuild.IsWaterSpot(id))
							hasInvalid = true;
						if(IsResHelper(bid))
							++resHelpers;
					}
				}
			}
			if(hasInvalid&& city.isOnWater)
			{
				city.FlipLayoutH(false,true);
			}

			var allowed = new AllowedToMove() { sorc = bc.sorcTowers == 1, storage = (resHelpers == 0) };

			var milBid = bc.GetMainMilitaryBid();

			// first try flips
			var overlap0 = CountResOverlaps(city, bds,ref allowed);
			city.FlipLayoutH();
			var overlap1 = CountResOverlaps(city,bds,ref allowed);
			city.FlipLayoutV();
			var overlap2 = CountResOverlaps(city,bds,ref allowed);
			city.FlipLayoutH();
			var overlap3 = CountResOverlaps(city,bds,ref allowed);
			if( overlap0 <= overlap1 && overlap0 <= overlap2 &&overlap0 <= overlap3)
			{
				city.FlipLayoutH(); // 0 is best one is best
				city.FlipLayoutV(); // 0 is best one is best
				city.FlipLayoutH(); // 0 is best one is best
			}
			else if( overlap1 <= overlap2 && overlap1 <= overlap3)
			{
				city.FlipLayoutH(); // this one is best
				city.FlipLayoutV(); // this one is best
			}
			else if (overlap2 <= overlap3)
			{
				city.FlipLayoutH(); // this one is best
			}
			else 
			{
			}
			//buildingCache.Return(layoutB);
		//	layoutB=city.GetLayoutBuildings();
			
			Note.Show("Flipped layout to reduce overlaps");

			for (int x = span0; x <= span1; ++x)
			{
				for (int y = span0; y <= span1; ++y)
				{
					var bdId = XYToId((x, y));
					var bd =city.GetLayoutBuilding(bdId);
					// correct building is here, leave it
					if (bds[bdId].id == bd.id)
						continue;

					var bdBid = bd.bid;
					if (bdBid == bidCastle || IsResHelper(bdBid) || (bdBid == bidSorcTower && allowed.sorc) || (bdBid == bidAcademy) || (bdBid==bidBarracks)
						|| (bdBid == bidMarketplace) || (bdBid == bidStorehouse))
					{
						bool foundOne = false;
						// is there a building that we can re-use?
						//for (int id = 0; id < City.citySpotCount; ++id)
						//{
						//	var b = bds[id];
						//	if (b.bid == bdBid && b.id != bdc[id].id)
						//	{
						//		foundOne = true;
						//		if (id != bdId)
						//		{
						//			Note.Show($"Found existing {bd.def.Bn}, moving");
						//			AUtil.Swap(ref bdc[bdId], ref bdc[id]);
						//		}
						//	}
						//}

						//if ( bds[bdId].isRes)
						{
							var score = GetScore(city, bds, (x,y), bd.bid, milBid);

							// move to a non res spot
							for (int r = 1;r< citySpan; ++r)
							{
								for (int x0 = span0.Max(x - r); x0 <= span1.Min(x + r); ++x0)
								{
									for (int y0 = span0.Max(y - r); y0 <= span1.Min(y + r); ++y0)
									{
										// only edge
										if (!((x0 == x - r) || (x0 == x + r) || (y0 == y - r) || (y0 == y + r)))
											continue;
										var id1 = XYToId((x0, y0));
										if (city.GetLayoutBuilding(id1).isBuilding)
											continue;
										if (!CityBuild.IsBuildingSpot(id1))
										{
											continue;
										}

										var score1 = GetScore(city,bds,(x0, y0), bd.bid, milBid);
										if(score1 > score)
										{

											Note.Show($"Moving {bd.def.Bn} from a res node to an empty spot");
											AUtil.Swap(ref city.layout[bdId], ref city.layout[id1]);
											foundOne = true;
											break;
										}
										if (foundOne)
											break;
									}
									if (foundOne)
										break;
								}
								if (foundOne)
									break;
							}
						}
					}
				}
			}
			Note.Show($"Moved buildings to reduce overlaps, final layout match score: {CountResOverlaps(city, bds, ref allowed) }");
			//city.TouchLayoutForWrite();
			//City.buildingCache.Return(layoutB);
			BuildingsChanged(city);
		//	if(wasPlanner == false && revertIsPlanner)
		//		await CityBuild._IsPlanner(false);
		}

		private static bool IsResHelper(int bid)
		{
			return bid == bidSawmill || bid == bidWindmill || bid == bidStonemason || bid == bidSmelter;
		}

		private void Done(object sender, RoutedEventArgs e)
		{
		//	CityBuild._IsPlanner(false);
			PlannerTab.instance.Close();
		}

		//public override void Close()
		//{ 
		//	base.Close();
		//	CityBuild._IsPlanner(false);

		//}

		private void RotateCenterClick(object sender, RoutedEventArgs e)
		{
			Rotate(City.GetBuild(), true, false);
		}

		private void RotateOuterClick(object sender, RoutedEventArgs e)
		{
			Rotate(City.GetBuild(),false, true);
		}

		private void UseBuildingsClick(object sender,RoutedEventArgs e)
		{
			var s = City.GetBuild();
			s.SetLayoutFromBuildings(s.postQueueBuildings,false);

		}
	}
}
