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

			   var who = instance.who.SelectedIndex;
			   ShellPage.WorkStart("Exporting");
			  await Task.Run(async () => {
				   List<int> alliances = new();
				   switch (who)
				   {
					   case 0:
					   case 1: alliances.Add(Alliance.MyId); break;
					   case 2: alliances.AddRange(from a in Alliance.all.Keys where a != 0 && a != Alliance.myId select a);
						   break;
					   case 3: alliances.Add(0);
						   break;
					   case 4:
						   alliances.AddRange(Alliance.all.Keys);
						   break;
				   }
				   var onlyMe = who == 0;


				   var sb = new StringBuilder();
				   int counter = 0;

				   foreach (var alliance in alliances)
				   {
					   sb.AppendLine(Alliance.all[alliance].name);
					   foreach (var p in Player.all.Values)
					   {
						   if (p.alliance != alliance)
							   continue;
						   if (onlyMe && !p.isMe)
							   continue;

						   sb.AppendLine("\t" + p.name);


						   foreach (var _cid in p.cities)
						   {
							   if (!TestContinentFilter(_cid))
								   continue;
							   var wi = World.GetInfoFromCid(_cid);
							   if (castles.HasValue && castles.Value != wi.isCastle)
								   continue;
							  if (temples.HasValue && temples.Value != wi.isTemple)
								  continue;
							  if ( water.HasValue && water.Value != wi.isWater)
								  continue;

							   var s = Spot.GetOrAdd(_cid);
							   var c = await s.ClassifyEx(true);

							  if (offence.HasValue)
							  {
								  if (offence.Value != c switch
								  {
									  Classification.sorcs 
									  or Classification.vanqs
									  or Classification.druids
									  or Classification.horses
									  or Classification.navy
									  or Classification.se => true,
									  _ => false
								  })
								  {
									  continue;
								  }
							  }


							  ++counter;

							  sb.Append($"\t\t{_cid.CidToContinent()}\t");
							   switch (c)
							   {
								   case Classification.sorcs:
									   sb.Append("Sorc\t");
									   break;
								   case Classification.druids:
									   sb.Append("Druids\t");
									   break;
								   case Classification.praetor:
									   sb.Append("prae\t");
									   break;
								   case Classification.priestess:
									   sb.Append("priest\t");
									   break;
								   // TODO
								   case Classification.horses:
								   case Classification.arbs:
									   sb.Append("Horses\t");
									   break;

								   case Classification.se:
									   sb.Append("Siege engines\t");
									   break;
								   case Classification.navy:
									   sb.Append("Warships\t");
									   break;
								  case Classification.stingers:
									  sb.Append("Stingers\t");
									  break;
								  default:
									   sb.Append("vanq\t");
									   break;
							   }
							   sb.Append(s.tsTotal + "\t");
							   sb.Append(s.hasAcademy.GetValueOrDefault() ? "Yes\t" : "No\t");
							   sb.Append(s.xy + "\n");

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
