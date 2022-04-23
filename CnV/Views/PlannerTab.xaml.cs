using CnV.Draw;
using CnV.Game;


using System;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.UI.Xaml;

using static CnV.Debug;
using static CnV.City;
using static CnV.Troops;
using static CnV.BuildingDef;
using static CnV.Building;
using System.Buffers;
//using static System.Buffers.ArrayPool<COTG.Building>;

namespace CnV.Views
{
	using Game;
	using Data;

	public sealed partial class PlannerTab : UserTab
	{
		public static PlannerTab instance;
		public static bool IsVisible() => instance.isFocused;

		public  override async Task VisibilityChanged(bool visible, bool longTerm)
		{
			if (visible)
			{
			//	await CityBuild._IsPlanner(true,false);
				PleaseRefresh.Go();
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
		
		private void SharestringToClipboardClick(object sender,RoutedEventArgs e)
		{

			var s = City.GetBuild();
			var t = City.LayoutToShareString( City.LayoutFromBuildings(s.GetLayoutBuildings(),true,s.buildings),s.isOnWater);
			AppS.CopyTextToClipboard(t);
			Note.Show(t);
		}


		static int GetScore(City city, Building[]cityB ,(int x,int y) xy, int bid, int sorcTowerCount, int academyCount )
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
					else if (  IsBidMilitary( (BuildingId)bid) )
					{
						if(bid1 == bidBarracks)
						{
							if(bid == bidSorcTower && sorcTowerCount <= 1)
								rv += 0;
							else if(bid == bidAcademy && academyCount<=1)
								rv += 4;
							else
								rv += 16;
						}
					}
					else if( bid == bidStorehouse)
					{
						if (IsResHelper(bid1))
							rv += 16;

					}
					else if (bid == bidBarracks)
					{
						if(IsBidMilitary((BuildingId)bid1))
						{
							if(bid1 == bidSorcTower && sorcTowerCount <= 1)
								rv += 0;
							else if(bid1 == bidAcademy && academyCount<=1)
								rv += 4;
							else
								rv += 16;
						}
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
			// recruit speeds
			
			var city = City.GetBuild();
			
			var bds = city.GetLayoutBuildingsWithRes();
			var s = City.StaticUpdateBuildingStats(city,bds);


			instance.ships.Text = $"{s.ships:N0}";
			instance.carts.Text = $"{s.carts:N0}";
			
			instance.maxTS.Text = $"{s.maxTs:N0}";

			instance.storage.Text = s.storage.Format("\n");
			instance.production.Text = s.production.Format("\n");
			instance.cs.Text = $"{s.cs:N0}%";

			if (SetParentVisible(s.rsInf != 0, instance.rsInf) )
			{
				var gain = 100.0 /s. rsInf.Max(1);
				instance.rsInf.Text = $"{s.rsInf:N0}%";
				instance.rtVanq.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttVanquisher].ps * gain);
				instance.rtRanger.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttRanger].ps * gain);
				instance.rtTriari.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttTriari].ps * gain);
			}
			if (SetParentVisible(s.rsMagic != 0, instance.rsMagic))
			{
				var gain = 100.0 / s.rsMagic.Max(1);
				instance.rsMagic.Text = $"{s.rsMagic:N0}%";
				instance.rtSorc.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttSorcerer].ps * gain);
				instance.rtDruid.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttDruid].ps * gain);
			}
			if (SetParentVisible(s.rsCav != 0, instance.rsCav))
			{
				var gain = 100.0 / s.rsCav.Max(1);
				instance.rsCav.Text = $"{s.rsCav:N0}%";
				instance.rtScout.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttScout].ps * gain);
				instance.rtArb.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttArbalist].ps * gain);
				instance.rtHorse.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttHorseman].ps * gain);
			}
			if (SetParentVisible(s.rsArt != 0, instance.rsArt))
			{
				var gain = 100.0 / s.rsArt.Max(1);
				instance.rsArt.Text = $"{s.rsArt:N0}%";
				instance.rtRam.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttRam].ps * gain);
				instance.rtBal.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttBallista].ps * gain);
				instance.rtCat.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttScorpion].ps * gain);
			}
			if (SetParentVisible(s.rsNavy != 0, instance.rsNavy))
			{
				var gain = 100.0 / s.rsNavy.Max(1);
				instance.rsNavy.Text = $"{s.rsNavy:N0}%";
				instance.rtStinger.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttStinger].ps * gain);
				instance.rtGalley.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttGalley].ps * gain);
				instance.rtWarship.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttWarship].ps * gain);
			}
			if (SetParentVisible(s.rsBlessed != 0, instance.rsBlessed))
			{
				var gain = 100.0 / s.rsBlessed.Max(1);
				instance.rsBlessed.Text = $"{s.rsBlessed:N0}%";
				instance.rtPriestess.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttPriestess].ps * gain);
				instance.rtPrae.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttPraetor].ps * gain);
				instance.rtSen.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttSenator ].ps * gain);
			}
			

		//	City.buildingCache.Return(bds);
			return Task.CompletedTask;

		}
	
		public static bool IsValidCityCoord((int x, int y) cc)
		{
			return (cc.x >= span0) && (cc.y>=span0) && (cc.x <= span1) && (cc.y <= span1);
		}
		public static Debounce PleaseRefresh = new (PlannerTab.UpdateStats) { runOnUiThread = true };

		

		private void ShareStringClick(object sender, RoutedEventArgs e)
		{
			ShareString.Show(City.build.cid,SetupFlags.layout);

		}
		public void Rotate(City city,bool center, bool outer)
		{
			//	if (layout == null)
			//		return;
			city.TouchLayoutForWrite();
			var bc = city.layout.ToArray();
			
			Assert(CityBuild.isPlanner);
			for (int y = span0; y <= span1; ++y)
			{
				for (int x = span0; x <= span1; ++x)
				{
					var c = new BuildC(x, y);
					if (c.isInCenter ? center : outer)
					{
						city.layout[c] = bc[XYToId((y, -x))];
					}

				}
			}
			city.BuildingsChanged();
			// SaveLayout();
		}


		private async void FlipHClick(object sender, RoutedEventArgs e)
		{
			AppS.UpdateKeyStates();
			Assert(CityBuild.isPlanner);
			//await CityBuild._IsPlanner(true);
			var city = GetBuild();
			city.FlipLayoutH(true,AppS.IsKeyPressedControl());
		//	CityBuild.BuildingsChanged(city,true);
		}
		private async void FlipVClick(object sender, RoutedEventArgs e)
		{
			AppS.UpdateKeyStates();

			Assert(CityBuild.isPlanner);
			//await CityBuild._IsPlanner(true);
			var city = GetBuild();
			city.FlipLayoutV(true,AppS.IsKeyPressedControl());
			//CityBuild.BuildingsChanged(city,true);
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
			for(BuildingSpot i=0;i<citySpotCount;++i)
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
//				if (!city.isOnWater)
				{
					// Oops!

					for (BuildingSpot id = 0; id < City.citySpotCount; ++id)
					{
						var bid = city.GetLayoutBid(id);
						if (bid != 0 && bid != bidShipyard && bid != bidPort)
						{
							if (CityBuild.IsWaterSpot(id,city))
								hasInvalid = true;
							if (IsResHelper(bid))
								++resHelpers;
						}
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
							var score = GetScore(city, bds, (x,y), bd.bid, bc.sorcTowers,bc.academies);

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
										if (!city.IsBuildingSpot(id1))
										{
											continue;
										}

										var score1 = GetScore(city,bds,(x0, y0), bd.bid, bc.sorcTowers,bc.academies);
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
			city.BuildingsChanged();
		//	if(wasPlanner == false && revertIsPlanner)
		//		await CityBuild._IsPlanner(false);
		}

		private static bool IsResHelper(int bid)
		{
			return bid == bidSawmill || bid == bidGrainMill || bid == bidStonemason || bid == bidSmelter;
		}

		private void Done(object sender, RoutedEventArgs e)
		{
			PlannerTab.instance.Close();
		}

		public override void Close()
		{ 
			base.Close();
			CityBuild._IsPlanner(false);

		}

		private void RotateCenterClick(object sender, RoutedEventArgs e)
		{
			Rotate(City.GetBuild(), true, false);
		}

		private void RotateOuterClick(object sender, RoutedEventArgs e)
		{
			Rotate(City.GetBuild(),false, true);
		}

		private async void UseBuildingsClick(object sender,RoutedEventArgs e)
		{
			var s = City.GetBuild();
			s.layout =  await LayoutFromBuildings(s.postQueueBuildings);
			s.BuildingsChanged();

		}

		
	}
}
