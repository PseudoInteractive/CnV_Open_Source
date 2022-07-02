using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Syncfusion.UI.Xaml.Charts;

using CnV.Chart;
// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace CnV
{
	using static ChartData;
	public sealed partial class ChartDialog:Views.UserTab
	{
		public static ChartDialog instance;
		int _metric;
		bool byAlliance {
			get { return _byAlliance; }
			set {
				if(_byAlliance!=value) {
					_byAlliance=value;
					AppS.QueueOnUIThread(UpdateData);
					OnPropertyChanged(nameof(byAlliance));
				}
			}
		}
		private int selectedSeries1;

		internal int metric {
			get {
				return _metric;
			}
			set {
				if(_metric != value) {
					_metric = value;
					AppS.QueueOnUIThread(UpdateData);
				}
			}
		}

		public ChartDialog() {
			Assert(instance == null);
			instance = this;
			this.InitializeComponent();
		}

		internal static readonly string[] metrics = {
			nameof(DataPoint.points),
			nameof(DataPoint.donations),
			nameof(DataPoint.cityFaith),
			nameof(DataPoint.raiding),
			nameof(DataPoint.cities),
			nameof(DataPoint.ts),
			nameof(DataPoint.tsOffense),
			nameof(DataPoint.tsDefense),
			nameof(DataPoint.kills),
			nameof(DataPoint.offReputation),
			nameof(DataPoint.rssProduced),

		};
		private bool _byAlliance;
		internal  bool isMetricHidden => _metric is (>=5 and <= 7);

		public int selectedSeries { get => selectedSeries1; set {
				Note.Show($"Sel series {value}");
				selectedSeries1=value;
			}
		}

		internal static async Task<bool> ShowPlayerMetric(string property, bool byAlliance) {
			var id = metrics.IndexOfIgnoreCase(property );
			if(id==-1)
				return false;
			await ShowOrAdd<ChartDialog>(true,onlyIfClosed: true);
			// will trigger callback
			instance.byAlliance = byAlliance;
			// will trigger callback
			instance.metricCombo.SelectedIndex = id;
		//	var exp = instance.playerOrAlliance.GetBindingExpression(ToggleSwitch.IsOnProperty);
		//	exp.UpdateSource();

			return true;
		}

		void UpdateData() {

			try {
				sfChart.Series.Clear();
				var member = metrics[_metric];
				var doAlliances = byAlliance;
				var hidden = isMetricHidden && !doAlliances;
				var items = doAlliances ? allianceStats.Values.ToArray() : playerStats.Values.ToArray(); // takea  copy in case it changes
				var playerIds = playerStats.Keys.ToArray();
				var keyId = -1;
				var seriesList = new List<FastLineSeries>();
				foreach(var item in items) {
					++keyId;
					var values = item.ToArray();
					string name;
					if( doAlliances ) {
						name = values.First().name;
					}
					else {
						var player = Player.Get(playerIds[keyId]);
						name = !hidden || player.sharesInfo ? player.shortName : "?";
						foreach(var v in values) {
							v.name = name;
						}

					}
					
					Assert(values.Length > 0);
					var series = new FastLineSeries() {
						YBindingPath=member==nameof(DataPoint.rssProduced) ? "rssProduced.wood" : member,
						XBindingPath="t",Label=name,
						ItemsSource=values,
						ShowTooltip=true,
						StrokeThickness=5,
					//	BorderThickness = new(5),
						ShowTrackballInfo=false,
						ShowDataLabels=false,
						TrackballLabelTemplate=sfChart.Resources["trackTemplate"] as DataTemplate,
						TooltipTemplate= sfChart.Resources["tooltipTemplate0"] as DataTemplate
					};
					//series.RightTapped+=(a,b) => {
					//	Note.Show("Right tapped");
					//};
					series.Tapped+=(a,b) => {
						Note.Show("left tapped");
					};
					//series.PointerPressed+=(a,b) => {
					//	var c = b.GetCurrentPoint(sfChart).Position;
					//	int id = series.GetDataPointIndex(c.X,c.Y);
					//	var t = (series.ItemsSource as DataPoint[])[id];
					//	var st = ServerTime.PseudoLocalAsServerTime(t.t); 
					//	Sim.GotoTime(st);
					//};
					seriesList.Add(series);
				}
				if(hidden)
					seriesList.ShuffleInPlace( (ulong)metric );
				sfChart.Series.AddRange(seriesList);

			}
			catch(Exception _ex) {
				LogEx(_ex);

			}

		}
		public override async Task Closed()
		{ 
			await base.Closed();
			instance = null;
		}

		public override TabPage defaultPage => TabPage.secondaryTabs;
		override public Task VisibilityChanged(bool visible,bool longTerm) {
			//   Log("Vis change" + visible);


			if(visible) {
				AppS.QueueOnUIThread(UpdateData);
			}
			return base.VisibilityChanged(visible,longTerm: longTerm);
		}

		private void OnLoaded(object sender,RoutedEventArgs e) {
			// Add series
		//	UpdateData();


		}

	

		//private void PrimaryAxisClicked(object sender,AxisLabelClickedEventArgs e) {
		//	var label = e.Label;
		//	Log(label);
		//	if( DateTime.TryParse(label.Content.ToString(), out var t))
		//	{
		//		Sim.GotoTime(ServerTime.PseudoLocalAsServerTime(t));
		//	}
		//}

		//private void SecondaryAxisClicked(object sender,AxisLabelClickedEventArgs e) {

		//	var label = e.Label;
		//	Log(label);
		//}

        private void sfChart_SelectionChanging(object sender,SelectionChangingEventArgs e) {
				Note.Show($"Sel changing {e.CurrentIndex} {e.PreviousIndex}");
        }

		private void sfChart_SelectionChanged(object sender,Syncfusion.UI.Xaml.Charts.SelectionChangedEventArgs e) {
						Note.Show($"Sel changed {e.CurrentIndex} {e.PreviousIndex}");
		}
	}
}
