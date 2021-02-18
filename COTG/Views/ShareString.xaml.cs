using COTG.Game;
using COTG.Helpers;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using static COTG.Debug;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace COTG.Views
{
	public sealed partial class ShareString : ContentDialog
	{
		static public ShareString instance;
		public ShareString()
		{
			instance = this;
			this.InitializeComponent();
			
		}

		
		static public async Task<bool> Show()
		{
			if (instance == null)
			{
				try
				{
					new ShareString();
				}
				catch(Exception ex)
				{
					Log(ex);
				}
				AddLayouts();
			}
			instance.onComplete.IsOn = CityBuild.isPlanner;
		//	instance.PlannerTeachingTip.Show();
			// todo: copy text 
			instance.shareString.Text = City.GetBuild().LayoutToShareString();
			var rv = await instance.ShowAsync2();
			
			// todo:  copy back sharestring

			if( rv == ContentDialogResult.Primary)
			{
				var city = City.GetBuild();
				city.SetLayoutFromShareString(GetShareString());
				if(instance.onComplete.IsOn != CityBuild.isPlanner)
				{
					if (CityBuild.isPlanner)
						PlannerTab.instance.Close();
					else
						PlannerTab.instance.Show();
				}
				return true;
			}
			else
			{
				return false;
			}
		}
		static string GetShareString()
		{
			return instance.shareString.Text.Replace("\n", "", StringComparison.Ordinal).Replace("\r", "", StringComparison.Ordinal);

		}
		private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
		{
		}

		private void UseBuildingsClick(object sender, RoutedEventArgs e)
		{
			instance.shareString.Text=City.GetBuild().BuildingsToShareString(); 
		}

		private async void FromClipboardClick(object sender, RoutedEventArgs e)
		{
			var text = await App.GetClipboardText();
			if (text == null)
			{
				Note.Show("Clipboard is empty");

			}
			else
			{
				Note.Show($"New Sharestring: {text}");
				instance.shareString.Text = text;
			}
		}

		private void ToClipboardClick(object sender, RoutedEventArgs e)
		{
			App.CopyTextToClipboard(GetShareString());
		}

		private void ShareItemInvoked(TreeView sender, TreeViewItemInvokedEventArgs args)
		{
			var i = args.InvokedItem as ShareStringItem;
			Assert(i != null);
			if(i!=null)
			{
				if(i.shareString!= null)
				{
					instance.shareString.Text= i.shareString.Replace("\n", "", StringComparison.Ordinal).Replace("\r", "", StringComparison.Ordinal);
					instance.description.Text = i.desc;
					instance.remarks.Text = i.notes;
				}
			}
		}

		private void TogglePlannerClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
		{
			onComplete.IsOn = true;
		}

		private void ClearClick(object sender, RoutedEventArgs e)
		{
			shareString.Text = string.Empty;
		}
	}
	public class ShareStringItem
	{
		public string path;
		public static List<ShareStringItem> all = new();
		public string shareString { get; set; }
		public string title { get; set; }
		public string desc { get; set; }
		public string notes { get; set; }
		public List<ShareStringItem> children { get; set; } = new();
		// group items
		public ShareStringItem(string _path)
		{
			path = _path;
			var dir = path.Split('~', StringSplitOptions.RemoveEmptyEntries);
			title = dir.Last();

		}

		public ShareStringItem(string path,string _notes,string _desc, string _share)
		{
			this.path = path;
			notes = _notes;
			title = path;
			desc = _desc;
			shareString = _share;
			var dir = path.Split('~', StringSplitOptions.RemoveEmptyEntries);
			title = dir.Last();
			var myList = all;
			var pathSoFar = String.Empty;
			for(int i=0;i<dir.Length-1;++i)
			{
				pathSoFar = pathSoFar +'~' + dir[i];
				var parent = myList.Find((a) => a.title == dir[i]);
				if (parent == null)
				{
					parent = new ShareStringItem(pathSoFar);
					myList.Add(parent);
				}
				myList = parent.children;
			}
			myList.Add(this);
		
		}
	}
}
