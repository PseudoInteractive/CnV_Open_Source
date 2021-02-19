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
	//	public static ShareStringItem tempItem = new ShareStringItem("temp~na") { notes = "nothing selected" };
		static ShareStringItem current = new();
		//[ShareString.1.3]:
		//[/ShareString]
		
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
				city.CopyBuidingCacheToShareString();
			SetFromSS();


			var rv = await instance.ShowAsync2();
			
			// todo:  copy back sharestring

			if( rv == ContentDialogResult.Primary)
			{
				
				city.SetShareString(GetShareStringWithJson());
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
		static string GetShareStringWithJson()
		{
			return GetShareString() + JsonSerializer.Serialize(current);

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
			current = current.Clone();
			current.path = $"{Player.myName}~{current.title}";
			instance.shareString.Text=City.BuildingsToShareString(s.buildings,s.isOnWater);
		}
		private static void SetFromSS()
		{
			var city = City.GetBuild();
			var s = city.splitShareString;
			current = JsonSerializer.Deserialize<ShareStringItem>(s.json);
			current.shareString = s.ss;
			instance.shareString.Text = current.shareString ?? string.Empty;
			instance.description.Text = current.desc??string.Empty;
			instance.notes.Text = current.notes ?? string.Empty;
			
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
					current = i;
					instance.shareString.Text= i.shareString.Replace("\n", "", StringComparison.Ordinal).Replace("\r", "", StringComparison.Ordinal);
					instance.description.Text = i.desc;
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
			var title = current.title;
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
			
			Cosmos.ShareShareString(Player.myName, current.title,GetShareStringWithJson());
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
		public ShareStringItem() 
		{
			path = $"{Player.myName}~tba";
			title = "tba";
		}
		public ShareStringItem Clone()
		{
			return JsonSerializer.Deserialize<ShareStringItem>(ToString());

		}
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
		public ShareStringItem()
		{
			path = $"{Player.myName}~tba";
			title = "tba";
		}
		public ShareStringItem Clone()
		{
			return JsonSerializer.Deserialize<ShareStringItem>(ToString());

		}
		public override string ToString() => JsonSerializer.Serialize(this);

		public ShareStringItem(string path, string _notes, string _desc, string _share)
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
			for (int i = 0; i < dir.Length - 1; ++i)
			{
				pathSoFar = pathSoFar + '~' + dir[i];
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
