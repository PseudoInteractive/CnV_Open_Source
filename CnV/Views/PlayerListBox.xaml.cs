using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;

using Syncfusion.UI.Xaml.Editors;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	public sealed partial class PlayerListBox:SfComboBox
	{
		public class ChangedEventArgs {
			internal PlayerId[] players;
		}

		internal PlayerId[] players = Array.Empty<PlayerId>();
		internal event EventHandler<ChangedEventArgs> Changed;
		public PlayerListBox() {
			this.InitializeComponent();
			
		}

		internal void SetPlayers(PlayerId[] ps) {

			var prior = this.players;
			// remove some
			foreach(var p in prior) {
				if(!ps.Contains(p)) {
					this.SelectedItems.Remove( Player.Get(p) );
				}
			}
			foreach(var p in ps) {
				if(!prior.Contains(p)) {
					this.SelectedItems.Add( Player.Get(p) );
				}
			}
			//Assert(ps.SequenceEqual(players));
			players = ps;
		}

		// Todo: Set players

		private void _SelectionChanged(object sender,Syncfusion.UI.Xaml.Editors.ComboBoxSelectionChangedEventArgs e) {
			players = this.SelectedItems.Select(a=> (a as Player).id).ToArray();
			Changed?.Invoke(this,new() { players=players });
		}

		
	}
}
