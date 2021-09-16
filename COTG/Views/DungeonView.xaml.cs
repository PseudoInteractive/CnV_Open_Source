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
		public const int raidStepCount = 15;

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
			
		}

		public static bool IsVisible() => openCity != 0;

		public static void Close()
		{
			instance.Hide();
		}
		static bool hasRunOnce;
		public static async Task Show(City city, List<Dungeon> dungeons)
		{
			if (instance == null)
				Initialize();

			if (openCity != 0)
			{
				if (city.CidOr0() == openCity && dungeons!=null)
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
				   if (city != null)
				   {
					   openCity = city.CidOr0();
					   instance.Title = city.nameAndRemarks;
					   instance.items.Set(dungeons);
					   instance.dungeonGrid.Visibility = Visibility.Visible;
				   }
				   else
				   {
					   openCity = 0;
					   instance.Title = "Raid Settings";
					   instance.dungeonGrid.Visibility = Visibility.Collapsed;
				   }
				   if (!hasRunOnce && city != null)
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

		private void SomethingChanged(Microsoft.UI.Xaml.Controls.NumberBox sender, Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
		{
			UpdateRaidPlans();

		}

		

		private static void EnsureCarryRanges(int changedId)
		{
			switch (changedId)
			{
				case 0:// min
					{
						raidCarryMax = raidCarryMax.Max(raidCarryMin);
						raidCarryTarget = raidCarryTarget.Max(raidCarryMin);
						break;
					}
				case 1:// target
					{
						raidCarryMax = raidCarryMax.Max(raidCarryTarget);
						raidCarryMin = raidCarryMin.Min(raidCarryTarget);
						break;
					}
				case 2:// max
					{
						raidCarryTarget = raidCarryTarget.Min(raidCarryMax);
						raidCarryMin = raidCarryMin.Min(raidCarryMax);
						break;
					}
			}
		}

		//private void RaidCarrySubmitted(ComboBox sender, ComboBoxTextSubmittedEventArgs args)
		//{
		//	//     Log("Submit: " + args.Text);
		//	if (args.Text.TryParseInt(out var _raidCarry))
		//	{
		//		var v = _raidCarry;
		//		var bestId = raidCarrySteps.IndexOfClosest(v);

		//		Log($"Add raid step {v}");

		//		raidCarrySteps[bestId] = v;

		//		//raidSteps;
		//		switch (sender.Name)
		//		{
		//			case nameof(raidCarryMaxBox):
		//				SetCarry(ref SettingsPage.raidCarryMax, _raidCarry, 2, true);
		//				break;
		//			case nameof(raidCarryMinBox):
		//				SetCarry(ref SettingsPage.raidCarryMin, _raidCarry, 0, true);
		//				break;
		//			default:
		//				SetCarry(ref SettingsPage.raidCarryTarget, _raidCarry, 1, true);
		//				break;
		//		}

		//		args.Handled = true;
		//	}
		//	else
		//	{
				
		//		Assert(false);
		//	}
		//}

		//private static void UpdateCarryBox(ComboBox box, float carry, bool sourceUpdated, bool textUpdated)
		//{
		//	if(sourceUpdated)
		//		box.ItemsSource = raidCarrySteps;
		//	var vI = (carry * 100).RoundToInt();
		//	var i = raidCarrySteps.IndexOfClosest(vI);
		//	if (i != box.SelectedIndex)
		//		box.SelectedIndex = i;
		//	if (textUpdated)
		//		box.Text = vI.ToString();

		//}

		public static async Task UpdateRaidPlans()
		{
			if(openCity!=0)
			 await City.GetOrAddCity(openCity).ShowDungeons();
			// tell UI that list data has changed
		}
		//private static bool SetCarry(ref float val, int src, int id, bool sourceUpdated)
		//{
		//	bool rv=sourceUpdated;
		//	var newVal = src * 0.01f;
		//	if ((newVal - val).Abs() <= 1.0f / 128.0f)
		//	{
		//		// no need to force update
		//	}
		//	else
		//	{
		//		val = newVal;
		//		EnsureCarryRanges(id);
		//		UpdateRaidPlans();
		//		rv = true;
		//	}
		//	if (rv)
		//	{
		//		UpdateCarryBox(instance.raidCarryMaxBox, raidCarryMax, sourceUpdated,id==2);
		//		UpdateCarryBox(instance.raidCarryMinBox, raidCarryMin, sourceUpdated,id==0);
		//		UpdateCarryBox(instance.raidCarryTargetBox, raidCarryTarget, sourceUpdated,id==1);
		//	}

		//	return rv;
		//}

		//private void RaidCarrySelChanged(object sender, object _)
		//{
		//	var box = sender as ComboBox;
		//	//   Log("Sel update");
		//	var sel = box.SelectedIndex;
		//	if (sel != -1 )
		//	{

		//		switch( box.Name )
		//		{
		//			case nameof(raidCarryMaxBox):
		//				SetCarry(ref SettingsPage.raidCarryMax, raidCarrySteps[sel], 2,false);
		//				break;
		//			case nameof(raidCarryMinBox):
		//				SetCarry(ref SettingsPage.raidCarryMin, raidCarrySteps[sel], 0, false) ;
		//				break;
		//			default:
		//				SetCarry(ref SettingsPage.raidCarryTarget, raidCarrySteps[sel], 1, false);
		//				break;
		//		}
						

		//	}
		//	else
		//	{
		//		Trace("No selectected");
		//	}
		//}

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

		private void RaidCarrySelChanged(Microsoft.UI.Xaml.Controls.NumberBox sender,Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
		{
			UpdateRaidPlans();
		}
	}
}
