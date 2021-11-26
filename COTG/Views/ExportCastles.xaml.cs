﻿using COTG.Game;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using static COTG.Game.Spot;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace COTG.Views
{
	public sealed partial class ExportCastles : ContentDialog
	{

		public static ExportCastles instance = new();
		public ExportCastles()
		{
			this.InitializeComponent();
		}
		public static async void Go()
		{
			SettingsPage.HideMe();
			{
				if (!await ContinentTagFilter.Show())
					return;

			}
			{ 
			   int sumDef=0;
			   int sumOff = 0;
			   int sumCastles=0;
			   int sumTemples = 0;


//			   if (instance == null)
//				   instance = new();
			   var rv = await instance.ShowAsync2();
			   if (rv != ContentDialogResult.Primary)
				   return;
			   var offence = SettingsPage.exportOffence;
			   var water = SettingsPage.exportWater;
			   var castles = SettingsPage.exportCastles;
			   var onlyTemples = SettingsPage.onlyTemples;
			   var headers = SettingsPage.exportHeaders;
				var exportPlayer = Player.FromNameOrNull(SettingsPage.exportPlayer);
				var exportAlliance = Alliance.NameToId(SettingsPage.exportAlliance);
			   var who = SettingsPage.exportWho;
			   var score = SettingsPage.exportScore;
			   ShellPage.WorkStart("Exporting");
			  await Task.Run(async () => {
				   List<int> alliances = new();
				  if (exportAlliance != 0)
				  {
					  alliances.Add(exportAlliance);
					  who = 6;
				  }
				  else if (exportPlayer != null)
				  {
					  alliances.Add(exportPlayer.alliance);
					  who = 6;
				  }
				  else
				  {
					  switch (who)
					  {
						  case 0:
						  case 1: alliances.Add(Alliance.MyId); break;
						  case 2:
							  alliances.AddRange(from a in Alliance.all.Keys where Alliance.GetDiplomacy(a) == Diplomacy.enemy select a);
							  break;
						  case 3:
							  alliances.AddRange(from a in Alliance.all.Keys where a != 0 && a != Alliance.myId select a);
							  break;
						  case 4:
							  alliances.AddRange(from a in Alliance.all.Keys where Alliance.GetDiplomacy(a) == Diplomacy.allied select a);
							  break;
						  case 5:
							  alliances.Add(0);
							  break;
						  default:
							  alliances.AddRange(Alliance.all.Keys);
							  break;
					  }
				  }
				   var onlyMe = who == 0;


				   var sb = new StringBuilder();
				   int counter = 0;
				  if (headers != false)
				  {
					  sb.AppendLine("Alliance\tPlayer\tContinent\tCoords\tCastle\tAcademy\tTemple\tWater\tOffense\tTroops\tTS\tPoints");

				  }
				
				  foreach (var alliance in alliances)
				   {
					  var allianceName = Alliance.all[alliance].name;
					
					   sb.AppendLine(allianceName);
					   foreach (var p in Player.all.Values)
					   {
						   if (p.alliance != alliance)
							   continue;
						   if (onlyMe && !p.isMe)
							   continue;
						  if (exportPlayer != null && exportPlayer != p)
							  continue;

						   if(headers.GetValueOrDefault() ==true )
							    sb.AppendLine($"---\t" + p.name);

						   foreach (var _cid in p.cities)
						   {
							  var cid = _cid;
							   if (!TestContinentFilter(cid))
								   continue;
							   var wi = World.GetInfoFromCid(cid);
							   if (castles!=0  && castles!= (wi.isCastle? 1: 2) )
								   continue;
							  if (onlyTemples && !wi.isTemple)
								  continue;
							  if ( water !=0 && water != (wi.isWater? 1: 2) )
								  continue;

							   var s = Spot.GetOrAdd(cid);
							   var c = await s.ClassifyEx(false);
							  //if(score.GetValueOrDefault() == true)
								 // await GetCity.Post()
							  var isO = c.IsOffense();
							  var isD = c.IsDefense();
							  if (offence!=0)
							  {
								  if (offence != (isO? 1 : 2) )
									  continue;
							  }


							  ++counter;
							  if ((counter % 10) == 0)
							  {
								  App.DispatchOnUIThreadLow(() => ShellPage.WorkUpdate($"Exporting .. {Alliance.all[alliance].name}, {p.name}, {counter}"));
								  if (App.IsEscDown())
								  {
									  Note.Show("Aborted");
									  break;
								  }
							  }
							  sb.Append($"{Alliance.all[alliance].name}\t{ p.name}\t{cid.CidToContinent()}\t{s.xy}\t{s.isCastle.As01()}\t{((s.isMine||Player.isSpecial)?(s.hasAcademy.GetValueOrDefault()?"Academy":"none") : "dunno") }\t{s.isTemple.As01()}\t{s.isOnWater.As01()}\t{(isO?1:isD?-1:0)}\t");
							   switch (c)
							   {
								   case Classification.sorcs:
									   sb.Append("Sorc\t");
									   break;
								   case Classification.druids:
									   sb.Append("Druid\t");
									   break;
								   case Classification.praetor:
									   sb.Append("Prae\t");
									   break;
								   case Classification.priestess:
									   sb.Append("Priest\t");
									   break;
								   // TODO
								   case Classification.arbs:
									  sb.Append("Arbs\t");
									  break;
								  case Classification.horses:
									   sb.Append("Horses\t");
									   break;

								   case Classification.se:
									   sb.Append("Scorp\t");
									   break;
								   case Classification.navy:
									   sb.Append("Warship?\t");
									   break;
								  case Classification.stingers:
									  sb.Append("Stinger?\t");
									  break;
								  case Classification.rt:
									  sb.Append("RT\t");
									  break;
								  case Classification.vanqs:
									  sb.Append("Vanq\t");
									  break;
								  case Classification.hub:
									  sb.Append("Hub\t");
									  break;
								  default:
									   sb.Append("wtf\t");
									   break;
							   }
							  sb.Append($"{s.tsTotal}\t{ s.points}");

							  sb.AppendLine();
						   }
					   }
				   }
				   App.CopyTextToClipboard(sb.ToString());
				   ShellPage.WorkEnd("Exporting");

				   Note.Show($"Copied {counter} castles to clipboard for sheets");

			   });
		   }
		   
		}
	}
}
