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
	public sealed partial class ToolTipWindow:UserControl
	{
		public static ToolTip tooltip;
		public static ToolTipWindow content;
		
		public ToolTipWindow()
		{
			this.InitializeComponent();
		}
		//const int width = 64+28;
		public static void TipChanged()
		{
			AppS.QueueOnUIThread( () =>
			{
				if(tooltip == null)
				{
					tooltip = new ToolTip();
					
					ToolTipService.SetToolTip(ShellPage.instance.toolTipHolder,tooltip);
					tooltip.Placement = PlacementMode.Bottom;
					content = new ToolTipWindow();
					tooltip.Content = content;
				//	tooltip.Placement = PlacementMode.Mouse;
				//	tooltip.VerticalOffset =32;
			//		tooltip.HorizontalOffset =-32;
				}
				string str;
				ImageSource i0;
				if(ToolTips.actionToolTip is not null)
				{
					(str,i0) = ToolTips.actionToolTip.WorldToolTip();
				}
				else
				{
					str = ToolTips.spotToolTip;
					i0 = ToolTips.spot?.avatarImage;
	//				i1=  null;
				}
				
				if(!str.IsNullOrEmpty())
				{
					content.t.Text = str;
					content.i0.Source = i0;
					//content.i1.Source = i0 == i1 ? null : i1;
					//tooltip.PlacementTarget =ShellPage.instance.commandBar;
					//tooltip.Placement = PlacementMode.Mouse;
					
				//	tooltip.VerticalOffset =32;
				//	tooltip.HorizontalOffset =-32;

					tooltip.IsOpen=true;

				}
				else
				{
					tooltip.IsOpen = false;
				}

			});

		}
	}
}
