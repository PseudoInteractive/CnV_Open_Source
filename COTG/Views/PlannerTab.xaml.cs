using COTG.Draw;
using COTG.Game;
using COTG.JSON;

using Windows.UI.Xaml;

using static COTG.Debug;
using static COTG.Game.City;
using static COTG.Game.Enum;

namespace COTG.Views
{
	public sealed partial class PlannerTab : UserTab
	{
		public static PlannerTab instance;
		public static bool IsVisible() => instance.isVisible;

		public async override void VisibilityChanged(bool visible)
		{
			if (visible)
			{
				CityBuild._isPlanner = true;
				BuildingsChanged();
			}
			else
			{
				CityBuild._isPlanner = false;

			}
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

		public static void UpdateStats()
		{
			if (statsDirty ==false || !IsVisible())
				return;
			statsDirty = false;
			// recruit speeds
			var city = City.GetBuild();
			var bds = buildingsCache;
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
						case bidTrainingground:
							rsInf += AddMilBuilding(bdef, bd, bds, x, y);
							break;
						case bidAcademy:
							rsBlessed += AddMilBuilding(bdef, bd, bds, x, y);
							break;
						case bidStable:
							rsCav += AddMilBuilding(bdef, bd, bds, x, y);
							break;
						case bidMage_tower:
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
				instance.rtVanq.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttVanquisher].Ps * gain);
				instance.rtRanger.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttRanger].Ps * gain);
				instance.rtTriari.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttTriari].Ps * gain);
			}
			if (SetParentVisible(rsMagic != 0, instance.rsMagic))
			{
				rsMagic += 100;
				var gain = 100.0 / rsMagic.Max(1);
				instance.rsMagic.Text = $"{rsMagic:N0}%";
				instance.rtSorc.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttSorcerer].Ps * gain);
				instance.rtDruid.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttDruid].Ps * gain);
			}
			if (SetParentVisible(rsCav != 0, instance.rsCav))
			{
					rsCav += 100;
				var gain = 100.0 / rsCav.Max(1);
				instance.rsCav.Text = $"{rsCav:N0}%";
				instance.rtScout.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttScout].Ps * gain);
				instance.rtArb.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttArbalist].Ps * gain);
				instance.rtHorse.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttHorseman].Ps * gain);
			}
			if (SetParentVisible(rsArt != 0, instance.rsArt))
			{
				rsArt += 100;
				var gain = 100.0 / rsArt.Max(1);
				instance.rsArt.Text = $"{rsArt:N0}%";
				instance.rtRam.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttRam].Ps * gain);
				instance.rtBal.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttBallista].Ps * gain);
				instance.rtCat.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttScorpion].Ps * gain);
			}
			if (SetParentVisible(rsNavy != 0, instance.rsNavy))
			{
				rsNavy += 100;
				var gain = 100.0 / rsNavy.Max(1);
				instance.rsNavy.Text = $"{rsNavy:N0}%";
				instance.rtStinger.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttStinger].Ps * gain);
				instance.rtGalley.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttGalley].Ps * gain);
				instance.rtWarship.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttWarship].Ps * gain);
			}
			if (SetParentVisible(rsBlessed != 0, instance.rsBlessed))
			{
				rsBlessed += 100;
				var gain = 100.0 / rsBlessed.Max(1);
				instance.rsBlessed.Text = $"{rsBlessed:N0}%";
				instance.rtPriestess.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttPriestess].Ps * gain);
				instance.rtPrae.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttPraetor].Ps * gain);
				instance.rtSen.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttSenator ].Ps * gain);
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
		}
		static bool statsDirty;
		public static bool IsValidCityCoord((int x, int y) cc)
		{
			return (cc.x >= span0) && (cc.y>=span0) && (cc.x <= span1) && (cc.y <= span1);
		}
		internal static void BuildingsChanged()
		{
			if (statsDirty == true || !IsVisible() )
				return;
			CityView.BuildingsOrQueueChanged();
			statsDirty = true;
			App.DispatchOnUIThreadLow(UpdateStats);
		}

		private void ShareStringClick(object sender, RoutedEventArgs e)
		{
			ShareString.Show();

		}

		private void FlipHClick(object sender, RoutedEventArgs e)
		{
			GetBuild().FlipLayoutH();
		}
		private void FlipVClick(object sender, RoutedEventArgs e)
		{
			GetBuild().FlipLayoutV();
		}

		ref struct AllowedToMove
		{
			public bool storage;
			public bool academy;
			public bool sorc;

		}
		static int CountResOverlaps( ref AllowedToMove allowed)
		{
			var build = City.GetBuild();
			int rv = 0;
			var bdc = City.buildingsCache;
			var bds = build.buildings;
			for(int i=0;i<citySpotCount;++i)
			{
				var des = bdc[i];
				if (!des.isBuilding)
					continue;
				if (!bds[i].isRes)
					continue;

				var bdBid = bdc[i].bid;
				// these ones can be arranged
				if (bdBid == bidCastle || (bdBid == bidMage_tower && allowed.sorc) || (bdBid == bidAcademy && allowed.academy) || (bdBid == bidMarketplace) || (bdBid == bidStorehouse && allowed.storage))
					continue;

				++rv;
			}
			return rv;
		}

		private void SmartRearrange(object sender, RoutedEventArgs e)
		{
			var build = City.GetBuild();

			var bdc = City.buildingsCache;
			var bds = build.buildings;

			int sorcTowers = 0;
			int academies = 0;
			int resHelpers = 0;
			{
				for (int id = 0; id < City.citySpotCount; ++id)
				{
					var bid = bdc[id].bid;
					if (bid == bidMage_tower)
						++sorcTowers;
					else if (bid == bidAcademy)
						++academies;
					else if (bid == bidSawmill || bid == bidWindmill || bid == bidStonemason || bid == bidSawmill)
					{
						++resHelpers;
					}

				}
			}
			var allowed = new AllowedToMove() { sorc = sorcTowers == 1, academy = academies == 1, storage = resHelpers == 0 };

			// first try flips
			var overlap0 = CountResOverlaps(ref allowed);
			build.FlipLayoutH();
			var overlap1 = CountResOverlaps(ref allowed);
			build.FlipLayoutV();
			var overlap2 = CountResOverlaps(ref allowed);
			build.FlipLayoutH();
			var overlap3 = CountResOverlaps(ref allowed);
			if( overlap0 <= overlap1 && overlap0 <= overlap2 &&overlap0 <= overlap3)
			{
				build.FlipLayoutV(); // this one is best
			}
			else if( overlap1 <= overlap2 && overlap1 <= overlap3)
			{
				build.FlipLayoutV(); // this one is best
				build.FlipLayoutH(); // this one is best
			}
			else if (overlap2 <= overlap3)
			{
				build.FlipLayoutH(); // this one is best
			}
			else 
			{
			}
			Note.Show("Flipped layout to reduce overalps");

			for (int x = span0; x <= span1; ++x)
			{
				for (int y = span0; y <= span1; ++y)
				{
					var bdId = XYToId((x, y));
					var bd = bdc[bdId];
					// correct building is here, leave it
					if (bds[bdId].id == bd.id)
						continue;

					var bdBid = bd.bid;
					if (bdBid == bidCastle || (bdBid == bidMage_tower && allowed.sorc) || (bdBid == bidAcademy && allowed.academy) || (bdBid == bidMarketplace) || (bdBid == bidStorehouse && allowed.storage))
					{
						bool foundOne = false;
						// is there a building that we can re-use?
						for (int id = 0; id < City.citySpotCount; ++id)
						{
							var b = bds[id];
							if (b.bid == bdBid && b.id != bdc[id].id)
							{
								foundOne = true;
								if (id != bdId)
								{
									Note.Show($"Found existing {bd.def.Bn}, moving");
									AUtil.Swap(ref bdc[bdId], ref bdc[id]);
								}
							}
						}

						if (!foundOne && bds[bdId].isRes)
						{
							// move to a non res spot
							for (int r = 1; ; ++r)
							{
								for (int x0 = span0.Max(x - r); x0 <= span1.Min(x + r); ++x0)
								{
									for (int y0 = span0.Max(y - r); y0 <= span1.Min(y + r); ++y0)
									{
										var id1 = XYToId((x0, y0));
										if(!CityBuild.IsBuildingSpot(id1))
										{
											continue;
										}
										if (bds[id1].isEmpty && (bdc[id1].isEmpty || bdc[id1].isRes))
										{
											Note.Show($"Moving {bd.def.Bn} from a res node to an empty spot");
											AUtil.Swap(ref bdc[bdId], ref bdc[id1]);
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
			Note.Show($"Moved buildings to reduce overalps: { CountResOverlaps(ref allowed)} overlaps remain");
			CityView.BuildingsOrQueueChanged();
		}

		private void Done(object sender, RoutedEventArgs e)
		{
			CityBuild._isPlanner = false;
			PlannerTab.instance.Close();
		}
	}
}
