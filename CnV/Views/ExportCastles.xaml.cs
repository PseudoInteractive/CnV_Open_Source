using CnV.Game;

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

using static CnV.City;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CnV.Views
{
	using Game;

	public sealed partial class ExportCastles : ContentDialog
	{

		public static ExportCastles instance = new();
		public ExportCastles()
		{
			this.InitializeComponent();
		}
		public static async void Go()
		{
			Settings.HideMe();
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
			   var offence = Settings.exportOffence;
			   var water = Settings.exportWater;
			   var castles = Settings.exportCastles;
			   var onlyTemples = Settings.onlyTemples;
			   var headers = Settings.exportHeaders;
				var exportPlayer = Player.FromNameOrNull(Settings.exportPlayer);
				var exportAlliance = Alliance.NameToId(Settings.exportAlliance);
			   var who = Settings.exportWho;
			   var score = Settings.exportScore;
			   ShellPage.WorkStart("Exporting");
			  await Task.Run( () => {
				   List<AllianceId> alliances = new();
				  if (exportAlliance != 0)
				  {
					  alliances.Add(exportAlliance);
					  who = 6;
				  }
				  else if (exportPlayer != null)
				  {
					  alliances.Add(exportPlayer.allianceId);
					  who = 6;
				  }
				  else
				  {
					  switch (who)
					  {
						  case 0:
						  case 1: alliances.Add(Alliance.MyId); break;
						  case 2:
							  alliances.AddRange(from a in Alliance.all.Select(a=>a.id) where Alliance.GetPlayerDiplomacyWith(a) == Diplomacy.foe select a);
							  break;
						  case 3:
							  alliances.AddRange(from a in Alliance.all.Select(a=>a.id) where a != 0 && a != Alliance.myId select a);
							  break;
						  case 4:
							  alliances.AddRange(from a in Alliance.all.Select(a=>a.id) where Alliance.GetPlayerDiplomacyWith(a) == Diplomacy.ally select a);
							  break;
						  case 5:
							  alliances.Add(0);
							  break;
						  default:
							  alliances.AddRange(Alliance.all.Select(a=>a.id)); // include lawless?
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
					  var allianceName = Alliance.Get(alliance).name;
					
					   sb.AppendLine(allianceName);
					   foreach (var p in Player.all)
					   {
						   if (p.allianceId != alliance)
							   continue;
						   if (onlyMe && !p.isMe)
							   continue;
						  if (exportPlayer != null && exportPlayer != p)
							  continue;

						   if(headers.GetValueOrDefault() ==true )
							    sb.AppendLine($"---\t" + p.name);

						   foreach (var _cid in p.cities)
						   {
							  Assert(false);
							  var cid = _cid;
							   if (!TestContinentFilter(cid))
								   continue;
							   var wi =  World.GetTile(cid.CidToWorldC()) ;
							  //var wi =  World.GetInfo(cid) ;
							   if (castles!=0  && castles!= (wi.isCastle? 1: 2) )
								   continue;
							  if (onlyTemples && !wi.isTemple)
								  continue;
							  if ( water !=0 && water != (wi.isOnWater? 1: 2) )
								  continue;

							   var s = Spot.GetOrAdd(cid);
							  // Assert false
							  var c = s.classification; // s.Classify(false);
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
								  AppS.QueueOnUIThreadIdle(() => ShellPage.WorkUpdate($"Exporting .. {Alliance.all[alliance].name}, {p.name}, {counter}"));
								  if (AppS.IsEscDown())
								  {
									  Note.Show("Aborted");
									  break;
								  }
							  }
							  sb.Append($"{Alliance.all[alliance].name}\t{ p.name}\t{cid.CidToContinentDigits()}\t{s.xy}\t{s.isCastle.As01()}\t{((s.isMine)?(s.hasAcademy.GetValueOrDefault()?"Academy":"none") : "dunno") }\t{s.isTemple.As01()}\t{s.isOnWater.As01()}\t{(isO?1:isD?-1:0)}\t");
							   switch (c)
							   {
								   case Spot.Classification.sorcs:
									   sb.Append("Sorc\t");
									   break;
								   case Spot.Classification.druids:
									   sb.Append("Druid\t");
									   break;
								   case Spot.Classification.praetor:
									   sb.Append("Prae\t");
									   break;
								   case Spot.Classification.priestess:
									   sb.Append("Priest\t");
									   break;
								   // TODO
								   case Spot.Classification.arbs:
									  sb.Append("Arbs\t");
									  break;
								  case Spot.Classification.horses:
									   sb.Append("Horses\t");
									   break;

								   case Spot.Classification.se:
									   sb.Append("Scorp\t");
									   break;
								   case Spot.Classification.navy:
									   sb.Append("Warship?\t");
									   break;
								  case Spot.Classification.stingers:
									  sb.Append("Stinger?\t");
									  break;
								  case Spot.Classification.rt:
									  sb.Append("RT\t");
									  break;
								  case Spot.Classification.vanqs:
									  sb.Append("Vanq\t");
									  break;
								  case Spot.Classification.hub:
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
				   AppS.CopyTextToClipboard(sb.ToString());
				   ShellPage.WorkEnd("Exporting");

				   Note.Show($"Copied {counter} castles to clipboard for sheets");

			   });
		   }
		   
		}
	}
}
