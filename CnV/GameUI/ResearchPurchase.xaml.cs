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
	
	public sealed partial class ResearchPurchase:DialogG, INotifyPropertyChanged
	{
		public static ResearchPurchase? instance;
		internal TechTreeStep r;
		public ResearchPurchase()
		{
			this.InitializeComponent();
			instance=this;
		}
		internal static void ShowInstance(TechTreeStep r)
		{
			var rv = instance ?? new ResearchPurchase();
			rv.r = r;
			rv.OnPropertyChanged();
			rv.Show();
			
		}
		private string Req(int r)
		{
			var have = Player.me.RefinesOrGold(r);
			return $"{have.Format()}, require {this.r.R(r).Format()}";
		}

			public event PropertyChangedEventHandler? PropertyChanged;
	public void OnPropertyChanged(string? member = null)
	{
		if (this.PropertyChanged is not null) 
			AppS.DispatchOnUIThread(() => PropertyChanged?.Invoke(this,new(member)));
	}
		public static void Changed(string? member = null)
		{
			if(instance is not null)
				instance.OnPropertyChanged(member);
		}

		bool hasEnough { get {
				for(int r=0;r<5;++r)
				{
					if(Player.me.RefinesOrGold(r) < this.r.R(r) )
						return false;
				}
				return true;
			} }
		private void DoResarch(object sender,RoutedEventArgs e)
		{
			Assert(hasEnough);
			new CnVEventResearch(r.id).Execute();
			Hide();
		}
	}
}
