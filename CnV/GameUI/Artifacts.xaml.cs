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
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class Artifacts:Page
	{
		public Artifacts()
		{
			this.InitializeComponent();
			
		}

		private void titleComboBox_SelectionChanged(object sender,SelectionChangedEventArgs e)
		{
			var sel = titleComboBox.SelectedIndex;
			//if(sel >= 1)
			//	--sel;
			relicsList.ItemsSource = Artifact.all.Where(a => a.level == sel).ToList();
		}
	}
}
