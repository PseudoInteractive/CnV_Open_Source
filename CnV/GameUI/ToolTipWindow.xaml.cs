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
	public sealed partial class ToolTipWindow:DialogG
	{
		protected override string title => "tmi";
		public static ToolTip tooltip;
		public static ToolTipWindow instance;
		
		public ToolTipWindow()
		{
			this.InitializeComponent();
			instance = this;
		}
		const int width = 64+28;
		public static void ShowInstance()
		{
			var rv = instance ?? new ToolTipWindow();
			Canvas.SetLeft(rv, (AGame.clientSpan.X-width));
			rv.Show(true);
			
		}
		public static void TipChanged()
		{
			AppS.QueueOnUIThread( () =>
			{
				if(tooltip == null)
				{
					tooltip = new ToolTip();
					ToolTipService.SetToolTip(ShellPage.gameUIFrame,tooltip);
					tooltip.PlacementTarget = null;// ShellPage.canvas;
					tooltip.Placement = PlacementMode.Mouse;

				}
				var str = ToolTips.toolTip;
				tooltip.IsOpen = false;
				if(!str.IsNullOrEmpty())
				{
					tooltip.Content = str;
					//tooltip.Placement = PlacementMode.Mouse;
					
					tooltip.IsOpen=true;

				}

			});

		}
	}
}
