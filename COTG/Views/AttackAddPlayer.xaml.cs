﻿using COTG.Game;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using static COTG.Game.Enum;
using static COTG.Game.Spot;

namespace COTG.Views
{
	public sealed partial class AttackAddPlayer : ContentDialog
	{
		static AttackAddPlayer instance;
		public AttackAddPlayer()
		{
			this.InitializeComponent();
		}

		private void Suggest_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
		{
			if (args.ChosenSuggestion != null)
			{
				sender.Text = args.ChosenSuggestion as string;
				sender.ItemsSource = null;
			}
		}

		
		public static void Show(object sender, RoutedEventArgs e)
		{
			if (instance == null)
				instance = new AttackAddPlayer();
			instance.ShowAsyncInternal();
		}
		static byte [] troopTypesFromRadio =
		{
			ttVanquisher,
			ttSorcerer,
			ttDruid,
			ttHorseman,
			ttPraetor,
			ttScorpion,
			ttGuard,
			ttGuard,
		};

		public async Task ShowAsyncInternal()
		{

			ElementSoundPlayer.Play(ElementSoundKind.Show);
			var rv = await this.ShowAsync2();

			if (rv == ContentDialogResult.Primary)
			{
				ElementSoundPlayer.Play(ElementSoundKind.Invoke);
				var p = Player.FromName(playerName.Text);
				if (p == null)
				{
					Note.Show($"Missing player: {playerName}");
					return;
				}
				var wantAcademy = this.academy.IsChecked; 
				var wantCount = count.Value.RoundToInt();
				var tt = troopTypesFromRadio[troopType.SelectedIndex];
				var ttCompare = tt;
				var wantContinent = cont.Value.RoundToInt();
				var hasTwoTT = tt == ttSorcerer || tt == ttDruid || tt == ttHorseman || tt == ttPraetor;
				var toAdd = new List<AttackPlanCity>();
				Spot.TryConvertTroopTypeToClassification(tt, out var baseClassification);
				
				
				for (int j = 0; j < (hasTwoTT ? 2 : 1); ++j)
				{
					Spot.TryConvertTroopTypeToClassification(ttCompare, out var classification);

					foreach (var cid in p.cities)
					{
						if (toAdd.Count >= wantCount)
							break;
						var wi = World.GetInfoFromCid(cid);
						if (!wi.isCastle)
							continue;
						if (cid.CidToContinent() != wantContinent)
							continue;
						var city = City.GetOrAdd(cid);
						var cls = await city.ClassifyEx(false);
						if (cls != classification)
							continue;
						if (wantAcademy.GetValueOrDefault() && !city.hasAcademy.GetValueOrDefault())
							continue;
						if (AttackTab.attacks.Any(a => a.cid==cid))
							continue;
						city.classification = baseClassification;
						toAdd.Add(new AttackPlanCity(city, baseClassification == Classification.se ? AttackType.se
							: ((wantAcademy.HasValue ? wantAcademy.Value : city.hasAcademy.GetValueOrDefault()) ? AttackType.senator : AttackType.assault)));

					}
					if (hasTwoTT)
					{
						ttCompare = ttCompare switch
						{
							ttSorcerer => ttDruid,
							ttDruid => ttSorcerer,
							ttHorseman => ttArbalist,
							ttPraetor => ttPriestess,
							_ => ttCompare
						};
					}
				}
				if(toAdd.Any())
					await AttackTab.AddAttacks(toAdd);
				if(toAdd.Count < wantCount )
				{
					Note.Show($"Only found {toAdd.Count} appropriate castles");
				}

			}
		}
	}
}
