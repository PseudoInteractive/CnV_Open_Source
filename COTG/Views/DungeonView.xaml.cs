using COTG.Game;

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
using COTG.Services;
using static COTG.Debug;
using static COTG.Game.Enum;
using static COTG.Views.SettingsPage;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace COTG.Views
{
	public sealed partial class DungeonView : ContentDialog
	{
		const int raidStepCount = 12;
		

		public static int openCity;
		public static DungeonView instance;
		public static void Initialize()
		{
			Dungeon.Initialize();
			instance = new();
		}
		static void AddStep()
		{

		}
		public DungeonView()
		{
			this.InitializeComponent();
			if (raidSteps == null)
			{
				raidSteps = new float[raidStepCount];
				for (int i = 0; i <= 4; ++i)
				{
					raidSteps[i] = SettingsPage.raidCarryMin * (i * 0.25f).Lerp(0.5f, 1.0f);
				}
				for (int i = 5; i <= 8; ++i)
				{
					raidSteps[i] = ((i - 4) * 0.25f).Lerp(SettingsPage.raidCarryMin, SettingsPage.raidCarryMax);
				}
				for (int i = 9; i < raidStepCount; ++i)
				{
					raidSteps[i] = SettingsPage.raidCarryMax + (i - 9) * 15; ;
				}
			}
			raidCarryMinBox.ItemsSource = raidSteps;
			raidCarryMinBox.SelectedIndex = 4;
			raidCarryMaxBox.ItemsSource = raidSteps;
			raidCarryMaxBox.SelectedIndex = 9;
		}

		public static bool IsVisible() => openCity != 0;

		public static void Close()
		{
			instance.Hide();
		}
		public static void RefreshDungeonList(int cid=0)
		{
			if(openCity!=0 && ((cid == openCity) || cid == 0 ) )
				Dungeon.raidDungeons.NotifyReset();

		}
		public static async Task Show(City city)
		{
			await AApp.popupSema.WaitAsync();
			try
			{
				await App.DispatchOnUIThreadTask(async () =>
			   {
				   openCity = city.cid;
				   instance.Title = city.nameAndRemarks;

				   await instance.ShowAsync();
			   });
			}
			finally
			{
				openCity = 0;
				AApp.popupSema.Release();
			}
		}

		private void RaidFraction_ValueChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
		{
			if (JSClient.ppdtInitialized)
				Raiding.UpdateTS(true, true);

		}

		private static void AddRaidStep(float v )
		{
			float bestError = float.MaxValue;
			var bestId = 0;
			for (int i = 0; i < raidStepCount; ++i)
			{
				float d = (v - raidSteps[i]).Abs();
				if (d < bestError)
				{
					bestError = d;
					bestId = i;
				}
			}
			raidSteps[bestId] = v;

		}


		private void RaidCarryMaxSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		{
			//     Log("Submit: " + args.Text);
			if (float.TryParse(args.Text, System.Globalization.NumberStyles.Number, null, out float _raidCarry))
			{
				AddRaidStep(_raidCarry);
				//raidSteps;
				if (SetCarryMax(_raidCarry))
				{
					//if(raidCity!=null)
					//    ScanDungeons.Post(raidCity.cid,false) ;
					RefreshDungeonList();
				}
			}
			else
			{
				args.Handled = true;
				Assert(false);
			}
		}


		private void RaidCarrySubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		{
			//     Log("Submit: " + args.Text);
			if (float.TryParse(args.Text, System.Globalization.NumberStyles.Number, null, out float _raidCarry))
			{
				AddRaidStep(_raidCarry);

				//raidSteps;
				if (SetCarry(_raidCarry))
				{
					//if(raidCity!=null)
					//    ScanDungeons.Post(raidCity.cid,false) ;
					RefreshDungeonList();
				}
			}
			else
			{
				args.Handled = true;
				Assert(false);
			}
		}

		public static void UpdateRaidPlans()
		{
			//// instance.Dispatcher.DispatchOnUIThread(() =>
			// {
			//     // trick it
			//     var temp = instance.dungeonGrid.ItemsSource;
			//     instance.dungeonGrid.ItemsSource = null;
			//     instance.dungeonGrid.ItemsSource = temp;
			// }
			// // tell UI that list data has changed
			Dungeon.raidDungeons.NotifyReset();
		}
		private static bool SetCarry(float src)
		{
			var newVal = (src) * 0.01f;
			if ((newVal - SettingsPage.raidCarryMin).Abs() <= 1.0f / 256.0f)
				return false;
			SettingsPage.raidCarryMin = newVal;
			SettingsPage.SaveAll();
			return true;
		}
		private static bool SetCarryMax(float src)
		{
			var newVal = (src) * 0.01f;
			if ((newVal - SettingsPage.raidCarryMax).Abs() <= 1.0f / 256.0f)
				return false;
			SettingsPage.raidCarryMax = newVal;
			SettingsPage.SaveAll();
			return true;
		}
		private void RaidCarrySelChanged(object sender, SelectionChangedEventArgs e)
		{
			//   Log("Sel update");
			if (e.AddedItems != null && e.AddedItems.Count > 0)
			{
				if (SetCarry((float)e.AddedItems[0]))
				{
					UpdateRaidPlans(); //Log("Sel changed");
									   //if (raidCity != null)
									   //    ScanDungeons.Post(raidCity.cid,false);
				}
			}
		}

		private void IncludeButtonClick(object sender, RoutedEventArgs e)
		{
			var button = sender as Microsoft.UI.Xaml.Controls.DropDownButton;
			var flyout = new MenuFlyout();
			for (int i = 0; i < ttCount; ++i)
			{
				if (IsRaider(i))
				{
					var but = new ToggleMenuFlyoutItem() { IsChecked = SettingsPage.includeRaiders[i], DataContext = (object)i, Text = ttNameWithCaps[i] };
					flyout.Items.Add(but);
				}
			}
			flyout.CopyXamlRoomFrom(button);
			flyout.Closing += Flyout_Closing;
			flyout.ShowAt(button);
		}

		private void Flyout_Closing(Windows.UI.Xaml.Controls.Primitives.FlyoutBase sender, Windows.UI.Xaml.Controls.Primitives.FlyoutBaseClosingEventArgs args)
		{
			var menu = (sender as MenuFlyout);
			int counter = 0;
			for (int i = 0; i < ttCount; ++i)
			{
				if (IsRaider(i))
				{
					var but = menu.Items[counter] as ToggleMenuFlyoutItem;
					SettingsPage.includeRaiders[i] = but.IsChecked;
					++counter;
				}
			}
			Raiding.UpdateTS(true, true);
		}
	}
}
