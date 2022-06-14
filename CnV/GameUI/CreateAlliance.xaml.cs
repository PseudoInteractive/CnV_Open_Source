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
	public sealed partial class CreateAlliance:DialogG
	{
		static CreateAlliance instance;
		protected override string title => "Found Alliance";
		public CreateAlliance()
		{
			this.InitializeComponent();
			instance = this;
		}
		internal static void ShowInstance()
		{
			var art = instance ?? new CreateAlliance();
			art.Show(true);
		}

		private async void Button_Click(object sender,RoutedEventArgs e)
		{
			var name = this.name.Text;
			var abbreviation = this.abbreviation.Text;
			if(abbreviation.Length< 2 || abbreviation.Length > 4 )
			{
				AppS.MessageBox("Abbreviation must be 2..4 characters");
				return;
			}
			if(name.Length< 1  )
			{
				AppS.MessageBox("Name must be >= 1 character");
				return;
			}
			// Todo: Interlock this
			var alliance = new Alliance(name,abbreviation,(AllianceId)Alliance.all.Length,0ul);
			await alliance.Upsert();
			Hide(true);
			await AppS.WaitWithProgress(2000,"Create Alliance");

			AppS.MessageBox(title:$"Founded {name}",hero:"UI/menues/newsletter/misc_newsletter_img_alliance.png", lightDismiss:false);
			await Alliance.UpdateAll();
			await Alliance.SetAlliance(Player.active,alliance.id,AllianceTitle.leader,false);

		}
	}
}
