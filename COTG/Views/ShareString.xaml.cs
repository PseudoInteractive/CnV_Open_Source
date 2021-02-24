using COTG.Game;
using COTG.Helpers;
using COTG.Services;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.Json;
using System.Text.Json.Serialization;
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
		//	public static ShareStringItem tempItem = new ShareStringItem("temp~na") { notes = "nothing selected" };
		//[ShareString.1.3]:
		//[/ShareString]
		public static (string root, string mid, string title) DecomposePath(string src)
		{
			if(src.IsNullOrEmpty())
				return (string.Empty, string.Empty, string.Empty);
			
			var vr = src.Split('~', StringSplitOptions.RemoveEmptyEntries);
			if(vr.Length >= 3)
			{
				var mid = vr.Skip(1).Take(vr.Length - 2);
				var midStr =String.Join('~', mid);
				return (vr[0], midStr, vr[vr.Length - 1]);
			}
			else if(vr.Length >= 2)
			{
				return (vr[0], string.Empty, vr[1]);
			}
			else if(vr.Length >= 1)
			{
				return (string.Empty, String.Empty, vr[0]);
			}
			else
			{
				return (string.Empty, string.Empty, string.Empty);
			}
		}
		private static string GetPath( (string root, string mid, string title) path)
		{
			return !path.mid.IsNullOrEmpty() ? path.root + '~' + path.mid +"~"+ path.title : path.root+ "~" +path.title;
		}
		public static string StripRoot( string path)
		{
			var strip = DecomposePath(path);
			return StripRoot(strip).subPath;
		}
		public static (string root, string subPath) StripRoot( (string root, string mid, string title) path)
		{
			return (path.root, path.mid.IsNullOrEmpty() ? path.title : path.mid + '~' + path.title);
		}

		public static bool IsValid(string ss)
		{
			if (ss.IsNullOrEmpty())
				return false;
			if (!ss.StartsWith(City.shareStringStart))
				return false;
			if (ss.Length < City.minShareStringLength)
				return false;
			return true;
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
			var city = City.GetBuild();
		//	instance.PlannerTeachingTip.Show();
			// todo: copy text 
			if(CityBuild.isPlanner)
				city.BuildingsCacheToShareString();
			SetFromSS();


			var rv = await instance.ShowAsync2();
			
			// todo:  copy back sharestring

			if( rv == ContentDialogResult.Primary)
			{
				
				city.SetShareString(instance.GetShareStringWithJson());
				city.SaveLayout();
				if (instance.onComplete.IsOn != CityBuild.isPlanner)
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
		string GetShareStringWithJson()
		{
			return GetShareString() + JsonSerializer.Serialize(new ShareStringMeta() { notes=notes.Text, desc=description.Text,path=  path.Text });

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
			var s = City.GetBuild();
			instance.shareString.Text=City.BuildingsToShareString(s.buildings,s.isOnWater);
		}
		private static void SetFromSS()
		{
			var city = City.GetBuild();
			var s = city.splitShareString;
			var meta = JsonSerializer.Deserialize<ShareStringMeta>(s.json);
			var path = DecomposePath(meta.path);
			instance.shareString.Text = s.ss ?? string.Empty;
			instance.description.Text = meta.desc ?? string.Empty;
			instance.notes.Text = meta.notes ?? string.Empty;
			instance.title.Text = path.title;
			instance.path.Text = GetPath( path);
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
					instance.path.Text = i.path;
					instance.title.Text = i.title;
					instance.notes.Text = i.notes;
					
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

		private void ShareClick(object sender, RoutedEventArgs e)
		{
			var title = this.title.Text;
			if(title.IsNullOrEmpty())
			{
				Note.Show("Please set title");
				return;
			}
			if(!IsValid(GetShareString()))
			{
				Note.Show("Share string is not valide");
				return;

			}
			
			Cosmos.ShareShareString(Player.myName, title,GetShareStringWithJson());
			Note.Show("Shared!");
		}
	}
	public class ShareStringItem
	{
		public string path { get; set; } = string.Empty;
		public static List<ShareStringItem> all = new();
		public string shareString { get; set; }
		[JsonIgnore]
		public string title { get; set; } = string.Empty;
		public string desc { get; set; }
		public string notes { get; set; }
		[JsonIgnore]
		public List<ShareStringItem> children { get; set; } = new();
		// group items
		public ShareStringItem(string _path)
		{
			path = _path;
			var dir = path.Split('~', StringSplitOptions.RemoveEmptyEntries);
			title = dir.Last();

		}
		//public ShareStringItem() 
		//{
		//	path = $"{Player.myName}~tba";
		//	title = "tba";
		//}
		public override string ToString() => JsonSerializer.Serialize(this);

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
	public struct ShareStringMeta
	{
		public string path { get; set; }
		public string desc { get; set; }
		public string notes { get; set; }

	}
}
