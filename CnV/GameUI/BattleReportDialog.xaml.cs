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
	using static Troops;
	public sealed partial class BattleReportDialog:DialogG
	{
		protected override string title => "Battle Report";
		internal BattleReport b;
		internal string attackType => $"↓ {b.attackArmy.typeS} ratio: {b.attackRatio:P} ↓";

		public BattleReportDialog(BattleReport _b)
		{
			this.b = _b;
			this.InitializeComponent();
		}

		
		public static void ShowInstance(BattleReport _b)
		{
			var rv = new BattleReportDialog(_b);
			
			rv.Show(false) ;
			
		}


	}
	

}
