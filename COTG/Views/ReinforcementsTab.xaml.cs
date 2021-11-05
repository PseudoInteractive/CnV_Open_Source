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
	}
}
