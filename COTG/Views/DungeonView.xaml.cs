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
using COTG.Helpers;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace COTG.Views
{
	public sealed partial class DungeonView : ContentDialog
	{
		const int raidStepCount = 15;

		ResetableCollection<Dungeon> items = new();

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
			if (raidCarrySteps == null)
			{
				raidCarrySteps = new int[raidStepCount];
				int put = 85;
				for (int i = 0; i < raidStepCount/3+2; ++i)
				{
					raidCarrySteps[i] = put;
					put += 5;
				}
				for (int i = raidStepCount / 3+2; i< raidStepCount*2 / 3; ++i)
				{
					raidCarrySteps[i] = put;
					put += 15;
				}
				for (int i = 2*raidStepCount / 3; i < raidStepCount ; ++i)
				{
					raidCarrySteps[i] = put;
					put += 25;
				}
			}
			raidCarryMinBox.ItemsSource = raidCarrySteps;
			raidCarryMaxBox.ItemsSource = raidCarrySteps;
			raidCarryMinBox.SelectedIndex = raidCarrySteps.IndexOfClosest( (raidCarryMin*100).RoundToInt() );
			raidCarryMaxBox.SelectedIndex = raidCarrySteps.IndexOfClosest((raidCarryMax * 100).RoundToInt());
		}

		public static bool IsVisible() => openCity != 0;

		public static void Close()
		{
			instance.Hide();
		}
		static bool hasRunOnce;
		public static async Task Show(City city, List<Dungeon> dungeons)
		{
			if (openCity != 0)
			{
				if (city.cid == openCity)
				{
					App.DispatchOnUIThreadLow(() => instance.items.Set(dungeons));
				}
				return;
			}
			await AApp.popupSema.WaitAsync();
			try
			{
				await App.DispatchOnUIThreadTask(async () =>
			   {
				   Log(city.nameAndRemarks);
				   openCity = city.cid;
				   instance.Title = city.nameAndRemarks;
				   instance.items.Set( dungeons);
				   if(!hasRunOnce)
				   {
					   hasRunOnce = true;
					   Task.Delay(1000).ContinueWith( (_)=> App.DispatchOnUIThreadLow(()=>instance.items.NotifyReset()));
				   }
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
			UpdateRaidPlans();

		}

		private static void AddRaidStep(bool isMax )
		{
			var v = ((isMax ? raidCarryMax : raidCarryMin)*100).RoundToInt();
			var bestId = raidCarrySteps.IndexOfClosest(v);

			Log($"Add raid step {isMax}, {v}");

			raidCarrySteps[bestId] = v;

			instance.raidCarryMinBox.ItemsSource = raidCarrySteps;
			instance.raidCarryMaxBox.ItemsSource = raidCarrySteps;
			if (isMax)
			{
				raidCarryMax = (raidCarrySteps[bestId]*0.01f);
			}
			else
			{
				raidCarryMin = raidCarrySteps[bestId]*0.01f;
			}
			instance.raidCarryMinBox.SelectedIndex = raidCarrySteps.IndexOfClosest((raidCarryMin * 100).RoundToInt());
			instance.raidCarryMaxBox.SelectedIndex = raidCarrySteps.IndexOfClosest((raidCarryMax * 100).RoundToInt());
		}


		private void RaidCarryMaxSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		{
			//     Log("Submit: " + args.Text);
			if (args.Text.TryParseInt(out var _raidCarry))
			{

				//raidSteps;
				SetCarryMax(_raidCarry);
				Log(_raidCarry);
				AddRaidStep(true);
			}
			else
			{
				args.Handled = true;
				Assert(false);
			}
		}


		private void RaidCarryMinSubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		{
			//     Log("Submit: " + args.Text);
			if (args.Text.TryParseInt(out var _raidCarry))
			{
				Log(_raidCarry);

				SetCarryMin(_raidCarry);
				AddRaidStep(false);
			}
			else
			{
				args.Handled = true;
				Assert(false);
			}
		}

		public static async Task UpdateRaidPlans()
		{
			if(openCity!=0)
			 await City.GetOrAddCity(openCity).ShowDungeons();
			// tell UI that list data has changed
		}
		private static bool SetCarryMin(int src)
		{
			var newVal = (src) * 0.01f;
			if ((newVal - SettingsPage.raidCarryMin).Abs() <= 1.0f / 256.0f)
				return false;
			SettingsPage.raidCarryMin = newVal;
			UpdateRaidPlans(); 
			return true;
		}
		private static bool SetCarryMax(int src)
		{
			var newVal = (src) * 0.01f;
			if ((newVal - SettingsPage.raidCarryMax).Abs() <= 1.0f / 256.0f)
				return false;
			SettingsPage.raidCarryMax = newVal;
			UpdateRaidPlans(); 
			return true;
		}
		private void RaidCarryMinChanged(object sender, object _)
		{
			var box = raidCarryMinBox;
			//   Log("Sel update");
			var sel = box.SelectedIndex;
			if (sel != -1 )
			{

				SetCarryMin(raidCarrySteps[sel]);
				

			}
			else
			{
				Trace("No selectected");
			}
		}
		private void RaidCarryMaxSelChanged(object sender, object _)
		{
			var box = raidCarryMaxBox;
			//   Log("Sel update");
			var sel = box.SelectedIndex;
			if (sel != -1)
			{

				SetCarryMax(raidCarrySteps[sel]);


			}
			else
			{
				Trace("No selectected");
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
