using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace CnV
{
	using static Building;

	public struct CityBuildingStats
	{
		public bool dirty;
		public ushort points; // score
		public ushort carts;
		public ushort ships;
		public ushort cs;
		public int maxTs;
		public int rsInf;
		public int rsBlessed;
		public int rsMagic;
		public int rsArt;
		public int rsNavy;
		public int rsCav;
		public Resources storage;
		public Resources production;
	}

	partial class City
	{
		public void UpdateBuildingStats()
		{
			if(!this.stats.dirty)
				return;
			var city = this;
			// current not future or whatever
			var bds = city.buildings;
			var tsPercentMultipler = 100;
			var townHallLevel = bds[bspotTownHall].bl;
			var stTownHall = BuildingDef.FromId(bidTownHall).St[townHallLevel];
			CityBuildingStats stats = new() {  cs=100, storage = new( stTownHall*100) };

			for(int x = span0; x <= span1; ++x)
			{
				for(int y = span0; y <= span1; ++y)
				{
					var cc = new BuildC(x, y);
					var bd = bds[cc];
					var bid = bd.bid;
					if(!bd.isBuilding)
						continue;
					Assert(bd.bl >= 1);
					Assert(!bd.isRes);
					//var bl = bd.bl;
					var bdef = BuildingDef.FromId(bid);
					switch(bid)
					{
						case bidCastle:
							tsPercentMultipler = (100+bdef.sc[bd.bl]);
							break;
						case bidTrainingGround:
							stats.rsInf += AddMilBuilding(bdef, bd, bds, cc);
							break;
						case bidAcademy:
							stats.rsBlessed += AddMilBuilding(bdef, bd, bds, cc);
							break;
						case bidStable:
							stats.rsCav += AddMilBuilding(bdef, bd, bds, cc);
							break;
						case bidSorcTower:
							stats.rsMagic += AddMilBuilding(bdef, bd, bds, cc);
							break;
						case bidShipyard:
							stats.rsNavy += AddMilBuilding(bdef, bd, bds, cc);
							break;
						case bidBlacksmith:
							stats.rsArt += AddMilBuilding(bdef, bd, bds, cc);
							break;
						case bidMarketplace:
							stats.carts += (ushort)bdef.Trn[bd.bl];
							break;
						case bidPort:
							stats.ships += (ushort)bdef.Trn[bd.bl];
							break;
						case bidCottage:
							stats.cs += (ushort)bdef.Cs[bd.bl];
							break;
						case bidBarracks:
							stats.maxTs += bdef.Tc[bd.bl];
							break;
						case bidStorehouse:
							{
								var str = bdef.St[bd.bl];
								// base storage
								stats.storage += new Resources(str*100);

								foreach(var delta in Octant.deltas)
								{
										var cc1 = cc + delta;
										if(!cc1.isInCity)
											continue;

										var bd1 = bds[cc1];
										switch(bd1.bid)
										{
											case bidSmelter:
												stats.storage.iron += bd1.def.St[bd1.bl] * str ;
												break;
											case bidSawmill:
												stats.storage.wood += bd1.def.St[bd1.bl] * str ;
												break;
											case bidStonemason:
												stats.storage.stone += bd1.def.St[bd1.bl] * str;												
												break;
											case bidGrainMill:
												stats.storage.food += bd1.def.St[bd1.bl] * str;
												break;
										}
									
								}

							}
							break;
						default:
							break;
					}
					stats.points += (ushort)bdef.sc[bd.bl];

				}
			}
			if(stats.rsInf != 0)
				stats.rsInf += 100;
			if(stats.rsCav != 0)
				stats.rsCav += 100;
			if(stats.rsArt != 0)
				stats.rsArt += 100;
			if(stats.rsMagic != 0)
				stats.rsMagic += 100;
			if(stats.rsNavy != 0)
				stats.rsNavy += 100;
			stats.storage /= 100;
			stats.maxTs = (stats.maxTs * tsPercentMultipler)/100;
		
			//instance.ships.Text = $"{ships:N0}";
			//instance.carts.Text = $"{carts:N0}";

			//instance.maxTS.Text = $"{ (ts * tsMultipler)/100:N0}";

			//instance.stWood.Text = $"{stWood:N0}";
			//instance.stStone.Text = $"{stStone:N0}";
			//instance.stIron.Text = $"{stIron:N0}";
			//instance.stFood.Text = $"{stFood:N0}";
			//instance.cs.Text = $"{cs:N0}%";

			//if(SetParentVisible(rsInf != 0, instance.rsInf))
			//{
			//	rsInf += 100;
			//	var gain = 100.0 / rsInf.Max(1);
			//	instance.rsInf.Text = $"{rsInf:N0}%";
			//	instance.rtVanq.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttVanquisher].ps * gain);
			//	instance.rtRanger.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttRanger].ps * gain);
			//	instance.rtTriari.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttTriari].ps * gain);
			//}
			//if(SetParentVisible(rsMagic != 0, instance.rsMagic))
			//{
			//	rsMagic += 100;
			//	var gain = 100.0 / rsMagic.Max(1);
			//	instance.rsMagic.Text = $"{rsMagic:N0}%";
			//	instance.rtSorc.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttSorcerer].ps * gain);
			//	instance.rtDruid.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttDruid].ps * gain);
			//}
			//if(SetParentVisible(rsCav != 0, instance.rsCav))
			//{
			//	rsCav += 100;
			//	var gain = 100.0 / rsCav.Max(1);
			//	instance.rsCav.Text = $"{rsCav:N0}%";
			//	instance.rtScout.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttScout].ps * gain);
			//	instance.rtArb.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttArbalist].ps * gain);
			//	instance.rtHorse.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttHorseman].ps * gain);
			//}
			//if(SetParentVisible(rsArt != 0, instance.rsArt))
			//{
			//	rsArt += 100;
			//	var gain = 100.0 / rsArt.Max(1);
			//	instance.rsArt.Text = $"{rsArt:N0}%";
			//	instance.rtRam.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttRam].ps * gain);
			//	instance.rtBal.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttBallista].ps * gain);
			//	instance.rtCat.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttScorpion].ps * gain);
			//}
			//if(SetParentVisible(rsNavy != 0, instance.rsNavy))
			//{
			//	rsNavy += 100;
			//	var gain = 100.0 / rsNavy.Max(1);
			//	instance.rsNavy.Text = $"{rsNavy:N0}%";
			//	instance.rtStinger.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttStinger].ps * gain);
			//	instance.rtGalley.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttGalley].ps * gain);
			//	instance.rtWarship.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttWarship].ps * gain);
			//}
			//if(SetParentVisible(rsBlessed != 0, instance.rsBlessed))
			//{
			//	rsBlessed += 100;
			//	var gain = 100.0 / rsBlessed.Max(1);
			//	instance.rsBlessed.Text = $"{rsBlessed:N0}%";
			//	instance.rtPriestess.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttPriestess].ps * gain);
			//	instance.rtPrae.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttPraetor].ps * gain);
			//	instance.rtSen.Text = AUtil.FormatDurationFromSeconds(TroopInfo.all[ttSenator].ps * gain);
			//}
			static int AddMilBuilding(in BuildingDef bdef, in Building bd, Building[] bds, BuildC cc)
			{
				var rv = bdef.Ts[bd.bl];
				foreach(var delta in Octant.deltas)
					{
						var cc1 = cc + delta;
						if(!cc1.isInCity)
							continue;
						var bd1 = bds[cc1];
						if(bd1.bid == bidBarracks)
						{

							rv += bd1.def.Ts[bd1.bl];
						}

					
				}

				return rv;
			}
			this.stats = stats;
			AppS.DispatchOnUIThread( CityStats.instance.OnPropertyChanged );

		}
	}
}
