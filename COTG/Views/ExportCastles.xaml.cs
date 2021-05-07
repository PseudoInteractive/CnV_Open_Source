using COTG.Game;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using static COTG.Game.Spot;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace COTG.Views
{
	public sealed partial class ExportCastles : ContentDialog
	{
		public static ExportCastles instance;
		public ExportCastles()
		{
			this.InitializeComponent();
		}
		public static void Go()
		{
			App.DispatchOnUIThreadSneaky( async () =>
		   {
			   SettingsPage.HideMe();

			   if (instance == null)
				   instance = new();
			   var rv = await instance.ShowAsync2();
			   if (rv != ContentDialogResult.Primary)
				   return;
			   var offence = instance.offence.IsChecked;
			   var water= instance.water.IsChecked;
			   var castles = instance.castles.IsChecked;
			   var temples = instance.temples.IsChecked;
			   var headers = instance.headers.IsChecked;
			   
			   var who = instance.who.SelectedIndex;
			   ShellPage.WorkStart("Exporting");
			  await Task.Run(async () => {
				   List<int> alliances = new();
				   switch (who)
				   {
					   case 0:
					   case 1: alliances.Add(Alliance.MyId); break;
					  case 2:
						  alliances.AddRange(from a in Alliance.all.Keys where Alliance.GetDiplomacy(a)== Diplomacy.enemy  select a);
						  break;
					  case 3: alliances.AddRange(from a in Alliance.all.Keys where a != 0 && a != Alliance.myId select a);
						   break;
					  case 4:
						  alliances.AddRange(from a in Alliance.all.Keys where Alliance.GetDiplomacy(a) == Diplomacy.allied select a);
						  break;
					  case 5: alliances.Add(0);
						   break;
					  default:
						   alliances.AddRange(Alliance.all.Keys);
						   break;
				   }
				   var onlyMe = who == 0;


				   var sb = new StringBuilder();
				   int counter = 0;
				   if(headers.GetValueOrDefault())
				  {
					  sb.AppendLine("Alliance\tPlayer\tContinent\tCoords\tCastle\tTemple\tWater\tOffense\tHasAcademy\tTroops\tTS\tPoints");

				  }

				  foreach (var alliance in alliances)
				   {
					   sb.AppendLine(Alliance.all[alliance].name);
					   foreach (var p in Player.all.Values)
					   {
						   if (p.alliance != alliance)
							   continue;
						   if (onlyMe && !p.isMe)
							   continue;

						   if(!headers.HasValue||headers.Value==true )
							    sb.AppendLine($"---\t" + p.name);


						   foreach (var _cid in p.cities)
						   {
							  var cid = _cid;
							   if (!TestContinentFilter(cid))
								   continue;
							   var wi = World.GetInfoFromCid(cid);
							   if (castles.HasValue && castles.Value != wi.isCastle)
								   continue;
							  if (temples.HasValue && temples.Value != wi.isTemple)
								  continue;
							  if ( water.HasValue && water.Value != wi.isWater)
								  continue;

							   var s = Spot.GetOrAdd(cid);
							   var c = await s.ClassifyEx(offence.GetValueOrDefault());

							  var isO = c.IsOffense();
							  var isD = c.IsDefense();
							  if (offence.HasValue)
							  {
								  if (offence.Value != isO)
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
							  sb.Append($"{Alliance.all[alliance].name}\t{ p.name}\t{cid.CidToContinent()}\t{s.xy}\t{s.isCastle.As01()}\t{s.isTemple.As01()}\t{s.isOnWater.As01()}\t{(isO?1:isD?-1:0)}\t{s.hasAcademy.GetValueOrDefault().As01()}\t");
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
								   case Classification.horses:
									  sb.Append("Arbs\t");
									  break;
								  case Classification.arbs:
									   sb.Append("Horses\t");
									   break;

								   case Classification.se:
									   sb.Append("Scorp\t");
									   break;
								   case Classification.navy:
									   sb.Append("Warship\t");
									   break;
								  case Classification.stingers:
									  sb.Append("Stinger\t");
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
							   sb.AppendLine($"{s.tsTotal}\t{s.points}");

						   }
					   }
				   }
				   App.CopyTextToClipboard(sb.ToString());
				   ShellPage.WorkEnd("Exporting");

				   Note.Show($"Copied {counter} castles to clipboard for sheets");

			   });
		   }
		   );
		}
	}
}
