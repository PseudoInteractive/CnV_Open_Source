using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using COTG.Game;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.Toolkit.Mvvm.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace COTG.Views
{

	public class ReinforceMentVM : ObservableObject, IEquatable<ReinforceMentVM>
	{
		public Reinforcement r;

		public bool Equals(ReinforceMentVM other)
		{
			return r.Equals(other);
		}
	}

	public sealed partial class ReinforcementsTab:UserTab
	{
		public override TabPage defaultPage => TabPage.secondaryTabs;
		public string reinInTitle;
		public string reinOutTitle;
		public NotifyCollection<Reinforcement> reinforcementsOut= new();
		public NotifyCollection<Reinforcement> reinforcementsIn = new();
		public static ReinforcementsTab instance;
		
		public ReinforcementsTab()
		{
			instance = this;
			this.InitializeComponent();
		}
		public void ReturnClick(object obj)
		{
			var r = (obj as Reinforcement);
			Assert(r!=null);
			Note.Show($"Returning {r.troopsString} from {r.targetCity} back to {r.sourceCity} ");
			r.ReturnAsync();
		}

		private void reinIn_CellTapped(object sender,Syncfusion.UI.Xaml.DataGrid.GridCellTappedEventArgs e)
		{

			try
			{
				Note.Show($"Cell Tap {e.Column.HeaderText??"NA"}  {e.RowColumnIndex} {e.RowColumnIndex} {e.Record.ToString} ");
				switch(e.Column.HeaderText)
				{
					case "Return": 
					{
						var r = e.Record as Reinforcement;

						r.ReturnAsync();
						break;

					}
					case nameof(Reinforcement.sourceCity):
					{

						var r = (Reinforcement)e.Record;
						var s = r.sourceCity;
						s.DoClick();
//						Spot.ProcessCoordClick(s.cid,true,)
						//					r.ReturnAsync();
						break;

					}
					case nameof(Reinforcement.targetCity):
					{

						var r = (Reinforcement)e.Record;
						var s = r.targetCity;
						s.DoClick();
						//						Spot.ProcessCoordClick(s.cid,true,)
						//					r.ReturnAsync();
						break;

					}
				}
			}
			catch (Exception exception)
			{
				Log(exception);
				throw;
			}



		}

		private void CelNavigate(object sender,Syncfusion.UI.Xaml.Grids.CurrentCellRequestNavigateEventArgs e)
		{
			var uri = new Uri(e.NavigateText);

			if (uri.AbsolutePath == Reinforcement.retUriS)
			{
				e.Handled=true;
				var args = new WwwFormUrlDecoder(e.NavigateText);
				
	//			var args = Uri.Par
				Reinforcement.ReturnAsync( args.GetFirstValueByName("order").ParseLong().GetValueOrDefault() , args.GetFirstValueByName("pid").ParseInt().GetValueOrDefault());
				

			}
		}

		private void CellToolTipOpening(object sender,Syncfusion.UI.Xaml.DataGrid.GridCellToolTipOpeningEventArgs e)
		{

		}
	}
}
