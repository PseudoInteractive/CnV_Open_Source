using CnV.Game;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using CnV.Services;
using static CnV.Debug;
using static CnV.Troops;
using CnV.Helpers;
using static CnV.Settings;
// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace CnV.Views
{
	using Game;

	using System.Text.Json;

	public sealed partial class DungeonView : ContentDialog
	{

		NotifyCollection<Dungeon> items = new();

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
			Settings.SaveAll();
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
					instance.items.Set(dungeons,true);
				}
				return;
			}
			if( AppS.isPopupOpen )
			{
				Assert(false);
				return;
			}
			try
			{
				await AppS.DispatchOnUIThreadTask(async () =>
			   {
				   if (city != null)
				   {
					   openCity = city.CidOr0();
					   instance.Title = city.nameAndRemarks;
					   instance.items.Set(dungeons,true);
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
					   Task.Delay(1000).ContinueWith( (_)=> AppS.DispatchOnUIThreadLow(()=>instance.items.NotifyReset(true)));
				   }
				   await instance.ShowAsync2();
			   });
			}
			catch(Exception ex)
			{
				Log(ex);
			}
			finally
			{
				openCity=0;
				Settings.SaveAll();
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
		//				SetCarry(ref Settings.raidCarryMax, _raidCarry, 2, true);
		//				break;
		//			case nameof(raidCarryMinBox):
		//				SetCarry(ref Settings.raidCarryMin, _raidCarry, 0, true);
		//				break;
		//			default:
		//				SetCarry(ref Settings.raidCarryTarget, _raidCarry, 1, true);
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
		//				SetCarry(ref Settings.raidCarryMax, raidCarrySteps[sel], 2,false);
		//				break;
		//			case nameof(raidCarryMinBox):
		//				SetCarry(ref Settings.raidCarryMin, raidCarrySteps[sel], 0, false) ;
		//				break;
		//			default:
		//				SetCarry(ref Settings.raidCarryTarget, raidCarrySteps[sel], 1, false);
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
			for (var i = ttZero; i < ttCount; ++i)
			{
				if (IsRaider(i))
				{
					var but = new ToggleMenuFlyoutItem() { IsChecked = Settings.includeRaiders[i], DataContext = (object)i, Text = ttNames[i] };
					flyout.Items.Add(but);
				}
			}

			flyout.SetXamlRoot(button);
			flyout.Closing += Flyout_Closing;
			flyout.ShowAt(button);
		}

		private void Flyout_Closing(Microsoft.UI.Xaml.Controls.Primitives.FlyoutBase sender, Microsoft.UI.Xaml.Controls.Primitives.FlyoutBaseClosingEventArgs args)
		{
			var menu = (sender as MenuFlyout);
			int counter = 0;
			for (var  i = ttZero; i < ttCount; ++i)
			{
				if (IsRaider(i))
				{
					var but = menu.Items[counter] as ToggleMenuFlyoutItem;
					Settings.includeRaiders[i] = but.IsChecked;
					++counter;
				}
			}
			Raiding.UpdateTS(true, true);
		}

		private void RaidCarrySelChanged(Microsoft.UI.Xaml.Controls.NumberBox sender,Microsoft.UI.Xaml.Controls.NumberBoxValueChangedEventArgs args)
		{
			UpdateRaidPlans();
		}

		private void DataGridLoaded(object sender, RoutedEventArgs e)
		{
			var dataGrid = (xDataGrid)sender;
			using var __ = ADataGrid.SetupDataGrid(null, dataGrid, false,typeof(Dungeon));
		}
		public static async Task<bool> ShowDungeonList(City city,  bool autoRaid)
		{
			var rv = new List<Dungeon>();
			//	rv.Clear();
			var idealType = city.GetIdealDungeonType();
			foreach(var _dung in Cavern.all)
			{
				var dung = _dung.Value;
				var type = dung.tileType;
				if(Settings.raidOffDungeons || (type == idealType) || type == World.TileType.typeWater)
				{
					var d = new Dungeon(city,Cavern.Get(dung.c));
					var r = Raiding.ComputeIdealReps(d, city);
					d.isValid = r.isValid;
					d.carry   = r.averageCarry;
					d.reps    = (byte)r.reps;
					if(d.isValid || !autoRaid)
						rv.Add(d);


				}
			}

			rv.Sort((a, b) => a.GetScore(idealType).CompareTo(b.GetScore(idealType)));

			if(autoRaid)
			{
				var sent = false;
				if(rv.Count >0)
				{
					foreach(var _i in rv)
					{
						if(!_i.isValid)
							continue;
						var i       = _i;
						int counter = 0;

						Raiding.SendRaids(i);
						//if(!good)
						//{
						//	Note.Show($"Raid send failed for {city.nameMarkdown}, will try again");
						//}
						sent = true;
						break;
					}
				}
				if(!sent)
				{
					Note.Show($"No appropriate dungeons for {city.nameMarkdown}");
				}
				return sent;
			}
			else
			{

				// dont wait on this 
				//COTG.Views.MainPage.UpdateDungeonList(rv);
				await DungeonView.Show(city, rv);
				return true;
			}
		}
	}
}
