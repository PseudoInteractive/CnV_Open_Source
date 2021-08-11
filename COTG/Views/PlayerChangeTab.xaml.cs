using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Collections.Concurrent;
using COTG.Helpers;
using Microsoft.Toolkit;
using Microsoft.Toolkit.Uwp.UI.Controls;
using COTG.Game;
namespace COTG.Views
{
	public class PlayerChanges
	{
		public int pid;
		public int activity => settled + castled*4 - abandonedCities + upgraded*1 - flattened*1 + castlesCappedEnemy*16 + castlesCappedAlly*2 - 
			castlesLostEnemy*4 - castlesLostAlly*4 - castlesAbandoned*4 + templesMade*32 -templesLost ; // approximate measure of overall non sub activity
																										//	public int nonSubActivity; // approximate measure of overall non sub activity
		public string alliance => Player.Get(pid).allianceName;
		
		public ushort allianceId => Player.Get(pid).alliance;
		public string name => Player.IdToName(pid);
		public int cities => Player.Get(pid).cities.Count();
		public int settled { get; set; }
		public int castled { get; set; }
		public int abandonedCities { get; set; }
		public int upgraded { get; set; }
		public int flattened { get; set; }
		public int castlesCappedEnemy { get; set; }
		public int castlesCappedAlly { get; set; }
		public int castlesLostEnemy { get; set; }
		public int castlesLostAlly { get; set; }
		public int castlesAbandoned { get; set; }
		public int templesMade { get; set; }
		public int templesLost { get; set; }

	}

	public sealed partial class PlayerChangeTab : UserTab
	{
		public static PlayerChangeTab instance;
		public static ResetableCollection<PlayerChanges> changes = new();

		public override TabPage defaultPage => TabPage.secondaryTabs;

		public PlayerChangeTab()
		{
			this.InitializeComponent();
			instance = this;
		}

		private void T0UITapped(object sender, PointerRoutedEventArgs e)
		{

		}

		private void playerChangesSorting(object sender, Microsoft.Toolkit.Uwp.UI.Controls.DataGridColumnEventArgs e)
		{
				var dg = sender as DataGrid;
				var cities = changes;
				var tag = e.Column.Header.ToString();
				//Use the Tag property to pass the bound column name for the sorting implementation
				Comparison<PlayerChanges> comparer = null;
				switch (tag)
				{
					case nameof(PlayerChanges.alliance): comparer = (a, b) => a.alliance.CompareTo(b.alliance); break;
					case nameof(PlayerChanges.activity): comparer = (a, b) => a.activity.CompareTo(b.activity); break;
					case nameof(PlayerChanges.name): comparer = (a, b) => a.name.CompareTo(b.name); break;
					case nameof(PlayerChanges.cities): comparer = (a, b) => a.cities.CompareTo(b.cities); break;
					case nameof(PlayerChanges.castled): comparer = (a, b) => a.castled.CompareTo(b.castled); break;
					case nameof(PlayerChanges.settled): comparer = (a, b) => a.settled.CompareTo(b.settled); break;
					case nameof(PlayerChanges.abandonedCities): comparer = (a, b) => a.abandonedCities.CompareTo(b.abandonedCities); break;
					case nameof(PlayerChanges.upgraded): comparer = (a, b) => a.upgraded.CompareTo(b.upgraded); break;
					case nameof(PlayerChanges.flattened): comparer = (a, b) => a.flattened.CompareTo(b.flattened); break;
					case nameof(PlayerChanges.castlesCappedEnemy): comparer = (a, b) => a.castlesCappedEnemy.CompareTo(b.castlesCappedEnemy); break;
					case nameof(PlayerChanges.castlesLostEnemy): comparer = (a, b) => a.castlesLostEnemy.CompareTo(b.castlesLostEnemy); break;
					case nameof(PlayerChanges.castlesCappedAlly): comparer = (a, b) => a.castlesCappedAlly.CompareTo(b.castlesCappedAlly); break;
					case nameof(PlayerChanges.castlesLostAlly): comparer = (a, b) => a.castlesLostAlly.CompareTo(b.castlesLostAlly); break;
					case nameof(PlayerChanges.castlesAbandoned): comparer = (a, b) => a.castlesAbandoned.CompareTo(b.castlesAbandoned); break;
					case nameof(PlayerChanges.templesMade): comparer = (a, b) => a.templesMade.CompareTo(b.templesMade); break;
					case nameof(PlayerChanges.templesLost): comparer = (a, b) => a.templesLost.CompareTo(b.templesLost); break;
				}

			if (comparer != null)
				{
					if (e.Column.SortDirection == null)
					{
						e.Column.SortDirection = DataGridSortDirection.Descending;
						cities.SortSmall(comparer);
						cities.NotifyReset();
					}
					else if (e.Column.SortDirection == DataGridSortDirection.Descending)
					{
						e.Column.SortDirection = DataGridSortDirection.Ascending;
						cities.SortSmall((b, a) => comparer(a, b)); // swap order of comparison
						cities.NotifyReset();
					}
					else
					{
						e.Column.SortDirection = null;

					}
				}
				// add code to handle sorting by other columns as required

				// Remove sorting indicators from other columns
				foreach (var dgColumn in dg.Columns)
				{
					if (dgColumn.Tag != null && dgColumn.Tag.ToString() != tag)
					{
						dgColumn.SortDirection = null;
					}
				}
			

		}

		//private void PlayerChangeAuto(object sender, DataGridAutoGeneratingColumnEventArgs e)
		//{
		//	e.Column.he
		//}
	}
}
