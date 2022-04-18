using CnVDiscord;

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

using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	public sealed partial class JoinAlliance:DialogG
	{
		static JoinAlliance instance;
		protected override string title => "Join Alliance";
		public JoinAlliance()
		{
			this.InitializeComponent();
			instance = this;
		}
		internal static void ShowInstance()
		{
			var art = instance ?? new JoinAlliance();
			var p = Player.active;
			if(p.alliance.isValid)
			{
				art.leave.Visibility = Visibility.Visible;
				art.leave.Content = $"Leave {p.alliance.name}";
			}
			else
			{
				art.leave.Visibility = Visibility.Collapsed;

			}
			art.invites.ItemsSource = p.allianceInvites;
//			art.invites.ItemsSource = Alliance.all.Where(a=>a.id != 0).Select(a=> new CnVEventAllianceInvite(p.id,a.id,AllianceTitle.leader,false)).ToArray();
		//	if(p.alliance.isValid)
		//		art.alliance.SelectedItem =p.alliance;
		//	art.allianceTitle.SelectedIndex = (int) p.allianceTitle;
			art.Show(true);
		}

		//private async void Button_Click(object sender,RoutedEventArgs e)
		//{
		//	try
		//	{
		//		var sel = this.alliance.SelectedItem as Alliance;

		//		if(sel == null)
		//		{
		//			AppS.MessageBox("Must select Alliance");
		//			return;
		//		}
		//		Hide(true);
		//		await Go(Player.active,sel.id,(AllianceTitle)allianceTitle.SelectedIndex);

		//	}
		//	catch(Exception _ex)
		//	{
		//		LogEx(_ex);

		//	}


		//}

		internal static  async Task Go(Player p,AllianceId sel,AllianceTitle title)
		{
		
			
			var entity = new PlayerEntity(p.pid,p.id,World.id) { allianceId =sel,allianceTitle=(AllianceTitleAzure)title };
			var t0= PlayerEntity.table.UpsertAsync(entity);
			// Do we need a delay in here?

			new CnVEventAlliance(p.id,sel,title).EnqueueAsap();
			await Task.Delay(1000);
			await Task.WhenAll(t0, CnVChatClient.UpdatePlayerAlliance(p));
		}

		private void InviteClick(object sender,ItemClickEventArgs e)
		{
			var i = e.ClickedItem as CnVEventAllianceInvite;
			Hide(true);
			Go(Player.active,i.allianceId,i.title);


		}

		private void LeaveAlliance(object sender,RoutedEventArgs e)
		{
			Hide(true);
			Go(Player.active,0,0);
		}
	}
}
