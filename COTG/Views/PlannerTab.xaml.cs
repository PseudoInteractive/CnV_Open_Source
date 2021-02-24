﻿using System;
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
using Windows.UI.Xaml.Navigation;

using static COTG.Debug;
using static COTG.Game.City;
using COTG.Game;
using COTG.JSON;
using static COTG.Game.Enum;
using COTG.Draw;

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
	}
}
